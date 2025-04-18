using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace StopwatchV2
{
    public partial class Form1 : Form
    {
        private readonly Timer GlobalRefreshTimer = new Timer { Interval = 16 };
        private readonly Stopwatch Stopwatch = new Stopwatch();
        private string StopwatchText;

        public Form1()
        {
            InitializeComponent();

            Width = 400;
            Height = 200;
            BackColor = Color.FromArgb(255, 35, 35, 35);
            DoubleBuffered = true;

            StopwatchText = "0.0";

            GlobalRefreshTimer.Tick += (s, ev) =>
            {
                StopwatchText = $"{Stopwatch.Elapsed.TotalSeconds:F1}";
                this.Invalidate();
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Paint += Form1_Paint;
            this.KeyPreview = true;
            this.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Space)
                {
                    if (Stopwatch.IsRunning)
                        Stopwatch.Stop();
                    else
                        Stopwatch.Start();
                }
                else if (ev.KeyCode == Keys.R)
                {
                    Stopwatch.Restart();
                }
            };
        }

        private void Form1_Paint(object sender, PaintEventArgs eventArgs)
        {
            Graphics g = eventArgs.Graphics;

            using(Font font = new Font("Comic Sans MS", 36f))
            using (Brush brush = new SolidBrush(Color.RoyalBlue))
            {
                g.DrawString(StopwatchText, font, brush, 5, 5);
                font.Dispose();
                brush.Dispose();
                
            } 
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GlobalRefreshTimer.Start();
        }
    }
}
