using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TronusDesktop.Models;
using TronusDesktop.Services;
using TronusDesktop.UI;

namespace TronusDesktop.Views
{
    public partial class UcPerfilAluno : UserControl
    {
        private PersonagemDto _heroiAtual;

        // HUD Superior
        private Panel topHudPanel;
        private PictureBox pbHudAvatar;
        private Label lblTituloHud;
        private Label lblSubtituloHud;
        private TronusButton btnVoltarMapa;

        // Card Esquerdo: Dados Cadastrais & Identidade do Herói
        private TronusPanel cardPerfil;
        private TextBox txtNomeAluno;
        private TextBox txtEmailAluno;
        private TextBox txtCelularAluno;
        private TextBox txtNomeHeroi;
        private Label lblPatenteBadge;
        private Label lblFeedbackPerfil;
        private TronusButton btnSalvarPerfil;
        private readonly List<PictureBox> _avatarBoxes = new();
        private string _selectedAvatarRel = @"img\masc1.png";
        private string _selectedGenero = "M";

        // Card Direito: Alteração de Senha & Segurança
        private TronusPanel cardSenha;
        private TextBox txtSenhaAtual;
        private PasswordToggleButton btnToggleAtual;
        private TextBox txtNovaSenha;
        private PasswordToggleButton btnToggleNova;
        private TextBox txtConfirmarSenha;
        private PasswordToggleButton btnToggleConfirmar;
        private Panel pnlForcaBg;
        private Panel pnlForcaFill;
        private Label lblForcaTexto;
        private Label lblRegraMinimo;
        private Label lblRegraAlfanum;
        private Label lblRegraCoincide;
        private Label lblFeedbackSenha;
        private TronusButton btnAlterarSenha;

        private readonly (string FileName, string Genero, string Label)[] _avataresDisponiveis = new[]
        {
            ("masc1.png", "M", "M1"),
            ("masc2.png", "M", "M2"),
            ("masc3.png", "M", "M3"),
            ("fem1.png",  "F", "F1"),
            ("fem2.png",  "F", "F2"),
            ("fem3.png",  "F", "F3")
        };

        public UcPerfilAluno(PersonagemDto heroiAtual = null)
        {
            _heroiAtual = heroiAtual ?? SessionManager.CachedHero ?? OfflineDataService.CurrentHero;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            SetupUI();
            CarregarDadosAtuais();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "UcPerfilAluno";
            this.Size = new Size(1024, 728);
            this.ResumeLayout(false);
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
                    using var brush = new SolidBrush(Color.FromArgb(150, 15, 12, 10));
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // ═══════════════════════════════════════════════════
            // 1. HUD SUPERIOR
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

            pbHudAvatar = new PictureBox
            {
                Location = new Point(22, 12),
                Size = new Size(50, 50),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(35, 30, 25)
            };
            pbHudAvatar.Paint += (s, e) =>
            {
                using var pen = new Pen(Theme.Gold, 1.5f);
                e.Graphics.DrawRectangle(pen, 0, 0, pbHudAvatar.Width - 1, pbHudAvatar.Height - 1);
            };
            topHudPanel.Controls.Add(pbHudAvatar);

            lblTituloHud = new Label
            {
                Text = "👤 MEU PERFIL & SEGURANÇA DA CONTA",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(84, 13),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblTituloHud);

            lblSubtituloHud = new Label
            {
                Text = "Atualize seus dados cadastrais, personalize seu Herói e altere sua senha de acesso",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                AutoSize = true,
                Location = new Point(86, 41),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblSubtituloHud);

            btnVoltarMapa = new TronusButton
            {
                Text = "← Voltar ao Mapa",
                Size = new Size(165, 38),
                Location = new Point(834, 18),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnVoltarMapa.Click += (s, e) => (this.FindForm() as MainForm)?.LoadControl(new UcReinos());
            topHudPanel.Controls.Add(btnVoltarMapa);

            // ═══════════════════════════════════════════════════
            // 2. CARD ESQUERDO: DADOS CADASTRAIS & HERÓI
            // ═══════════════════════════════════════════════════
            cardPerfil = new TronusPanel
            {
                Location = new Point(22, 92),
                Size = new Size(478, 610),
                BackColor = Color.Transparent
            };
            this.Controls.Add(cardPerfil);

            var lblTituloPerfil = new Label
            {
                Text = "🛡️ DADOS DO ALUNO & IDENTIDADE DO HERÓI",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, 18),
                BackColor = Color.Transparent
            };
            cardPerfil.Controls.Add(lblTituloPerfil);

            var lblDescPerfil = new Label
            {
                Text = "Ajuste suas informações pessoais e a aparência do seu boneco no Reino.",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 8.8F),
                AutoSize = true,
                Location = new Point(22, 42),
                BackColor = Color.Transparent
            };
            cardPerfil.Controls.Add(lblDescPerfil);

