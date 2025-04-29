using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translate_Rotate
{
    public partial class Form1 : Form
    {
        private readonly Random random = new Random();
        private readonly Timer UpdateTimer = new Timer() { Interval = 1000 };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Width = 1000;
            Height = 800;
            BackColor = Color.FromArgb(255, 35, 35, 35);
            DoubleBuffered = true;

            Rectangle rect = new Rectangle() { Size = new Size(1000, 1000) };
            List<Rectangle> rectList = new List<Rectangle>();
            float angle = 0f;
            UpdateTimer.Tick += (s, ev) => { angle += 5f; if(angle >= 360f) angle = 0f; this.Invalidate(); };
            UpdateTimer.Start();

            this.Paint += (s, ev) =>
            {
                Graphics g = ev.Graphics;

                float cx = this.ClientSize.Width / 2f;
                float cy = this.ClientSize.Height / 2f;

                GraphicsState state = g.Save();

                g.TranslateTransform(cx, cy);
                g.RotateTransform(angle);

                rectList.Clear();
                for (int i = rect.Width = rect.Height; i > 1; i -= 50)
                { rectList.Add(new Rectangle(rect.Location, new Size(i, i))); }
                foreach (Rectangle rectg in rectList)
                {
                    for (int i = 0; i < 256; i++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, random.Next(i), random.Next(i), random.Next(i))), -(rectg.Width / 2f), -(rectg.Height / 2f), rectg.Width, rectg.Height);
                    }
                }
            };
        }
    }
}
