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
        private Button btnSetup;
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
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
            this.DoubleBuffered = true;

            InitializeComponent();
            SetupUI();
            Theme.EnableDoubleBuffering(this);
            AutoAjustarAoIniciar();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED — compõe todos os controles filhos sem piscar
                return cp;
            }
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

            // Container para os botões da janela, acessibilidade, QA e conectividade (painel com posições absolutas)
            windowButtonsPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 530,
                Height = 40,
                BackColor = Theme.TopBarBackground
            };

            // Botão Setup (Menu de QA / Testes e Acessibilidade de Fonte)
            btnSetup = new Button
            {
                Text = "⚙️ Setup",
                Size = new Size(105, 30),
                Location = new Point(10, 5),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Theme.Gold,
                BackColor = Color.FromArgb(30, 34, 48),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnSetup.FlatAppearance.BorderSize = 1;
            btnSetup.FlatAppearance.BorderColor = Theme.Gold;
            btnSetup.Click += (s, e) => AbrirMenuSetup();

            // Botão/Indicador de Conectividade (Modo Online / Modo Offline Demo)
            btnOfflineStatus = new Button
            {
                Size = new Size(115, 30),
                Location = new Point(123, 5),
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
                Location = new Point(246, 5)
            };

            // Botão Minimizar
            btnMinimize = new Button
            {
                Text = "—",
                ForeColor = Theme.Gold,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(45, 40),
                Location = new Point(385, 0),
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
                Location = new Point(430, 0),
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
                Location = new Point(475, 0),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };
            closeBtn.FlatAppearance.BorderSize = 0;
            closeBtn.Click += (s, e) => Application.Exit();
            closeBtn.MouseEnter += (s, e) => closeBtn.BackColor = Color.FromArgb(60, 255, 50, 50);
            closeBtn.MouseLeave += (s, e) => closeBtn.BackColor = Color.Transparent;

            windowButtonsPanel.Controls.Add(btnSetup);
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
            contentPanel.SuspendLayout();
            try
            {
                var oldControls = new Control[contentPanel.Controls.Count];
                contentPanel.Controls.CopyTo(oldControls, 0);

                uc.Dock = DockStyle.Fill;
                Theme.ApplyTheme(uc);
                contentPanel.Controls.Add(uc);

                foreach (var old in oldControls)
                {
                    contentPanel.Controls.Remove(old);
                    old.Dispose();
                }
            }
            finally
            {
                contentPanel.ResumeLayout(true);
            }
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

            if (btnSetup != null)
            {
                btnSetup.ForeColor = Theme.Gold;
                btnSetup.BackColor = Theme.IsDarkMode ? Color.FromArgb(30, 34, 48) : Color.FromArgb(240, 232, 212);
                btnSetup.FlatAppearance.BorderColor = Theme.Gold;
            }
            AtualizarVisualModoOffline();
        }

        /// <summary>
        /// Abre o painel de Setup contendo o botão QA (para ações de teste de software)
        /// e as opções de Acessibilidade (aumentar/diminuir tamanho da fonte em toda a tela).
        /// </summary>
        private void AbrirMenuSetup()
        {
            using var setupDialog = new Form
            {
                Text = "Setup — QA & Acessibilidade",
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Size = new Size(360, 310),
                BackColor = Theme.IsDarkMode ? Color.FromArgb(20, 22, 30) : Color.FromArgb(245, 240, 230),
                ShowInTaskbar = false,
                TopMost = true
            };

            // Posiciona logo abaixo do botão Setup na barra superior
            Point btnScreenPos = btnSetup.PointToScreen(new Point(0, btnSetup.Height + 4));
            int x = Math.Min(btnScreenPos.X, Screen.FromControl(this).WorkingArea.Right - setupDialog.Width - 10);
            int y = Math.Max(btnScreenPos.Y, Screen.FromControl(this).WorkingArea.Top + 10);
            setupDialog.Location = new Point(x, y);

            setupDialog.Paint += (s, e) =>
            {
                using var borderPen = new Pen(Theme.Gold, 2);
                e.Graphics.DrawRectangle(borderPen, 1, 1, setupDialog.Width - 2, setupDialog.Height - 2);
            };

            var lblHeader = new Label
            {
                Text = "⚙️ CONFIGURAÇÕES (SETUP)",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Theme.Gold,
                Location = new Point(16, 12),
                AutoSize = true
            };

            var btnFecharPopup = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Theme.Gold,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(28, 26),
                Location = new Point(320, 8),
                Cursor = Cursors.Hand
            };
            btnFecharPopup.FlatAppearance.BorderSize = 0;
            btnFecharPopup.Click += (s, e) => setupDialog.Close();

            // ─────────────────────────────────────────────────────
            // SEÇÃO 1: QA — AÇÕES DE TESTE DE SOFTWARE
            // ─────────────────────────────────────────────────────
            var lblSecaoQa = new Label
            {
                Text = "🧪 MODO DE TESTE DE SOFTWARE (QA)",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Theme.DimText,
                Location = new Point(16, 46),
                AutoSize = true
            };

            var btnAbrirQa = new TronusButton
            {
                Text = "🧪 QA: Abrir Central de Testes",
                Size = new Size(328, 38),
                Location = new Point(16, 68),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnAbrirQa.Click += (s, e) =>
            {
                setupDialog.Close();
                LoadControl(new Views.UcTestRunner(autoRun: false));
            };

            var btnExecutarQaDireto = new Button
            {
                Text = "▶ QA: Executar Todos os Testes Agora",
                Size = new Size(328, 32),
                Location = new Point(16, 112),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Theme.Gold,
                BackColor = Theme.IsDarkMode ? Color.FromArgb(30, 36, 48) : Color.FromArgb(235, 228, 210),
                Cursor = Cursors.Hand
            };
            btnExecutarQaDireto.FlatAppearance.BorderColor = Theme.Gold;
            btnExecutarQaDireto.FlatAppearance.BorderSize = 1;
            btnExecutarQaDireto.Click += (s, e) =>
            {
                setupDialog.Close();
                LoadControl(new Views.UcTestRunner(autoRun: true));
            };

            // ─────────────────────────────────────────────────────
            // SEÇÃO 2: ACESSIBILIDADE — TAMANHO DA FONTE
            // ─────────────────────────────────────────────────────
            var lblSecaoFonte = new Label
            {
                Text = "🔠 ACESSIBILIDADE — TAMANHO DA FONTE",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Theme.DimText,
                Location = new Point(16, 160),
                AutoSize = true
            };

            var lblEscalaAtual = new Label
            {
                Text = $"Tamanho Atual: {(int)Math.Round(Theme.FontScale * 100)}%",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Theme.PrimaryText,
                Location = new Point(16, 182),
                AutoSize = true
            };

            var btnDiminuirFonte = new Button
            {
                Text = "A-  Diminuir",
                Size = new Size(104, 38),
                Location = new Point(16, 210),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Theme.Gold,
                BackColor = Theme.IsDarkMode ? Color.FromArgb(32, 34, 44) : Color.FromArgb(235, 228, 210),
                Cursor = Cursors.Hand
            };
            btnDiminuirFonte.FlatAppearance.BorderColor = Theme.Gold;

            var btnPadraoFonte = new Button
            {
                Text = "100% Padrão",
                Size = new Size(104, 38),
                Location = new Point(128, 210),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Theme.PrimaryText,
                BackColor = Theme.IsDarkMode ? Color.FromArgb(32, 34, 44) : Color.FromArgb(235, 228, 210),
                Cursor = Cursors.Hand
            };
            btnPadraoFonte.FlatAppearance.BorderColor = Theme.BorderColor;

            var btnAumentarFonte = new Button
            {
                Text = "A+  Aumentar",
                Size = new Size(104, 38),
                Location = new Point(240, 210),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Theme.Gold,
                BackColor = Theme.IsDarkMode ? Color.FromArgb(32, 34, 44) : Color.FromArgb(235, 228, 210),
                Cursor = Cursors.Hand
            };
            btnAumentarFonte.FlatAppearance.BorderColor = Theme.Gold;

            void AtualizarLabelEscala()
            {
                int pct = (int)Math.Round(Theme.FontScale * 100);
                string desc = pct == 100 ? "(Padrão)" : pct > 100 ? "(Ampliado)" : "(Compacto)";
                lblEscalaAtual.Text = $"Tamanho Atual: {pct}% {desc}";
            }

            btnDiminuirFonte.Click += (s, e) =>
            {
                Theme.DecreaseFontSize();
                AtualizarLabelEscala();
            };

            btnPadraoFonte.Click += (s, e) =>
            {
                Theme.ResetFontSize();
                AtualizarLabelEscala();
            };

            btnAumentarFonte.Click += (s, e) =>
            {
                Theme.IncreaseFontSize();
                AtualizarLabelEscala();
            };

            var lblDicaAtalho = new Label
            {
                Text = "Dica: Atalhos rápidos Ctrl + (+) / Ctrl + (-) / F12 (QA)",
                Font = new Font("Segoe UI", 7.8F, FontStyle.Italic),
                ForeColor = Theme.DimText,
                Location = new Point(16, 262),
                AutoSize = true
            };

            AtualizarLabelEscala();

            setupDialog.Controls.Add(lblHeader);
            setupDialog.Controls.Add(btnFecharPopup);
            setupDialog.Controls.Add(lblSecaoQa);
            setupDialog.Controls.Add(btnAbrirQa);
            setupDialog.Controls.Add(btnExecutarQaDireto);
            setupDialog.Controls.Add(lblSecaoFonte);
            setupDialog.Controls.Add(lblEscalaAtual);
            setupDialog.Controls.Add(btnDiminuirFonte);
            setupDialog.Controls.Add(btnPadraoFonte);
            setupDialog.Controls.Add(btnAumentarFonte);
            setupDialog.Controls.Add(lblDicaAtalho);

            setupDialog.ShowDialog(this);
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
            if (keyData == (Keys.Control | Keys.Q) || keyData == Keys.F12)
            {
                LoadControl(new Views.UcTestRunner());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Oemplus) || keyData == (Keys.Control | Keys.Add))
            {
                Theme.IncreaseFontSize();
                return true;
            }
            if (keyData == (Keys.Control | Keys.OemMinus) || keyData == (Keys.Control | Keys.Subtract))
            {
                Theme.DecreaseFontSize();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D0) || keyData == (Keys.Control | Keys.NumPad0))
            {
                Theme.ResetFontSize();
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
