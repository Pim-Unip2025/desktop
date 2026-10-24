using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TronusDesktop.UI
{
    /// <summary>
    /// Gerenciador central de temas e acessibilidade visual da aplicação Tronus Desktop.
    /// Oferece alternância dinâmica entre o Modo Escuro (Dark Fantasy) e o Modo Claro (Parchment Acessível de Alto Contraste).
    /// </summary>
    public static class Theme
    {
        public static bool IsDarkMode { get; private set; } = true;
        public static event Action? ThemeChanged;

        // ═══════════════════════════════════════════════════
        // CORES DINÂMICAS DO TEMA
        // ═══════════════════════════════════════════════════
        public static Color Background { get; private set; } = Color.FromArgb(26, 26, 26);
        public static Color CardBackground { get; private set; } = Color.FromArgb(127, 0, 0, 0);
        public static Color PrimaryText { get; private set; } = Color.FromArgb(240, 230, 200);
        public static Color DimText { get; private set; } = Color.FromArgb(168, 144, 96);
        public static Color Gold { get; private set; } = Color.FromArgb(212, 175, 55);
        public static Color ButtonPrimaryStart { get; private set; } = Color.FromArgb(26, 107, 74);
        public static Color ButtonPrimaryEnd { get; private set; } = Color.FromArgb(26, 74, 107);
        public static Color ButtonPrimaryHoverStart { get; private set; } = Color.FromArgb(34, 136, 94);
        public static Color ButtonPrimaryHoverEnd { get; private set; } = Color.FromArgb(30, 92, 133);
        public static Color BorderColor { get; private set; } = Color.FromArgb(76, 212, 175, 55);

        // Cores Complementares de UI e Acessibilidade
        public static Color TopBarBackground { get; private set; } = Color.FromArgb(18, 16, 14);
        public static Color InputBackground { get; private set; } = Color.FromArgb(40, 40, 40);
        public static Color InputForeground { get; private set; } = Color.White;
        public static Color InputBorder { get; private set; } = Color.FromArgb(70, 70, 70);
        public static Color CardBorder { get; private set; } = Color.FromArgb(76, 212, 175, 55);

        static Theme()
        {
            ApplyDarkPalette();
        }

        /// <summary>
        /// Alterna entre modo escuro e modo claro de acessibilidade.
        /// </summary>
        public static void ToggleTheme()
        {
            SetDarkMode(!IsDarkMode);
        }

        /// <summary>
        /// Define explicitamente o modo visual ativo.
        /// </summary>
        public static void SetDarkMode(bool dark)
        {
            IsDarkMode = dark;
            if (IsDarkMode)
            {
                ApplyDarkPalette();
            }
            else
            {
                ApplyLightPalette();
            }

            ThemeChanged?.Invoke();
        }

        private static void ApplyDarkPalette()
        {
            Background = Color.FromArgb(26, 26, 26);
            CardBackground = Color.FromArgb(127, 0, 0, 0);
            PrimaryText = Color.FromArgb(240, 230, 200);
            DimText = Color.FromArgb(168, 144, 96);
            Gold = Color.FromArgb(212, 175, 55);
            ButtonPrimaryStart = Color.FromArgb(26, 107, 74);
            ButtonPrimaryEnd = Color.FromArgb(26, 74, 107);
            ButtonPrimaryHoverStart = Color.FromArgb(34, 136, 94);
            ButtonPrimaryHoverEnd = Color.FromArgb(30, 92, 133);
            BorderColor = Color.FromArgb(76, 212, 175, 55);

            TopBarBackground = Color.FromArgb(18, 16, 14);
            InputBackground = Color.FromArgb(40, 40, 40);
            InputForeground = Color.White;
            InputBorder = Color.FromArgb(70, 70, 70);
            CardBorder = Color.FromArgb(76, 212, 175, 55);
        }

        private static void ApplyLightPalette()
        {
            // Paleta Pergaminho / Alto Contraste para Acessibilidade
            Background = Color.FromArgb(244, 241, 234);
            CardBackground = Color.FromArgb(240, 255, 255, 255);
            PrimaryText = Color.FromArgb(28, 25, 22); // Contraste máximo WCAG AAA
            DimText = Color.FromArgb(100, 80, 60);
            Gold = Color.FromArgb(170, 120, 15); // Ouro envelhecido perfeitamente legível em fundo claro
            ButtonPrimaryStart = Color.FromArgb(22, 115, 75);
            ButtonPrimaryEnd = Color.FromArgb(24, 85, 120);
            ButtonPrimaryHoverStart = Color.FromArgb(28, 135, 90);
            ButtonPrimaryHoverEnd = Color.FromArgb(30, 105, 145);
            BorderColor = Color.FromArgb(160, 170, 120, 15);

            TopBarBackground = Color.FromArgb(235, 230, 220);
            InputBackground = Color.FromArgb(255, 255, 255);
            InputForeground = Color.FromArgb(20, 20, 20);
            InputBorder = Color.FromArgb(170, 170, 170);
            CardBorder = Color.FromArgb(160, 170, 120, 15);
        }

        /// <summary>
        /// Aplica as cores do tema ativo de forma recursiva a qualquer árvore de controles do Windows Forms.
        /// Preserva dados digitados e estado de foco dos componentes.
        /// </summary>
        public static void ApplyTheme(Control root)
        {
            if (root == null) return;

            // Tratamento de UserControl e Form
            if (root is UserControl or Form)
            {
                root.BackColor = Background;

                string bgPath = Path.Combine(Application.StartupPath, "img", "fundo_reinos.jpg");
                if (IsDarkMode && File.Exists(bgPath))
                {
                    if (root.BackgroundImage == null && root.Tag?.ToString() != "no_bg")
                    {
                        try
                        {
                            root.BackgroundImage = Image.FromFile(bgPath);
                            root.BackgroundImageLayout = ImageLayout.Stretch;
                        }
                        catch { }
                    }
                }
                else if (!IsDarkMode)
                {
                    // No modo claro acessível, remove o papel de parede escuro para focar no conteúdo de alto contraste
                    root.BackgroundImage = null;
                }

                root.Invalidate();
            }

            // Tratamento de controles específicos
            if (root is TextBox tb)
            {
                tb.BackColor = InputBackground;
                tb.ForeColor = InputForeground;
            }
            else if (root is Label lbl)
            {
                // Mapeia estilos originais de títulos e subtítulos
                if (lbl.Font.Bold || lbl.ForeColor == Color.FromArgb(212, 175, 55) || lbl.ForeColor == Color.FromArgb(170, 120, 15))
                {
                    lbl.ForeColor = Gold;
                }
                else if (lbl.ForeColor == Color.FromArgb(168, 144, 96) || lbl.ForeColor == Color.FromArgb(100, 80, 60))
                {
                    lbl.ForeColor = DimText;
                }
                else if (lbl.ForeColor == Color.FromArgb(240, 230, 200) || lbl.ForeColor == Color.White || lbl.ForeColor == Color.FromArgb(28, 25, 22))
                {
                    lbl.ForeColor = PrimaryText;
                }
            }
            else if (root is TronusPanel tp)
            {
                tp.Invalidate();
            }
            else if (root is TronusButton tbtn)
            {
                tbtn.Invalidate();
            }
            else if (root is PasswordToggleButton ptb)
            {
                ptb.BackColor = InputBackground;
                ptb.Invalidate();
            }
            else if (root is AccessibilityThemeButton ab)
            {
                ab.Invalidate();
            }
            else if (root is Panel p && (p.Height == 75 || p.Height == 40 || p.Dock == DockStyle.Top))
            {
                p.BackColor = TopBarBackground;
                p.Invalidate();
            }
            else if (root is DataGridView dgv)
            {
                dgv.BackgroundColor = Background;
                dgv.DefaultCellStyle.BackColor = InputBackground;
                dgv.DefaultCellStyle.ForeColor = PrimaryText;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = TopBarBackground;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Gold;
                dgv.EnableHeadersVisualStyles = false;
                dgv.Invalidate();
            }

            foreach (Control child in root.Controls)
            {
                ApplyTheme(child);
            }
        }
    }
}
