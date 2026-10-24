using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TronusDesktop.Models;
using TronusDesktop.Services;
using TronusDesktop.UI;

namespace TronusDesktop.Views
{
    public partial class UcRanking : UserControl
    {
        private Panel topHudPanel;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private TronusButton btnVoltar;

        private Panel mainScrollContainer;
        private FlowLayoutPanel flowPodium;
        private TronusPanel tableCard;
        private TronusPanel userStatusCard;

        public UcRanking()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "UcRanking";
            this.Size = new Size(1024, 728);
            this.Load += UcRanking_Load;
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

            // Suave camada escura sobre o fundo apenas no Modo Escuro
            this.Paint += (s, e) =>
            {
                if (Theme.IsDarkMode)
                {
                    using var brush = new SolidBrush(Color.FromArgb(150, 15, 12, 10));
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // ═══════════════════════════════════════════════════
            // HUD SUPERIOR
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
                Text = "🏆 HALL DOS HERÓIS — RANKING DO REINO",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 14),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblTitulo);

            lblSubtitulo = new Label
            {
                Text = "Conheça os mais bravos estudantes e seus feitos acadêmicos lendários",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                AutoSize = true,
                Location = new Point(28, 42),
                BackColor = Color.Transparent
            };
            topHudPanel.Controls.Add(lblSubtitulo);

            btnVoltar = new TronusButton
            {
                Text = "← Voltar ao Mapa",
                Size = new Size(170, 40),
                Location = new Point(820, 17)
            };
            btnVoltar.Click += (s, e) => (this.FindForm() as MainForm)?.LoadControl(new UcReinos());
            topHudPanel.Controls.Add(btnVoltar);

            void ReposRankingButtons()
            {
                if (btnVoltar != null && topHudPanel != null)
                {
                    btnVoltar.Location = new Point(topHudPanel.Width - btnVoltar.Width - 25, 17);
                }
            }

            topHudPanel.Resize += (s, e) => ReposRankingButtons();
            ReposRankingButtons();

            // ═══════════════════════════════════════════════════
            // CONTAINER PRINCIPAL (SCROLLABLE)
            // ═══════════════════════════════════════════════════
            mainScrollContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(25, 20, 25, 30)
            };
            this.Controls.Add(mainScrollContainer);
            mainScrollContainer.BringToFront();

            // 1. PÓDIO (TOP 3)
            ConstruirPodio();

            // 2. TABELA DOS DEMAIS HERÓIS
            ConstruirTabelaRanking();

            // 3. CARD DE POSIÇÃO DO JOGADOR LOGADO
            ConstruirCardJogador();

