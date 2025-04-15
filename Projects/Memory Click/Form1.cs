using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Memory_Click
{
    public partial class Form1 : Form
    {
        Random random = new Random();

        Button easyMode;
        Button normalMode;
        Button hardMode;
        Button absurdMode;

        Timer buttonShowTime = new Timer();
        int interval;

        Button randomButton;
        Button Begin;
        List<Button> allButtons;
        List<Button> keyButtons;
        List<Button> correctClickList;

        int correctClicks;
        int wrongClicks;
        int totalWrongClicks;
        int fails;
        int keyButtonAmount;

        Label Victory;
        Label TotalClicks;
        Label WrongClicks;
        Button PlayAgain;
        Button GoBack;

        List<Control> PostGameControls = new List<Control>();
        List<Control> PreGameControls = new List<Control>();

        public Form1()
        {
            InitializeComponent();
            modeButtonInitialization();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(255, 35, 35, 35);
            Width = 600;
            Height = 800;
            LoadImageFromUrl("https://i.postimg.cc/XqPSNBPc/base-memory-click-template.png");
        }
        private Button CreateButton(string text)
        {
            Button button = new Button();

            button.AutoSize = true;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.FromArgb(255, 35, 35, 35);
            button.Font = new Font("Bombardier", 20f);
            button.Text = text;
            Controls.Add(button);

            return button;
        }
        private async void LoadImageFromUrl(string url)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var data = await httpClient.GetStreamAsync(url);
                this.BackgroundImage = Image.FromStream(data);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch (Exception)
            {
                this.BackColor = Color.FromArgb(255, 35, 35, 35);
            }
        }
        private void modeButtonInitialization()
        {
            easyMode = CreateButton("Easy");
            easyMode.ForeColor = Color.Green;
            easyMode.Location = new Point(
                Width * 1 / 6 - easyMode.Width,
                80
                );
            easyMode.Click += easyMode_Click;

            normalMode = CreateButton("Normal");
            normalMode.ForeColor = Color.Yellow;
            normalMode.Location = new Point(
                Width * 2 / 6 - normalMode.Width,
                80
                );
            normalMode.Click += normalMode_Click;

            hardMode = CreateButton("Hard");
            hardMode.ForeColor = Color.Red;
            hardMode.Location = new Point(
                Width * 3 / 6 - hardMode.Width,
                80
                );
            hardMode.Click += hardMode_Click;

            absurdMode = CreateButton("Absurd");
            absurdMode.ForeColor = Color.CornflowerBlue;
            absurdMode.Location = new Point(
                Width * 4 / 6 - absurdMode.Width,
                80
                );
            absurdMode.Click += absurdMode_Click;
        }
        private void easyMode_Click(object send, EventArgs eventArgs)
        {
            HideModeButtons();
            GameAreaInitialization("easy");
        }
        private void normalMode_Click(object send, EventArgs eventArgs)
        {
            HideModeButtons();
            GameAreaInitialization("normal");
        }
        private void hardMode_Click(object send, EventArgs eventArgs)
        {
            HideModeButtons();
            GameAreaInitialization("hard");
        }
        private void absurdMode_Click(object send, EventArgs eventArgs)
        {
            HideModeButtons();
            GameAreaInitialization("absurd");
        }

        private void GameAreaInitialization(string mode)
        {
            SetGameArea(mode);
            GoBackButton();
            BeginButton();
        }
        private async Task CheckGameState()
        {
            if (wrongClicks == 3)
            {
                fails++;
                wrongClicks = 0;

                DisableClicks();

                ShowKeyButtons();
                await Task.Delay(1000);
                HideKeyButtonsExceptGuessed();
                await Task.Delay(200);

                EnableClicks();
            }
            if (correctClicks == keyButtonAmount)
            {
                DisableClicks();
                EndOfGameScreen();
            }
        }
        private void SetGameArea(string mode)
        {
            Point TopLeftCorner = new Point(74, 250);
            Point TopRightCorner = new Point(525, 250);
            Point BottomLeftCorner = new Point(74, 715);
            Point BottomRightCorner = new Point(525, 715);

            int width = TopRightCorner.X - TopLeftCorner.X;
            int height = BottomLeftCorner.Y - TopLeftCorner.Y;

            Rectangle GameArea = new Rectangle(TopLeftCorner.X, TopLeftCorner.Y, width, height);

            int rows = 0;
            int deadSpace = 0;

            keyButtonAmount = 0;
            correctClicks = 0;
            wrongClicks = 0;
            totalWrongClicks = 0;
            fails = 0;


            if (mode == "easy")
            {
                rows = 3;
                interval = 1200;
                deadSpace = 6;
                keyButtonAmount = 4;
            }
            else if (mode == "normal")
            {
                rows = 5;
                interval = 1500;
                deadSpace = 5;
                keyButtonAmount = 7;
            }
            else if (mode == "hard")
            {
                rows = 7;
                interval = 2000;
                deadSpace = 4;
                keyButtonAmount = 10;
            }
            else if (mode == "absurd")
            {
                rows = 8;
                interval = 3875;
                deadSpace = 3;
                keyButtonAmount = 15;
            }
            buttonShowTime.Interval = interval;
            buttonShowTime.Tick += buttonShowTime_Tick;

            int cols = rows;
            int squareWidth = width / cols;
            int squareHeight = height / rows;

            allButtons = new List<Button>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Button square = new Button();
                    square.Size = new Size(squareWidth - deadSpace, squareHeight - deadSpace);
                    square.FlatStyle = FlatStyle.Flat;
                    square.FlatAppearance.BorderSize = 0;
                    square.BackColor = Color.FromArgb(255, 35, 35, 35);
                    square.Location = new Point(
                        GameArea.Location.X + squareWidth * r,
                        GameArea.Location.Y + squareHeight * c
                        );
                    square.TabStop = false;
                    Controls.Add(square);
                    allButtons.Add(square);
                    PreGameControls.Add(square);
                }
            }
        }
        private void GoBackButton()
        {
            GoBack = new Button();
            GoBack.Font = new Font("Bombardier", 18f);
            GoBack.ForeColor = Color.FromArgb(255, 215, 215, 215);
            GoBack.Text = "Return";
            GoBack.AutoSize = true;
            GoBack.BackColor = Color.Transparent;
            GoBack.FlatStyle = FlatStyle.Flat;
            GoBack.FlatAppearance.BorderSize = 1;
            GoBack.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            GoBack.Location = new Point(
                11,
                11
                );
            Controls.Add(GoBack);
            PreGameControls.Add(GoBack);
            GoBack.Click += GoBack_Click;
        }

        private void BeginButton()
        {
            Begin = CreateButton("Start");
            PreGameControls.Add(Begin);
            Begin.Font = new Font("Bombardier", 50f);
            Begin.BackColor = Color.FromArgb(255, 42, 42, 42);
            Begin.ForeColor = Color.FromArgb(255, 240, 240, 240);
            Begin.Location = new Point(
                (ClientSize.Width - Begin.Width) / 2,
                easyMode.Location.Y - 25
                );
            Begin.Click += (sender, e) =>
            {
                Controls.Remove(Begin);
                Begin.Dispose();
            };
            Begin.Click += Begin_Click;
        }
        private async void Begin_Click(object sender, EventArgs eventArgs)
        {
            keyButtons = new List<Button>();

            for (int i = 0; i < keyButtonAmount; i++)
            {
                randomButton = allButtons[random.Next(0, allButtons.Count())];

                if (keyButtons.Contains(randomButton))
                {
                    i--;
                    continue;
                }

                keyButtons.Add(randomButton);
            }

            ShowKeyButtons();
            buttonShowTime.Start();

            correctClickList = new List<Button>();
            foreach (Button button in allButtons)
            {
                if (keyButtons.Contains(button))
                {
                    button.Click += (s, ev) =>
                    {
                        correctClicks++;
                        correctClickList.Add(button);
                        CheckGameState();

                        button.Enabled = false;
                        button.BackColor = Color.FromArgb(255, 185, 185, 185);
                    };
                }
                else
                {
                    button.Click += async (s, ev) =>
                    {
                        wrongClicks++;
                        totalWrongClicks++;
                        await CheckGameState();

                        button.BackColor = Color.FromArgb(255, 217, 84, 77);
                        await Task.Delay(100);
                        button.BackColor = Color.FromArgb(255, 35, 35, 35);
                    };
                }
            }
        }
        private async void PlayAgain_Click(object sender, EventArgs eventArgs)
        {
            DeleteOldGame();

            string mode = "";
            if (keyButtonAmount == 4)
            {
                mode = "easy";
            }
            else if (keyButtonAmount == 7)
            {
                mode = "normal";
            }
            else if (keyButtonAmount == 10)
            {
                mode = "hard";
            }
            else if (keyButtonAmount == 15)
            {
                mode = "absurd";
            }

            SetGameArea(mode);

            keyButtons = new List<Button>();

            for (int i = 0; i < keyButtonAmount; i++)
            {
                randomButton = allButtons[random.Next(0, allButtons.Count())];

                if (keyButtons.Contains(randomButton))
                {
                    i--;
                    continue;
                }

                keyButtons.Add(randomButton);
            }

            correctClickList = new List<Button>();
            ShowKeyButtons();
            DisableClicks();
            buttonShowTime.Start();

            foreach (Button button in allButtons)
            {
                if (keyButtons.Contains(button))
                {
                    button.Click += (s, ev) =>
                    {
                        correctClicks++;
                        correctClickList.Add(button);
                        CheckGameState();

                        button.Enabled = false;
                        button.BackColor = Color.FromArgb(255, 185, 185, 185);
                    };
                }
                else
                {
                    button.Click += async (s, ev) =>
                    {
                        wrongClicks++;
                        totalWrongClicks++;
                        await CheckGameState();

                        button.BackColor = Color.FromArgb(255, 217, 84, 77);
                        await Task.Delay(100);
                        button.BackColor = Color.FromArgb(255, 35, 35, 35);
                    };
                }
            }
        }


        private void EndOfGameScreen()
        {
            Victory = new Label();

            Victory.Font = new Font("Bombardier", 50f);
            Victory.BackColor = Color.FromArgb(255, 45, 45, 45);
            if (fails == 0)
            {
                Victory.ForeColor = Color.FromArgb(255, 250, 144, 45);
                Victory.Text = "Victory!";
            }
            else if (fails <= 2)
            {
                Victory.ForeColor = Color.FromArgb(255, 225, 114, 70);
                Victory.Text = "Victory";
            }
            else if (fails <= 5)
            {
                Victory.ForeColor = Color.FromArgb(255, 216, 90, 79);
                Victory.Text = "Victory..";
            }
            else if (fails <= 10)
            {
                Victory.ForeColor = Color.FromArgb(255, 168, 33, 23);
                Victory.Text = "Victory!?";
            }
            else
            {
                Victory.ForeColor = Color.FromArgb(255, 170, 0, 246);

                try
                {
                    PictureBox skullEmoji = new PictureBox();
                    string skullEmojiUrl = @"https://i.imgflip.com/7gr4qw.png";
                    skullEmoji.Size = new Size(160, 210);
                    skullEmoji.SizeMode = PictureBoxSizeMode.StretchImage;

                    skullEmoji.Location = new Point(
                        ClientSize.Width / 2 - skullEmoji.Width / 2,
                        Victory.Location.Y + 5
                        );

                    skullEmoji.Load(skullEmojiUrl);
                    Controls.Add(skullEmoji);
                    PostGameControls.Add(skullEmoji);
                }
                catch (Exception)
                {
                    Victory.Text = "Victory... i guess";
                }
            }

            Victory.AutoSize = true;
            Victory.Location = new Point(
                (ClientSize.Width / 2) - Victory.Width - 20,
                easyMode.Location.Y - 68
                );
            Controls.Add(Victory);
            PostGameControls.Add(Victory);

            TotalClicks = new Label
            {
                Font = new Font("Bombardier", 20f),
                ForeColor = Color.FromArgb(255, 105, 204, 86),
                BackColor = Color.FromArgb(255, 45, 45, 45),
                Text = $"Total Clicks: {correctClicks + totalWrongClicks}",
                AutoSize = true
            };
            TotalClicks.Location = new Point(
                100 - TotalClicks.Width / 2,
                Victory.Location.Y + Victory.Height + TotalClicks.Height / 2 + 40
                );
            Controls.Add(TotalClicks);
            PostGameControls.Add(TotalClicks);

            WrongClicks = new Label
            {
                Font = new Font("Bombardier", 20f),
                ForeColor = Color.FromArgb(255, 240, 110, 110),
                BackColor = Color.FromArgb(255, 45, 45, 45),
                Text = $"Wrong Clicks: {totalWrongClicks}",
                AutoSize = true
            };
            WrongClicks.Location = new Point(
                450 - WrongClicks.Width / 2,
                TotalClicks.Location.Y
                );
            Controls.Add(WrongClicks);
            PostGameControls.Add(WrongClicks);

            PlayAgain = CreateButton("Play again");
            PostGameControls.Add(PlayAgain);
            PlayAgain.ForeColor = Color.FromArgb(255, 185, 185, 185);
            PlayAgain.FlatStyle = FlatStyle.Flat;
            PlayAgain.FlatAppearance.BorderSize = 1;
            PlayAgain.Location = new Point(
                300 - PlayAgain.Width / 2,
                TotalClicks.Location.Y - 10
                );
            PlayAgain.Click += PlayAgain_Click;

            TotalClicks.BringToFront();
            PlayAgain.BringToFront();
            WrongClicks.BringToFront();
        }

        private void DeleteOldGame()
        {
            foreach (Control control in PostGameControls)
            {
                if (control != null)
                {
                    Controls.Remove(control);
                    control.Dispose();
                }
            }

            foreach (Button button in allButtons)
            {
                Controls.Remove(button);
                button.Dispose();
            }
            allButtons.Clear();
        }

        private void GoBack_Click(object sender, EventArgs eventArgs)
        {
            DeletePreGameControls();
            DeletePostGameControls();
            ShowModeButtons();
        }
        private void DeletePreGameControls()
        {
            foreach (Control control in PreGameControls)
            {
                Controls.Remove(control);
                control.Dispose();
            }
        }
        private void DeletePostGameControls()
        {
            foreach (Control control in PostGameControls)
            {
                Controls.Remove(control);
                control.Dispose();
            }
        }
        private void buttonShowTime_Tick(object sender, EventArgs e)
        {
            buttonShowTime.Stop();
            HideKeyButtons();
            EnableClicks();
        }

        private void DisableClicks()
        {
            foreach (Button button in allButtons)
            {
                button.Enabled = false;
            }
        }
        private void EnableClicks()
        {
            foreach (Button button in allButtons)
            {
                button.Enabled = true;
            }
        }

        private void ShowKeyButtons()
        {
            foreach (Button keyButton in keyButtons)
            {
                keyButton.BackColor = Color.FromArgb(255, 185, 185, 185);
            }
        }
        private void HideKeyButtons()
        {
            foreach (Button keyButton in keyButtons)
            {
                keyButton.BackColor = Color.FromArgb(255, 35, 35, 35);
            }
        }
        private void HideKeyButtonsExceptGuessed()
        {
            foreach (Button keyButton in keyButtons)
            {
                if (!correctClickList.Contains(keyButton))
                {
                    keyButton.BackColor = Color.FromArgb(255, 35, 35, 35);
                }
            }
        }

        private void HideModeButtons()
        {
            Button[] modeButtons = { easyMode, normalMode, hardMode, absurdMode };

            foreach (Button button in modeButtons)
            {
                button.Visible = false;
                button.Enabled = false;
            }
        }
        private void ShowModeButtons()
        {
            Button[] modeButtons = { easyMode, normalMode, hardMode, absurdMode };

            foreach (Button button in modeButtons)
            {
                button.Visible = true;
                button.Enabled = true;
            }
        }
    }
}
