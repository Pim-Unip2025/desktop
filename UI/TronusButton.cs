using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TronusDesktop.UI
{
    public class TronusButton : Button
    {
        private int borderRadius = 6;
        private bool isHovered = false;

        public TronusButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Size = new Size(200, 45);
            this.BackColor = Color.Transparent;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rectSurface = new Rectangle(0, 0, this.Width, this.Height);
            int radius = borderRadius;

            GraphicsPath pathSurface = new GraphicsPath();
            pathSurface.AddArc(rectSurface.X, rectSurface.Y, radius, radius, 180, 90);
            pathSurface.AddArc(rectSurface.Width - radius, rectSurface.Y, radius, radius, 270, 90);
            pathSurface.AddArc(rectSurface.Width - radius, rectSurface.Height - radius, radius, radius, 0, 90);
            pathSurface.AddArc(rectSurface.X, rectSurface.Height - radius, radius, radius, 90, 90);
            pathSurface.CloseFigure();

            this.Region = new Region(pathSurface);

            Color startColor = isHovered ? Theme.ButtonPrimaryHoverStart : Theme.ButtonPrimaryStart;
            Color endColor = isHovered ? Theme.ButtonPrimaryHoverEnd : Theme.ButtonPrimaryEnd;

            using (LinearGradientBrush brush = new LinearGradientBrush(rectSurface, startColor, endColor, 135F))
            {
                pevent.Graphics.FillPath(brush, pathSurface);
            }

            TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, rectSurface, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
