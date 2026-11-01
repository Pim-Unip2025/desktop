using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TronusDesktop.Models;
using TronusDesktop.Services;
using TronusDesktop.UI;

namespace TronusDesktop.Views
{
    public partial class UcAdmin : UserControl
    {
        private Panel topHudPanel = null!;
        private Label lblTitulo = null!;
        private Label lblSubtitulo = null!;
        private TronusButton btnAtualizar = null!;
        private TronusButton btnLogout = null!;

        private Panel mainScrollContainer = null!;
        private FlowLayoutPanel flowKpis = null!;
        private TronusPanel cardTabelaUsuarios = null!;
        private Panel headerTabelaPanel = null!;
        private TextBox txtBusca = null!;
        private ComboBox cbFiltroPapel = null!;
        private Label lblContadorCadastros = null!;
        private Panel listUsuariosPanel = null!;

        // Referências para atualizar os KPIs dinamicamente
        private Label lblKpiTotalUsuarios = null!;
        private Label lblKpiAlunosAtivos = null!;
        private Label lblKpiProfessoresAdmins = null!;
        private Label lblKpiPersonagens = null!;
        private Label lblKpiQuestoes = null!;
        private Label lblKpiTaxaGeral = null!;

        private class UsuarioAdminItem
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Papel { get; set; } = "aluno";
            public int? ProfessorReinoId { get; set; }
            public string ReinoOuHeroi { get; set; } = "—";
            public string DataCadastro { get; set; } = string.Empty;
            public bool Ativo { get; set; } = true;
            public bool OrigemApi { get; set; } = false;
        }

        private readonly List<UsuarioAdminItem> usuarios = new();

        private static readonly Dictionary<int, string> MapaReinos = new()
        {
            { 1, "Reino Python" },
            { 2, "Reino C++" },
            { 3, "Reino Banco de Dados" },
            { 4, "Reino Redes" },
            { 5, "Reino Eng. Software" }
        };

        public UcAdmin()
        {
            this.DoubleBuffered = true;
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
            this.UpdateStyles();

            InitializeComponent();
            SetupUI();
            this.Load += async (s, e) => await CarregarCadastrosAtivosAsync();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "UcAdmin";
            this.Size = new Size(1024, 728);
            this.ResumeLayout(false);
        }

