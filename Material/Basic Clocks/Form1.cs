using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheBasicestOfClocks
{
    public partial class Form1 : Form
    {
        private readonly Timer UpdateTime = new Timer { Interval = 1000 }; // 1 second interval between updates
        private readonly Label timeLabel;

        public Form1()
        {
            InitializeComponent();

            timeLabel = new Label();
            timeLabel.Font = new Font("Comic Sans MS", 40f);
            timeLabel.ForeColor = Color.IndianRed;
            timeLabel.Text = "Current Time";
            timeLabel.AutoSize = true;
            timeLabel.Location = new Point(1, 1);
            Controls.Add(timeLabel);

            UpdateTime.Tick += UpdateTime_Tick;
            UpdateTime.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Width = 700;
            Height = 120;
            BackColor = Color.FromArgb(255, 35, 35, 35);
        }
        private void UpdateTime_Tick(object sender, EventArgs eventArgs)
        {
            timeLabel.Text = $"{DateTime.Now}"; // display current time, uses default format
        }
    }
}
