using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TronusDesktop.Models;
using TronusDesktop.Services;
using TronusDesktop.UI;

namespace TronusDesktop.Views
{
    public partial class UcAdmin : UserControl
    {
        private Panel topHudPanel;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private TronusButton btnLogout;
        private AccessibilityThemeButton btnTheme;

        private Panel mainScrollContainer;
        private FlowLayoutPanel flowKpis;
        private TronusPanel cardTabelaUsuarios;
        private TextBox txtBusca;
        private Panel listUsuariosPanel;

        private class UsuarioAdminItem
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public string Email { get; set; }
            public string Papel { get; set; }
            public string DataCadastro { get; set; }
        }

        private readonly List<UsuarioAdminItem> usuarios = new();

        public UcAdmin()
        {
            InitializeComponent();
            CarregarDadosIniciais();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "UcAdmin";
            this.Size = new Size(1024, 728);
            this.ResumeLayout(false);
        }

        private void CarregarDadosIniciais()
        {
            usuarios.Add(new UsuarioAdminItem { Id = 1, Nome = "admin", Email = "admin@tronus.com", Papel = "admin", DataCadastro = "01/02/2026" });
            usuarios.Add(new UsuarioAdminItem { Id = 2, Nome = "prof_python", Email = "prof.python@tronus.com", Papel = "professor", DataCadastro = "02/02/2026" });
            usuarios.Add(new UsuarioAdminItem { Id = 3, Nome = "prof_bd", Email = "prof.bd@tronus.com", Papel = "professor", DataCadastro = "05/02/2026" });
            usuarios.Add(new UsuarioAdminItem { Id = 4, Nome = "Arthur Pendragon", Email = "arthur@unip.br", Papel = "aluno", DataCadastro = "10/02/2026" });
            usuarios.Add(new UsuarioAdminItem { Id = 5, Nome = "Morgana Le Fay", Email = "morgana@unip.br", Papel = "aluno", DataCadastro = "12/02/2026" });
            usuarios.Add(new UsuarioAdminItem { Id = 6, Nome = "Roland Deschain", Email = "roland@unip.br", Papel = "aluno", DataCadastro = "15/02/2026" });
            usuarios.Add(new UsuarioAdminItem { Id = 7, Nome = "Brenda Soares", Email = "brenda@unip.br", Papel = "aluno", DataCadastro = "20/02/2026" });
            usuarios.Add(new UsuarioAdminItem { Id = 8, Nome = "Lucas Silva", Email = "lucas.silva@unip.br", Papel = "aluno", DataCadastro = "25/02/2026" });
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
                    using var brush = new SolidBrush(Color.FromArgb(165, 15, 12, 10));
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // ═══════════════════════════════════════════════════
            // HUD SUPERIOR DO ADMINISTRADOR
            // ═══════════════════════════════════════════════════
            topHudPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 75,
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
                Location = new Point(25, 14),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblTitulo);

            string admNome = SessionManager.CurrentUser?.Nome ?? "admin";
            lblSubtitulo = new Label
            {
                Text = $"Governança: {admNome} · Módulo de Gestão de Usuários e Métricas · UNIP 2026",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                AutoSize = true,
                Location = new Point(28, 42),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblSubtitulo);

