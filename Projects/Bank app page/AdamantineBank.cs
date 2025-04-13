using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Engine;

namespace AdamantineBank
{
    public partial class AdamantineBank : Form
    {
        private const int widthMiddle = 999 / 2;
        private const int heightMiddle = 611 / 2;

        Random random = new Random();

        private Label LoginText;
        private Label EnterName;
        private Label EnterPassword;
        private Label Result;
        private Button AttemptLogin;
        private TextBox EnterNameField;
        private TextBox EnterPasswordField;

        private bool grantAccess;
        private Label WelcomeText;
        private Timer timer;

        private Label BalanceField;
        private Label BalanceText;
        private Label textAdamantineBank;
        private Button MakeTransfer;
        private Button PayBill;
        private Button Transfers;
        private Button Accounts;
        private Button Transactions;

        private Label TransactionsText;


        public AdamantineBank()
        {
            InitializeComponent();

            Width = 999;
            Height = 611;
            BackColor = Color.DarkSeaGreen;

            int defaultWidth = widthMiddle - 150;
            int dynamicHeight = heightMiddle - 150;
            {
                LoginText = AddLabel(defaultWidth, dynamicHeight);
                LoginText.Font = new Font("Bombardier", 30f);
                LoginText.ForeColor = Color.RoyalBlue;
                LoginText.Text = "Login";

                EnterName = AddLabel(defaultWidth, dynamicHeight + 60);
                EnterName.Text = "Enter Name: ";

                EnterNameField = AddTextBox(defaultWidth, dynamicHeight + 100);

                EnterPassword = AddLabel(defaultWidth, dynamicHeight + 140);
                EnterPassword.Text = "Enter Password: ";

                EnterPasswordField = AddTextBox(defaultWidth, dynamicHeight + 180);

                Result = AddLabel(defaultWidth, dynamicHeight + 230);

                AttemptLogin = AddButton(defaultWidth, dynamicHeight + 290);
                AttemptLogin.ForeColor = Color.Blue;
                AttemptLogin.Font = new Font("Bombardier", 24f);
                AttemptLogin.Text = "Login";

                timer = new Timer();
                timer.Interval = 2000;
                timer.Tick += Timer_Tick;

                AttemptLogin.Click += (object sender, EventArgs e) =>
                {
                    grantAccess = LoginCheck(EnterNameField.Text, EnterPasswordField.Text, Result);

                    if (grantAccess)
                    {
                        LoginPageControlsHide();

                        WelcomeText = AddLabel(0, 0);
                        WelcomeText.Font = new Font("Bombardier", 36f);
                        WelcomeText.ForeColor = Color.DarkGreen;
                        WelcomeText.Text = "Welcome!";
                        WelcomeText.Location = new Point(
                            ClientSize.Width / 2 - WelcomeText.Width / 2,
                            ClientSize.Height / 2 - WelcomeText.Height / 2
                            );

                        timer.Start();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(EnterNameField.Text) || string.IsNullOrEmpty(EnterPasswordField.Text))
                        {
                            Result.ForeColor = Color.DarkCyan;
                            Result.Text = "Both fields must be filled!";
                        }
                        else
                        {
                            Result.ForeColor = Color.DarkRed;
                            Result.Text = "Invalid credentials!";

                            EnterNameField.Clear();
                            EnterPasswordField.Clear();
                        }
                    }
                };
            }
        }

        private Label AddLabel(int defaultX, int dynamicY)
        {
            Label label = new Label
            {
                Font = new Font("Bombardier", 22f),
                Text = "",
                AutoSize = true
            };
            Controls.Add(label);

            label.Location = new Point(
                defaultX,
                dynamicY
                );
            return label;
        }
        private TextBox AddTextBox(int defaultX, int dynamicY)
        {
            TextBox textBox = new TextBox
            {
                Font = new Font("Bombardier", 16f),
                Text = "",
                Width = 200,
                Height = 25
            };
            Controls.Add(textBox);

            textBox.Location = new Point(
                defaultX,
                dynamicY
                );
            return textBox;
        }
        private Button AddButton(int defaultX, int dynamicY)
        {
            Button button = new Button
            {
                Font = new Font("Bombardier", 16f),
                Text = "",
                AutoSize = true
            };
            Controls.Add(button);

            button.Location = new Point(
                defaultX,
                dynamicY
                );
            return button;
        }
        private bool LoginCheck(string name, string password, Label result)
        {
            string correctName = "Levron";
            string correctPassword = "NinjaTurtleRecall1234";

            return name.Trim() == correctName || name.Trim() == "admin" && password.Trim() == correctPassword || password.Trim() == "admin";
        }



        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Controls.Remove(WelcomeText);
            BackColor = Color.FromArgb(255, 25, 25, 25);