        private async Task CarregarCadastrosAtivosAsync()
        {
            usuarios.Clear();
            EstatisticasAdminDto? statsApi = null;

            // 1. Tenta buscar todos os usuários cadastrados na API em nuvem (se estiver Online)
            if (!ApiService.IsOfflineMode)
            {
                try
                {
                    var listaApi = await ApiService.Instance.GetAsync<List<UsuarioDto>>("/admin/usuarios");
                    if (listaApi != null)
                    {
                        foreach (var u in listaApi)
                        {
                            usuarios.Add(ConverterDtoParaItem(u, origemApi: true));
                        }
                    }

                    statsApi = await ApiService.Instance.GetAsync<EstatisticasAdminDto>("/admin/estatisticas");
                }
                catch
                {
                    // Em caso de token local ou indisponibilidade momentânea da API, faz merge transparente com o catálogo ativo local
                }
            }

            // 2. Sincroniza todos os cadastros ativos do ecossistema local/sessão (evitando duplicatas por e-mail)
            foreach (var uLocal in OfflineDataService.UsuariosSistema)
            {
                if (!usuarios.Any(existing => existing.Email.Equals(uLocal.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    usuarios.Add(ConverterDtoParaItem(uLocal, origemApi: false));
                }
            }

            // Ordena por ID crescente
            var ordenados = usuarios.OrderBy(u => u.Id).ToList();
            usuarios.Clear();
            usuarios.AddRange(ordenados);

            AtualizarKpisDinamicos(statsApi);
            FiltrarUsuarios();
        }

        private UsuarioAdminItem ConverterDtoParaItem(UsuarioDto u, bool origemApi)
        {
            string papel = string.IsNullOrWhiteSpace(u.Papel) ? "aluno" : u.Papel.ToLowerInvariant();
            string reinoOuHeroi = u.HeroiNome ?? "";

            if (string.IsNullOrWhiteSpace(reinoOuHeroi))
            {
                if (papel == "professor")
                {
                    int rId = u.ProfessorReinoId ?? 1;
                    reinoOuHeroi = MapaReinos.TryGetValue(rId, out var rNome) ? rNome : $"Reino #{rId}";
                }
                else if (papel == "admin")
                {
                    reinoOuHeroi = "Governança Global";
                }
                else
                {
                    reinoOuHeroi = "Herói Ativo";
                }
            }

            return new UsuarioAdminItem
            {
                Id = u.Id,
                Nome = u.Nome ?? "Usuário",
                Email = u.Email ?? "",
                Papel = papel,
                ProfessorReinoId = u.ProfessorReinoId,
                ReinoOuHeroi = reinoOuHeroi,
                DataCadastro = string.IsNullOrWhiteSpace(u.DataCadastro) ? DateTime.Now.ToString("dd/MM/yyyy") : u.DataCadastro,
                Ativo = true,
                OrigemApi = origemApi
            };
        }

        private void AtualizarKpisDinamicos(EstatisticasAdminDto? statsApi = null)
        {
            int totalCadastros = usuarios.Count;
            int totalAlunos = usuarios.Count(u => u.Papel == "aluno");
            int totalDocentesAdm = usuarios.Count(u => u.Papel == "professor" || u.Papel == "admin");
            int totalPersonagens = statsApi != null && statsApi.Personagens > totalAlunos
                ? statsApi.Personagens
                : totalAlunos;
            int totalQuestoes = statsApi?.Questoes > 0 ? statsApi.Questoes : 75;
            decimal taxa = statsApi?.TaxaAcerto > 0 ? statsApi.TaxaAcerto : 86.6m;

            if (lblKpiTotalUsuarios != null) lblKpiTotalUsuarios.Text = totalCadastros.ToString();
            if (lblKpiAlunosAtivos != null) lblKpiAlunosAtivos.Text = totalAlunos.ToString();
            if (lblKpiProfessoresAdmins != null) lblKpiProfessoresAdmins.Text = totalDocentesAdm.ToString();
            if (lblKpiPersonagens != null) lblKpiPersonagens.Text = totalPersonagens.ToString();
            if (lblKpiQuestoes != null) lblKpiQuestoes.Text = totalQuestoes.ToString();
            if (lblKpiTaxaGeral != null) lblKpiTaxaGeral.Text = $"{taxa:0.#}%";
        }

        private void SetupUI()
        {
            this.BackColor = Theme.Background;
            string bgPath = Path.Combine(Application.StartupPath, "img", "fundo_reinos.jpg");
            if (Theme.IsDarkMode && File.Exists(bgPath))
            {
                this.BackgroundImage = Image.FromFile(bgPath);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            this.Paint += (s, e) =>
            {
                if (Theme.IsDarkMode)
                {
                    using var brush = new SolidBrush(Color.FromArgb(180, 12, 10, 8));
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // ═══════════════════════════════════════════════════
            // HUD SUPERIOR DO ADMINISTRADOR
            // ═══════════════════════════════════════════════════
            topHudPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = Theme.TopBarBackground
            };
            topHudPanel.Paint += (s, e) =>
            {
                using var pen = new Pen(Theme.BorderColor, 1);
                e.Graphics.DrawLine(pen, 0, topHudPanel.Height - 1, topHudPanel.Width, topHudPanel.Height - 1);
            };
            this.Controls.Add(topHudPanel);

            lblTitulo = new Label
            {
                Text = "🛡️ PAINEL ADMINISTRATIVO — GOVERNANÇA GLOBAL",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 12),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblTitulo);

            string admNome = SessionManager.CurrentUser?.Nome ?? "admin";
            lblSubtitulo = new Label
            {
                Text = $"Governança: {admNome} · Visualização Completa de Todos os Cadastros Ativos · UNIP 2026",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 9.2F, FontStyle.Italic),
                AutoSize = true,
                Location = new Point(28, 40),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblSubtitulo);

            btnAtualizar = new TronusButton
            {
                Text = "🔄 Atualizar",
                Size = new Size(125, 36),
                Location = new Point(720, 18),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnAtualizar.Click += async (s, e) => await CarregarCadastrosAtivosAsync();
            topHudPanel.Controls.Add(btnAtualizar);

            btnLogout = new TronusButton
            {
                Text = "Sair / Logout",
                Size = new Size(130, 36),
                Location = new Point(860, 18),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnLogout.Click += (s, e) =>
            {
                var confirm = MessageBox.Show("Deseja realmente sair do Painel Administrativo?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    SessionManager.Logout();
                    (this.FindForm() as MainForm)?.LoadControl(new UcLogin());
                }
            };
            topHudPanel.Controls.Add(btnLogout);

            void ReposAdminButtons()
            {
                if (btnLogout != null && btnAtualizar != null && topHudPanel != null)
                {
                    btnLogout.Location = new Point(topHudPanel.Width - btnLogout.Width - 25, 18);
                    btnAtualizar.Location = new Point(btnLogout.Left - btnAtualizar.Width - 12, 18);
                }
            }

            topHudPanel.Resize += (s, e) => ReposAdminButtons();
            ReposAdminButtons();

            // ═══════════════════════════════════════════════════
            // CONTAINER PRINCIPAL
            // ═══════════════════════════════════════════════════
            mainScrollContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false,
                BackColor = Color.Transparent,
                Padding = new Padding(25, 12, 25, 20)
            };
            this.Controls.Add(mainScrollContainer);
            mainScrollContainer.BringToFront();

            ConstruirKpis();
            ConstruirTabelaUsuarios();

            this.Resize += (s, e) => AdjustLayout();
            AdjustLayout();
        }

        private void ConstruirKpis()
        {
            var lblSecaoKpi = new Label
            {
                Text = "📊 MÉTRICAS EM TEMPO REAL DOS CADASTROS ATIVOS",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 10),
                BackColor = Color.Transparent
            };
            mainScrollContainer.Controls.Add(lblSecaoKpi);

            flowKpis = new FlowLayoutPanel
            {
                Location = new Point(25, 36),
                Size = new Size(960, 88),
                BackColor = Color.Transparent,
                WrapContents = false,
                AutoScroll = false
            };
            mainScrollContainer.Controls.Add(flowKpis);

            flowKpis.Controls.Add(CriarCardKpi("👥 Cadastros Ativos", "0", "Total no sistema", out lblKpiTotalUsuarios));
            flowKpis.Controls.Add(CriarCardKpi("🧙 Alunos Ativos", "0", "Contas discentes", out lblKpiAlunosAtivos));
            flowKpis.Controls.Add(CriarCardKpi("🎓 Docentes & Adm", "0", "Professores e Admins", out lblKpiProfessoresAdmins));
            flowKpis.Controls.Add(CriarCardKpi("⚔️ Heróis Forjados", "0", "Personagens ativos", out lblKpiPersonagens));
            flowKpis.Controls.Add(CriarCardKpi("📜 Questões", "75", "Nos 5 Reinos", out lblKpiQuestoes));
            flowKpis.Controls.Add(CriarCardKpi("🎯 Taxa Geral", "86.6%", "Média de acertos", out lblKpiTaxaGeral));
        }

        private Panel CriarCardKpi(string titulo, string valor, string desc, out Label lblValorRef)
        {
            var card = new TronusPanel
            {
                Size = new Size(152, 80),
                Margin = new Padding(0, 0, 10, 0),
                BackColor = Color.FromArgb(200, 18, 16, 14)
            };

            var lblT = new Label
            {
                Text = titulo,
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Location = new Point(10, 7),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblT);

            lblValorRef = new Label
            {
                Text = valor,
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 16.5F, FontStyle.Bold),
                Location = new Point(10, 24),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblValorRef);

            var lblD = new Label
            {
                Text = desc,
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Italic),
                Location = new Point(10, 56),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblD);

            return card;
        }

