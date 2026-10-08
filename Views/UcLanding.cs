using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TronusDesktop.UI;

namespace TronusDesktop.Views
{
    public partial class UcLanding : UserControl
    {
        private Label tagLabel;
        private PictureBox logoBox;
        private Label subtitleLabel;
        private Label descLabel;
        private TronusButton btnRegister;
        private TronusButton btnLogin;

        public UcLanding()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.BackColor = Theme.Background;

            string heroPath = Path.Combine(Application.StartupPath, "img", "hero.png");
            if (File.Exists(heroPath))
            {
                this.BackgroundImage = Image.FromFile(heroPath);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            this.Paint += (s, e) =>
            {
                int gradientWidth = this.Width / 2 + 150;
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(0, 0), new Point(gradientWidth, 0),
                    Color.FromArgb(240, 10, 10, 10), Color.Transparent))
                {
                    // Apenas desenha até o ponto em que o gradiente fica transparente,
                    // evitando que a classe repita o gradiente do lado direito.
                    e.Graphics.FillRectangle(brush, new Rectangle(0, 0, gradientWidth, this.Height));
                }
            };

            tagLabel = new Label
            {
                Text = " ⚔ UNIP - 2026 ",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(80, 80),
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(tagLabel);

            logoBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(280, 100),
                Location = new Point(80, 130),
                BackColor = Color.Transparent
            };

            string logoPath = Path.Combine(Application.StartupPath, "img", "logo.png");
            if (File.Exists(logoPath))
            {
                logoBox.Image = Image.FromFile(logoPath);
            }
            this.Controls.Add(logoBox);

            subtitleLabel = new Label
            {
                Text = "O CAMINHO DO HERÓI ACADÊMICO",
                ForeColor = Theme.Gold,
                Font = new Font("Georgia", 11F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(80, 260),
                BackColor = Color.Transparent
            };
            this.Controls.Add(subtitleLabel);

            descLabel = new Label
            {
                Text = "Sua jornada rumo ao título de REI começa aqui. Enfrente os desafios das matérias-reinos e forje seu futuro.",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                AutoSize = true,
                MaximumSize = new Size(450, 0),
                Location = new Point(80, 300),
                BackColor = Color.Transparent
            };
            this.Controls.Add(descLabel);

            btnRegister = new TronusButton
            {
                Text = "CADASTRE-SE AGORA →",
                Location = new Point(80, 400),
                Size = new Size(250, 45)
            };
            btnRegister.Click += (s, e) =>
            {
                (this.FindForm() as MainForm)?.LoadControl(new UcRegister());
            };
            this.Controls.Add(btnRegister);

            btnLogin = new TronusButton
            {
                Text = "JÁ TENHO CONTA",
                Location = new Point(350, 400),
                Size = new Size(200, 45)
            };
            btnLogin.Click += (s, e) =>
            {
                (this.FindForm() as MainForm)?.LoadControl(new UcLogin());
            };
            this.Controls.Add(btnLogin);
        }
    }
}
