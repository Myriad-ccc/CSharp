using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Moving_Square
{
    public partial class Form1 : Form
    {
        private readonly Random random = new Random();

        private readonly Timer TickUpdate = new Timer();
        private readonly PlayerSquare playerSquare = new PlayerSquare(new Rectangle(new Point(450, 550), new Size(50, 50)), 5, Color.CornflowerBlue, Color.RoyalBlue);

        private bool IsNearBorder;
        private bool IsOutOfBounds;
        private int OutOfBoundMoves = 0;
        private readonly Timer SuccesiveOutOfBoundMovesTimer = new Timer();

        private const int iconSize = 64;

        Effect PepperBoost;

        private readonly Timer OutOfBoundsTimer = new Timer { Interval = 500 };
        private readonly Timer CrashingTimer = new Timer { Interval = 240 };

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

            TickUpdate.Interval = 16;
            TickUpdate.Tick += Tick_Update;

            SuccesiveOutOfBoundMovesTimer.Interval = 5000;
            SuccesiveOutOfBoundMovesTimer.Tick += (s, ev) =>
            {
                SuccesiveOutOfBoundMovesTimer.Stop();
                OutOfBoundMoves = 0;
            };

            OutOfBoundsTimer.Tick += (s, ev) =>
            {
                OutOfBoundsTimer.Stop();
                EvaluatePlayerState();
            };

            CrashingTimer.Tick += (s, ev) =>
            {
                CrashingTimer.Stop();
                this.BackColor = Color.FromArgb(255, 35, 35, 35);
                playerSquare.X = 450;
                playerSquare.Y = 550;
            };
        }

        private async void Form1_Load_1(object sender, EventArgs e)
        {
            this.Paint += Form1_Paint;

            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;

            PepperBoost = new Effect
            (
                effectIsActive: false,
                canSpawn: true,
                effectHitBox: new Rectangle(
                    new Point(),
                    new Size(iconSize, iconSize)
                    ),
                effectDuration: new Timer(),
                // Optional parameters:
                effectDespawnTimer: new Timer(),
                spawnChance: 20,
                effectPictureBox: new PictureBox
                {
                    Size = new Size(iconSize, iconSize),
                    SizeMode = PictureBoxSizeMode.Zoom
                },
                effectPictureBoxImage: await AsyncImageUrlLoad("https://i.postimg.cc/jdhR3t4c/pepper3-64x64.png"),
                newSquareBorderColor: Color.DarkGreen,
                newSquareFillColor: Color.MediumSeaGreen,
                newSquareSpeed: 10
            );

            PepperBoost.EffectDuration.Interval = 6000; // buff length timer
            PepperBoost.EffectDuration.Tick += (s, ev) => // on buff end
            {
                PepperBoost.EffectDuration.Stop();
                playerSquare.Speed = 5;
                PepperBoost.EffectIsActive = false;

                EvaluatePlayerState();
            };

            PepperBoost.EffectDespawnTimer.Interval = 6000; // despawn timer
            PepperBoost.EffectDespawnTimer.Tick += (s, ev) => // on despawn
            {
                PepperBoost.EffectDespawnTimer.Stop();
                DeletePowerUp(PepperBoost);
                PepperBoost.CanSpawn = true;
                // last power up despawned
            };

            TickUpdate.Start(); // screen is updated every 16ms
        }
        private async Task<Image> AsyncImageUrlLoad(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var data = await httpClient.GetByteArrayAsync(url);
                using (MemoryStream memoryStream = new MemoryStream(data))
                {
                    return Image.FromStream(memoryStream);
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs eventArgs)
        {
            Graphics g = eventArgs.Graphics;

            using (Brush fillBrush = new SolidBrush(playerSquare.FillColor))
            {
                g.FillRectangle(fillBrush, playerSquare.Rectangle);
                ControlPaint.DrawBorder(g, playerSquare.Rectangle, playerSquare.BorderColor, ButtonBorderStyle.Solid);
            }
            using (Brush tempBrush = new SolidBrush(Color.Azure))
            {
                g.DrawString($"OOBM: {OutOfBoundMoves}", new Font("Gilroy-Medium", 40f), tempBrush, 5, 5);
            }
        }

        // Movement event handlers
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


        private void MovementResponse()
        {
            float dx = 0;
            float dy = 0;

            if (IsWKeyDown) dy -= playerSquare.Speed;
            if (IsAKeyDown) dx -= playerSquare.Speed;
            if (IsSKeyDown) dy += playerSquare.Speed;
            if (IsDKeyDown) dx += playerSquare.Speed;

            if (dx != 0 || dy != 0)
            {
                // Total vector length formula
                float length = (float)Math.Sqrt(dx * dx + dy * dy);

                // Normalize Vector and multiply by speed
                dx = dx / length * playerSquare.Speed;
                dy = dy / length * playerSquare.Speed;
            }
            playerSquare.X += dx;
            playerSquare.Y += dy;
        }
        private void BoundaryCheck()
        {
            IsNearBorder = (
                playerSquare.X <= 25 ||
                playerSquare.X + playerSquare.Rectangle.Width >= ClientSize.Width - 25 ||
                playerSquare.Y <= 25 ||
                playerSquare.Y + playerSquare.Rectangle.Height >= ClientSize.Height - 25);

            if (currentState != PlayerState.Error &&
                currentState != PlayerState.Crash &&
                currentState != PlayerState.Buffed &&
                currentState != PlayerState.Debuffed)
            {
                if (IsNearBorder) // "Warn" state triggered near border
                {
                    SetPlayerState(PlayerState.Warn);
                }
                else
                {
                    SetPlayerState(PlayerState.Normal);
                }
            }

            // "Error" state triggered on border collision
            IsOutOfBounds = false;
            if (playerSquare.X < 0)
            {
                IsOutOfBounds = true;
                playerSquare.X = (int)(playerSquare.Rectangle.Width * 0.25);
            }
            else if (playerSquare.X + playerSquare.Rectangle.Width > ClientSize.Width)
            {
                IsOutOfBounds = true;
                playerSquare.X = ClientSize.Width - (int)(playerSquare.Rectangle.Width * 0.25) - playerSquare.Rectangle.Width;
            }
            else if (playerSquare.Y < 0)
            {
                IsOutOfBounds = true;
                playerSquare.Y = (int)(playerSquare.Rectangle.Height * 0.25);
            }
            else if (playerSquare.Y + playerSquare.Rectangle.Height > ClientSize.Height)
            {
                IsOutOfBounds = true;
                playerSquare.Y = ClientSize.Height - (int)(playerSquare.Rectangle.Height * 0.25) - playerSquare.Rectangle.Height;
            }
            if (IsOutOfBounds)
            {
                OutOfBoundMoves++;
                SetPlayerState(PlayerState.Error);

                if (OutOfBoundMoves == 10) // "Crash" state triggered on successive border collisions
                {
                    OutOfBoundMoves = 0;
                    SetPlayerState(PlayerState.Crash);
                    if (OutOfBoundsTimer.Enabled)
                    {
                        OutOfBoundsTimer.Stop();
                    }
                }
                else
                {
                    SuccesiveOutOfBoundMovesTimer.Stop();
                    SuccesiveOutOfBoundMovesTimer.Start();
                }
            }
        }
        private void PowerUpOperations(Effect PowerUp)
        {
            CreatePowerUp(PepperBoost);
            PickUpPowerUp(PepperBoost);
        }

        private void CreatePowerUp(Effect PowerUp)
        {
            // a tick update happens every 16ms so 62.5 times per second
            // 1/625 chance would average out to 1 correct key every 10 seconds
            if (random.Next((int)PowerUp.SpawnChance) == 0)
            {
                if (PowerUp.CanSpawn)
                {
                    PowerUp.CanSpawn = false;

                    PowerUp.EffectHitBox = new Rectangle(
                        new Point(
                            random.Next(20, ClientSize.Width - 40 - iconSize),
                            random.Next(20, ClientSize.Height - 40 - iconSize)
                            ),
                        new Size(iconSize, iconSize)
                        );
                    PowerUp.EffectPictureBox.Location = PowerUp.EffectHitBox.Location;

                    PowerUp.EffectPictureBox.Image = PowerUp.EffectPictureBoxImage;
                    Controls.Add(PowerUp.EffectPictureBox);

                    PowerUp.EffectDespawnTimer.Stop();
                    PowerUp.EffectDespawnTimer.Start();
                }
            }
        }
        private void PickUpPowerUp(Effect PowerUp)
        {
            if (PowerUp.EffectPictureBox != null && playerSquare.Rectangle.IntersectsWith(
                new Rectangle(
                    new Point(PowerUp.EffectHitBox.Location.X - 2, PowerUp.EffectHitBox.Location.Y - 2),
                    new Size(PowerUp.EffectHitBox.Width + 4, PowerUp.EffectHitBox.Height + 4))))
            {
                DeletePowerUp(PowerUp);
                PowerUp.CanSpawn = true;

                PowerUp.EffectIsActive = false;
                SetPlayerState(PlayerState.Buffed);
            }
        }
        private void DeletePowerUp(Effect PowerUp)
        {
            if (PowerUp.EffectPictureBox != null)
            {
                Controls.Remove(PowerUp.EffectPictureBox);
                PowerUp.EffectPictureBox.Dispose();
            }
        }

        private void Tick_Update(object sender, EventArgs eventArgs)
        {
            // Move logic
            MovementResponse();

            // Boundary logic
            BoundaryCheck();

            // Power up processing logic
            PowerUpOperations(PepperBoost);

            playerSquare.Rectangle = new Rectangle(new Point((int)playerSquare.X, (int)playerSquare.Y), playerSquare.Rectangle.Size);
            this.Invalidate();
            // Redraws square after all of the checks
        }


        private enum PlayerState
        {
            Normal,
            Warn,
            Error,
            Crash,

            Buffed,
            Debuffed
        }
        private PlayerState currentState = PlayerState.Normal;

        private void SetPlayerState(PlayerState newState)
        {
            if (currentState == newState) return;

            currentState = newState;

            switch (newState)
            {
                case PlayerState.Normal:
                    playerSquare.BorderColor = Color.RoyalBlue;
                    playerSquare.FillColor = Color.CornflowerBlue;
                    break;
                case PlayerState.Warn:
                    playerSquare.BorderColor = Color.FromArgb(255, 181, 91, 7);
                    playerSquare.FillColor = Color.FromArgb(255, 250, 144, 45);
                    break;
                case PlayerState.Error:
                    playerSquare.BorderColor = Color.Brown;
                    playerSquare.FillColor = Color.IndianRed;

                    OutOfBoundsTimer.Stop();
                    OutOfBoundsTimer.Start();
                    break;
                case PlayerState.Crash:
                    this.BackColor = Color.FromArgb(255, 40, 40, 40);

                    CrashingTimer.Start();
                    break;
                case PlayerState.Buffed:
                    playerSquare.Speed = (int)PepperBoost.NewSquareSpeed;
                    playerSquare.BorderColor = (Color)PepperBoost.NewSquareBorderColor;
                    playerSquare.FillColor = (Color)PepperBoost.NewSquareFillColor;

                    PepperBoost.EffectDuration.Stop();
                    PepperBoost.EffectDuration.Start();
                    break;
                case PlayerState.Debuffed:
                    playerSquare.BorderColor = Color.DarkSlateGray;
                    playerSquare.FillColor = Color.GhostWhite;
                    break;
            }
        }
        private void EvaluatePlayerState()
        {
            if (PepperBoost.EffectIsActive && PepperBoost.EffectDuration.Enabled)
            {
                playerSquare.BorderColor = (Color)PepperBoost.NewSquareBorderColor;
                playerSquare.FillColor = (Color)PepperBoost.NewSquareFillColor;
            }
            else if (PepperBoost.EffectIsActive)
            {
                SetPlayerState(PlayerState.Buffed);
            }
            else if (IsNearBorder)
            {
                SetPlayerState(PlayerState.Warn);
            }
            else
            {
                SetPlayerState(PlayerState.Normal);
            }
        }
    }
}