            // Campo 1: Nome Completo
            CriarLabelCampo(cardPerfil, "Nome Completo do Estudante:", 22, 74);
            txtNomeAluno = CriarTextBox(cardPerfil, 22, 96, 434);

            // Campo 2: E-mail Acadêmico
            CriarLabelCampo(cardPerfil, "E-mail Acadêmico:", 22, 138);
            txtEmailAluno = CriarTextBox(cardPerfil, 22, 160, 255);

            // Campo 3: Celular / WhatsApp
            CriarLabelCampo(cardPerfil, "Celular (Recuperação):", 292, 138);
            txtCelularAluno = CriarTextBox(cardPerfil, 292, 160, 164);

            // Campo 4: Nome do Herói
            CriarLabelCampo(cardPerfil, "Nome do Herói (Exibido no Mapa e Ranking):", 22, 202);
            txtNomeHeroi = CriarTextBox(cardPerfil, 22, 224, 434);

            // Badge de Patente e Estrelas
            lblPatenteBadge = new Label
            {
                Text = "🦅 Patente Atual: Duque  ·  ⭐ 38 estrelas conquistadas",
                ForeColor = Theme.Gold,
                BackColor = Color.FromArgb(40, 212, 175, 55),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.2F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(434, 30),
                Location = new Point(22, 266)
            };
            cardPerfil.Controls.Add(lblPatenteBadge);

            // Seletor de Avatar (Boneco do Usuário)
            CriarLabelCampo(cardPerfil, "Escolha o Boneco / Avatar do seu Herói (clique para selecionar):", 22, 310);

            int startX = 22;
            for (int i = 0; i < _avataresDisponiveis.Length; i++)
            {
                var item = _avataresDisponiveis[i];
                string relPath = Path.Combine("img", item.FileName);
                string fullPath = Path.Combine(Application.StartupPath, relPath);

                var pb = new PictureBox
                {
                    Location = new Point(startX + (i * 73), 336),
                    Size = new Size(66, 66),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.FromArgb(30, 26, 22),
                    Cursor = Cursors.Hand,
                    Tag = (relPath, item.Genero)
                };

                if (File.Exists(fullPath))
                {
                    pb.Image = Image.FromFile(fullPath);
                }

                pb.Paint += AvatarBox_Paint;
                pb.Click += (s, e) =>
                {
                    _selectedAvatarRel = relPath;
                    _selectedGenero = item.Genero;
                    AtualizarSelecaoAvatarVisual();
                };

                _avatarBoxes.Add(pb);
                cardPerfil.Controls.Add(pb);

                var lblTag = new Label
                {
                    Text = item.Genero == "M" ? $"Herói {i + 1}" : $"Heroína {i - 2}",
                    ForeColor = Theme.DimText,
                    Font = new Font("Segoe UI", 7.5F),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(66, 16),
                    Location = new Point(startX + (i * 73), 405),
                    BackColor = Color.Transparent
                };
                cardPerfil.Controls.Add(lblTag);
            }

            lblFeedbackPerfil = new Label
            {
                Text = "",
                ForeColor = Color.FromArgb(90, 220, 130),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(434, 36),
                Location = new Point(22, 492),
                BackColor = Color.Transparent
            };
            cardPerfil.Controls.Add(lblFeedbackPerfil);

            btnSalvarPerfil = new TronusButton
            {
                Text = "💾 SALVAR DADOS E AVATAR",
                Size = new Size(434, 44),
                Location = new Point(22, 538),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold)
            };
            btnSalvarPerfil.Click += BtnSalvarPerfil_Click;
            cardPerfil.Controls.Add(btnSalvarPerfil);

