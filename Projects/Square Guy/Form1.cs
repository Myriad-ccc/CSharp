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

        Effect currentEffect;
        Effect visibleEffect;
        private readonly List<Effect> EffectList = new List<Effect>();

        private readonly Timer OutOfBoundsTimer = new Timer { Interval = 500 };
        private readonly Timer CrashingTimer = new Timer { Interval = 240 };

        CustomButton statsBreakdown;
        Statistics stats;
        public int totalEffects;
        private bool showStats;

        public Form1()
        {
            InitializeComponent();

            Width = 1000;
            Height = 800;
            BackColor = Color.FromArgb(255, 35, 35, 35);

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            this.DoubleBuffered = true;
            this.KeyPreview = true;

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

            Effect PepperEffect = new Effect
            (
                effectName: "Pepper",
                effectHitBox: new Rectangle(new Point(), new Size(iconSize, iconSize)),
                effectDuration: new Timer { Interval = 6000 },
                // Optional parameters:
                effectDespawnTimer: new Timer { Interval = 5000 },
                spawnChance: 626, // on average every 625 ticks (10 seconds)
                effectImage: await AsyncImageUrlLoad("https://i.postimg.cc/jdhR3t4c/pepper3-64x64.png"),
                newSquareBorderColor: Color.DarkGreen,
                newSquareFillColor: Color.MediumSeaGreen,
                newSquareSpeed: 10
            );
            EffectList.Add(PepperEffect);

            Effect SpiderWeb = new Effect
            (
                effectName: "CobWeb",
                effectHitBox: new Rectangle(new Point(), new Size(iconSize, iconSize)),
                effectDuration: new Timer { Interval = 4000 },
                // Optional parameters:
                effectDespawnTimer: new Timer { Interval = 3500 },
                spawnChance: 1251, // on average every 1250 ticks (20 seconds)
                effectImage: await AsyncImageUrlLoad("https://i.postimg.cc/52S8bgZ1/spiderweb-fixed-64x64.png"),
                newSquareBorderColor: Color.GhostWhite,
                newSquareFillColor: Color.FromArgb(255, 104, 104, 104),
                newSquareSpeed: 3
            );
            EffectList.Add(SpiderWeb);

            Effect Confusion = new Effect
                (
                effectName: "Fog",
                effectHitBox: new Rectangle(new Point(), new Size(iconSize * 2, iconSize * 2)),
                effectDuration: new Timer { Interval = 4000 },
                // Optional parameters:
                effectDespawnTimer: new Timer { Interval = 8000 },
                spawnChance: 2501, // on average every 2500 ticks (40 seconds)
                effectImage: await AsyncImageUrlLoad("https://i.postimg.cc/FRXHnXHx/question-mark-128x128-solid.png"),
                newSquareBorderColor: Color.FromArgb(255, 255, 143, 0),
                newSquareFillColor: Color.FromArgb(255, 255, 167, 0),
                newSquareSpeed: 5
                );
            EffectList.Add(Confusion);

            Effect Warp = new Effect
                (
                effectName: "Teleport",
                effectHitBox: new Rectangle(new Point(), new Size(50, 70)),
                effectDuration: new Timer { Interval = 1 },
                // Optional parameters:
                effectDespawnTimer: new Timer { Interval = 4000 },
                spawnChance: 1563, // on average every 1562.5 ticks (25 seconds)
                effectImage: await AsyncImageUrlLoad("https://i.postimg.cc/tRKG3N7h/bottle3-48x68.png"),
                newSquareBorderColor: Color.Purple,
                newSquareFillColor: Color.MediumPurple
                );
            EffectList.Add(Warp);


            foreach (Effect effect in EffectList)
            {
                StartTryingToSpawnEffect(effect); // start trying to spawn the Effect (depends on spawnChance)

                effect.EffectDuration.Tick += (s, ev) => // on Effect end
                {
                    effect.EffectDuration.Stop();

                    playerSquare.Speed = 5;
                    effect.EffectIsActive = false;
                    currentEffect = null;

                    SetPlayerState(IsNearBorder ? PlayerState.Warn : PlayerState.Normal);
                };

                effect.EffectDespawnTimer.Tick += (s, ev) => // on despawn
                {
                    effect.EffectDespawnTimer.Stop();
                    effect.CanDrawEffect = false;
                    effect.CanSpawn = true; // last power up despawned
                    effect.CanBeIncremented = true;

                    StartTryingToSpawnEffect(effect);
                };
            }

            statsBreakdown = new CustomButton()
            {
                Text = "Stats",
                Location = new Point(15, 15)
            };
            statsBreakdown.Click += (s, ev) =>
            {
                if (!showStats)
                {
                    statsBreakdown.Font = new Font("Gilroy-Medium", 24f, FontStyle.Underline);
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

            foreach (Effect effect in EffectList)
            {
                if ((bool)effect.CanDrawEffect)
                {
                    g.DrawImage(effect.EffectImage, effect.EffectHitBox);
                }
            }

            if (showStats)
            {
                visibleEffect = EffectList.FirstOrDefault(x => (x.CanDrawEffect ?? false));
                stats = new Statistics
                (
                    playerPosition: playerSquare.Rectangle.Location,
                    playerSpeed: playerSquare.Speed,

                    recentEffectLength: currentEffect != null && currentEffect.EffectStartTime > DateTime.MinValue
                        ? $"{(TimeSpan.FromMilliseconds(currentEffect.EffectDuration.Interval) - (TimeSpan)(DateTime.Now - currentEffect.EffectStartTime)).TotalSeconds:F1}"
                        : "N/A",
                    /* inline shorthand for

                    TimeSpan Duration = TimeSpan.FromMilliseconds(currentEffect.EffectDuration.Interval);
                    TimeSpan ElapsedTime = DateTime.Now - currentEffect.EffectStartTime;
                    stats.RecentEffectLength = currentEffect.EffectStartTime > DateTime.MinValue
                       ? $"{(Duration - ElapsedTime).TotalSeconds:F1}"
                       : "N/A";

                    (saves having to create class level attributes)
                    */
                    recentEffectDespawn: visibleEffect != null && visibleEffect.EffectSpawnTime != null && visibleEffect.EffectSpawnTime > DateTime.MinValue && (visibleEffect.CanDrawEffect ?? false)
                        ? $"{(TimeSpan.FromMilliseconds(visibleEffect.EffectDespawnTimer.Interval) - (DateTime.Now - (DateTime)visibleEffect.EffectSpawnTime)).TotalSeconds:F1}"
                        : "N/A",

                    totalEffectsCollected: totalEffects,
                    currentOutOfBoundMoves: OutOfBoundMoves
                );

                int padding = 33;
                Point basePoint = new Point(
                statsBreakdown.Location.X - 3,
                statsBreakdown.Location.Y + statsBreakdown.Height - 15);

                using (Brush playerPositionBrush = new SolidBrush(Color.FromArgb(32, 167, 219)))
                using (Brush playerSpeedBrush = new SolidBrush(Color.FromArgb(32, 219, 131)))
                using (Brush EffectLengthBrush = new SolidBrush(Color.FromArgb(32, 219, 47)))
                using (Brush timeUntilDespawnBrush = new SolidBrush(Color.FromArgb(219, 32, 33)))
                using (Brush totalEffectsCollectedBrush = new SolidBrush(Color.FromArgb(219, 202, 32)))
                using (Brush currentOutOfBoundMovesBrush = new SolidBrush(Color.FromArgb(253, 55, 76)))
                using (Font font = new Font("Gilroy-Medium", 22f))
                {
                    g.DrawString($"PlayerPos: {stats.PlayerPosition.X}, {stats.PlayerPosition.Y}",
                        font, playerPositionBrush, new Point(basePoint.X, basePoint.Y + padding));
                    basePoint.Y += padding;

                    g.DrawString($"PlayerSpeed: {stats.PlayerSpeed}",
                        font, playerSpeedBrush, new Point(basePoint.X, basePoint.Y + padding));
                    basePoint.Y += padding;

                    g.DrawString($"RecentEffect: {stats.RecentEffectLength}",
                        font, EffectLengthBrush, new Point(basePoint.X, basePoint.Y + padding));
                    basePoint.Y += padding;

                    g.DrawString($"EffectDespawn: {stats.RecentEffectDespawn}",
                        font, timeUntilDespawnBrush, new Point(basePoint.X, basePoint.Y + padding));
                    basePoint.Y += padding;

                    g.DrawString($"All-time effects: {totalEffects}",
                        font, totalEffectsCollectedBrush, new Point(basePoint.X, basePoint.Y + padding));
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

        private void StartTryingToSpawnEffect(Effect effect)
        {
            Timer EffectRoll = new Timer { Interval = (int)effect.SpawnChance / 10 };
            EffectRoll.Tick += (s, ev) =>
            {
                EffectRoll.Stop();

                if ((bool)effect.CanBeIncremented && (bool)effect.CanSpawn)
                {
                    effect.CanBeIncremented = false;
                    effect.Ticks += random.Next((int)(-effect.SpawnChance / 2), (int)effect.SpawnChance / 2);
                }
                if (effect.Ticks >= (int)effect.SpawnChance && (bool)effect.CanSpawn)
                {
                    effect.Ticks = 0;
                    effect.CanSpawn = false;
                    CreateEffect(effect);
                }
                else
                {
                    EffectRoll.Start();
                }
            };
            EffectRoll.Start();
        }

        private void MovementResponse()
        {
            float dx = 0;
            float dy = 0;

            bool isConfused = currentEffect != null && currentEffect.EffectName == "Fog";

            if (isConfused)
            {
                if (IsWKeyDown) dy += playerSquare.Speed;
                if (IsAKeyDown) dx += playerSquare.Speed;
                if (IsSKeyDown) dy -= playerSquare.Speed;
                if (IsDKeyDown) dx -= playerSquare.Speed;
            }
            else
            {
                if (IsWKeyDown) dy -= playerSquare.Speed;
                if (IsAKeyDown) dx -= playerSquare.Speed;
                if (IsSKeyDown) dy += playerSquare.Speed;
                if (IsDKeyDown) dx += playerSquare.Speed;
            }
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
            currentState != PlayerState.Affected &&
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
            foreach (Effect effect in EffectList)
            {
                effect.Ticks++;
                ObtainEffect(effect);
            }
        }

        private void CreateEffect(Effect effect)
        {
            Point newPosition = new Point(
                random.Next(20, ClientSize.Width - 40 - iconSize),
                random.Next(20, ClientSize.Height - 40 - iconSize)
                );
            Size declaredSize = effect.EffectHitBox.Size;

            effect.EffectHitBox = new Rectangle(newPosition, declaredSize);

            effect.EffectSpawnTime = DateTime.Now;
            effect.CanDrawEffect = true;

            effect.EffectDespawnTimer.Stop();
            effect.EffectDespawnTimer.Start();
        }
        private void ObtainEffect(Effect effect)
        {
            if ((bool)effect.CanDrawEffect && playerSquare.Rectangle.IntersectsWith(
            new Rectangle(
            new Point(effect.EffectHitBox.Location.X - 2, effect.EffectHitBox.Location.Y - 2),
            new Size(effect.EffectHitBox.Width + 4, effect.EffectHitBox.Height + 4))))
            {
                totalEffects++;

                effect.CanDrawEffect = false;
                effect.CanSpawn = true;
                effect.CanBeIncremented = true;
                effect.EffectIsActive = true;

                currentEffect = effect;
                SetPlayerState(PlayerState.Affected);
                StartTryingToSpawnEffect(effect);
            }
        }
        private void ApplyEffect(Effect effect)
        {
            playerSquare.Speed = (int)effect.NewSquareSpeed;
            playerSquare.BorderColor = (Color)effect.NewSquareBorderColor;
            playerSquare.FillColor = (Color)effect.NewSquareFillColor;
        }

        private void Tick_Update(object sender, EventArgs eventArgs)
        {
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

            Affected,
            Debuffed
        }
        private PlayerState currentState = PlayerState.Normal;

        private void AffectedState(Effect effect)
        {
            ApplyEffect(effect);

            if (effect.EffectName == "Teleport")
            {
                int constrainedX = random.Next(25, ClientSize.Width - 50);
                int constrainedY = random.Next(25, ClientSize.Height - 50);
                Size oldSize = playerSquare.Rectangle.Size;

                playerSquare.X = constrainedX;
                playerSquare.Y = constrainedY;
                playerSquare.Rectangle = new Rectangle(new Point(constrainedX, constrainedY), oldSize);

                effect.EffectIsActive = false;
                currentEffect = null;
                EvaluatePlayerState();
            }
            else
            {
                effect.EffectStartTime = DateTime.Now;
                effect.EffectDuration.Stop();
                effect.EffectDuration.Start();
            }

        }

        private void SetPlayerState(PlayerState newState)
        {
            if (currentState == newState && newState != PlayerState.Affected) return;

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
                case PlayerState.Affected:
                    AffectedState(currentEffect);
                    break;
                case PlayerState.Debuffed:
                    playerSquare.BorderColor = Color.DarkSlateGray;
                    playerSquare.FillColor = Color.GhostWhite;
                    break;
            }
        }
        private void EvaluatePlayerState()
        {
            if (currentEffect != null)
            {
                if ((bool)currentEffect.EffectIsActive)
                {
                    ApplyEffect(currentEffect); // applies Effects without resetting timer
                }
                else
                {
                    SetPlayerState(PlayerState.Affected); // resets Effect timer & reapplies Effects
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