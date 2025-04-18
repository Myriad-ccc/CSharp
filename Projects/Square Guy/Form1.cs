using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Moving_Square
{
    public partial class Form1 : Form
    {
        private readonly Random random = new Random();

        private readonly Timer UpdateUI = new Timer();
        private readonly PlayerSquare playerSquare = new PlayerSquare(new Rectangle(new Point(450, 550), new Size(50, 50)), 5, Color.CornflowerBlue, Color.RoyalBlue);

        private bool IsNearBorder;
        private bool IsOutOfBounds;
        private int OutOfBoundMoves = 0;
        private readonly Timer SuccesiveOutOfBoundMovesTimer = new Timer();

        private const int iconSize = 64;

        Effect currentBuff;
        Effect visibleBuff;
        private readonly List<Effect> BuffList = new List<Effect>();
        Effect PepperBuff;

        private readonly Timer OutOfBoundsTimer = new Timer { Interval = 500 };
        private readonly Timer CrashingTimer = new Timer { Interval = 240 };

        CustomButton statsBreakdown;
        Statistics stats;
        public int totalBuffs;
        private bool showStats;

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

            UpdateUI.Interval = 16;
            UpdateUI.Tick += Tick_Update;

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

            CrashingTimer.Tick += async (s, ev) =>
            {
                CrashingTimer.Stop();
                this.BackColor = Color.FromArgb(255, 35, 35, 35);
                playerSquare.X = 450;
                playerSquare.Y = 550;
                await Task.Delay(500);
                EvaluatePlayerState();
            };
        }

        private async void Form1_Load_1(object sender, EventArgs e)
        {
            this.Paint += Form1_Paint;

            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;

            PepperBuff = new Effect
            (
                effectName: "Pepper",
                effectIsActive: false,
                canSpawn: true,
                canBeIncremented: true,
                effectHitBox: new Rectangle(
                    new Point(),
                    new Size(iconSize, iconSize)
                    ),
                effectDuration: new Timer { Interval = 6000 },
                effectStartTime: DateTime.MinValue,
                // Optional parameters:
                effectDespawnTimer: new Timer { Interval = 6000 },
                effectSpawnTime: DateTime.MinValue,
                spawnChance: 626,
                canDrawPowerUp: false,
                effectImage: await AsyncImageUrlLoad("https://i.postimg.cc/jdhR3t4c/pepper3-64x64.png"),
                newSquareBorderColor: Color.DarkGreen,
                newSquareFillColor: Color.MediumSeaGreen,
                newSquareSpeed: 10
            );
            BuffList.Add(PepperBuff);

            foreach (Effect PowerUp in BuffList)
            {
                StartTryingToSpawnBuff(PowerUp); // start trying to spawn the buff (depends on spawnChance)

                PowerUp.EffectDuration.Tick += (s, ev) => // on buff end
                {
                    PowerUp.EffectDuration.Stop();
                    playerSquare.Speed = 5;
                    PowerUp.EffectIsActive = false;
                    currentBuff = null;

                    SetPlayerState(IsNearBorder ? PlayerState.Warn : PlayerState.Normal);
                };

                PowerUp.EffectDespawnTimer.Tick += (s, ev) => // on despawn
                {
                    PowerUp.EffectDespawnTimer.Stop();
                    PowerUp.CanDrawPowerUp = false;
                    PowerUp.CanSpawn = true; // last power up despawned
                    PowerUp.CanBeIncremented = true;

                    StartTryingToSpawnBuff(PowerUp);
                };
            }

            statsBreakdown = new CustomButton()
            {
                Text = "Stats",
                Location = new Point(15, 15)
            };
            int statsBreakdownClicks = 1;
            statsBreakdown.Click += (s, ev) =>
            {
                statsBreakdownClicks++;
                if (statsBreakdownClicks % 2 == 0)
                {
                    statsBreakdown.Font = new Font("Gilroy-Medium", 24f, FontStyle.Bold);
                    showStats = true;
                }
                else
                {
                    statsBreakdown.Font = new Font("Gilroy-Medium", 24f);
                    showStats = false;
                }
            };

            Controls.Add(statsBreakdown);
            this.Invalidate();

            statsBreakdown.DoubleClick += (s, ev) =>
            {
                showStats = false;
                Controls.Remove(statsBreakdown);
                statsBreakdown.Dispose();
                statsBreakdown = null;
            };


            UpdateUI.Start(); // screen is updated every 16ms
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

            foreach (Effect PowerUp in BuffList)
            {
                if ((bool)PowerUp.CanDrawPowerUp)
                {
                    g.DrawImage(PowerUp.EffectImage, PowerUp.EffectHitBox.Location);
                }
            }

            if (showStats)
            {
                visibleBuff = BuffList.FirstOrDefault(x => (x.CanDrawPowerUp ?? false));
                stats = new Statistics
                (
                    playerPosition: playerSquare.Rectangle.Location,
                    playerSpeed: playerSquare.Speed,

                    recentBuffLength: currentBuff != null && currentBuff.EffectStartTime > DateTime.MinValue
                        ? $"{(TimeSpan.FromMilliseconds(currentBuff.EffectDuration.Interval) - (DateTime.Now - currentBuff.EffectStartTime)).TotalSeconds:F1}"
                        : "N/A",
                    /* inline shorthand for
                      
                    TimeSpan Duration = TimeSpan.FromMilliseconds(currentBuff.EffectDuration.Interval);
                    TimeSpan ElapsedTime = DateTime.Now - currentBuff.EffectStartTime;
                    stats.RecentBuffLength = currentBuff.EffectStartTime > DateTime.MinValue
                       ? $"{(Duration - ElapsedTime).TotalSeconds:F1}"
                       : "N/A";

                    (saves having to create class level attributes)
                    */
                    recentBuffDespawn: visibleBuff != null && visibleBuff.EffectSpawnTime != null && visibleBuff.EffectSpawnTime > DateTime.MinValue && (visibleBuff.CanDrawPowerUp ?? false)
                        ? $"{(TimeSpan.FromMilliseconds(visibleBuff.EffectDespawnTimer.Interval) - (DateTime.Now - (DateTime)visibleBuff.EffectSpawnTime)).TotalSeconds:F1}"
                        : "N/A",

                    totalBuffsCollected: totalBuffs,
                    currentOutOfBoundMoves: OutOfBoundMoves
                );

                int padding = 33;
                Point basePoint = new Point(
                statsBreakdown.Location.X - 3,
                statsBreakdown.Location.Y + statsBreakdown.Height - 15);

                using (Brush playerPositionBrush = new SolidBrush(Color.FromArgb(54, 124, 193)))
                using (Brush playerSpeedBrush = new SolidBrush(Color.FromArgb(51, 170, 74)))
                using (Brush buffLengthBrush = new SolidBrush(Color.FromArgb(219, 41, 50)))
                using (Brush timeUntilDespawnBrush = new SolidBrush(Color.FromArgb(242, 111, 46)))
                using (Brush totalBuffsCollectedBrush = new SolidBrush(Color.FromArgb(232, 178, 40)))
                using (Brush currentOutOfBoundMovesBrush = new SolidBrush(Color.FromArgb(169, 78, 211)))
                using (Font font = new Font("Gilroy-Medium", 22f))
                {
                    g.DrawString($"PlayerPos: {stats.PlayerPosition.X}, {stats.PlayerPosition.Y}",
                        font, playerPositionBrush, new Point(basePoint.X, basePoint.Y + padding));
                    basePoint.Y += padding;

                    g.DrawString($"PlayerSpeed: {stats.PlayerSpeed}",
                        font, playerSpeedBrush, new Point(basePoint.X, basePoint.Y + padding));
                    basePoint.Y += padding;

                    g.DrawString($"RecentBuff: {stats.RecentBuffLength}",
                        font, buffLengthBrush, new Point(basePoint.X, basePoint.Y + padding));
                    basePoint.Y += padding;

                    g.DrawString($"BuffDespawn: {stats.RecentBuffDespawn}",
                        font, timeUntilDespawnBrush, new Point(basePoint.X, basePoint.Y + padding));
                    basePoint.Y += padding;

                    g.DrawString($"All-time buffs: {totalBuffs}",
                        font, totalBuffsCollectedBrush, new Point(basePoint.X, basePoint.Y + padding));
                    basePoint.Y += padding;

                    g.DrawString($"OOBM: {OutOfBoundMoves}",
                        font, currentOutOfBoundMovesBrush, new Point(basePoint.X, basePoint.Y + padding));
                    basePoint.Y += padding;
                }
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

        private void StartTryingToSpawnBuff(Effect PowerUp)
        {
            Timer BuffRoll = new Timer { Interval = (int)PowerUp.SpawnChance / 10 };
            BuffRoll.Tick += (s, ev) =>
            {
                BuffRoll.Stop();

                if (PowerUp.CanBeIncremented && PowerUp.CanSpawn)
                {
                    PowerUp.CanBeIncremented = false;
                    PowerUp.Ticks += random.Next((int)(-PowerUp.SpawnChance / 2), (int)PowerUp.SpawnChance / 2);
                }
                if (PowerUp.Ticks >= (int)PowerUp.SpawnChance && PowerUp.CanSpawn)
                {
                    PowerUp.Ticks = 0;
                    PowerUp.CanSpawn = false;
                    CreateEffect(PowerUp);
                }
                else
                {
                    BuffRoll.Start();
                }
            };
            BuffRoll.Start();
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
        private void HandleEffects()
        {
            foreach (Effect PowerUp in BuffList)
            {
                ObtainEffect(PowerUp);
            }
        }

        private void CreateEffect(Effect PowerUp)
        {
            PowerUp.EffectHitBox = new Rectangle(
            new Point(
            random.Next(20, ClientSize.Width - 40 - iconSize),
            random.Next(20, ClientSize.Height - 40 - iconSize)
            ),
            new Size(iconSize, iconSize)
            );

            PowerUp.EffectSpawnTime = DateTime.Now;
            PowerUp.CanDrawPowerUp = true;

            PowerUp.EffectDespawnTimer.Stop();
            PowerUp.EffectDespawnTimer.Start();
        }
        private void ObtainEffect(Effect PowerUp)
        {
            if ((bool)PowerUp.CanDrawPowerUp && playerSquare.Rectangle.IntersectsWith(
            new Rectangle(
            new Point(PowerUp.EffectHitBox.Location.X - 2, PowerUp.EffectHitBox.Location.Y - 2),
            new Size(PowerUp.EffectHitBox.Width + 4, PowerUp.EffectHitBox.Height + 4))))
            {
                totalBuffs++;

                PowerUp.CanDrawPowerUp = false;
                PowerUp.CanSpawn = true;
                PowerUp.CanBeIncremented = true;
                PowerUp.EffectIsActive = true;

                currentBuff = PowerUp;
                SetPlayerState(PlayerState.Buffed);
                StartTryingToSpawnBuff(PowerUp);
            }
        }
        private void ApplyBuff(Effect PowerUp)
        {
            playerSquare.Speed = (int)PowerUp.NewSquareSpeed;
            playerSquare.BorderColor = (Color)PowerUp.NewSquareBorderColor;
            playerSquare.FillColor = (Color)PowerUp.NewSquareFillColor;
        }

        private void Tick_Update(object sender, EventArgs eventArgs)
        {
            foreach (Effect PowerUp in BuffList)
            {
                PowerUp.Ticks++;
            }

            // Move logic
            MovementResponse();

            // Boundary logic
            BoundaryCheck();

            // Power up processing logic
            HandleEffects();

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

        private void BuffedState(Effect PowerUp)
        {
            ApplyBuff(PowerUp);

            PowerUp.EffectStartTime = DateTime.Now;

            PowerUp.EffectDuration.Stop();
            PowerUp.EffectDuration.Start();
        }

        private void SetPlayerState(PlayerState newState)
        {
            if (currentState == newState && newState != PlayerState.Buffed) return;

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
                    CrashingTimer.Stop();
                    CrashingTimer.Start();
                    break;
                case PlayerState.Buffed:
                    BuffedState(currentBuff);
                    break;
                case PlayerState.Debuffed:
                    playerSquare.BorderColor = Color.DarkSlateGray;
                    playerSquare.FillColor = Color.GhostWhite;
                    break;
            }
        }
        private void EvaluatePlayerState()
        {
            if (currentBuff != null)
            {
                if (currentBuff.EffectIsActive)
                {
                    ApplyBuff(currentBuff); // applies buffs without resetting timer
                }
                else
                {
                    SetPlayerState(PlayerState.Buffed); // resets buff timer & reapplies buffs
                }
            }
            else
            {
                if (IsNearBorder)
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
}