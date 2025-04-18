using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;   

namespace Moving_Square
{
    public class CustomButton : Control
    {
        private bool IsPressed;
        private bool IsHovering;

        public CustomButton()
        {
            this.Size = new Size(180, 60);
            this.Font = new Font("Gilroy-Medium", 24f);
            this.BackColor = Color.FromArgb(255, 25, 25, 25);
            this.ForeColor = Color.FromArgb(255, 225, 225, 225);
            this.DoubleBuffered = true;

            this.MouseEnter += (s, ev) => { IsHovering = true; this.Invalidate(); };
            this.MouseLeave += (s, ev) => { IsHovering = false; IsPressed = false; this.Invalidate(); };
            this.MouseDown += (s, ev) => { IsPressed = true; this.Invalidate(); };
            this.MouseUp += (s, ev) => { IsPressed = false; this.Invalidate(); };
        }
        protected override void OnPaint(PaintEventArgs eventArgs)
        {
            base.OnPaint(eventArgs);

            Graphics g = eventArgs.Graphics;

            Color fill = IsPressed ? Color.FromArgb(255, 73, 73, 73)
                : IsHovering ? Color.FromArgb(255, 43, 43, 43)
                : this.BackColor;

            using (Brush fillBrush = new SolidBrush(fill))
            {
                g.FillRectangle(fillBrush, this.ClientRectangle); 
            }
            using (Pen borderPen = new Pen(Color.FromArgb(255, 185, 185, 185)))
            {
                g.DrawRectangle(borderPen, new Rectangle(new Point(0, 0), new Size(this.Width - 1, this.Height - 1)));
            }
            TextRenderer.DrawText(g, this.Text, this.Font, this.ClientRectangle, this.ForeColor, 
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            this.SetAutoSizeMode(AutoSizeMode.GrowOnly);
        }
    }
}