            btnLogout = new TronusButton
            {
                Text = "Sair / Logout",
                Size = new Size(130, 38),
                Location = new Point(860, 18)
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

            // Botão de acessibilidade contextual no HUD do Administrador
            btnTheme = new AccessibilityThemeButton
            {
                Size = new Size(130, 30),
                Location = new Point(715, 22)
            };
            topHudPanel.Controls.Add(btnTheme);

            void ReposAdminButtons()
            {
                if (btnLogout != null && topHudPanel != null)
                {
                    btnLogout.Location = new Point(topHudPanel.Width - btnLogout.Width - 25, 18);
                    if (btnTheme != null)
                    {
                        btnTheme.Location = new Point(btnLogout.Left - btnTheme.Width - 12, 22);
                    }
                }
            }

            topHudPanel.Resize += (s, e) => ReposAdminButtons();
            ReposAdminButtons();

            // ═══════════════════════════════════════════════════
            // CONTAINER PRINCIPAL (SCROLLABLE)
            // ═══════════════════════════════════════════════════
            mainScrollContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(25, 15, 25, 25)
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
                Text = "📊 MÉTRICAS GERAIS DA PLATAFORMA",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 15),
                BackColor = Color.Transparent
            };
            mainScrollContainer.Controls.Add(lblSecaoKpi);

            flowKpis = new FlowLayoutPanel
            {
                Location = new Point(25, 45),
                Size = new Size(960, 95),
                BackColor = Color.Transparent,
                WrapContents = false,
                AutoScroll = false
            };
            mainScrollContainer.Controls.Add(flowKpis);

            flowKpis.Controls.Add(CriarCardKpi("👥 Usuários", "48", "Alunos e Professores"));
            flowKpis.Controls.Add(CriarCardKpi("⚔️ Heróis Ativos", "35", "Personagens forjados"));
            flowKpis.Controls.Add(CriarCardKpi("📜 Questões", "75", "Nos 5 Reinos"));
            flowKpis.Controls.Add(CriarCardKpi("⭐ Estrelas", "412", "Conquistadas"));
            flowKpis.Controls.Add(CriarCardKpi("🎯 Taxa Geral", "86%", "Média de Acertos"));
        }

        private Panel CriarCardKpi(string titulo, string valor, string desc)
        {
            var card = new TronusPanel
            {
                Size = new Size(180, 85),
                Margin = new Padding(0, 0, 12, 0),
                BackColor = Color.Transparent
            };

            var lblT = new Label
            {
                Text = titulo,
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Location = new Point(10, 8),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblT);

            var lblV = new Label
            {
                Text = valor,
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(10, 26),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblV);

            var lblD = new Label
            {
                Text = desc,
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Italic),
                Location = new Point(10, 58),
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
                Text = "👥 GERENCIAMENTO DE USUÁRIOS E PERMISSÕES",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 155),
                BackColor = Color.Transparent
            };
            mainScrollContainer.Controls.Add(lblSecaoUsers);

            var lblBusca = new Label
            {
                Text = "🔍 Filtrar:",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(480, 157),
                BackColor = Color.Transparent
            };
            mainScrollContainer.Controls.Add(lblBusca);

            txtBusca = new TextBox
            {
                Location = new Point(540, 155),
                Size = new Size(260, 26),
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Buscar por nome ou e-mail..."
            };
            txtBusca.TextChanged += (s, e) => FiltrarUsuarios();
            mainScrollContainer.Controls.Add(txtBusca);

            var btnNovo = new TronusButton
            {
                Text = "➕ Novo Usuário",
                Size = new Size(150, 32),
                Location = new Point(835, 152),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnNovo.Click += BtnNovoUsuario_Click;
            mainScrollContainer.Controls.Add(btnNovo);

            cardTabelaUsuarios = new TronusPanel
            {
                Location = new Point(25, 195),
                Size = new Size(960, 430),
                BackColor = Color.Transparent
            };
            mainScrollContainer.Controls.Add(cardTabelaUsuarios);

            // Cabeçalho da Tabela
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = Color.FromArgb(40, 212, 175, 55)
            };
            cardTabelaUsuarios.Controls.Add(header);

            AdicionarColuna(header, "ID", 15, 45);
            AdicionarColuna(header, "NOME", 70, 200);
            AdicionarColuna(header, "E-MAIL", 280, 220);
            AdicionarColuna(header, "PAPEL / PERMISSÃO", 510, 150);
            AdicionarColuna(header, "CADASTRO", 670, 100);
            AdicionarColuna(header, "AÇÕES DE GOVERNANÇA", 780, 170);

            listUsuariosPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent
            };
            cardTabelaUsuarios.Controls.Add(listUsuariosPanel);
            listUsuariosPanel.BringToFront();

            FiltrarUsuarios();
        }

        private void AdicionarColuna(Panel p, string texto, int x, int width)
        {
            var lbl = new Label
            {
                Text = texto,
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(width, 35),
                Location = new Point(x, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            p.Controls.Add(lbl);
        }

        private void FiltrarUsuarios()
        {
            listUsuariosPanel.Controls.Clear();
            string termo = txtBusca?.Text.Trim().ToLower() ?? "";

            var filtrados = usuarios
                .Where(u => string.IsNullOrEmpty(termo) || u.Nome.ToLower().Contains(termo) || u.Email.ToLower().Contains(termo) || u.Papel.ToLower().Contains(termo))
                .ToList();

            int y = 5;
            foreach (var u in filtrados)
            {
                var row = new Panel
                {
                    Location = new Point(5, y),
                    Size = new Size(930, 36),
                    BackColor = (y % 72 == 5) ? Color.FromArgb(20, 255, 255, 255) : Color.Transparent
                };

                row.Controls.Add(CriarLabel(u.Id.ToString(), 10, 45, Theme.Gold, true));
                row.Controls.Add(CriarLabel(u.Nome, 65, 200, Color.White, true));
                row.Controls.Add(CriarLabel(u.Email, 275, 220, Theme.DimText, false));

                // Badge de papel colorido
                Color corPapel = u.Papel switch
                {
                    "admin" => Color.FromArgb(255, 120, 120),
                    "professor" => Theme.Gold,
                    _ => Color.FromArgb(130, 210, 255)
                };
                string papelBadge = u.Papel.ToUpper() switch
                {
                    "ADMIN" => "🛡️ ADMIN",
                    "PROFESSOR" => "🎓 PROFESSOR",
                    _ => "🧙 ALUNO"
                };
                row.Controls.Add(CriarLabel(papelBadge, 505, 150, corPapel, true));

                row.Controls.Add(CriarLabel(u.DataCadastro, 665, 100, Theme.PrimaryText, false));

                // Botão Alterar Papel
                var btnMudarPapel = new Button
                {
                    Text = "Papel ⟳",
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = Theme.Gold,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(68, 24),
                    Location = new Point(775, 6),
                    Cursor = Cursors.Hand
                };
                btnMudarPapel.FlatAppearance.BorderColor = Theme.BorderColor;
                btnMudarPapel.Click += (s, e) =>
                {
                    AlternarPapel(u);
                };
                row.Controls.Add(btnMudarPapel);

                // Botão Excluir
                var btnDel = new Button
                {
                    Text = "Excluir ✕",
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = Color.FromArgb(240, 100, 100),
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(72, 24),
                    Location = new Point(850, 6),
                    Cursor = Cursors.Hand
                };
                btnDel.FlatAppearance.BorderColor = Color.FromArgb(100, 240, 100, 100);
                btnDel.Click += (s, e) =>
                {
                    if (u.Papel == "admin")
                    {
                        MessageBox.Show("Não é permitido excluir a conta mestre de Administrador.", "Operação Bloqueada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    var conf = MessageBox.Show($"Deseja realmente remover o usuário {u.Nome} ({u.Email}) do sistema?", "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (conf == DialogResult.Yes)
                    {
                        usuarios.Remove(u);
                        FiltrarUsuarios();
                    }
                };
                row.Controls.Add(btnDel);

                listUsuariosPanel.Controls.Add(row);
                y += 38;
            }
        }

        private void AlternarPapel(UsuarioAdminItem u)
        {
            string novoPapel = u.Papel switch
            {
                "aluno" => "professor",
                "professor" => "admin",
                _ => "aluno"
            };

            var conf = MessageBox.Show($"Deseja alterar o papel do usuário {u.Nome} de '{u.Papel}' para '{novoPapel}'?", "Governança de Permissões", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (conf == DialogResult.Yes)
            {
                u.Papel = novoPapel;
                FiltrarUsuarios();
                MessageBox.Show($"Papel atualizado com sucesso para '{novoPapel}'!", "Permissão Concedida", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnNovoUsuario_Click(object sender, EventArgs e)
        {
            using var formDialog = new Form
            {
                Text = "Cadastrar Novo Usuário — Governança",
                Size = new Size(420, 360),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Theme.Background,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblT = new Label { Text = "NOVO USUÁRIO NO REINO", ForeColor = Theme.Gold, Font = new Font("Segoe UI", 12F, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            formDialog.Controls.Add(lblT);

            formDialog.Controls.Add(new Label { Text = "Nome Completo:", ForeColor = Theme.PrimaryText, Location = new Point(20, 50), AutoSize = true });
            var txtNome = new TextBox { Location = new Point(20, 70), Size = new Size(360, 26), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.White };
            formDialog.Controls.Add(txtNome);

            formDialog.Controls.Add(new Label { Text = "E-mail:", ForeColor = Theme.PrimaryText, Location = new Point(20, 105), AutoSize = true });
            var txtEmail = new TextBox { Location = new Point(20, 125), Size = new Size(360, 26), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.White };
            formDialog.Controls.Add(txtEmail);

            formDialog.Controls.Add(new Label { Text = "Papel:", ForeColor = Theme.PrimaryText, Location = new Point(20, 160), AutoSize = true });
            var cbP = new ComboBox { Location = new Point(20, 180), Size = new Size(360, 26), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(40, 40, 40), ForeColor = Theme.Gold };
            cbP.Items.AddRange(new object[] { "aluno", "professor", "admin" });
            cbP.SelectedIndex = 0;
            formDialog.Controls.Add(cbP);

            var btnCriar = new TronusButton { Text = "CADASTRAR", Location = new Point(20, 230), Size = new Size(360, 42) };
            btnCriar.Click += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Preencha nome e e-mail.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                usuarios.Add(new UsuarioAdminItem
                {
                    Id = usuarios.Count + 1,
                    Nome = txtNome.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Papel = cbP.SelectedItem?.ToString() ?? "aluno",
                    DataCadastro = DateTime.Now.ToString("dd/MM/yyyy")
                });
                FiltrarUsuarios();
                formDialog.Close();
                MessageBox.Show("Usuário criado com sucesso no sistema!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                Font = new Font("Segoe UI", 9F, bold ? FontStyle.Bold : FontStyle.Regular),
                AutoSize = false,
                Size = new Size(width, 36),
                Location = new Point(x, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
        }

        private void AdjustLayout()
        {
            int w = Math.Max(700, this.ClientSize.Width - 50);

            if (flowKpis != null)
            {
                flowKpis.Width = w;
            }

            if (cardTabelaUsuarios != null)
            {
                cardTabelaUsuarios.Width = w;
                cardTabelaUsuarios.Height = Math.Max(300, this.ClientSize.Height - cardTabelaUsuarios.Top - 30);
            }
        }
    }
}
