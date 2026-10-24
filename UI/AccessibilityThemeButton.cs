using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TronusDesktop.UI
{
    /// <summary>
    /// Botão de acessibilidade para alternar entre Modo Escuro (Dark Fantasy) e Modo Claro (Parchment Acessível de Alto Contraste).
    /// Atualiza automaticamente ícone, texto, cores e tooltip em resposta a mudanças de tema.
    /// </summary>
    public class AccessibilityThemeButton : Button
    {
        private readonly ToolTip _toolTip;
        private bool _isHovered = false;

        public AccessibilityThemeButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Size = new Size(125, 30);
            this.Cursor = Cursors.Hand;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.DoubleBuffered = true;
            this.TabStop = false;

            _toolTip = new ToolTip
            {
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                AutoPopDelay = 4000
            };

            this.Click += (s, e) => Theme.ToggleTheme();

            Theme.ThemeChanged += OnThemeChanged;
            UpdateVisuals();
        }

        private void OnThemeChanged()
        {
            UpdateVisuals();
            this.Invalidate();
        }

        private void UpdateVisuals()
        {
            if (Theme.IsDarkMode)
            {
                this.Text = "☀️ Modo Claro";
                this.ForeColor = Theme.Gold;
                _toolTip.SetToolTip(this, "Acessibilidade: Alternar para Modo Claro de Alto Contraste (Ctrl+T)");
            }
            else
            {
                this.Text = "🌙 Modo Escuro";
                this.ForeColor = Theme.Gold;
                _toolTip.SetToolTip(this, "Acessibilidade: Alternar para Modo Escuro Medieval (Ctrl+T)");
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            int radius = 6;

            using (var path = CreateRoundRect(rect, radius))
            {
                // Fundo do botão com realce dinâmico
                Color bg = _isHovered
                    ? (Theme.IsDarkMode ? Color.FromArgb(50, 212, 175, 55) : Color.FromArgb(40, 170, 120, 15))
                    : (Theme.IsDarkMode ? Color.FromArgb(30, 255, 255, 255) : Color.FromArgb(25, 0, 0, 0));

                using (var brush = new SolidBrush(bg))
                {
                    g.FillPath(brush, path);
                }

                // Borda estilizada
                Color borderColor = _isHovered ? Theme.Gold : Theme.BorderColor;
                using (var pen = new Pen(borderColor, 1.2F))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Texto com ícone centralizado
            TextRenderer.DrawText(
                g,
                this.Text,
                this.Font,
                this.ClientRectangle,
                this.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );
        }

        private static GraphicsPath CreateRoundRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Theme.ThemeChanged -= OnThemeChanged;
                _toolTip?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