            this.Resize += (s, e) => AdjustLayout();
            AdjustLayout();
        }

        private void ConstruirPodio()
        {
            var lblSecaoPodio = new Label
            {
                Text = "👑 PÓDIO DE HONRA",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 15),
                BackColor = Color.Transparent
            };
            mainScrollContainer.Controls.Add(lblSecaoPodio);

            flowPodium = new FlowLayoutPanel
            {
                Location = new Point(25, 45),
                Size = new Size(960, 215),
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false
            };
            mainScrollContainer.Controls.Add(flowPodium);

            // 1º Lugar (Ouro)
            flowPodium.Controls.Add(CriarCardPodio(
                posicao: "🥇 1º LUGAR",
                corDestaque: Color.FromArgb(255, 215, 0),
                nome: "Sir Arthur",
                titulo: "👑 Rei do Código",
                materia: "Reino de Python",
                estrelas: "45 ★",
                acerto: "98% de Acertos",
                avatarFile: "masc1.png"
            ));

            // 2º Lugar (Prata)
            flowPodium.Controls.Add(CriarCardPodio(
                posicao: "🥈 2º LUGAR",
                corDestaque: Color.FromArgb(200, 205, 215),
                nome: "Lady Morgana",
                titulo: "🦅 Duquesa dos Dados",
                materia: "Banco de Dados & SQL",
                estrelas: "42 ★",
                acerto: "94% de Acertos",
                avatarFile: "fem2.png"
            ));

            // 3º Lugar (Bronze)
            flowPodium.Controls.Add(CriarCardPodio(
                posicao: "🥉 3º LUGAR",
                corDestaque: Color.FromArgb(205, 127, 50),
                nome: "Mestre Roland",
                titulo: "🎖️ Cavaleiro do Software",
                materia: "Engenharia de Software",
                estrelas: "38 ★",
                acerto: "90% de Acertos",
                avatarFile: "masc3.png"
            ));
        }

        private Panel CriarCardPodio(string posicao, Color corDestaque, string nome, string titulo, string materia, string estrelas, string acerto, string avatarFile)
        {
            var card = new TronusPanel
            {
                Size = new Size(305, 200),
                Margin = new Padding(0, 0, 15, 0),
                BackColor = Color.Transparent
            };
            card.Paint += (s, e) =>
            {
                using var p = new Pen(corDestaque, 1.8F);
                e.Graphics.DrawRectangle(p, 1, 1, card.Width - 3, card.Height - 3);
            };

            var lblPos = new Label
            {
                Text = posicao,
                ForeColor = corDestaque,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(305, 28),
                Location = new Point(0, 10),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblPos);

            // Avatar miniatura
            var pb = new PictureBox
            {
                Size = new Size(48, 48),
                Location = new Point(20, 45),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(40, 35, 30)
            };
            string imgP = Path.Combine(Application.StartupPath, "img", avatarFile);
            if (File.Exists(imgP)) pb.Image = Image.FromFile(imgP);
            card.Controls.Add(pb);

            var lblNome = new Label
            {
                Text = nome,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(80, 45),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblNome);

            var lblTit = new Label
            {
                Text = titulo,
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(80, 70),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblTit);

            var lblMat = new Label
            {
                Text = $"Especialidade: {materia}",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                AutoSize = true,
                Location = new Point(20, 108),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblMat);

            var lblEst = new Label
            {
                Text = $"⭐ {estrelas}",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 138),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblEst);

            var lblAc = new Label
            {
                Text = acerto,
                ForeColor = Color.FromArgb(130, 220, 130),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(160, 142),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblAc);

            return card;
        }

        private void ConstruirTabelaRanking()
        {
            var lblSecaoTabela = new Label
            {
                Text = "📜 HERÓIS EM ASCENSÃO (4º AO 10º LUGAR)",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 275),
                BackColor = Color.Transparent
            };
            mainScrollContainer.Controls.Add(lblSecaoTabela);

            tableCard = new TronusPanel
            {
                Location = new Point(25, 305),
                Size = new Size(960, 280),
                BackColor = Color.Transparent
            };
            mainScrollContainer.Controls.Add(tableCard);

            // Cabeçalho da tabela
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = Color.FromArgb(40, 212, 175, 55)
            };
            tableCard.Controls.Add(headerPanel);

            AdicionarColunaCabecalho(headerPanel, "#", 15, 45);
            AdicionarColunaCabecalho(headerPanel, "HERÓI", 70, 180);
            AdicionarColunaCabecalho(headerPanel, "PATENTE / TÍTULO", 260, 180);
            AdicionarColunaCabecalho(headerPanel, "REINO EM DESTAQUE", 450, 190);
            AdicionarColunaCabecalho(headerPanel, "ESTRELAS", 650, 120);
            AdicionarColunaCabecalho(headerPanel, "TAXA DE ACERTO", 780, 140);

            // Lista de Heróis
            var listPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent
            };
            tableCard.Controls.Add(listPanel);
            listPanel.BringToFront();

            var dadosRanking = new[]
            {
                new { Pos = "4º", Nome = "Valquíria Brenda", Titulo = "🎖️ Cavaleiro", Reino = "C++ e Estruturas", Estrelas = "35 ★", Taxa = "89%" },
                new { Pos = "5º", Nome = "Lorde Cedric", Titulo = "🎖️ Cavaleiro", Reino = "Redes de Computadores", Estrelas = "33 ★", Taxa = "86%" },
                new { Pos = "6º", Nome = "Maga Beatrice", Titulo = "🎖️ Cavaleiro", Reino = "Banco de Dados", Estrelas = "30 ★", Taxa = "84%" },
                new { Pos = "7º", Nome = "Arqueiro Gabriel", Titulo = "🛡️ Plebeu de Honra", Reino = "Python", Estrelas = "27 ★", Taxa = "82%" },
                new { Pos = "8º", Nome = "Cavaleiro Lucas", Titulo = "🛡️ Plebeu de Honra", Reino = "Engenharia de Software", Estrelas = "24 ★", Taxa = "79%" },
                new { Pos = "9º", Nome = "Lady Clarissa", Titulo = "🛡️ Plebeu", Reino = "Python", Estrelas = "20 ★", Taxa = "76%" },
                new { Pos = "10º", Nome = "Bardo Guilherme", Titulo = "🛡️ Plebeu", Reino = "C++ e Estruturas", Estrelas = "18 ★", Taxa = "73%" }
            };

            int y = 5;
            foreach (var h in dadosRanking)
            {
                var row = new Panel
                {
                    Location = new Point(5, y),
                    Size = new Size(930, 32),
                    BackColor = (y % 64 == 5) ? Color.FromArgb(20, 255, 255, 255) : Color.Transparent
                };

                row.Controls.Add(CriarLabelLinha(h.Pos, 10, 45, Theme.Gold, true));
                row.Controls.Add(CriarLabelLinha(h.Nome, 65, 180, Color.White, true));
                row.Controls.Add(CriarLabelLinha(h.Titulo, 255, 180, Theme.PrimaryText, false));
                row.Controls.Add(CriarLabelLinha(h.Reino, 445, 190, Theme.DimText, false));
                row.Controls.Add(CriarLabelLinha(h.Estrelas, 645, 120, Theme.Gold, true));
                row.Controls.Add(CriarLabelLinha(h.Taxa, 775, 140, Color.FromArgb(140, 220, 140), true));

                listPanel.Controls.Add(row);
                y += 34;
            }
        }

        private void ConstruirCardJogador()
        {
            userStatusCard = new TronusPanel
            {
                Location = new Point(25, 600),
                Size = new Size(960, 60),
                BackColor = Color.Transparent
            };
            userStatusCard.Paint += (s, e) =>
            {
                using var p = new Pen(Theme.Gold, 1.5F);
                e.Graphics.DrawRectangle(p, 1, 1, userStatusCard.Width - 3, userStatusCard.Height - 3);
            };
            mainScrollContainer.Controls.Add(userStatusCard);

            string nomeLogado = SessionManager.CurrentUser?.Nome ?? "Você (Herói)";
            var lblMinhaPos = new Label
            {
                Text = $"⭐ SUA POSIÇÃO ATUAL: Herói {nomeLogado} · Acompanhe suas fases no mapa para subir no pódio!",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 0, 0, 0),
                BackColor = Color.Transparent
            };
            userStatusCard.Controls.Add(lblMinhaPos);
        }

        private void AdicionarColunaCabecalho(Panel p, string texto, int x, int width)
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

        private Label CriarLabelLinha(string texto, int x, int width, Color cor, bool bold)
        {
            return new Label
            {
                Text = texto,
                ForeColor = cor,
                Font = new Font("Segoe UI", 9F, bold ? FontStyle.Bold : FontStyle.Regular),
                AutoSize = false,
                Size = new Size(width, 32),
                Location = new Point(x, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
        }

        private void AdjustLayout()
        {
            int w = Math.Max(700, this.ClientSize.Width - 50);

            if (flowPodium != null)
            {
                flowPodium.Width = w;
            }

            if (tableCard != null)
            {
                tableCard.Width = w;
            }

            if (userStatusCard != null)
            {
                userStatusCard.Width = w;
            }
        }

        private async void UcRanking_Load(object sender, EventArgs e)
        {
            try
            {
                // Se o usuário logado tiver personagem, atualiza o card inferior
                if (SessionManager.IsLoggedIn)
                {
                    var personagem = await ApiService.Instance.GetAsync<PersonagemDto>("/personagens/me");
                    if (personagem != null && userStatusCard != null && userStatusCard.Controls.Count > 0)
                    {
                        var lbl = userStatusCard.Controls[0] as Label;
                        if (lbl != null)
                        {
                            lbl.Text = $"⭐ SUA POSIÇÃO ATUAL: {personagem.Nome} ({personagem.TituloAtual}) · {personagem.TotalEstrelas} Estrelas Conquistadas · Continue jogando para alcançar o Top 3!";
                        }
                    }
                }
            }
            catch
            {
                // Silencioso em caso de erro na consulta
            }
        }
    }
}