            double BalanceTrack = 1234567;

            textAdamantineBank = AddLabel(0, 0);
            textAdamantineBank.Font = new Font("Bombardier", 46f);
            textAdamantineBank.ForeColor = Color.WhiteSmoke;
            textAdamantineBank.Text = "Adamantine Banking";

            textAdamantineBank.Location = new Point(
                widthMiddle - textAdamantineBank.Width / 2,
                65
                );


            BalanceText = AddLabel(0, 0);
            BalanceText.Font = new Font("Bombardier", 20f);
            BalanceText.ForeColor = Color.LightGray;
            BalanceText.Text = "Balance";

            BalanceText.Location = new Point(
                widthMiddle - BalanceText.Width / 2,
                textAdamantineBank.Location.Y + 95
                );


            BalanceField = AddLabel(0, 0);
            BalanceField.Font = new Font("Bombardier", 38f);
            BalanceField.ForeColor = Color.WhiteSmoke;
            BalanceField.Text = $"${BalanceTrack.ToString("N2")}";

            BalanceField.Location = new Point(
                widthMiddle - BalanceField.Width / 2,
                BalanceText.Location.Y + 25
                );


            MakeTransfer = AddButton(0, 0);
            MakeTransfer.AutoSize = false;
            MakeTransfer.Width = 200;
            MakeTransfer.Height = 70;
            MakeTransfer.BackColor = Color.FromArgb(255, 201, 55, 54);
            MakeTransfer.ForeColor = Color.FromArgb(255, 45, 45, 45);
            MakeTransfer.Font = new Font("Bombardier", 20f);
            MakeTransfer.Text = "Make Transfer";

            MakeTransfer.Location = new Point(
                BalanceField.Location.X - 60,
                BalanceField.Location.Y + 90
                );

            MakeTransfer.Click += MakeTransfer_Click;


            PayBill = AddButton(0, 0);
            PayBill.AutoSize = false;
            PayBill.Width = 200;
            PayBill.Height = 70;
            PayBill.BackColor = Color.FromArgb(255, 0, 169, 255);
            PayBill.ForeColor = Color.FromArgb(255, 45, 45, 45);
            PayBill.Font = new Font("Bombardier", 20f);
            PayBill.Text = "Pay Bill";

            PayBill.Location = new Point(
                MakeTransfer.Location.X + MakeTransfer.Width + 40,
                MakeTransfer.Location.Y
                );

            PayBill.Click += PayBill_Click;


            Transactions = AddButton(0, 0);
            Transactions.AutoSize = false;
            Transactions.Width = 200;
            Transactions.Height = 70;
            Transactions.BackColor = Color.ForestGreen;
            Transactions.ForeColor = Color.FromArgb(255, 45, 45, 45);
            Transactions.Font = new Font("Bombardier", 20f);
            Transactions.Text = "Transactions";

            Transactions.Location = new Point(
                MakeTransfer.Location.X + 120,
                MakeTransfer.Location.Y + MakeTransfer.Height + 10
                );

            Transactions.Click += Transactions_Click;


            Accounts = AddButton(0, 0);
            Accounts.ForeColor = Color.WhiteSmoke;
            Accounts.Font = new Font("Bombardier", 18f);
            Accounts.Text = "Accounts";
            Accounts.Location = new Point(
                ClientSize.Width / 4 - 10,
                0
                );


            Transfers = AddButton(0, 0);
            Transfers.ForeColor = Color.WhiteSmoke;
            Transfers.Font = new Font("Bombardier", 18f);
            Transfers.Text = "Transfers";

