using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TronusDesktop.UI
{
    public class TronusPanel : Panel
    {
        private int borderRadius = 8;
        public TronusPanel()
        {
            this.BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rectSurface = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            int radius = borderRadius;

            GraphicsPath pathSurface = new GraphicsPath();
            pathSurface.AddArc(rectSurface.X, rectSurface.Y, radius, radius, 180, 90);
            pathSurface.AddArc(rectSurface.Width - radius, rectSurface.Y, radius, radius, 270, 90);
            pathSurface.AddArc(rectSurface.Width - radius, rectSurface.Height - radius, radius, radius, 0, 90);
            pathSurface.AddArc(rectSurface.X, rectSurface.Height - radius, radius, radius, 90, 90);
            pathSurface.CloseFigure();

            using (SolidBrush brush = new SolidBrush(Theme.CardBackground))
            {
                e.Graphics.FillPath(brush, pathSurface);
            }
            using (Pen pen = new Pen(Color.FromArgb(20, 255, 255, 255), 1))
            {
                e.Graphics.DrawPath(pen, pathSurface);
            }
        }
    }
}
