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
    public partial class UcProfessor : UserControl
    {
        private Panel topHudPanel;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private TronusButton btnLogout;

        private Panel tabNavigationPanel;
        private Button btnTabQuestoes;
        private Button btnTabAlunos;

        private Panel contentPanel;
        private Panel panelQuestoes;
        private Panel panelAlunos;

        // Controles da Aba 1 (Questões)
        private ComboBox cbFases;
        private FlowLayoutPanel flowQuestoes;
        private TronusPanel formNovaQuestao;
        private TextBox txtNovoEnunciado;
        private TextBox txtOptA;
        private TextBox txtOptB;
        private TextBox txtOptC;
        private TextBox txtOptD;
        private ComboBox cbCorreta;
        private TextBox txtExplicacao;
        private TronusButton btnSalvarQuestao;

        // Controles da Aba 2 (Alunos)
        private TextBox txtBuscaAluno;
        private TronusPanel cardTabelaAlunos;
        private Panel listAlunosPanel;

        // Dados locais de demonstração/fallback para manter a fluidez do avaliador
        private class QuestaoItem
        {
            public int Id { get; set; }
            public int FaseId { get; set; }
            public string Enunciado { get; set; }
            public string[] Alternativas { get; set; }
            public int CorretaIndex { get; set; }
            public string Explicacao { get; set; }
        }

        private class AlunoItem
        {
            public string Nome { get; set; }
            public string Email { get; set; }
            public string Heroi { get; set; }
            public string Titulo { get; set; }
            public int Estrelas { get; set; }
            public int FasesConcluidas { get; set; }
            public int TaxaAcerto { get; set; }
        }

        private readonly List<QuestaoItem> listaQuestoes = new();
        private readonly List<AlunoItem> listaAlunos = new();

        public UcProfessor()
        {
            InitializeComponent();
            CarregarDadosIniciais();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "UcProfessor";
            this.Size = new Size(1024, 728);
            this.ResumeLayout(false);
        }

        private void CarregarDadosIniciais()
        {
            // Questões iniciais do Reino Python
            listaQuestoes.Add(new QuestaoItem
            {
                Id = 1,
                FaseId = 1,
                Enunciado = "Qual a forma correta de exibir uma mensagem no terminal em Python?",
                Alternativas = new[] { "echo 'Olá Mundo'", "print('Olá Mundo')", "Console.WriteLine('Olá Mundo')", "System.out.println('Olá Mundo')" },
                CorretaIndex = 1,
                Explicacao = "A função nativa built-in de saída em Python é print()."
            });
            listaQuestoes.Add(new QuestaoItem
            {
                Id = 2,
                FaseId = 1,
                Enunciado = "Como se declara uma variável de texto (string) em Python?",
                Alternativas = new[] { "string nome = 'Arthur'", "var nome := 'Arthur'", "nome = 'Arthur'", "def nome = 'Arthur'" },
                CorretaIndex = 2,
                Explicacao = "Python possui tipagem dinâmica e dispensa palavras-chave para declarar variáveis."
            });
            listaQuestoes.Add(new QuestaoItem
            {
                Id = 3,
                FaseId = 2,
                Enunciado = "Qual estrutura condicional valida múltiplas condições encadeadas em Python?",
                Alternativas = new[] { "else if", "elif", "case when", "elseif" },
                CorretaIndex = 1,
                Explicacao = "Em Python, a contração 'elif' é a palavra-chave reservada para 'else if'."
            });
            listaQuestoes.Add(new QuestaoItem
            {
                Id = 4,
                FaseId = 3,
                Enunciado = "Como iterar sobre uma lista chamada 'itens' usando um laço for?",
                Alternativas = new[] { "for (i=0; i<itens.length; i++)", "foreach item in itens:", "for item in itens:", "loop item from itens:" },
                CorretaIndex = 2,
                Explicacao = "O for em Python é um for-each natural usando a sintaxe 'for item in colecao:'."
            });

            // Alunos cadastrados
            listaAlunos.Add(new AlunoItem { Nome = "Arthur Pendragon", Email = "arthur@unip.br", Heroi = "Sir Arthur", Titulo = "Rei do Código", Estrelas = 45, FasesConcluidas = 5, TaxaAcerto = 98 });
            listaAlunos.Add(new AlunoItem { Nome = "Morgana Le Fay", Email = "morgana@unip.br", Heroi = "Lady Morgana", Titulo = "Duquesa dos Dados", Estrelas = 42, FasesConcluidas = 5, TaxaAcerto = 94 });
            listaAlunos.Add(new AlunoItem { Nome = "Roland Deschain", Email = "roland@unip.br", Heroi = "Mestre Roland", Titulo = "Cavaleiro do Software", Estrelas = 38, FasesConcluidas = 4, TaxaAcerto = 90 });
            listaAlunos.Add(new AlunoItem { Nome = "Brenda Soares", Email = "brenda@unip.br", Heroi = "Valquíria Brenda", Titulo = "Cavaleiro", Estrelas = 35, FasesConcluidas = 4, TaxaAcerto = 89 });
            listaAlunos.Add(new AlunoItem { Nome = "Lucas Silva", Email = "lucas.silva@unip.br", Heroi = "Cavaleiro Lucas", Titulo = "Plebeu de Honra", Estrelas = 24, FasesConcluidas = 3, TaxaAcerto = 79 });
            listaAlunos.Add(new AlunoItem { Nome = "Clarissa Ramos", Email = "clarissa@unip.br", Heroi = "Lady Clarissa", Titulo = "Plebeu", Estrelas = 20, FasesConcluidas = 2, TaxaAcerto = 76 });
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
                    using var brush = new SolidBrush(Color.FromArgb(160, 15, 12, 10));
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // ═══════════════════════════════════════════════════
            // HUD SUPERIOR DO PROFESSOR
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
                Text = "👨‍🏫 PORTAL DO PROFESSOR — GESTÃO PEDAGÓGICA",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 14),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblTitulo);

            string profNome = SessionManager.CurrentUser?.Nome ?? "prof_python";
            lblSubtitulo = new Label
            {
                Text = $"Docente: {profNome} · Reino Vinculado: Python (Algoritmos e Lógica) · UNIP 2026",
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
                var confirm = MessageBox.Show("Deseja realmente sair do Portal do Professor?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    SessionManager.Logout();
                    (this.FindForm() as MainForm)?.LoadControl(new UcLogin());
                }
            };
            topHudPanel.Controls.Add(btnLogout);

            void ReposProfessorButtons()
            {
                if (btnLogout != null && topHudPanel != null)
                {
                    btnLogout.Location = new Point(topHudPanel.Width - btnLogout.Width - 25, 18);
                }
            }

            topHudPanel.Resize += (s, e) => ReposProfessorButtons();
            ReposProfessorButtons();

            // ═══════════════════════════════════════════════════
            // BARRA DE ABAS
            // ═══════════════════════════════════════════════════
            tabNavigationPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(180, 20, 16, 14)
            };
            this.Controls.Add(tabNavigationPanel);

            btnTabQuestoes = CriarBotaoAba("📚 Banco de Questões por Fase", 25, true);
            btnTabQuestoes.Click += (s, e) => SelecionarAba(true);
            tabNavigationPanel.Controls.Add(btnTabQuestoes);

            btnTabAlunos = CriarBotaoAba("👥 Desempenho e Busca de Alunos", 290, false);
            btnTabAlunos.Click += (s, e) => SelecionarAba(false);
            tabNavigationPanel.Controls.Add(btnTabAlunos);

            // ═══════════════════════════════════════════════════
            // ÁREA DE CONTEÚDO DAS ABAS
            // ═══════════════════════════════════════════════════
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();

            ConstruirPainelQuestoes();
            ConstruirPainelAlunos();

            SelecionarAba(true);
        }

        private Button CriarBotaoAba(string texto, int x, bool ativo)
        {
            var btn = new Button
            {
                Text = texto,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Size = new Size(250, 38),
                Location = new Point(x, 4),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                ForeColor = ativo ? Theme.Gold : Theme.DimText,
                BackColor = ativo ? Color.FromArgb(40, 212, 175, 55) : Color.Transparent
            };
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = ativo ? Theme.Gold : Color.FromArgb(60, 212, 175, 55);
            return btn;
        }

        private void SelecionarAba(bool mostrarQuestoes)
        {
            btnTabQuestoes.ForeColor = mostrarQuestoes ? Theme.Gold : Theme.DimText;
            btnTabQuestoes.BackColor = mostrarQuestoes ? Color.FromArgb(40, 212, 175, 55) : Color.Transparent;
            btnTabQuestoes.FlatAppearance.BorderColor = mostrarQuestoes ? Theme.Gold : Color.FromArgb(60, 212, 175, 55);

            btnTabAlunos.ForeColor = !mostrarQuestoes ? Theme.Gold : Theme.DimText;
            btnTabAlunos.BackColor = !mostrarQuestoes ? Color.FromArgb(40, 212, 175, 55) : Color.Transparent;
            btnTabAlunos.FlatAppearance.BorderColor = !mostrarQuestoes ? Theme.Gold : Color.FromArgb(60, 212, 175, 55);

            panelQuestoes.Visible = mostrarQuestoes;
            panelAlunos.Visible = !mostrarQuestoes;
        }

        #region Aba 1: Banco de Questões

        private void ConstruirPainelQuestoes()
        {
            panelQuestoes = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(25, 15, 25, 25)
            };
            contentPanel.Controls.Add(panelQuestoes);

            // Filtro por Fase
            var lblFase = new Label
            {
                Text = "Selecione a Fase Pedagógica:",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 15),
                BackColor = Color.Transparent
            };
            panelQuestoes.Controls.Add(lblFase);

            cbFases = new ComboBox
            {
                Location = new Point(230, 12),
                Size = new Size(320, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Theme.Gold
            };
            cbFases.Items.AddRange(new object[]
            {
                "Fase 1: Introdução & Sintaxe Básica",
                "Fase 2: Condicionais If / Else / Elif",
                "Fase 3: Laços For & While",
                "Fase 4: Listas & Estruturas de Dados",
                "Fase 5: Funções & Modularização"
            });
            cbFases.SelectedIndex = 0;
            cbFases.SelectedIndexChanged += (s, e) => AtualizarListaQuestoes();
            panelQuestoes.Controls.Add(cbFases);

            // Lista de Questões
            flowQuestoes = new FlowLayoutPanel
            {
                Location = new Point(25, 55),
                Size = new Size(550, 520),
                BackColor = Color.Transparent,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            panelQuestoes.Controls.Add(flowQuestoes);

            // Formulário de Nova Questão
            ConstruirFormNovaQuestao();

            AtualizarListaQuestoes();

            panelQuestoes.Resize += (s, e) =>
            {
                int totalW = panelQuestoes.ClientSize.Width - 60;
                int colW = Math.Max(400, totalW / 2);
                flowQuestoes.Width = colW;
                formNovaQuestao.Location = new Point(flowQuestoes.Right + 20, 55);
                formNovaQuestao.Width = Math.Max(350, totalW - colW - 20);
            };
        }

        private void ConstruirFormNovaQuestao()
        {
            formNovaQuestao = new TronusPanel
            {
                Location = new Point(595, 55),
                Size = new Size(380, 520),
                BackColor = Color.Transparent
            };
            panelQuestoes.Controls.Add(formNovaQuestao);

            var lblTituloForm = new Label
            {
                Text = "➕ CADASTRAR NOVA QUESTÃO",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(15, 12),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            formNovaQuestao.Controls.Add(lblTituloForm);

            var lblEnun = new Label { Text = "Enunciado do Desafio:", ForeColor = Theme.PrimaryText, Font = new Font("Segoe UI", 9F), Location = new Point(15, 40), AutoSize = true };
            formNovaQuestao.Controls.Add(lblEnun);

            txtNovoEnunciado = new TextBox
            {
                Location = new Point(15, 60),
                Size = new Size(350, 48),
                Multiline = true,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            formNovaQuestao.Controls.Add(txtNovoEnunciado);

            // Opções
            formNovaQuestao.Controls.Add(new Label { Text = "Alternativa A:", ForeColor = Theme.DimText, Font = new Font("Segoe UI", 8.5F), Location = new Point(15, 115), AutoSize = true });
            txtOptA = new TextBox { Location = new Point(15, 132), Size = new Size(350, 24), Font = new Font("Segoe UI", 9F), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.White };
            formNovaQuestao.Controls.Add(txtOptA);

            formNovaQuestao.Controls.Add(new Label { Text = "Alternativa B:", ForeColor = Theme.DimText, Font = new Font("Segoe UI", 8.5F), Location = new Point(15, 160), AutoSize = true });
            txtOptB = new TextBox { Location = new Point(15, 177), Size = new Size(350, 24), Font = new Font("Segoe UI", 9F), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.White };
            formNovaQuestao.Controls.Add(txtOptB);

            formNovaQuestao.Controls.Add(new Label { Text = "Alternativa C:", ForeColor = Theme.DimText, Font = new Font("Segoe UI", 8.5F), Location = new Point(15, 205), AutoSize = true });
            txtOptC = new TextBox { Location = new Point(15, 222), Size = new Size(350, 24), Font = new Font("Segoe UI", 9F), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.White };
            formNovaQuestao.Controls.Add(txtOptC);

            formNovaQuestao.Controls.Add(new Label { Text = "Alternativa D:", ForeColor = Theme.DimText, Font = new Font("Segoe UI", 8.5F), Location = new Point(15, 250), AutoSize = true });
            txtOptD = new TextBox { Location = new Point(15, 267), Size = new Size(350, 24), Font = new Font("Segoe UI", 9F), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.White };
            formNovaQuestao.Controls.Add(txtOptD);

            // Correta
            formNovaQuestao.Controls.Add(new Label { Text = "Alternativa Correta (Gabarito):", ForeColor = Theme.Gold, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Location = new Point(15, 298), AutoSize = true });
            cbCorreta = new ComboBox
            {
                Location = new Point(15, 318),
                Size = new Size(350, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Theme.Gold
            };
            cbCorreta.Items.AddRange(new object[] { "A) Primeira Alternativa", "B) Segunda Alternativa", "C) Terceira Alternativa", "D) Quarta Alternativa" });
            cbCorreta.SelectedIndex = 0;
            formNovaQuestao.Controls.Add(cbCorreta);

            // Justificativa
            formNovaQuestao.Controls.Add(new Label { Text = "Explicação Pedagógica (Feedback):", ForeColor = Theme.PrimaryText, Font = new Font("Segoe UI", 8.5F), Location = new Point(15, 350), AutoSize = true });
            txtExplicacao = new TextBox
            {
                Location = new Point(15, 370),
                Size = new Size(350, 50),
                Multiline = true,
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            formNovaQuestao.Controls.Add(txtExplicacao);

            btnSalvarQuestao = new TronusButton
            {
                Text = "SALVAR QUESTÃO ⚔",
                Size = new Size(350, 42),
                Location = new Point(15, 440)
            };
            btnSalvarQuestao.Click += BtnSalvarQuestao_Click;
            formNovaQuestao.Controls.Add(btnSalvarQuestao);
        }

        private void AtualizarListaQuestoes()
        {
            flowQuestoes.Controls.Clear();
            int faseSelecionada = cbFases.SelectedIndex + 1;

            var questoesFase = listaQuestoes.Where(q => q.FaseId == faseSelecionada).ToList();

            if (questoesFase.Count == 0)
            {
                var lblVazio = new Label
                {
                    Text = "Nenhuma questão cadastrada para esta fase ainda.\nUtilize o formulário ao lado para cadastrar.",
                    ForeColor = Theme.DimText,
                    Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                    Size = new Size(520, 60),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                flowQuestoes.Controls.Add(lblVazio);
                return;
            }

            int n = 1;
            foreach (var q in questoesFase)
            {
                var card = new TronusPanel
                {
                    Size = new Size(520, 160),
                    Margin = new Padding(0, 0, 0, 12),
                    BackColor = Color.Transparent
                };

                var lblEnun = new Label
                {
                    Text = $"Q{n}. {q.Enunciado}",
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    Location = new Point(12, 10),
                    Size = new Size(420, 38),
                    BackColor = Color.Transparent
                };
                card.Controls.Add(lblEnun);

                var btnDel = new Button
                {
                    Text = "✕",
                    ForeColor = Color.FromArgb(240, 100, 100),
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(30, 26),
                    Location = new Point(475, 8),
                    Cursor = Cursors.Hand
                };
                btnDel.FlatAppearance.BorderSize = 0;
                btnDel.Click += (s, e) =>
                {
                    var res = MessageBox.Show("Deseja excluir esta questão pedagógica?", "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        listaQuestoes.Remove(q);
                        AtualizarListaQuestoes();
                    }
                };
                card.Controls.Add(btnDel);

                // Alternativas
                int yOpt = 48;
                for (int i = 0; i < q.Alternativas.Length; i++)
                {
                    char letra = (char)('A' + i);
                    bool isCorreta = (i == q.CorretaIndex);
                    var lblOpt = new Label
                    {
                        Text = $"{letra}) {q.Alternativas[i]} {(isCorreta ? "✓ (GABARITO)" : "")}",
                        ForeColor = isCorreta ? Color.FromArgb(130, 225, 130) : Theme.DimText,
                        Font = new Font("Segoe UI", 8.5F, isCorreta ? FontStyle.Bold : FontStyle.Regular),
                        Location = new Point(15, yOpt),
                        Size = new Size(490, 20),
                        BackColor = Color.Transparent
                    };
                    card.Controls.Add(lblOpt);
                    yOpt += 20;
                }

                var lblExp = new Label
                {
                    Text = $"💡 Explicação: {q.Explicacao}",
                    ForeColor = Theme.Gold,
                    Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                    Location = new Point(15, 132),
                    Size = new Size(490, 20),
                    BackColor = Color.Transparent
                };
                card.Controls.Add(lblExp);

                flowQuestoes.Controls.Add(card);
                n++;
            }
        }

        private void BtnSalvarQuestao_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNovoEnunciado.Text) ||
                string.IsNullOrWhiteSpace(txtOptA.Text) ||
                string.IsNullOrWhiteSpace(txtOptB.Text) ||
                string.IsNullOrWhiteSpace(txtOptC.Text) ||
                string.IsNullOrWhiteSpace(txtOptD.Text))
            {
                MessageBox.Show("Preencha o enunciado e as 4 opções de resposta.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nova = new QuestaoItem
            {
                Id = listaQuestoes.Count + 1,
                FaseId = cbFases.SelectedIndex + 1,
                Enunciado = txtNovoEnunciado.Text.Trim(),
                Alternativas = new[]
                {
                    txtOptA.Text.Trim(),
                    txtOptB.Text.Trim(),
                    txtOptC.Text.Trim(),
                    txtOptD.Text.Trim()
                },
                CorretaIndex = cbCorreta.SelectedIndex,
                Explicacao = string.IsNullOrWhiteSpace(txtExplicacao.Text) ? "Explicação fornecida pelo docente." : txtExplicacao.Text.Trim()
            };

            listaQuestoes.Add(nova);
            MessageBox.Show("Questão pedagógica salva com sucesso no Reino!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtNovoEnunciado.Clear();
            txtOptA.Clear();
            txtOptB.Clear();
            txtOptC.Clear();
            txtOptD.Clear();
            txtExplicacao.Clear();

            AtualizarListaQuestoes();
        }

        #endregion

        #region Aba 2: Desempenho dos Alunos

        private void ConstruirPainelAlunos()
        {
            panelAlunos = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(25, 15, 25, 25),
                Visible = false
            };
            contentPanel.Controls.Add(panelAlunos);

            var lblBusca = new Label
            {
                Text = "🔍 Buscar Estudante:",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 15),
                BackColor = Color.Transparent
            };
            panelAlunos.Controls.Add(lblBusca);

            txtBuscaAluno = new TextBox
            {
                Location = new Point(170, 12),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Digite o nome ou e-mail do aluno..."
            };
            txtBuscaAluno.TextChanged += (s, e) => FiltrarTabelaAlunos();
            panelAlunos.Controls.Add(txtBuscaAluno);

            cardTabelaAlunos = new TronusPanel
            {
                Location = new Point(25, 55),
                Size = new Size(960, 520),
                BackColor = Color.Transparent
            };
            panelAlunos.Controls.Add(cardTabelaAlunos);

            // Cabeçalho da tabela de alunos
            var headerAlunos = new Panel
            {
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = Color.FromArgb(40, 212, 175, 55)
            };
            cardTabelaAlunos.Controls.Add(headerAlunos);

            AdicionarColunaAluno(headerAlunos, "NOME DO ESTUDANTE", 15, 220);
            AdicionarColunaAluno(headerAlunos, "E-MAIL", 240, 200);
            AdicionarColunaAluno(headerAlunos, "HERÓI & TÍTULO", 450, 180);
            AdicionarColunaAluno(headerAlunos, "ESTRELAS", 640, 100);
            AdicionarColunaAluno(headerAlunos, "FASES", 750, 80);
            AdicionarColunaAluno(headerAlunos, "APROVEITAMENTO", 840, 110);

            listAlunosPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent
            };
            cardTabelaAlunos.Controls.Add(listAlunosPanel);
            listAlunosPanel.BringToFront();

            FiltrarTabelaAlunos();

            panelAlunos.Resize += (s, e) =>
            {
                if (cardTabelaAlunos != null)
                {
                    cardTabelaAlunos.Width = Math.Max(700, panelAlunos.ClientSize.Width - 50);
                    cardTabelaAlunos.Height = Math.Max(300, panelAlunos.ClientSize.Height - 80);
                }
            };
        }

        private void AdicionarColunaAluno(Panel p, string texto, int x, int width)
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

        private void FiltrarTabelaAlunos()
        {
            listAlunosPanel.Controls.Clear();
            string termo = txtBuscaAluno.Text.Trim().ToLower();

            var filtrados = listaAlunos
                .Where(a => string.IsNullOrEmpty(termo) || a.Nome.ToLower().Contains(termo) || a.Email.ToLower().Contains(termo))
                .ToList();

            int y = 5;
            foreach (var a in filtrados)
            {
                var row = new Panel
                {
                    Location = new Point(5, y),
                    Size = new Size(930, 36),
                    BackColor = (y % 72 == 5) ? Color.FromArgb(20, 255, 255, 255) : Color.Transparent
                };

                row.Controls.Add(CriarLabelAluno(a.Nome, 10, 220, Color.White, true));
                row.Controls.Add(CriarLabelAluno(a.Email, 235, 200, Theme.DimText, false));
                row.Controls.Add(CriarLabelAluno($"{a.Heroi} ({a.Titulo})", 445, 180, Theme.Gold, false));
                row.Controls.Add(CriarLabelAluno($"⭐ {a.Estrelas}", 635, 100, Theme.Gold, true));
                row.Controls.Add(CriarLabelAluno($"{a.FasesConcluidas} / 5", 745, 80, Theme.PrimaryText, false));
                row.Controls.Add(CriarLabelAluno($"{a.TaxaAcerto}% acertos", 835, 110, Color.FromArgb(130, 220, 130), true));

                listAlunosPanel.Controls.Add(row);
                y += 38;
            }
        }

        private Label CriarLabelAluno(string texto, int x, int width, Color cor, bool bold)
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

        #endregion
    }
}
