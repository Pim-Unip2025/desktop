using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TronusDesktop.UI;

namespace TronusDesktop
{
    public partial class MainForm : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        private Panel topBar;
        private Label titleLabel;
        private Panel windowButtonsPanel;
        private Button btnOfflineStatus;
        private AccessibilityThemeButton btnTheme;
        private Button btnMinimize;
        private Button btnMaximize;
        private Button closeBtn;
        private Panel contentPanel;
        private bool isDragging = false;
        private Point startPoint = new Point(0, 0);

        public MainForm()
        {
            InitializeComponent();
            SetupUI();
            AutoAjustarAoIniciar();
        }

        private void SetupUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Theme.Background;
            this.MinimumSize = new Size(960, 680);
            this.Resize += (s, e) =>
            {
                if (btnMaximize != null)
                {
                    btnMaximize.Text = (this.WindowState == FormWindowState.Maximized) ? "🗗" : "🗖";
                }
            };

            // ═══════════════════════════════════════════════════
            // BARRA DE TÍTULO CUSTOMIZADA (TOPBAR)
            // ═══════════════════════════════════════════════════
            topBar = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = Theme.TopBarBackground
            };
            topBar.Paint += (s, e) =>
            {
                using var pen = new Pen(Theme.BorderColor, 1);
                e.Graphics.DrawLine(pen, 0, topBar.Height - 1, topBar.Width, topBar.Height - 1);
            };
            topBar.MouseDown += TopBar_MouseDown;
            topBar.MouseMove += TopBar_MouseMove;
            topBar.MouseUp += TopBar_MouseUp;
            topBar.DoubleClick += (s, e) => ToggleMaximize();

            titleLabel = new Label
            {
                Text = "TRONUS — O Caminho do Herói Acadêmico",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = false,
                AutoEllipsis = true,
                Location = new Point(15, 9),
                Size = new Size(500, 24),
                BackColor = Color.Transparent
            };
            titleLabel.MouseDown += TopBar_MouseDown;
            titleLabel.MouseMove += TopBar_MouseMove;
            titleLabel.MouseUp += TopBar_MouseUp;
            titleLabel.DoubleClick += (s, e) => ToggleMaximize();
            topBar.Controls.Add(titleLabel);

            // Container para os botões da janela, acessibilidade e conectividade (painel com posições absolutas)
            windowButtonsPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 415,
                Height = 40,
                BackColor = Theme.TopBarBackground
            };

            // Botão/Indicador de Conectividade (Modo Online / Modo Offline Demo)
            btnOfflineStatus = new Button
            {
                Size = new Size(115, 30),
                Location = new Point(10, 5),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnOfflineStatus.FlatAppearance.BorderSize = 1;
            AtualizarVisualModoOffline();
            btnOfflineStatus.Click += (s, e) =>
            {
                Services.ApiService.IsOfflineMode = !Services.ApiService.IsOfflineMode;
                AtualizarVisualModoOffline();
                string statusTxt = Services.ApiService.IsOfflineMode
                    ? "⚡ MODO OFFLINE ATIVADO!\n\nA aplicação agora utiliza dados locais simulados de alta fidelidade para apresentações sem dependência de internet."
                    : "🌐 MODO ONLINE ATIVADO!\n\nA aplicação agora realiza requisições diretamente para a API na nuvem.";
                MessageBox.Show(statusTxt, "Conectividade", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            Services.ApiService.OfflineModeChanged += (isOffline) => AtualizarVisualModoOffline();

            // Botão de Acessibilidade (Modo Claro / Modo Escuro)
            btnTheme = new AccessibilityThemeButton
            {
                Size = new Size(130, 30),
                Location = new Point(135, 5)
            };

            // Botão Minimizar
            btnMinimize = new Button
            {
                Text = "—",
                ForeColor = Theme.Gold,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(45, 40),
                Location = new Point(275, 0),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            btnMinimize.MouseEnter += (s, e) => btnMinimize.BackColor = Color.FromArgb(40, 212, 175, 55);
            btnMinimize.MouseLeave += (s, e) => btnMinimize.BackColor = Color.Transparent;

            // Botão Maximizar / Restaurar (Auto Ajuste de Janela)
            btnMaximize = new Button
            {
                Text = "🗖",
                ForeColor = Theme.Gold,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(45, 40),
                Location = new Point(320, 0),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.Click += (s, e) => ToggleMaximize();
            btnMaximize.MouseEnter += (s, e) => btnMaximize.BackColor = Color.FromArgb(40, 212, 175, 55);
            btnMaximize.MouseLeave += (s, e) => btnMaximize.BackColor = Color.Transparent;

            // Botão Fechar
            closeBtn = new Button
            {
                Text = "✕",
                ForeColor = Theme.Gold,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(45, 40),
                Location = new Point(365, 0),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };
            closeBtn.FlatAppearance.BorderSize = 0;
            closeBtn.Click += (s, e) => Application.Exit();
            closeBtn.MouseEnter += (s, e) => closeBtn.BackColor = Color.FromArgb(60, 255, 50, 50);
            closeBtn.MouseLeave += (s, e) => closeBtn.BackColor = Color.Transparent;

            windowButtonsPanel.Controls.Add(btnOfflineStatus);
            windowButtonsPanel.Controls.Add(btnTheme);
            windowButtonsPanel.Controls.Add(btnMinimize);
            windowButtonsPanel.Controls.Add(btnMaximize);
            windowButtonsPanel.Controls.Add(closeBtn);
            topBar.Controls.Add(windowButtonsPanel);

            topBar.Resize += (s, e) =>
            {
                int maxTitleWidth = Math.Max(100, topBar.Width - windowButtonsPanel.Width - 30);
                titleLabel.Size = new Size(maxTitleWidth, 24);
            };

            Theme.ThemeChanged += OnThemeChanged;

            // Painel central hospedeiro dos UserControls
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Background
            };
            this.Controls.Add(topBar);
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();

            LoadControl(new Views.UcLanding());
        }

        /// <summary>
        /// Detecta o tamanho da área de trabalho do sistema (WorkingArea) e auto-ajusta a janela
        /// para a melhor visualização possível, sem cortar elementos ou cobrir a barra de tarefas.
        /// </summary>
        private void AutoAjustarAoIniciar()
        {
            Rectangle workArea = Screen.PrimaryScreen.WorkingArea;

            // Se o monitor for compacto (notebooks 1366x768 ou telas menores):
            // Inicia maximizado ajustado ao WorkingArea para não cortar o rodapé.
            if (workArea.Width <= 1366 || workArea.Height <= 768)
            {
                this.MaximizedBounds = workArea;
                this.WindowState = FormWindowState.Maximized;
                if (btnMaximize != null) btnMaximize.Text = "🗗";
            }
            else
            {
                // Monitores com alta resolução (Full HD, 2K, 4K):
                // Inicia em modo janela confortável (1200x780 ou 80% da tela) centralizado.
                int w = Math.Min(1200, (int)(workArea.Width * 0.75));
                int h = Math.Min(800, (int)(workArea.Height * 0.85));
                this.Size = new Size(Math.Max(1024, w), Math.Max(720, h));
                this.Location = new Point(
                    workArea.Left + (workArea.Width - this.Width) / 2,
                    workArea.Top + (workArea.Height - this.Height) / 2
                );
                this.WindowState = FormWindowState.Normal;
                if (btnMaximize != null) btnMaximize.Text = "🗖";
            }
        }

        /// <summary>
        /// Alterna entre modo janela e modo maximizado inteligente (respeitando a barra de tarefas).
        /// </summary>
        public void ToggleMaximize()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                if (btnMaximize != null) btnMaximize.Text = "🗖";
            }
            else
            {
                Rectangle workArea = Screen.FromControl(this).WorkingArea;
                this.MaximizedBounds = workArea;
                this.WindowState = FormWindowState.Maximized;
                if (btnMaximize != null) btnMaximize.Text = "🗗";
            }
        }

        private void TopBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.WindowState == FormWindowState.Normal)
            {
                isDragging = true;
                startPoint = new Point(e.X, e.Y);
            }
        }

        private void TopBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && this.WindowState == FormWindowState.Normal)
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
            Theme.ApplyTheme(uc);
            contentPanel.Controls.Add(uc);
        }

        private void OnThemeChanged()
        {
            this.BackColor = Theme.Background;
            topBar.BackColor = Theme.TopBarBackground;
            if (windowButtonsPanel != null) windowButtonsPanel.BackColor = Theme.TopBarBackground;
            if (contentPanel != null) contentPanel.BackColor = Theme.Background;
            titleLabel.ForeColor = Theme.Gold;
            btnMinimize.ForeColor = Theme.Gold;
            btnMaximize.ForeColor = Theme.Gold;
            closeBtn.ForeColor = Theme.Gold;
            topBar.Invalidate();

            if (contentPanel != null && contentPanel.Controls.Count > 0)
            {
                Theme.ApplyTheme(contentPanel.Controls[0]);
            }

            AtualizarVisualModoOffline();
        }

        private void AtualizarVisualModoOffline()
        {
            if (btnOfflineStatus == null) return;

            if (Services.ApiService.IsOfflineMode)
            {
                btnOfflineStatus.Text = "⚡ Offline (Demo)";
                btnOfflineStatus.ForeColor = Theme.IsDarkMode ? Color.FromArgb(255, 215, 0) : Color.FromArgb(170, 120, 15);
                btnOfflineStatus.BackColor = Theme.IsDarkMode ? Color.FromArgb(50, 42, 20) : Color.FromArgb(245, 235, 205);
                btnOfflineStatus.FlatAppearance.BorderColor = Theme.Gold;
            }
            else
            {
                btnOfflineStatus.Text = "🌐 Online";
                btnOfflineStatus.ForeColor = Theme.IsDarkMode ? Color.FromArgb(80, 220, 120) : Color.FromArgb(20, 140, 60);
                btnOfflineStatus.BackColor = Theme.IsDarkMode ? Color.FromArgb(20, 45, 30) : Color.FromArgb(220, 245, 225);
                btnOfflineStatus.FlatAppearance.BorderColor = Theme.IsDarkMode ? Color.FromArgb(60, 180, 100) : Color.FromArgb(40, 160, 70);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.T) || keyData == (Keys.Alt | Keys.A))
            {
                Theme.ToggleTheme();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Permite que a janela sem bordas nativas seja redimensionada livremente
        /// pelo usuário arrastando as bordas e cantos do formulário, e impede que
        /// a janela cubra a barra de tarefas ao ser maximizada via Aero Snap.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int WM_GETMINMAXINFO = 0x24;
            const int RESIZE_HANDLE_SIZE = 8;

            if (m.Msg == WM_GETMINMAXINFO)
            {
                Screen screen = Screen.FromHandle(this.Handle);
                Rectangle wa = screen.WorkingArea;
                Rectangle bounds = screen.Bounds;
                MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(MINMAXINFO))!;
                mmi.ptMaxPosition.X = Math.Abs(wa.Left - bounds.Left);
                mmi.ptMaxPosition.Y = Math.Abs(wa.Top - bounds.Top);
                mmi.ptMaxSize.X = wa.Width;
                mmi.ptMaxSize.Y = wa.Height;
                Marshal.StructureToPtr(mmi, m.LParam, true);
            }

            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && this.WindowState == FormWindowState.Normal)
            {
                Point cursor = this.PointToClient(Cursor.Position);

                if (cursor.X >= this.ClientSize.Width - RESIZE_HANDLE_SIZE)
                {
                    if (cursor.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
                        m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    else if (cursor.Y <= RESIZE_HANDLE_SIZE)
                        m.Result = (IntPtr)14; // HTTOPRIGHT
                    else
                        m.Result = (IntPtr)11; // HTRIGHT
                }
                else if (cursor.X <= RESIZE_HANDLE_SIZE)
                {
                    if (cursor.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
                        m.Result = (IntPtr)16; // HTBOTTOMLEFT
                    else if (cursor.Y <= RESIZE_HANDLE_SIZE)
                        m.Result = (IntPtr)13; // HTTOPLEFT
                    else
                        m.Result = (IntPtr)10; // HTLEFT
                }
                else if (cursor.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
                {
                    m.Result = (IntPtr)15; // HTBOTTOM
                }
                else if (cursor.Y <= RESIZE_HANDLE_SIZE)
                {
                    m.Result = (IntPtr)12; // HTTOP
                }
            }
        }
    }
}
