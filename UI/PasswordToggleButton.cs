using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TronusDesktop.UI
{
    /// <summary>
    /// Botão interativo com ícone de olho renderizado via GDI+ para alternar a visibilidade de senhas em TextBoxes.
    /// Suporta estados de hover, clique, tooltips e preservação automática de foco no campo de texto.
    /// </summary>
    public class PasswordToggleButton : Control
    {
        private readonly TextBox _targetBox;
        private readonly ToolTip _toolTip;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private bool _isPasswordVisible = false;

        public PasswordToggleButton(TextBox targetBox)
        {
            _targetBox = targetBox ?? throw new ArgumentNullException(nameof(targetBox));
            this.Size = new Size(36, 29);
            this.Cursor = Cursors.Hand;
            this.DoubleBuffered = true;
            this.TabStop = false; // Mantém o ciclo natural de Tab do formulário
            this.BackColor = Color.FromArgb(40, 40, 40);

            _toolTip = new ToolTip
            {
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Theme.PrimaryText,
                AutoPopDelay = 3000
            };
            UpdateToolTip();

            this.Click += (s, e) => ToggleVisibility();
        }

        public bool IsPasswordVisible => _isPasswordVisible;

        public void Reset()
        {
            _isPasswordVisible = false;
            _targetBox.PasswordChar = '•';
            UpdateToolTip();
            this.Invalidate();
        }

        public void ToggleVisibility()
        {
            _isPasswordVisible = !_isPasswordVisible;
            _targetBox.PasswordChar = _isPasswordVisible ? '\0' : '•';

            UpdateToolTip();
            this.Invalidate();

            if (_targetBox.CanFocus)
            {
                _targetBox.Focus();
                _targetBox.SelectionStart = _targetBox.Text.Length;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _toolTip?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateToolTip()
        {
            _toolTip.SetToolTip(this, _isPasswordVisible ? "Ocultar senha" : "Ver senha");
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
            _isPressed = false;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _isPressed = true;
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isPressed = false;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fundo integrado ao tema dos campos de texto (claro ou escuro)
            Color baseInputBg = Theme.InputBackground;
            Color bg = _isPressed
                ? (Theme.IsDarkMode ? Color.FromArgb(28, 28, 28) : Color.FromArgb(220, 220, 220))
                : (_isHovered ? (Theme.IsDarkMode ? Color.FromArgb(52, 52, 52) : Color.FromArgb(238, 238, 238)) : baseInputBg);

            Color borderColor = _isHovered ? Theme.Gold : Theme.InputBorder;

            using (SolidBrush bgBrush = new SolidBrush(bg))
            {
                g.FillRectangle(bgBrush, 0, 0, Width - 1, Height - 1);
            }

            using (Pen borderPen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            }

            // Cores do ícone conforme estado
            Color iconColor = _isPasswordVisible
                ? Theme.Gold
                : (_isHovered ? Theme.PrimaryText : Theme.DimText);

            float cx = Width / 2f;
            float cy = Height / 2f;

            using (Pen pen = new Pen(iconColor, 1.8f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Curvas do olho (almond shape)
                g.DrawBezier(pen, cx - 9, cy, cx - 4, cy - 6, cx + 4, cy - 6, cx + 9, cy);
                g.DrawBezier(pen, cx - 9, cy, cx - 4, cy + 6, cx + 4, cy + 6, cx + 9, cy);

                if (_isPasswordVisible)
                {
                    // Pupila iluminada e ponto de reflexo
                    using (SolidBrush pupilBrush = new SolidBrush(iconColor))
                    {
                        g.FillEllipse(pupilBrush, cx - 3.5f, cy - 3.5f, 7f, 7f);
                    }
                    using (SolidBrush glowBrush = new SolidBrush(Color.White))
                    {
                        g.FillEllipse(glowBrush, cx - 1.5f, cy - 2.5f, 2.5f, 2.5f);
                    }
                }
                else
                {
                    // Pupila menor central
                    using (SolidBrush pupilBrush = new SolidBrush(iconColor))
                    {
                        g.FillEllipse(pupilBrush, cx - 2.5f, cy - 2.5f, 5f, 5f);
                    }

                    // Linha diagonal cortando o olho (indicando que a senha está oculta)
                    using (Pen clearPen = new Pen(bg, 3f))
                    {
                        clearPen.StartCap = LineCap.Round;
                        clearPen.EndCap = LineCap.Round;
                        g.DrawLine(clearPen, cx - 9.5f, cy - 6.5f, cx + 9.5f, cy + 6.5f);
                    }

                    using (Pen slashPen = new Pen(iconColor, 1.8f))
                    {
                        slashPen.StartCap = LineCap.Round;
                        slashPen.EndCap = LineCap.Round;
                        g.DrawLine(slashPen, cx - 10f, cy - 7f, cx + 10f, cy + 7f);
                    }
                }
            }
        }
    }
}
