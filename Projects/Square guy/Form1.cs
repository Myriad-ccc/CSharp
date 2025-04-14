using System;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Moving_Square
{
    public partial class Form1 : Form
    {
        private Random random = new Random();

        private readonly Timer MoveTimer = new Timer();
        private readonly Timer ErrorFlashTimer = new Timer();
        private readonly Timer PepperBoostTimer = new Timer();
        private readonly Timer DespawnPepperBoostTimer = new Timer();
        private bool OnErrorFlash = false;
        private readonly Timer PositionResetTimer = new Timer();
        private readonly PlayerSquare playerSquare = new PlayerSquare(new Rectangle(new Point(450, 550), new Size(50, 50)), 5, Color.CornflowerBlue, Color.RoyalBlue);
        private int OutOfBoundMoves = 0;
        private bool IsOutOfBounds;

        private const int iconSize = 64;
        PictureBox PepperPowerUp;
        private Image PepperPowerUpImage;
        private int spawnKey = 0;
        private bool LastPowerUpCollected = true;

        public Form1()
        {
            InitializeComponent();

            Width = 1000;
            Height = 800;
            BackColor = Color.FromArgb(255, 35, 35, 35);

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            DoubleBuffered = true;
            KeyPreview = true;

            MoveTimer.Interval = 16;
            MoveTimer.Tick += MoveTimer_Tick;

            ErrorFlashTimer.Interval = 500;
            ErrorFlashTimer.Tick += ErrorFlashTimer_Tick;

            PositionResetTimer.Interval = 2500;
            PositionResetTimer.Tick += PositionResetTimer_Tick;

            PepperBoostTimer.Interval = 5000;
            PepperBoostTimer.Tick += PepperBoostTimer_Tick;

            DespawnPepperBoostTimer.Interval = 4000;
            DespawnPepperBoostTimer.Tick += DespawnPepperBoostTimer_Tick;
        }

        private async void Form1_Load_1(object sender, EventArgs e)
        {
            this.Paint += Form1_Paint;

            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;

            try
            {
                PepperPowerUpImage = await AsyncImageUrlLoad("https://i.postimg.cc/fyyzp62g/pepper2.png");
            }
            catch (Exception)
            {
                var label = new Label
                {
                    Font = new Font("Ariel", 30f),
                    Text = "Could not load image",
                    Location = new Point(ClientSize.Width - 50, ClientSize.Height - 50)
                };
                Controls.Add(label);
            }

            MoveTimer.Start();
        }
        private async Task<Image> AsyncImageUrlLoad(string url)
        {
            HttpClient httpClient = new HttpClient();
            var data = await httpClient.GetByteArrayAsync(url);
            MemoryStream memoryStream = new MemoryStream(data);

            return Image.FromStream(memoryStream);
        }
        private Task<PictureBox> AsyncPowerUpPictureBoxCreate()
        {
            PictureBox pictureBox = new PictureBox();

            pictureBox.Size = new Size(iconSize, iconSize);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Location = new Point(
                    random.Next(20, ClientSize.Width - 19 - iconSize),
                    random.Next(20, ClientSize.Height - 19 - iconSize));

            pictureBox.Image = PepperPowerUpImage;

            return Task.FromResult(pictureBox);
        }

        private void Form1_Paint(object sender, PaintEventArgs eventArgs)
        {
            Graphics g = eventArgs.Graphics;

            using (Brush fillBrush = new SolidBrush(playerSquare.FillColor))
            {
                g.FillRectangle(fillBrush, playerSquare.Rectangle);
                ControlPaint.DrawBorder(g, playerSquare.Rectangle, playerSquare.BorderColor, ButtonBorderStyle.Solid);
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

            if (dx != 0 || dy != 0)
            {
                // Total vector length formula
                double length = Math.Sqrt(dx * dx + dy * dy);

                // Normalize Vector and multiply by speed
                dx = (int)(dx / length * playerSquare.Speed);
                dy = (int)(dy / length * playerSquare.Speed);
            }

            position.X += dx;
            position.Y += dy;

            if (position.X <= 10 || position.X + playerSquare.Rectangle.Width >= ClientSize.Width - 10 ||
                position.Y <= 10 || position.Y + playerSquare.Rectangle.Height >= ClientSize.Height - 10)
            {
                if (!OnErrorFlash)
                {
                    playerSquare.FillColor = Color.FromArgb(255, 250, 144, 45);
                    playerSquare.BorderColor = Color.FromArgb(255, 181, 91, 7);
                }
            }

            IsOutOfBounds = false;

            if (position.X < 0)
            {
                IsOutOfBounds = true;
                position.X = (int)(playerSquare.Rectangle.Width * 0.5);
            }
            else if (position.X + playerSquare.Rectangle.Width > ClientSize.Width)
            {
                IsOutOfBounds = true;
                position.X = ClientSize.Width - (int)(playerSquare.Rectangle.Width * 0.5) - playerSquare.Rectangle.Width;
            }
            else if (position.Y < 0)
            {
                IsOutOfBounds = true;
                position.Y = (int)(playerSquare.Rectangle.Height * 0.5);
            }
            else if (position.Y + playerSquare.Rectangle.Height > ClientSize.Height)
            {
                IsOutOfBounds = true;
                position.Y = ClientSize.Height - (int)(playerSquare.Rectangle.Height * 0.5) - playerSquare.Rectangle.Height;
            }
            if (IsOutOfBounds)
            {
                OutOfBoundMoves++;
                playerSquare.BorderColor = Color.FromArgb(255, 217, 84, 77);
                playerSquare.FillColor = Color.IndianRed;
                ErrorFlashTimer.Stop();
                OnErrorFlash = true;
                ErrorFlashTimer.Start();
            }
            if (OutOfBoundMoves == 10)
            {
                OutOfBoundMoves = 0;

                this.BackColor = Color.FromArgb(255, 40, 40, 40);
                await Task.Delay(160);
                this.BackColor = Color.FromArgb(255, 35, 35, 35);

                PositionResetTimer.Stop();
                playerSquare.BorderColor = Color.IndianRed;
                playerSquare.FillColor = Color.FromArgb(255, 204, 53, 53);
                PositionResetTimer.Start();

                position.X = 450;
                position.Y = 550;
            }

            if (spawnKey != 47)
            {
                spawnKey = random.Next(1, 626);
            }
            else
            {
                if (LastPowerUpCollected)
                {
                    spawnKey = 0;
                    LastPowerUpCollected = false;

                    PepperPowerUp = await AsyncPowerUpPictureBoxCreate();
                    Controls.Add(PepperPowerUp);

                    DespawnPepperBoostTimer.Stop();
                    DespawnPepperBoostTimer.Start();
                }
            }

            if (PepperPowerUp != null && playerSquare.Rectangle.IntersectsWith(
                new Rectangle(
                    new Point(PepperPowerUp.Location.X - 2, PepperPowerUp.Location.Y - 2),
                    new Size(PepperPowerUp.Width + 4, PepperPowerUp.Height + 4))))
            {
                LastPowerUpCollected = true;

                DeletePeperBoost();

                playerSquare.FillColor = Color.MediumSeaGreen;
                playerSquare.Speed = 10;

                PepperBoostTimer.Stop();
                PepperBoostTimer.Start();
            }

            playerSquare.Rectangle = new Rectangle(position, playerSquare.Rectangle.Size);
            this.Invalidate();
        }
        private void ErrorFlashTimer_Tick(object sender, EventArgs eventArgs)
        {
            ErrorFlashTimer.Stop();
            OnErrorFlash = false;

            playerSquare.BorderColor = Color.RoyalBlue;
            playerSquare.FillColor = Color.CornflowerBlue;
        }
        private void PositionResetTimer_Tick(object sender, EventArgs eventArgs)
        {
            PositionResetTimer.Stop();

            playerSquare.BorderColor = Color.RoyalBlue;
            playerSquare.FillColor = Color.CornflowerBlue;
        }
        private void PepperBoostTimer_Tick(object sender, EventArgs eventArgs)
        {
            PepperBoostTimer.Stop();
            playerSquare.Speed = 5;
            playerSquare.FillColor = Color.CornflowerBlue;
        }
        private void DespawnPepperBoostTimer_Tick(object sender, EventArgs eventArgs)
        {
            DespawnPepperBoostTimer.Stop();
            DeletePeperBoost();
            LastPowerUpCollected = true;
        }
        private void DeletePeperBoost()
        {
            if (PepperPowerUp != null)
            {
                Controls.Remove(PepperPowerUp);
                PepperPowerUp.Dispose();
                PepperPowerUp = null;
            }
        }
    }
}