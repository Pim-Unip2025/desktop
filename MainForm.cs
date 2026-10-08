using System;
using System.Drawing;
using System.Windows.Forms;
using TronusDesktop.UI;

namespace TronusDesktop
{
    public partial class MainForm : Form
    {
        private Panel topBar;
        private Label titleLabel;
        private Button closeBtn;
        private Panel contentPanel;
        private bool isDragging = false;
        private Point startPoint = new Point(0, 0);

        public MainForm()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1024, 768);
            this.BackColor = Theme.Background;

            topBar = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = Theme.CardBackground
            };
            topBar.MouseDown += TopBar_MouseDown;
            topBar.MouseMove += TopBar_MouseMove;
            topBar.MouseUp += TopBar_MouseUp;

            titleLabel = new Label
            {
                Text = "TRONUS — O Caminho do Herói Acadêmico",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 9)
            };
            titleLabel.MouseDown += TopBar_MouseDown;
            titleLabel.MouseMove += TopBar_MouseMove;
            titleLabel.MouseUp += TopBar_MouseUp;
            topBar.Controls.Add(titleLabel);

            closeBtn = new Button
            {
                Text = "X",
                ForeColor = Theme.Gold,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(45, 40),
                Dock = DockStyle.Right,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            closeBtn.FlatAppearance.BorderSize = 0;
            closeBtn.Click += (s, e) => Application.Exit();
            closeBtn.MouseEnter += (s, e) => closeBtn.BackColor = Color.FromArgb(50, 255, 0, 0);
            closeBtn.MouseLeave += (s, e) => closeBtn.BackColor = Color.Transparent;
            topBar.Controls.Add(closeBtn);

            this.Controls.Add(topBar);

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();
            topBar.BringToFront();

            LoadControl(new Views.UcLanding());
        }

        private void TopBar_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            startPoint = new Point(e.X, e.Y);
        }

        private void TopBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this.startPoint.X, p.Y - this.startPoint.Y);
            }
        }

        private void TopBar_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        public void LoadControl(UserControl uc)
        {
            contentPanel.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(uc);
        }
    }
}
