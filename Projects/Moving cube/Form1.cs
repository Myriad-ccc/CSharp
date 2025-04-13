using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Moving_Square
{
    public partial class Form1 : Form
    {
        Timer moveTimer = new Timer();
        Color defaultSquareColor = Color.CornflowerBlue;
        Color OutOfBoundColor = Color.IndianRed;
        PlayerSquare playerSquare = new PlayerSquare(new Rectangle(new Point(450, 550), new Size(50, 50)), 5, Color.CornflowerBlue);
        int OutOfBoundMoves = 0;
        bool OutOfBoundsMove;

        public Form1()
        {
            InitializeComponent();

            Width = 1000;
            Height = 800;
            BackColor = Color.FromArgb(255, 35, 35, 35);

            DoubleBuffered = true;
            KeyPreview = true;

            moveTimer.Interval = 16;
            moveTimer.Tick += MoveTimer_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Paint += Form1_Paint;

            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;

            moveTimer.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs eventArgs)
        {
            Graphics g = eventArgs.Graphics;

            ControlPaint.DrawBorder(g, new Rectangle(new Point(5, 5), new Size(ClientSize.Width - 10, ClientSize.Height - 10)), Color.WhiteSmoke, ButtonBorderStyle.Solid);

            using (Brush playerSquareBrush = new SolidBrush(playerSquare.Color))
            {
                g.FillRectangle(playerSquareBrush, playerSquare.Rectangle);
            }
        }


        private bool IsWKeyDown = false;
        private bool IsSKeyDown = false;
        private bool IsDKeyDown = false;
        private bool IsAKeyDown = false;
        private void Form1_KeyDown(object sender, KeyEventArgs eventArgs)
        {
            if (eventArgs.KeyCode == Keys.W && !IsWKeyDown)
            {
                IsWKeyDown = true;
            }
            else if (eventArgs.KeyCode == Keys.S && !IsSKeyDown)
            {
                IsSKeyDown = true;
            }
            else if (eventArgs.KeyCode == Keys.D && !IsDKeyDown)
            {
                IsDKeyDown = true;
            }
            else if (eventArgs.KeyCode == Keys.A && !IsAKeyDown)
            {
                IsAKeyDown = true;
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs eventArgs)
        {
            if (eventArgs.KeyCode == Keys.W)
            {
                IsWKeyDown = false;
            }
            else if (eventArgs.KeyCode == Keys.S)
            {
                IsSKeyDown = false;
            }
            else if (eventArgs.KeyCode == Keys.D)
            {
                IsDKeyDown = false;
            }
            else if (eventArgs.KeyCode == Keys.A)
            {
                IsAKeyDown = false;
            }
        }

        private async void MoveTimer_Tick(object sender, EventArgs eventArgs)
        {
            Point position = playerSquare.Rectangle.Location;
            int dx = 0;
            int dy = 0;

            if (IsWKeyDown) dy -= playerSquare.Speed;
            if (IsAKeyDown) dx -= playerSquare.Speed;
            if (IsSKeyDown) dy += playerSquare.Speed;
            if (IsDKeyDown) dx += playerSquare.Speed;

            position.X += dx;
            position.Y += dy;

            if (position.X <= 10 || position.X + playerSquare.Rectangle.Width >= ClientSize.Width - 10 ||
                position.Y  <= 10 || position.Y  + playerSquare.Rectangle.Height >= ClientSize.Height - 10)
            {
                playerSquare.Color = Color.Orange;
            }
            else
            {
                playerSquare.Color = defaultSquareColor;
            }

            OutOfBoundsMove = false;

            if (position.X < 0)
            {
                OutOfBoundsMove = true;
                position.X = (int)(playerSquare.Rectangle.Width * 1.5);
            }
            else if (position.X + playerSquare.Rectangle.Width > ClientSize.Width)
            {
                OutOfBoundsMove = true;
                position.X = ClientSize.Width - (int)(playerSquare.Rectangle.Width * 1.5);
            }
            else if (position.Y < 0)
            {
                OutOfBoundsMove = true;
                position.Y = (int)(playerSquare.Rectangle.Height * 1.5);
            }
            else if (position.Y + playerSquare.Rectangle.Height > ClientSize.Height)
            {
                OutOfBoundsMove = true;
                position.Y = ClientSize.Height - playerSquare.Rectangle.Height - 10;
            }
            if (OutOfBoundsMove)
            {
                OutOfBoundMoves++;
                playerSquare.Color = OutOfBoundColor;
            }
            if (OutOfBoundMoves == 7)
            {
                OutOfBoundMoves = 0;
                this.BackColor = Color.FromArgb(255, 45, 45, 45);
                await Task.Delay(64);
                this.BackColor = Color.FromArgb(255, 35, 35, 35);
                position.X = 450;
                position.Y = 550;
            }

            playerSquare.Rectangle = new Rectangle(position, playerSquare.Rectangle.Size);
            this.Invalidate();
        }
    }
}
