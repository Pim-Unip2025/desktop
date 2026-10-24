using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TronusDesktop.UI
{
    /// <summary>
    /// Botão de acessibilidade de alta precisão para alternar entre Modo Escuro (Dark Fantasy) e Modo Claro (Parchment Acessível).
    /// Utiliza ícones vetoriais anti-aliasing nativos (Sol e Lua Crescente) desenhados via GDI+,
    /// garantindo renderização impecável sem dependência de suporte a emojis de fontes do sistema.
    /// </summary>
    public class AccessibilityThemeButton : Button
    {
        private readonly ToolTip _toolTip;
        private bool _isHovered = false;

        public AccessibilityThemeButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Size = new Size(130, 30);
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
                this.Text = "Modo Claro";
                _toolTip.SetToolTip(this, "Acessibilidade: Alternar para Modo Claro de Alto Contraste (Ctrl+T)");
            }
            else
            {
                this.Text = "Modo Escuro";
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

            int width = this.Width;
            int height = this.Height;
            Rectangle rect = new Rectangle(0, 0, width - 1, height - 1);
            int radius = 14;

            // 1. Fundo em formato Pill (Cápsula)
            using (var path = new GraphicsPath())
            {
                int d = radius * 2;
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();

                Color bg = Theme.IsDarkMode
                    ? (_isHovered ? Color.FromArgb(60, 52, 40) : Color.FromArgb(36, 32, 28))
                    : (_isHovered ? Color.FromArgb(222, 214, 200) : Color.FromArgb(246, 241, 232));

                using (var brush = new SolidBrush(bg))
                {
                    g.FillPath(brush, path);
                }

                // Borda de contraste
                Color border = Theme.IsDarkMode
                    ? (_isHovered ? Color.FromArgb(255, 215, 0) : Color.FromArgb(212, 175, 55))
                    : (_isHovered ? Color.FromArgb(140, 95, 10) : Color.FromArgb(170, 120, 15));

                using (var pen = new Pen(border, 1.2f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // 2. Ícone Vetorial Nativo de Alta Definição (Sem risco de glifos corrompidos)
            int iconCx = 18;
            int iconCy = height / 2;
            Color iconColor = Theme.IsDarkMode ? Color.FromArgb(255, 215, 0) : Color.FromArgb(170, 120, 15);

            if (Theme.IsDarkMode)
            {
                // Ícone Vetorial de Sol (Indica a ação de ir para o Modo Claro)
                using (var brush = new SolidBrush(iconColor))
                {
                    g.FillEllipse(brush, iconCx - 4, iconCy - 4, 8, 8);
                }

                using (var rayPen = new Pen(iconColor, 1.4f))
                {
                    rayPen.StartCap = LineCap.Round;
                    rayPen.EndCap = LineCap.Round;
                    int r1 = 6;
                    int r2 = 9;
                    for (int i = 0; i < 8; i++)
                    {
                        double angle = i * Math.PI / 4.0;
                        float x1 = (float)(iconCx + r1 * Math.Cos(angle));
                        float y1 = (float)(iconCy + r1 * Math.Sin(angle));
                        float x2 = (float)(iconCx + r2 * Math.Cos(angle));
                        float y2 = (float)(iconCy + r2 * Math.Sin(angle));
                        g.DrawLine(rayPen, x1, y1, x2, y2);
                    }
                }
            }
            else
            {
                // Ícone Vetorial de Lua Crescente (Indica a ação de ir para o Modo Escuro)
                using (var brush = new SolidBrush(iconColor))
                using (var pathOuter = new GraphicsPath())
                {
                    pathOuter.AddEllipse(iconCx - 7, iconCy - 7, 14, 14);

                    using (var pathInner = new GraphicsPath())
                    {
                        pathInner.AddEllipse(iconCx - 3.5f, iconCy - 7.5f, 13, 13);

                        using (var moonRegion = new Region(pathOuter))
                        {
                            moonRegion.Exclude(pathInner);
                            g.FillRegion(brush, moonRegion);
                        }
                    }
                }
            }

            // 3. Tipografia de Alto Contraste
            Color textColor = Theme.IsDarkMode ? Color.FromArgb(240, 230, 200) : Color.FromArgb(32, 28, 24);
            using (var textBrush = new SolidBrush(textColor))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(this.Text, this.Font, textBrush, new RectangleF(34, 0, width - 36, height), sf);
            }
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
