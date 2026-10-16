using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TronusDesktop.Models;
using TronusDesktop.Services;
using TronusDesktop.UI;

namespace TronusDesktop.Views
{
    public partial class UcReinos : UserControl
    {
        private FlowLayoutPanel flowReinos;
        private Label titleLabel;

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
            this.Load += new EventHandler(this.UcReinos_Load);
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            this.BackColor = Theme.Background;

            titleLabel = new Label
            {
                Text = "MAPA DOS REINOS",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(40, 40)
            };
            this.Controls.Add(titleLabel);

            var btnDashboard = new TronusButton
            {
                Text = "Meu Desempenho",
                Size = new Size(200, 40),
                Location = new Point(780, 40)
            };
            btnDashboard.Click += (s, e) => (this.FindForm() as MainForm)?.LoadControl(new UcDashboard());
            this.Controls.Add(btnDashboard);

            flowReinos = new FlowLayoutPanel
            {
                Location = new Point(40, 100),
                Size = new Size(940, 580),
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10)
            };
            this.Controls.Add(flowReinos);
        }

        private async void UcReinos_Load(object sender, EventArgs e)
        {
            try
            {
                var reinos = await ApiService.Instance.GetAsync<List<ReinoDto>>("/reinos");
                var progresso = await ApiService.Instance.GetAsync<List<ProgressoFaseDto>>("/personagens/me/progresso");

                RenderizarReinos(reinos, progresso);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar o mapa: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RenderizarReinos(List<ReinoDto> reinos, List<ProgressoFaseDto> progresso)
        {
            flowReinos.Controls.Clear();

            foreach (var reino in reinos)
            {
                var card = new TronusPanel
                {
                    Size = new Size(280, 250),
                    Margin = new Padding(15)
                };

                var lblNome = new Label
                {
                    Text = reino.Nome,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(280, 40),
                    Location = new Point(0, 10)
                };
                card.Controls.Add(lblNome);

                // Filtrar progresso desse reino
                var progressoReino = progresso.Where(p => p.ReinoId == reino.Id).OrderBy(p => p.Ordem).ToList();
                int yFase = 60;

                foreach (var fase in progressoReino)
                {
                    var lblFase = new Label
                    {
                        Text = $"{fase.FaseNome}: {new string('★', fase.Estrelas)}{new string('☆', 3 - fase.Estrelas)}",
                        ForeColor = fase.Jogada ? Theme.Gold : Theme.DimText,
                        Font = new Font("Segoe UI", 12F),
                        AutoSize = true,
                        Location = new Point(20, yFase)
                    };
                    card.Controls.Add(lblFase);
                    
                    if (fase.Jogada || fase.Ordem == 1 || (fase.Ordem > 1 && progressoReino.Any(p => p.Ordem == fase.Ordem - 1 && p.Estrelas > 0)))
                    {
                        var btnJogar = new TronusButton
                        {
                            Text = "Jogar",
                            Size = new Size(70, 30),
                            Location = new Point(190, yFase - 5),
                            Font = new Font("Segoe UI", 9F)
                        };
                        btnJogar.Click += (s, e) => (this.FindForm() as MainForm)?.LoadControl(new UcQuiz(fase.FaseId, fase.FaseNome, reino.Nome));
                        card.Controls.Add(btnJogar);
                    }

                    yFase += 40;
                }

                flowReinos.Controls.Add(card);
            }
        }
    }
}