            // ═══════════════════════════════════════════════════
            // 3. CARD DIREITO: SEGURANÇA & ALTERAÇÃO DE SENHA
            // ═══════════════════════════════════════════════════
            cardSenha = new TronusPanel
            {
                Location = new Point(522, 92),
                Size = new Size(478, 610),
                BackColor = Color.Transparent
            };
            this.Controls.Add(cardSenha);

            var lblTituloSenha = new Label
            {
                Text = "🔐 SEGURANÇA & ALTERAÇÃO DE SENHA",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, 18),
                BackColor = Color.Transparent
            };
            cardSenha.Controls.Add(lblTituloSenha);

            var lblDescSenha = new Label
            {
                Text = "Mantenha sua conta protegida atualizando periodicamente sua senha.",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 8.8F),
                AutoSize = true,
                Location = new Point(22, 42),
                BackColor = Color.Transparent
            };
            cardSenha.Controls.Add(lblDescSenha);

            // Painel de resumo de segurança
            var pnlInfoSeguranca = new Panel
            {
                Location = new Point(22, 74),
                Size = new Size(434, 62),
                BackColor = Color.FromArgb(32, 28, 22),
                BorderStyle = BorderStyle.FixedSingle
            };
            var lblInfoSegTitulo = new Label
            {
                Text = "🛡️ Conta Acadêmica Verificada · Proteção JWT & Hash BCrypt",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10),
                BackColor = Color.Transparent
            };
            var lblInfoSegSub = new Label
            {
                Text = "Ao alterar sua senha, o novo código passará a valer imediatamente.",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 8.5F),
                AutoSize = true,
                Location = new Point(12, 33),
                BackColor = Color.Transparent
            };
            pnlInfoSeguranca.Controls.Add(lblInfoSegTitulo);
            pnlInfoSeguranca.Controls.Add(lblInfoSegSub);
            cardSenha.Controls.Add(pnlInfoSeguranca);

            // Senha Atual
            CriarLabelCampo(cardSenha, "Senha Atual:", 22, 154);
            txtSenhaAtual = CriarTextBox(cardSenha, 22, 176, 392, isPassword: true);
            btnToggleAtual = new PasswordToggleButton(txtSenhaAtual)
            {
                Size = new Size(38, 29),
                Location = new Point(418, 176)
            };
            cardSenha.Controls.Add(btnToggleAtual);

            // Nova Senha
            CriarLabelCampo(cardSenha, "Nova Senha:", 22, 222);
            txtNovaSenha = CriarTextBox(cardSenha, 22, 244, 392, isPassword: true);
            txtNovaSenha.TextChanged += (s, e) => AtualizarIndicadoresSenha();
            btnToggleNova = new PasswordToggleButton(txtNovaSenha)
            {
                Size = new Size(38, 29),
                Location = new Point(418, 244)
            };
            cardSenha.Controls.Add(btnToggleNova);

            // Barra de força da senha
            pnlForcaBg = new Panel
            {
                Location = new Point(22, 282),
                Size = new Size(300, 7),
                BackColor = Color.FromArgb(45, 45, 45)
            };
            cardSenha.Controls.Add(pnlForcaBg);

            pnlForcaFill = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(0, 7),
                BackColor = Theme.Gold
            };
            pnlForcaBg.Controls.Add(pnlForcaFill);

            lblForcaTexto = new Label
            {
                Text = "Força: Digite a nova senha",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(330, 277),
                BackColor = Color.Transparent
            };
            cardSenha.Controls.Add(lblForcaTexto);

            // Confirmar Nova Senha
            CriarLabelCampo(cardSenha, "Confirmar Nova Senha:", 22, 304);
            txtConfirmarSenha = CriarTextBox(cardSenha, 22, 326, 392, isPassword: true);
            txtConfirmarSenha.TextChanged += (s, e) => AtualizarIndicadoresSenha();
            btnToggleConfirmar = new PasswordToggleButton(txtConfirmarSenha)
            {
                Size = new Size(38, 29),
                Location = new Point(418, 326)
            };
            cardSenha.Controls.Add(btnToggleConfirmar);

            // Checklist de requisitos de senha
            var pnlRegras = new Panel
            {
                Location = new Point(22, 372),
                Size = new Size(434, 88),
                BackColor = Color.FromArgb(26, 22, 18),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblRegraMinimo = new Label
            {
                Text = "○ Possuir pelo menos 6 caracteres",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 8.8F),
                AutoSize = true,
                Location = new Point(12, 10),
                BackColor = Color.Transparent
            };
            lblRegraAlfanum = new Label
            {
                Text = "○ Combinar letras e números (recomendado)",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 8.8F),
                AutoSize = true,
                Location = new Point(12, 34),
                BackColor = Color.Transparent
            };
            lblRegraCoincide = new Label
            {
                Text = "○ Confirmação de senha idêntica à nova senha",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 8.8F),
                AutoSize = true,
                Location = new Point(12, 58),
                BackColor = Color.Transparent
            };
            pnlRegras.Controls.Add(lblRegraMinimo);
            pnlRegras.Controls.Add(lblRegraAlfanum);
            pnlRegras.Controls.Add(lblRegraCoincide);
            cardSenha.Controls.Add(pnlRegras);

            lblFeedbackSenha = new Label
            {
                Text = "",
                ForeColor = Color.FromArgb(90, 220, 130),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(434, 36),
                Location = new Point(22, 492),
                BackColor = Color.Transparent
            };
            cardSenha.Controls.Add(lblFeedbackSenha);

            btnAlterarSenha = new TronusButton
            {
                Text = "🔑 ATUALIZAR MINHA SENHA",
                Size = new Size(434, 44),
                Location = new Point(22, 538),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold)
            };
            btnAlterarSenha.Click += BtnAlterarSenha_Click;
            cardSenha.Controls.Add(btnAlterarSenha);

            this.Resize += (s, e) => AjustarLayoutResponsivo();
            AjustarLayoutResponsivo();
        }

        private void CriarLabelCampo(Control parent, string texto, int x, int y)
        {
            var lbl = new Label
            {
                Text = texto,
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 9.2F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(x, y),
                BackColor = Color.Transparent
            };
            parent.Controls.Add(lbl);
        }

        private TextBox CriarTextBox(Control parent, int x, int y, int width, bool isPassword = false)
        {
            var txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 29),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(38, 34, 28),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            if (isPassword)
            {
                txt.PasswordChar = '•';
            }
            parent.Controls.Add(txt);
            return txt;
        }

        private void AvatarBox_Paint(object sender, PaintEventArgs e)
        {
            if (sender is not PictureBox pb || pb.Tag is not ValueTuple<string, string> tag) return;

            string normSelected = (_selectedAvatarRel ?? "").Replace("/", "\\").ToLowerInvariant();
            string normBox = tag.Item1.Replace("/", "\\").ToLowerInvariant();
            bool selecionado = normSelected.EndsWith(Path.GetFileName(normBox));

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(selecionado ? Theme.Gold : Color.FromArgb(90, 212, 175, 55), selecionado ? 3f : 1f);
            e.Graphics.DrawRectangle(pen, 1, 1, pb.Width - 3, pb.Height - 3);
        }

        private void CarregarDadosAtuais()
        {
            var user = SessionManager.CurrentUser ?? OfflineDataService.CurrentUser ?? new UsuarioDto
            {
                Id = 101,
                Nome = "Arthur Pendragon",
                Email = "arthur.demo@tronus.unip.br",
                Celular = "(11) 98765-4321",
                Papel = "aluno"
            };

            var heroi = _heroiAtual ?? SessionManager.CachedHero ?? OfflineDataService.CurrentHero ?? new PersonagemDto
            {
                Id = 1,
                Nome = "Sir Arthur",
                Genero = "M",
                Avatar = @"img\masc1.png",
                TituloAtual = "Duque",
                TotalEstrelas = 38
            };

            txtNomeAluno.Text = user.Nome;
            txtEmailAluno.Text = user.Email;
            txtCelularAluno.Text = !string.IsNullOrWhiteSpace(user.Celular) ? user.Celular : "(11) 98765-4321";
            txtNomeHeroi.Text = heroi.Nome;

            _selectedAvatarRel = !string.IsNullOrWhiteSpace(heroi.Avatar) ? heroi.Avatar : @"img\masc1.png";
            _selectedGenero = !string.IsNullOrWhiteSpace(heroi.Genero) ? heroi.Genero : "M";

            string badgeIcon = heroi.TituloAtual switch
            {
                "Rei" => "👑",
                "Duque" => "🦅",
                "Cavaleiro" => "🎖️",
                _ => "🛡️"
            };
            lblPatenteBadge.Text = $"{badgeIcon} Patente Atual: {heroi.TituloAtual}   ·   ⭐ {heroi.TotalEstrelas} estrelas conquistadas";

            AtualizarSelecaoAvatarVisual();
        }

        private void AtualizarSelecaoAvatarVisual()
        {
            string avatarFullPath = Path.Combine(Application.StartupPath, _selectedAvatarRel.Replace("/", "\\"));
            if (File.Exists(avatarFullPath))
            {
                pbHudAvatar.Image = Image.FromFile(avatarFullPath);
            }

            foreach (var pb in _avatarBoxes)
            {
                pb.Invalidate();
            }
        }

        private void AtualizarIndicadoresSenha()
        {
            string nova = txtNovaSenha.Text ?? "";
            string conf = txtConfirmarSenha.Text ?? "";

            bool temMinimo = nova.Length >= 6;
            bool temAlfanum = nova.Any(char.IsLetter) && nova.Any(char.IsDigit);
            bool temSimboloOuMaiusc = nova.Any(char.IsUpper) || nova.Any(ch => !char.IsLetterOrDigit(ch));
            bool coincide = !string.IsNullOrEmpty(nova) && nova == conf;

            lblRegraMinimo.Text = (temMinimo ? "✔ " : "○ ") + "Possuir pelo menos 6 caracteres";
            lblRegraMinimo.ForeColor = temMinimo ? Color.FromArgb(90, 220, 130) : Theme.DimText;

            lblRegraAlfanum.Text = (temAlfanum ? "✔ " : "○ ") + "Combinar letras e números (recomendado)";
            lblRegraAlfanum.ForeColor = temAlfanum ? Color.FromArgb(90, 220, 130) : Theme.DimText;

            lblRegraCoincide.Text = (coincide ? "✔ " : "○ ") + "Confirmação de senha idêntica à nova senha";
            lblRegraCoincide.ForeColor = coincide ? Color.FromArgb(90, 220, 130) : Theme.DimText;

            int pontos = 0;
            if (nova.Length >= 6) pontos++;
            if (nova.Length >= 8) pontos++;
            if (temAlfanum) pontos++;
            if (temSimboloOuMaiusc) pontos++;

            if (string.IsNullOrEmpty(nova))
            {
                pnlForcaFill.Width = 0;
                lblForcaTexto.Text = "Força: Digite a nova senha";
                lblForcaTexto.ForeColor = Theme.DimText;
            }
            else if (pontos <= 1)
            {
                pnlForcaFill.Width = 75;
                pnlForcaFill.BackColor = Color.FromArgb(220, 80, 80);
                lblForcaTexto.Text = "Força: Fraca";
                lblForcaTexto.ForeColor = Color.FromArgb(220, 80, 80);
            }
            else if (pontos == 2)
            {
                pnlForcaFill.Width = 155;
                pnlForcaFill.BackColor = Color.FromArgb(235, 175, 55);
                lblForcaTexto.Text = "Força: Média";
                lblForcaTexto.ForeColor = Color.FromArgb(235, 175, 55);
            }
            else if (pontos == 3)
            {
                pnlForcaFill.Width = 230;
                pnlForcaFill.BackColor = Color.FromArgb(90, 210, 130);
                lblForcaTexto.Text = "Força: Forte";
                lblForcaTexto.ForeColor = Color.FromArgb(90, 210, 130);
            }
            else
            {
                pnlForcaFill.Width = 300;
                pnlForcaFill.BackColor = Color.FromArgb(60, 235, 150);
                lblForcaTexto.Text = "Força: Excelente";
                lblForcaTexto.ForeColor = Color.FromArgb(60, 235, 150);
            }
        }

        private void BtnSalvarPerfil_Click(object sender, EventArgs e)
        {
            string nomeAluno = txtNomeAluno.Text?.Trim() ?? "";
            string emailAluno = txtEmailAluno.Text?.Trim() ?? "";
            string celularAluno = txtCelularAluno.Text?.Trim() ?? "";
            string nomeHeroi = txtNomeHeroi.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(nomeAluno) || string.IsNullOrWhiteSpace(emailAluno) || string.IsNullOrWhiteSpace(nomeHeroi))
            {
                lblFeedbackPerfil.ForeColor = Color.FromArgb(240, 95, 95);
                lblFeedbackPerfil.Text = "⚠ Preencha Nome Completo, E-mail e Nome do Herói.";
                return;
            }

            if (!emailAluno.Contains("@") || !emailAluno.Contains("."))
            {
                lblFeedbackPerfil.ForeColor = Color.FromArgb(240, 95, 95);
                lblFeedbackPerfil.Text = "⚠ Informe um endereço de e-mail válido.";
                return;
            }

            var req = new AtualizarPerfilRequest
            {
                Nome = nomeAluno,
                Email = emailAluno,
                Celular = celularAluno,
                NomeHeroi = nomeHeroi,
                Genero = _selectedGenero,
                Avatar = _selectedAvatarRel.Replace("\\", "/")
            };

            _heroiAtual = OfflineDataService.AtualizarPerfilCompleto(req);

            lblFeedbackPerfil.ForeColor = Color.FromArgb(90, 220, 130);
            lblFeedbackPerfil.Text = "✔ Dados cadastrais e avatar do Herói salvos com sucesso!";

            MessageBox.Show(
                $"Seus dados foram atualizados com sucesso!\n\n" +
                $"• Estudante: {nomeAluno}\n" +
                $"• E-mail: {emailAluno}\n" +
                $"• Herói: {_heroiAtual.Nome} ({_heroiAtual.TituloAtual})",
                "Perfil Atualizado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void BtnAlterarSenha_Click(object sender, EventArgs e)
        {
            string senhaAtual = txtSenhaAtual.Text ?? "";
            string novaSenha = txtNovaSenha.Text ?? "";
            string confirmar = txtConfirmarSenha.Text ?? "";

            if (string.IsNullOrWhiteSpace(senhaAtual))
            {
                lblFeedbackSenha.ForeColor = Color.FromArgb(240, 95, 95);
                lblFeedbackSenha.Text = "⚠ Informe sua senha atual para confirmar sua identidade.";
                txtSenhaAtual.Focus();
                return;
            }

            if (novaSenha.Length < 6)
            {
                lblFeedbackSenha.ForeColor = Color.FromArgb(240, 95, 95);
                lblFeedbackSenha.Text = "⚠ A nova senha deve ter no mínimo 6 caracteres.";
                txtNovaSenha.Focus();
                return;
            }

            if (novaSenha != confirmar)
            {
                lblFeedbackSenha.ForeColor = Color.FromArgb(240, 95, 95);
                lblFeedbackSenha.Text = "⚠ A confirmação de senha não coincide com a nova senha.";
                txtConfirmarSenha.Focus();
                return;
            }

            if (senhaAtual == novaSenha)
            {
                lblFeedbackSenha.ForeColor = Color.FromArgb(235, 175, 55);
                lblFeedbackSenha.Text = "⚠ A nova senha deve ser diferente da senha atual.";
                txtNovaSenha.Focus();
                return;
            }

            if (!OfflineDataService.AlterarSenhaUsuario(senhaAtual, novaSenha, out string msg))
            {
                lblFeedbackSenha.ForeColor = Color.FromArgb(240, 95, 95);
                lblFeedbackSenha.Text = "⚠ " + msg;
                return;
            }

            txtSenhaAtual.Clear();
            txtNovaSenha.Clear();
            txtConfirmarSenha.Clear();
            AtualizarIndicadoresSenha();

            lblFeedbackSenha.ForeColor = Color.FromArgb(90, 220, 130);
            lblFeedbackSenha.Text = "✔ " + msg;

            MessageBox.Show(
                "Sua senha foi alterada com sucesso!\nUtilize a nova senha em seus próximos acessos ao Reino.",
                "Senha Atualizada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void AjustarLayoutResponsivo()
        {
            if (topHudPanel != null && btnVoltarMapa != null)
            {
                btnVoltarMapa.Location = new Point(topHudPanel.Width - btnVoltarMapa.Width - 24, 18);
            }

            if (cardPerfil != null && cardSenha != null)
            {
                int totalWidth = 478 + 20 + 478;
                int startX = Math.Max(20, (this.ClientSize.Width - totalWidth) / 2);
                int topY = Math.Max(88, 90 + (this.ClientSize.Height - 90 - 610) / 2);

                cardPerfil.Location = new Point(startX, topY);
                cardSenha.Location = new Point(startX + 478 + 20, topY);
            }
        }
    }
}

