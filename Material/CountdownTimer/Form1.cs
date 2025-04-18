using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Timer_Length_Display
{
    public partial class Form1 : Form
    {
        private readonly Timer GlobalTimer = new Timer(); // initialize a timer
        private TimeSpan Duration;
        private DateTime StartTime;
        private string TimerDisplay;
        private bool initialWait;

        Font timerDisplayFont = new Font("Comic Sans MS", 36f);
        Brush timerDisplayBrush = new SolidBrush(Color.PaleGreen);
        Point timerDisplayPosition = new Point(3, 3);

        public Form1()
        {
            InitializeComponent();

            Width = 400;
            Height = 130;
            BackColor = Color.FromArgb(255, 35, 35, 35);
            DoubleBuffered = true;

            GlobalTimer.Interval = 16; // updates every 16 ticks - 62.5 times per second (for ~60 fps apps)
            GlobalTimer.Tick += (s, ev) =>
            {
                string newDisplay;

                TimeSpan elapsedTime = DateTime.Now - StartTime; // time from start to now
                TimeSpan remainingTime = Duration - elapsedTime; // self-explanatory

                if (remainingTime <= TimeSpan.Zero) // if the timer is done
                {
                    GlobalTimer.Stop(); // stops timer when it's over
                    newDisplay = "Timer Finished"; // on timer end text
                }
                else
                {
                    newDisplay = $"{remainingTime.TotalSeconds:F1}"; // remaining time is formatted with one number after the decimal point
                }

                if (newDisplay != TimerDisplay)
                {
                    TimerDisplay = newDisplay;
                    this.Invalidate(); // only updates timer display if it's different from its' previous state
                }
            };
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            this.Paint += Form1_Paint;

            TimerDisplay = "Starting.."; // element that shows timer tick down
            this.Invalidate(); // redraw initial statement

            if (!initialWait)
            {
                initialWait = true;
                await Task.Delay(1000);
            }
            TimerDisplay = "6.0";
            this.Invalidate();
            Duration = TimeSpan.FromSeconds(6);
            StartTime = DateTime.Now;

            GlobalTimer.Start();
        }
        private void Form1_Paint(object sender, PaintEventArgs eventArgs)
        {
            Graphics g = eventArgs.Graphics;

            g.DrawString(TimerDisplay ?? "N/A", timerDisplayFont, timerDisplayBrush, timerDisplayPosition);
        }
    }
}
