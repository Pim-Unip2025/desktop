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
    public partial class UcReinos : UserControl
    {
        // HUD Superior (Player Topbar)
        private Panel topHudPanel;
        private PictureBox pbAvatar;
        private Label lblHeroName;
        private Label lblHeroTitle;
        private Label lblEstrelas;
        private Label lblProgressText;
        private Panel progressBarBg;
        private Panel progressBarFill;
        private TronusButton btnRanking;
        private TronusButton btnDashboard;
        private TronusButton btnLogout;

        // Cabeçalho da página
        private TronusPanel headerCard;
        private Label titleLabel;
        private Label subtitleLabel;

        // Container dos reinos
        private FlowLayoutPanel flowReinos;

        // Dicionário de imagens dos reinos
        private readonly Dictionary<string, string> reinoImagens = new()
        {
            { "Python", "python.png" },
            { "C++", "c++.png" },
            { "Engenharia de Software", "soft.png" },
            { "Banco de Dados", "bd.png" },
            { "Redes", "redes.png" }
        };

        public UcReinos()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "UcReinos";
            this.Size = new Size(1024, 728);
            this.Load += UcReinos_Load;
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

            // Suave camada escura sobre o mapa apenas no Modo Escuro
            this.Paint += (s, e) =>
            {
                if (Theme.IsDarkMode)
                {
                    using var brush = new SolidBrush(Color.FromArgb(90, 15, 12, 10));
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // ═══════════════════════════════════════════════════
            // 1. HUD SUPERIOR DO ALUNO (PLAYER TOPBAR)
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

            // Avatar do herói
            pbAvatar = new PictureBox
            {
                Location = new Point(25, 12),
                Size = new Size(50, 50),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(35, 30, 25)
            };
            topHudPanel.Controls.Add(pbAvatar);

            // Nome e Título do Herói
            lblHeroName = new Label
            {
                Text = "Carregando...",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(85, 14),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblHeroName);

            lblHeroTitle = new Label
            {
                Text = "🛡️ Plebeu",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(85, 38),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblHeroTitle);

            // Contador de estrelas
            lblEstrelas = new Label
            {
                Text = "⭐ 0 estrelas",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(340, 15),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblEstrelas);

            // Barra de progresso para o próximo título
            lblProgressText = new Label
            {
                Text = "0 / 15 para Cavaleiro",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 8.5F),
                AutoSize = true,
                Location = new Point(340, 42),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblProgressText);

            progressBarBg = new Panel
            {
                Location = new Point(480, 45),
                Size = new Size(160, 9),
                BackColor = Color.FromArgb(40, 40, 40)
            };
            topHudPanel.Controls.Add(progressBarBg);

            progressBarFill = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(0, 9),
                BackColor = Theme.Gold
            };
            progressBarBg.Controls.Add(progressBarFill);

            // Botão Ranking dos Heróis
            btnRanking = new TronusButton
            {
                Text = "Ranking 🏆",
                Size = new Size(130, 38),
                Location = new Point(578, 18),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnRanking.Click += (s, e) => (this.FindForm() as MainForm)?.LoadControl(new UcRanking());
            topHudPanel.Controls.Add(btnRanking);

            // Botão Meu Desempenho
            btnDashboard = new TronusButton
            {
                Text = "Meu Desempenho 📊",
                Size = new Size(165, 38),
                Location = new Point(720, 18),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnDashboard.Click += (s, e) => (this.FindForm() as MainForm)?.LoadControl(new UcDashboard());
            topHudPanel.Controls.Add(btnDashboard);

            // Botão Sair / Logout
            btnLogout = new TronusButton
            {
                Text = "Sair",
                Size = new Size(80, 38),
                Location = new Point(905, 18),
                Font = new Font("Segoe UI", 9F)
            };
            btnLogout.Click += (s, e) =>
            {
                var confirm = MessageBox.Show("Deseja realmente sair da sua conta?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    SessionManager.Logout();
                    (this.FindForm() as MainForm)?.LoadControl(new UcLogin());
                }
            };
            topHudPanel.Controls.Add(btnLogout);

            topHudPanel.Resize += (s, e) => RepositionHudButtons();
            RepositionHudButtons();

            // ═══════════════════════════════════════════════════
            // 2. CABEÇALHO DO MAPA (CARD DE ALTO CONTRASTE)
            // ═══════════════════════════════════════════════════
            headerCard = new TronusPanel
            {
                Location = new Point(20, 90),
                Size = new Size(Math.Max(300, this.ClientSize.Width - 40), 68),
                BackColor = Color.Transparent
            };
            this.Controls.Add(headerCard);

            titleLabel = new Label
            {
                Text = "⚔️ MAPA DOS REINOS",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 10),
                BackColor = Color.Transparent
            };
            headerCard.Controls.Add(titleLabel);

            subtitleLabel = new Label
            {
                Text = "Escolha uma matéria, enfrente os desafios das fases e conquiste suas estrelas.",
                ForeColor = Color.FromArgb(235, 230, 215),
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(18, 38),
                BackColor = Color.Transparent
            };
            headerCard.Controls.Add(subtitleLabel);

            // ═══════════════════════════════════════════════════
            // 3. CONTAINER DOS REINOS (FLOWLAYOUTPANEL)
            // ═══════════════════════════════════════════════════
            flowReinos = new FlowLayoutPanel
            {
                Location = new Point(20, 168),
                Size = new Size(984, 540),
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(5),
                BackColor = Color.Transparent
            };
            this.Controls.Add(flowReinos);

            this.Resize += (s, e) =>
            {
                if (headerCard != null)
                {
                    headerCard.Size = new Size(Math.Max(300, this.ClientSize.Width - 40), 68);
                }
                if (flowReinos != null)
                {
                    flowReinos.Size = new Size(
                        Math.Max(300, this.ClientSize.Width - 40),
                        Math.Max(200, this.ClientSize.Height - flowReinos.Top - 20)
                    );
                }
                RepositionHudButtons();
            };
        }

        private void RepositionHudButtons()
        {
            if (btnLogout != null && btnDashboard != null && topHudPanel != null)
            {
                btnLogout.Location = new Point(topHudPanel.Width - btnLogout.Width - 25, 18);
                btnDashboard.Location = new Point(btnLogout.Left - btnDashboard.Width - 10, 18);
                if (btnRanking != null)
                {
                    btnRanking.Location = new Point(btnDashboard.Left - btnRanking.Width - 10, 18);
                }
            }
        }

        private async void UcReinos_Load(object sender, EventArgs e)
        {
            try
            {
                // Carrega herói, lista de reinos e progresso em paralelo
                var taskHeroi = ApiService.Instance.GetAsync<PersonagemDto>("/personagens/me");
                var taskReinos = ApiService.Instance.GetAsync<List<ReinoDto>>("/reinos");
                var taskProgresso = ApiService.Instance.GetAsync<List<ProgressoFaseDto>>("/personagens/me/progresso");

                await System.Threading.Tasks.Task.WhenAll(taskHeroi, taskReinos, taskProgresso);

                var heroi = await taskHeroi;
                var reinos = await taskReinos;
                var progresso = await taskProgresso;

                AtualizarHudHeroi(heroi);
                RenderizarReinos(reinos, progresso);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar o mapa dos reinos: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarHudHeroi(PersonagemDto heroi)
        {
            if (heroi == null) return;

            lblHeroName.Text = heroi.Nome;

            // Insígnia por título
            string badgeIcon = heroi.TituloAtual switch
            {
                "Rei" => "👑",
                "Duque" => "🦅",
                "Cavaleiro" => "🎖️",
                _ => "🛡️"
            };
            lblHeroTitle.Text = $"{badgeIcon} {heroi.TituloAtual}";
            lblEstrelas.Text = $"⭐ {heroi.TotalEstrelas} estrelas";

            // Imagem do Avatar
            if (!string.IsNullOrEmpty(heroi.Avatar))
            {
                string avatarLocal = Path.Combine(Application.StartupPath, heroi.Avatar.Replace("/", "\\"));
                if (File.Exists(avatarLocal))
                {
                    pbAvatar.Image = Image.FromFile(avatarLocal);
                }
            }

            // Metas de evolução de títulos
            int estrelas = heroi.TotalEstrelas;
            int metaAtual = 15;
            string proxTitulo = "Cavaleiro";

            if (estrelas < 15)
            {
                metaAtual = 15;
                proxTitulo = "Cavaleiro";
            }
            else if (estrelas < 30)
            {
                metaAtual = 30;
                proxTitulo = "Duque";
            }
            else if (estrelas < 50)
            {
                metaAtual = 50;
                proxTitulo = "Rei";
            }
            else
            {
                metaAtual = 50;
                proxTitulo = "Trono Conquistado";
            }

            if (estrelas >= 50)
            {
                lblProgressText.Text = "👑 Todos os Reinos Dominados!";
                progressBarFill.Width = progressBarBg.Width;
            }
            else
            {
                lblProgressText.Text = $"{estrelas} / {metaAtual} para {proxTitulo}";
                int fillWidth = (int)Math.Round((double)estrelas / metaAtual * progressBarBg.Width);
                progressBarFill.Width = Math.Max(4, Math.Min(progressBarBg.Width, fillWidth));
            }
        }

        private void RenderizarReinos(List<ReinoDto> reinos, List<ProgressoFaseDto> progresso)
        {
            flowReinos.Controls.Clear();

            foreach (var reino in reinos)
            {
                var card = new TronusPanel
                {
                    Size = new Size(305, 255),
                    Margin = new Padding(8)
                };

                // Imagem do Reino
                var pbReino = new PictureBox
                {
                    Location = new Point(15, 12),
                    Size = new Size(42, 42),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Transparent
                };
                string fileName = reinoImagens.ContainsKey(reino.Nome) ? reinoImagens[reino.Nome] : "python.png";
                string imgPath = Path.Combine(Application.StartupPath, "img", fileName);
                if (File.Exists(imgPath))
                {
                    pbReino.Image = Image.FromFile(imgPath);
                }
                card.Controls.Add(pbReino);

                // Nome do Reino
                var lblNome = new Label
                {
                    Text = reino.Nome,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(65, 14)
                };
                card.Controls.Add(lblNome);

                // Badge "EM BREVE" para reinos sem questões (no modo offline todos os 5 reinos possuem questões simuladas)
                bool isReinoAtivo = reino.Nome.Equals("Python", StringComparison.OrdinalIgnoreCase) || ApiService.IsOfflineMode;
                if (!isReinoAtivo)
                {
                    var lblEmBreve = new Label
                    {
                        Text = "EM BREVE",
                        ForeColor = Theme.DimText,
                        Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                        BackColor = Color.FromArgb(40, 20, 15),
                        BorderStyle = BorderStyle.FixedSingle,
                        AutoSize = true,
                        Location = new Point(67, 36)
                    };
                    card.Controls.Add(lblEmBreve);
                }
                else
                {
                    var lblAtivo = new Label
                    {
                        Text = "DISPONÍVEL",
                        ForeColor = Theme.Gold,
                        Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                        BackColor = Color.FromArgb(20, 45, 30),
                        BorderStyle = BorderStyle.FixedSingle,
                        AutoSize = true,
                        Location = new Point(67, 36)
                    };
                    card.Controls.Add(lblAtivo);
                }

                // Filtrar progresso desse reino
                var progressoReino = progresso.Where(p => p.ReinoId == reino.Id).OrderBy(p => p.Ordem).ToList();
                int yFase = 68;

                foreach (var fase in progressoReino)
                {
                    // Painel da fase
                    var pnlFase = new Panel
                    {
                        Location = new Point(12, yFase),
                        Size = new Size(280, 52),
                        BackColor = Color.FromArgb(30, 25, 20)
                    };

                    string estrelasVisual = new string('★', fase.Estrelas) + new string('☆', 3 - fase.Estrelas);
                    var lblFase = new Label
                    {
                        Text = $"{fase.FaseNome}\n{estrelasVisual}",
                        ForeColor = fase.Estrelas > 0 ? Theme.Gold : (fase.Jogada ? Color.White : Theme.DimText),
                        Font = new Font("Segoe UI", 9.5F, fase.Estrelas > 0 ? FontStyle.Bold : FontStyle.Regular),
                        AutoSize = false,
                        Size = new Size(185, 42),
                        Location = new Point(8, 5)
                    };
                    pnlFase.Controls.Add(lblFase);

                    // Regra de desbloqueio:
                    // 1. O reino deve estar ativo (com questões)
                    // 2. Primeira fase liberada; fases seguintes exigem que a anterior tenha ao menos 1 estrela
                    bool faseLiberada = isReinoAtivo && (fase.Jogada || fase.Ordem == 1 || (fase.Ordem > 1 && progressoReino.Any(p => p.Ordem == fase.Ordem - 1 && p.Estrelas > 0)));

                    if (faseLiberada)
                    {
                        var btnJogar = new TronusButton
                        {
                            Text = fase.Estrelas > 0 ? "Refazer" : "Jogar",
                            Size = new Size(72, 32),
                            Location = new Point(200, 10),
                            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
                        };
                        btnJogar.Click += (s, e) => (this.FindForm() as MainForm)?.LoadControl(new UcQuiz(fase.FaseId, fase.FaseNome, reino.Nome));
                        pnlFase.Controls.Add(btnJogar);
                    }
                    else
                    {
                        var lblLock = new Label
                        {
                            Text = !isReinoAtivo ? "⏳ Breve" : "🔒 Bloqueado",
                            ForeColor = Theme.DimText,
                            Font = new Font("Segoe UI", 8F),
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Size = new Size(76, 32),
                            Location = new Point(198, 10)
                        };
                        pnlFase.Controls.Add(lblLock);
                    }

                    card.Controls.Add(pnlFase);
                    yFase += 58;
                }

                flowReinos.Controls.Add(card);
            }
        }
    }
}
