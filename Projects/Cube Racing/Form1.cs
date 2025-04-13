using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace cubeRace
{
    public partial class Form1 : Form
    {
        Random random = new Random();
        Timer timer = new Timer();

        int lanes = 0;

        PlayerList playerList = new PlayerList();
        Color saveColor;
        int playerIndex = 0;

        Player Winner;
        bool GameIsFinished = false;
        Button playAgainButton;

        public Form1()
        {
            InitializeComponent();

            timer.Interval = 10;
            timer.Tick += Timer_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.Width = 1400;
            this.Height = 839;
            this.BackColor = Color.FromArgb(255, 35, 35, 35);
            this.Paint += Form_Paint;

            StartRace();
        }
        private void Form_Paint(object sender, PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.DrawLine(new Pen(Color.FromArgb(255, 185, 185, 185)), 120, 0, 120, ClientSize.Height);
            paintEventArgs.Graphics.DrawLine(new Pen(Color.FromArgb(255, 185, 185, 185)), ClientSize.Width - 80, 0, ClientSize.Width - 80, ClientSize.Height);

            for (int i = 0; i < ClientSize.Height; i += 100)
            {
                int height = i + 25;

                saveColor = playerList.playerList[i / 100].Color;
                    
                using (Font font = new Font("Bombardier", 32f))
                using (Brush brush = new SolidBrush(saveColor))
                {
                    string text = $"#{i / 100 + 1}";
                    paintEventArgs.Graphics.DrawString(text, font, brush, new Point(25, height));
                }

                paintEventArgs.Graphics.DrawLine(new Pen(Color.FromArgb(255, 185, 185, 185)), 0, i, ClientSize.Width, i);
            }

            foreach (Player player in playerList.playerList)
            {
                paintEventArgs.Graphics.DrawRectangle(new Pen(player.Color), player.Rectangle.Location.X, player.Rectangle.Location.Y, player.Rectangle.Width, player.Rectangle.Height);
            }
            if (GameIsFinished)
            {
                using (Font font = new Font("Bombardier", 60f))
                using (Brush victoryBrush = new SolidBrush(Color.Orange))
                using (Brush winnerBrush = new SolidBrush(Winner.Color))
                {
                    string victoryText = $"Game Over!";
                    SizeF victorySize = paintEventArgs.Graphics.MeasureString(victoryText, font);

                    int centerX = (int)(ClientSize.Width - victorySize.Width) / 2;
                    int centerY = (int)(ClientSize.Height - victorySize.Height) / 2;

                    paintEventArgs.Graphics.DrawString(victoryText, font, victoryBrush, centerX, centerY - 40);

                    string winnerNumberText = $"#{Winner.Number} has won!";
                    SizeF winnerNumberSize = paintEventArgs.Graphics.MeasureString(winnerNumberText, font);
                    paintEventArgs.Graphics.DrawString(winnerNumberText, font, winnerBrush, centerX, centerY + 40);
                }

                if (playAgainButton == null)
                {
                    playAgainButton = new Button
                    {
                        Font = new Font("Bombardier", 36f),
                        BackColor = Color.FromArgb(255, 32, 32, 32),
                        ForeColor = saveColor,
                        Text = "Race Again?",
                        AutoSize = true,
                        FlatStyle = FlatStyle.Flat
                    };
                    playAgainButton.FlatAppearance.BorderSize = 1;
                    playAgainButton.FlatAppearance.BorderColor = Color.Black;
                    playAgainButton.Location = new Point(
                        ((ClientSize.Width - playAgainButton.Width) / 2) - 80,
                        ((ClientSize.Height - playAgainButton.Height) / 2) + 130
                        );
                    playAgainButton.Click += (se, e) =>
                    {
                        Controls.Remove(playAgainButton);
                        playAgainButton.Dispose();
                        playAgainButton = null;

                        lanes = 0;
                        playerIndex = 0;
                        GameIsFinished = false;
                        Winner = null;

                        playerList.playerList.Clear();

                        StartRace();
                    };
                    Controls.Add(playAgainButton);
                }
            }
        }
        private void Timer_Tick(object sender, EventArgs eventArgs)
        {
            this.Refresh();

            foreach (Player player in playerList.playerList)
            {
                float stepSpeed = random.Next(0, 51);
                stepSpeed /= 10;
                player.Speed += stepSpeed;

                var PlayerWidthLocation = player.Rectangle.Location.X;
                PlayerWidthLocation += (int)Math.Round(player.Speed);

                player.Rectangle = new Rectangle(new Point(PlayerWidthLocation, player.Rectangle.Location.Y), player.Rectangle.Size);
                player.Speed -= stepSpeed;

                var PlayerWidth = player.Rectangle.Width;
                PlayerWidth += random.Next(-7, 8);
                if (PlayerWidth < 1)
                {
                    PlayerWidth += random.Next(16);
                }
                if (PlayerWidth > 120)
                {
                    PlayerWidth += random.Next(-25, 0);
                }

                var PlayerHeight = player.Rectangle.Height;
                PlayerHeight += random.Next(-7, 8);
                if (PlayerHeight < 1)
                {
                    PlayerHeight += random.Next(16);
                }
                if (player.Rectangle.Location.Y + player.Rectangle.Height > ClientSize.Height)
                {
                    PlayerHeight += random.Next(-25, 0);
                }
                else if (player.Rectangle.Height > 100)
                {
                    PlayerHeight += random.Next(-25, 0);
                }

                player.Rectangle = new Rectangle(new Point(player.Rectangle.Location.X, player.Rectangle.Location.Y), new Size(PlayerWidth, PlayerHeight));

                if (player.Rectangle.Location.X > ClientSize.Width - 77)
                {
                    if (player.Rectangle.Width > 80 && player.Rectangle.Height > 80)
                    {
                        timer.Stop();
                        Winner = player;
                        GameIsFinished = true;
                        this.Invalidate();
                    }
                    else
                    {
                        player.Rectangle = new Rectangle(new Point(150, player.Rectangle.Location.Y), player.Rectangle.Size);
                    }
                }
            }
        }

        private void StartRace()
        {
            lanes = ClientSize.Height / 100;

            for (int i = 0; i <= lanes; i++)
            {
                playerIndex++;

                int randomRed = random.Next(256);
                int randomGreen = random.Next(256);
                int randomBlue = random.Next(256);

                Color randomColor = Color.FromArgb(randomRed, randomGreen, randomBlue);

                Rectangle PlayerRectangle = new Rectangle(new Point(150, i * 100 + 2), new Size(50, 50));

                int randomInnateSpeed = random.Next(0, 31);

                playerList.playerList.Add(new Player(PlayerRectangle, randomColor, randomInnateSpeed / 10, playerIndex));
            }
            timer.Start();
            this.Invalidate();
        }
    }
}