            Transfers.Location = new Point(
                ClientSize.Width * 3 / 4 - 100,
                0
                );
        }

        private void LoginPageControlsHide()
        {
            LoginText.Visible = false;
            EnterName.Visible = false;
            EnterNameField.Visible = false;
            EnterPassword.Visible = false;
            EnterPasswordField.Visible = false;
            AttemptLogin.Visible = false;
            Result.Visible = false;
        }

        private void MainPageControlsHide()
        {
            Accounts.Visible = false;
            Transfers.Visible = false;
            textAdamantineBank.Visible = false;
            BalanceText.Visible = false;
            BalanceField.Visible = false;
            MakeTransfer.Visible = false;
            PayBill.Visible = false;
            Transactions.Visible = false;
        }

        private Label recentTransaction;

        private void MakeTransfer_Click(object sender, EventArgs eventArgs)
        {
            
        }

        private void PayBill_Click(object sender, EventArgs eventArgs)
        {

        }

        private void Transactions_Click(object sender, EventArgs eventArgs)
        {
            {
                MainPageControlsHide();

                TransactionsText = AddLabel(0, 0);
                TransactionsText.Font = new Font("Bombardier", 46f);
                TransactionsText.ForeColor = Color.WhiteSmoke;
                TransactionsText.Text = "Transactions";

                TransactionsText.Location = new Point(
                    widthMiddle - TransactionsText.Width / 2,
                    35
                    );


                List<string> firstNames = new List<string>();
                List<string> lastNames = new List<string>();

                string namesPath = @"J:\Stuff\truly obscure\1001RandomNames.txt";

                string[] entries = File.ReadAllText(namesPath).Split(new char[] { ',', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < entries.Length; i += 2)
                {
                    firstNames.Add(entries[i]);
                    lastNames.Add(entries[i + 1]);
                }

                string transactionTypesPath = @"J:\Stuff\truly obscure\transactionTypes.txt";
                string transactionTypes = File.ReadAllText(transactionTypesPath);
                transactionTypes = transactionTypes.Replace("\"", "");

                List<string> validTransactionTypes = new List<string>(
                    transactionTypes
                    .Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList());


                string validDatesPath = @"J:\Stuff\truly obscure\everyValidDateCombo.txt";

                List<string> validDates = new List<string>(
                    File.ReadAllText(validDatesPath)
                    .Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList());


                Transactions transactions = new Transactions();

                foreach (string name in firstNames.Take(20))
                {
                    string randomName = firstNames[random.Next(0, firstNames.Count)] + " " + lastNames[random.Next(0, lastNames.Count)];
                    string randomTransactionType = validTransactionTypes[random.Next(0, validTransactionTypes.Count)];
                    string randomDate = validDates[random.Next(0, validDates.Count)];
                    int amount = random.Next(0, 199999);

                    Transaction transaction = new Transaction($"${amount.ToString("N2")}", randomTransactionType, randomDate, randomName);
                    transactions.TransactionList.Add(transaction);
                }
                int defaultWidth = 25, temp = TransactionsText.Location.Y + 25, defaultHeight = temp;
                foreach (Transaction transaction in transactions.TransactionList)
                {
                    defaultHeight += 125;

                    if (ClientSize.Height - defaultHeight < 125)
                    {
                        defaultHeight = temp;
                        defaultWidth += 200;
                    }
                    if (ClientSize.Width - defaultWidth < 75)
                    {
                        continue;
                    }

                    Label transactionAmount = new Label();
                    Label transactionType = new Label();
                    Label transactionDate = new Label();
                    Label transactionSender = new Label();

                    transactionSender.AutoSize = true;
                    transactionSender.Font = new Font("Bombardier", 21f);
                    transactionSender.ForeColor = Color.FromArgb(255, 125, 125, 125);
                    transactionSender.Text = transaction.Sender;
                    Controls.Add(transactionSender);

                    transactionSender.Location = new Point(
                        defaultWidth,
                        defaultHeight
                        );

                    transactionType.AutoSize = true;
                    transactionType.Font = new Font("Bombardier", 17f);
                    transactionType.ForeColor = Color.WhiteSmoke;
                    transactionType.Text = transaction.Type;
                    Controls.Add(transactionType);

                    transactionType.Location = new Point(
                        transactionSender.Location.X,
                        transactionSender.Location.Y + transactionSender.Height
                        );

                    transactionDate.AutoSize = true;
                    transactionDate.Font = new Font("Bombardier", 12f);
                    transactionDate.ForeColor = Color.LightGray;
                    transactionDate.Text = transaction.Date;
                    Controls.Add(transactionDate);

                    transactionDate.Location = new Point(
                        transactionType.Location.X + 5,
                        transactionType.Location.Y + 25
                        );

                    transactionAmount.AutoSize = true;
                    transactionAmount.Font = new Font("Bombardier", 16f);
                    
                    if (transactionType.Text.Contains("Withdrawal"))
                    {
                        transactionAmount.ForeColor = Color.MediumVioletRed;
                    }
                    else
                    {
                        transactionAmount.ForeColor = Color.PaleGreen;
                    }
                    transactionAmount.Text = transaction.Amount;

                    Controls.Add(transactionAmount);

                    transactionAmount.Location = new Point(
                        transactionDate.Location.X + transactionDate.Width + 25,
                        transactionType.Location.Y + 25
                        );
                }
            }
        }

        private void AdamantineBank_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}