        private void ConstruirTabelaUsuarios()
        {
            var lblSecaoUsers = new Label
            {
                Text = "👥 TODOS OS CADASTROS ATIVOS NO SISTEMA",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 136),
                BackColor = Color.Transparent
            };
            mainScrollContainer.Controls.Add(lblSecaoUsers);

            lblContadorCadastros = new Label
            {
                Text = "Carregando cadastros...",
                ForeColor = Color.FromArgb(130, 225, 150),
                Font = new Font("Segoe UI", 8.8F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 158),
                BackColor = Color.Transparent
            };
            mainScrollContainer.Controls.Add(lblContadorCadastros);

            cbFiltroPapel = new ComboBox
            {
                Location = new Point(375, 142),
                Size = new Size(165, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(32, 30, 26),
                ForeColor = Theme.Gold
            };
            cbFiltroPapel.Items.AddRange(new object[]
            {
                "Todos os Perfis",
                "Apenas Alunos",
                "Apenas Professores",
                "Apenas Admins"
            });
            cbFiltroPapel.SelectedIndex = 0;
            cbFiltroPapel.SelectedIndexChanged += (s, e) => FiltrarUsuarios();
            mainScrollContainer.Controls.Add(cbFiltroPapel);

            txtBusca = new TextBox
            {
                Location = new Point(550, 142),
                Size = new Size(260, 28),
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.FromArgb(35, 33, 28),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "🔍 Buscar por nome, e-mail, ID ou herói..."
            };
            txtBusca.TextChanged += (s, e) => FiltrarUsuarios();
            mainScrollContainer.Controls.Add(txtBusca);

            var btnNovo = new TronusButton
            {
                Text = "➕ Novo Cadastro",
                Size = new Size(155, 32),
                Location = new Point(825, 139),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnNovo.Click += BtnNovoUsuario_Click;
            mainScrollContainer.Controls.Add(btnNovo);

            cardTabelaUsuarios = new TronusPanel
            {
                Location = new Point(25, 182),
                Size = new Size(960, 440),
                BackColor = Color.FromArgb(215, 16, 14, 12)
            };
            mainScrollContainer.Controls.Add(cardTabelaUsuarios);

            // Cabeçalho da Tabela
            headerTabelaPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = Color.FromArgb(55, 212, 175, 55)
            };
            cardTabelaUsuarios.Controls.Add(headerTabelaPanel);

            AdicionarColuna(headerTabelaPanel, "ID", 12, 42);
            AdicionarColuna(headerTabelaPanel, "NOME DE USUÁRIO", 58, 170);
            AdicionarColuna(headerTabelaPanel, "E-MAIL CADASTRADO", 235, 205);
            AdicionarColuna(headerTabelaPanel, "PERFIL", 445, 115);
            AdicionarColuna(headerTabelaPanel, "REINO / HERÓI", 565, 175);
            AdicionarColuna(headerTabelaPanel, "STATUS", 745, 78);
            AdicionarColuna(headerTabelaPanel, "AÇÕES", 828, 125);

            listUsuariosPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 4, 0, 8)
            };
            cardTabelaUsuarios.Controls.Add(listUsuariosPanel);
            listUsuariosPanel.BringToFront();
        }

        private void AdicionarColuna(Panel p, string texto, int x, int width)
        {
            var lbl = new Label
            {
                Text = texto,
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 8.2F, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(width, 36),
                Location = new Point(x, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            p.Controls.Add(lbl);
        }

        private void FiltrarUsuarios()
        {
            if (listUsuariosPanel == null) return;

            listUsuariosPanel.SuspendLayout();
            try
            {
                listUsuariosPanel.Controls.Clear();
                string termo = txtBusca?.Text.Trim().ToLowerInvariant() ?? "";
                string filtroPapel = cbFiltroPapel?.SelectedItem?.ToString() ?? "Todos os Perfis";

                var filtrados = usuarios.Where(u =>
                {
                    bool passaPapel = filtroPapel switch
                    {
                        "Apenas Alunos" => u.Papel == "aluno",
                        "Apenas Professores" => u.Papel == "professor",
                        "Apenas Admins" => u.Papel == "admin",
                        _ => true
                    };
                    if (!passaPapel) return false;

                    if (string.IsNullOrEmpty(termo)) return true;

                    return u.Id.ToString().Contains(termo) ||
                           u.Nome.ToLowerInvariant().Contains(termo) ||
                           u.Email.ToLowerInvariant().Contains(termo) ||
                           u.Papel.ToLowerInvariant().Contains(termo) ||
                           u.ReinoOuHeroi.ToLowerInvariant().Contains(termo);
                }).ToList();

                if (lblContadorCadastros != null)
                {
                    int qtdAlunos = filtrados.Count(x => x.Papel == "aluno");
                    int qtdProfs = filtrados.Count(x => x.Papel == "professor");
                    int qtdAdms = filtrados.Count(x => x.Papel == "admin");
                    lblContadorCadastros.Text = $"🟢 Exibindo {filtrados.Count} de {usuarios.Count} cadastros ativos ({qtdAlunos} Alunos · {qtdProfs} Professores · {qtdAdms} Admins)";
                }

                int rowWidth = Math.Max(920, listUsuariosPanel.ClientSize.Width - 26);
                int y = 4;
                int index = 0;

                foreach (var u in filtrados)
                {
                    var row = new Panel
                    {
                        Location = new Point(6, y),
                        Size = new Size(rowWidth, 38),
                        BackColor = (index % 2 == 0)
                            ? Color.FromArgb(28, 255, 255, 255)
                            : Color.FromArgb(10, 212, 175, 55)
                    };

                    row.Controls.Add(CriarLabel($"#{u.Id}", 8, 42, Theme.Gold, true));
                    row.Controls.Add(CriarLabel(u.Nome, 54, 170, Color.White, true));
                    row.Controls.Add(CriarLabel(u.Email, 230, 205, Theme.DimText, false));

                    Color corPapel = u.Papel switch
                    {
                        "admin" => Color.FromArgb(255, 195, 75),
                        "professor" => Color.FromArgb(120, 230, 140),
                        _ => Color.FromArgb(130, 205, 255)
                    };
                    string papelBadge = u.Papel.ToUpperInvariant() switch
                    {
                        "ADMIN" => "🛡️ ADMIN",
                        "PROFESSOR" => "🎓 PROFESSOR",
                        _ => "🧙 ALUNO"
                    };
                    row.Controls.Add(CriarLabel(papelBadge, 440, 115, corPapel, true));

                    row.Controls.Add(CriarLabel(u.ReinoOuHeroi, 560, 175, Theme.PrimaryText, false));
                    row.Controls.Add(CriarLabel("🟢 Ativo", 740, 75, Color.FromArgb(90, 225, 130), true));

                    // Botão Alterar Papel
                    var btnMudarPapel = new Button
                    {
                        Text = "Papel ⟳",
                        Font = new Font("Segoe UI", 7.8F, FontStyle.Bold),
                        ForeColor = Theme.Gold,
                        BackColor = Color.FromArgb(30, 28, 24),
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(64, 25),
                        Location = new Point(822, 6),
                        Cursor = Cursors.Hand
                    };
                    btnMudarPapel.FlatAppearance.BorderColor = Theme.BorderColor;
                    var usuarioCapturado = u;
                    btnMudarPapel.Click += (s, e) => AlternarPapel(usuarioCapturado);
                    row.Controls.Add(btnMudarPapel);

                    // Botão Excluir (Protegido para admin/professor conforme regra do sistema)
                    var btnDel = new Button
                    {
                        Text = (u.Papel == "admin" || u.Papel == "professor") ? "🔒 Fixo" : "Excluir ✕",
                        Font = new Font("Segoe UI", 7.8F, FontStyle.Bold),
                        ForeColor = (u.Papel == "admin" || u.Papel == "professor")
                            ? Color.FromArgb(150, 145, 130)
                            : Color.FromArgb(245, 105, 105),
                        BackColor = Color.FromArgb(30, 24, 24),
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(68, 25),
                        Location = new Point(892, 6),
                        Cursor = Cursors.Hand
                    };
                    btnDel.FlatAppearance.BorderColor = (u.Papel == "admin" || u.Papel == "professor")
                        ? Color.FromArgb(80, 150, 145, 130)
                        : Color.FromArgb(120, 240, 100, 100);

                    btnDel.Click += async (s, e) => await ExcluirUsuarioAsync(usuarioCapturado);
                    row.Controls.Add(btnDel);

                    listUsuariosPanel.Controls.Add(row);
                    y += 41;
                    index++;
                }
            }
            finally
            {
                Theme.EnableDoubleBuffering(listUsuariosPanel);
                listUsuariosPanel.ResumeLayout(true);
            }
        }

        private async Task ExcluirUsuarioAsync(UsuarioAdminItem u)
        {
            if (u.Papel == "admin" || u.Papel == "professor")
            {
                MessageBox.Show(
                    $"Contas com perfil '{u.Papel.ToUpperInvariant()}' são protegidas pela governança do sistema e não podem ser excluídas.",
                    "Conta Protegida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var conf = MessageBox.Show(
                $"Deseja realmente remover o cadastro ativo de '{u.Nome}' ({u.Email})?\n\nO personagem e o histórico de progresso também serão removidos.",
                "Confirmar Exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (conf != DialogResult.Yes) return;

            if (u.OrigemApi && !ApiService.IsOfflineMode)
            {
                try
                {
                    await ApiService.Instance.DeleteAsync($"/admin/usuarios/{u.Id}");
                }
                catch
                {
                    // Continua remoção no catálogo local
                }
            }

            OfflineDataService.RemoverUsuario(u.Id);
            usuarios.Remove(u);
            AtualizarKpisDinamicos();
            FiltrarUsuarios();
        }

        private void AlternarPapel(UsuarioAdminItem u)
        {
            string novoPapel = u.Papel switch
            {
                "aluno" => "professor",
                "professor" => "admin",
                _ => "aluno"
            };

            var conf = MessageBox.Show(
                $"Deseja alterar o papel do cadastro '{u.Nome}' de '{u.Papel}' para '{novoPapel}'?",
                "Governança de Permissões",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (conf == DialogResult.Yes)
            {
                u.Papel = novoPapel;
                u.ReinoOuHeroi = novoPapel switch
                {
                    "professor" => "Reino Python",
                    "admin" => "Governança Global",
                    _ => "Herói Ativo"
                };

                OfflineDataService.RegistrarOuAtualizarUsuario(new UsuarioDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Papel = u.Papel,
                    HeroiNome = u.ReinoOuHeroi,
                    DataCadastro = u.DataCadastro,
                    Ativo = true
                });

                AtualizarKpisDinamicos();
                FiltrarUsuarios();
                MessageBox.Show($"Papel atualizado com sucesso para '{novoPapel}'!", "Permissão Concedida", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnNovoUsuario_Click(object? sender, EventArgs e)
        {
            using var formDialog = new Form
            {
                Text = "Novo Cadastro Ativo — Governança Tronus",
                Size = new Size(420, 390),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Theme.Background,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblT = new Label
            {
                Text = "CADASTRAR USUÁRIO NO SISTEMA",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };
            formDialog.Controls.Add(lblT);

            formDialog.Controls.Add(new Label { Text = "Nome de Usuário / Completo:", ForeColor = Theme.PrimaryText, Location = new Point(20, 50), AutoSize = true });
            var txtNome = new TextBox { Location = new Point(20, 70), Size = new Size(360, 26), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.White };
            formDialog.Controls.Add(txtNome);

            formDialog.Controls.Add(new Label { Text = "E-mail Institucional:", ForeColor = Theme.PrimaryText, Location = new Point(20, 105), AutoSize = true });
            var txtEmail = new TextBox { Location = new Point(20, 125), Size = new Size(360, 26), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.White };
            formDialog.Controls.Add(txtEmail);

            formDialog.Controls.Add(new Label { Text = "Perfil de Acesso:", ForeColor = Theme.PrimaryText, Location = new Point(20, 160), AutoSize = true });
            var cbP = new ComboBox
            {
                Location = new Point(20, 180),
                Size = new Size(360, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Theme.Gold
            };
            cbP.Items.AddRange(new object[] { "aluno", "professor", "admin" });
            cbP.SelectedIndex = 0;
            formDialog.Controls.Add(cbP);

            formDialog.Controls.Add(new Label { Text = "Herói ou Reino Vinculado:", ForeColor = Theme.PrimaryText, Location = new Point(20, 215), AutoSize = true });
            var txtHeroi = new TextBox
            {
                Location = new Point(20, 235),
                Size = new Size(360, 26),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                PlaceholderText = "Ex: Sir Galahad (Plebeu) ou Reino Python"
            };
            formDialog.Controls.Add(txtHeroi);

            var btnCriar = new TronusButton { Text = "SALVAR CADASTRO ATIVO", Location = new Point(20, 280), Size = new Size(360, 42) };
            btnCriar.Click += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Preencha nome e e-mail.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int novoId = (usuarios.Count > 0 ? usuarios.Max(u => u.Id) : 0) + 1;
                string papelEscolhido = cbP.SelectedItem?.ToString() ?? "aluno";
                string heroiOuReino = string.IsNullOrWhiteSpace(txtHeroi.Text)
                    ? (papelEscolhido == "professor" ? "Reino Python" : papelEscolhido == "admin" ? "Governança Global" : "Herói Recém-Cadastrado")
                    : txtHeroi.Text.Trim();

                var novoDto = new UsuarioDto
                {
                    Id = novoId,
                    Nome = txtNome.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Papel = papelEscolhido,
                    HeroiNome = heroiOuReino,
                    DataCadastro = DateTime.Now.ToString("dd/MM/yyyy"),
                    Ativo = true
                };

                OfflineDataService.RegistrarOuAtualizarUsuario(novoDto);
                usuarios.Add(ConverterDtoParaItem(novoDto, origemApi: false));

                AtualizarKpisDinamicos();
                FiltrarUsuarios();
                formDialog.Close();
                MessageBox.Show("Novo cadastro ativo registrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            formDialog.Controls.Add(btnCriar);

            formDialog.ShowDialog(this);
        }

        private Label CriarLabel(string texto, int x, int width, Color cor, bool bold)
        {
            return new Label
            {
                Text = texto,
                ForeColor = cor,
                Font = new Font("Segoe UI", 8.8F, bold ? FontStyle.Bold : FontStyle.Regular),
                AutoSize = false,
                AutoEllipsis = true,
                Size = new Size(width, 38),
                Location = new Point(x, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
        }

        private void AdjustLayout()
        {
            int w = Math.Max(760, this.ClientSize.Width - 50);

            if (flowKpis != null)
            {
                flowKpis.Width = w;
                int count = flowKpis.Controls.Count;
                if (count > 0)
                {
                    int cardW = Math.Max(135, (w - (count * 10)) / count);
                    foreach (Control c in flowKpis.Controls)
                    {
                        c.Width = cardW;
                    }
                }
            }

            if (cardTabelaUsuarios != null)
            {
                cardTabelaUsuarios.Width = w;
                int availableHeight = this.ClientSize.Height - topHudPanel.Height - cardTabelaUsuarios.Top - 20;
                cardTabelaUsuarios.Height = Math.Max(260, availableHeight);
            }
        }
    }
}
