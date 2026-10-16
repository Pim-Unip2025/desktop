using System;
using System.Drawing;
using System.Windows.Forms;
using TronusDesktop.Models;
using TronusDesktop.Services;
using TronusDesktop.UI;

namespace TronusDesktop.Views
{
    public partial class UcDashboard : UserControl
    {
        private TronusPanel statsCard;
        private Label titleLabel;
        private TronusButton btnVoltar;

        public UcDashboard()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "UcDashboard";
            this.Size = new Size(1024, 728);
            this.Load += new EventHandler(this.UcDashboard_Load);
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            this.BackColor = Theme.Background;

            titleLabel = new Label
            {
                Text = "MEU DESEMPENHO",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(40, 40)
            };
            this.Controls.Add(titleLabel);

            btnVoltar = new TronusButton
            {
                Text = "Voltar ao Mapa",
                Size = new Size(150, 40),
                Location = new Point(830, 40)
            };
            btnVoltar.Click += (s, e) => (this.FindForm() as MainForm)?.LoadControl(new UcReinos());
            this.Controls.Add(btnVoltar);

            statsCard = new TronusPanel
            {
                Location = new Point(40, 100),
                Size = new Size(940, 580)
            };
            this.Controls.Add(statsCard);

            this.Resize += (s, e) => AdjustLayout();
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            if (btnVoltar != null)
            {
                btnVoltar.Location = new Point(this.ClientSize.Width - btnVoltar.Width - 40, 40);
            }
            if (statsCard != null)
            {
                statsCard.Size = new Size(
                    Math.Max(500, this.ClientSize.Width - 80),
                    Math.Max(300, this.ClientSize.Height - statsCard.Top - 30)
                );
            }
        }

        private async void UcDashboard_Load(object sender, EventArgs e)
        {
            try
            {
                var resumo = await ApiService.Instance.GetAsync<ResumoDashboardDto>("/personagens/me/resumo");
                
                // Exemplo simplificado de renderização
                string textoResumo = $@"
Total de Respostas: {resumo.TotalRespostas}
Total de Acertos: {resumo.TotalAcertos}
Taxa de Acerto Geral: {resumo.TaxaGeral}%
Total de Estrelas: {resumo.TotalEstrelas}
Fases Iniciadas: {resumo.FasesIniciadas}
Fases 100% Completas (3 estrelas): {resumo.FasesCompletas}
";
                var lblResumo = new Label
                {
                    Text = textoResumo,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 16F),
                    AutoSize = true,
                    Location = new Point(40, 40)
                };
                statsCard.Controls.Add(lblResumo);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar o dashboard: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
