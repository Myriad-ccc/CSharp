using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ShapeShift;

namespace Dragging
{
    public partial class Form1 : Form
    {
        private readonly Random random = new Random();

        private bool isFormDragging = false;
        private Point lastCursorPos;
        private Panel FormBorder;
        private CustomButton CloseButton;

        private CustomPanel GameArea;
        private Shape shape;
        private readonly List<Wall> Walls = new List<Wall>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureForm();
            AddFormBorder();
            AddGameArea();
        }

        private void ConfigureForm()
        {
            Width = 1000;
            Height = 800;
            Width -= Width - ClientSize.Width;
            Height -= Height - ClientSize.Height;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.FromArgb(255, 27, 27, 27);
            DoubleBuffered = true;

            Paint += Form1_Paint;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
        }

        private void AddFormBorder()
        {
            FormBorder = new Panel()
            {
                BackColor = Color.FromArgb(255, 25, 25, 25),
                Location = new Point(0, 0),
                Size = new Size(ClientSize.Width, 40)
            };
            FormBorder.MouseDown += (object s, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Left && e.Y <= 40 && e.X < ClientSize.Width - 50)
                {
                    isFormDragging = true;
                    lastCursorPos = Cursor.Position;
                }
            };
            FormBorder.MouseUp += (object s, MouseEventArgs e) =>
            {
                isFormDragging = false;
            };
            FormBorder.MouseMove += (object s, MouseEventArgs e) =>
            {
                if (isFormDragging)
                {
                    int deltaX = Cursor.Position.X - lastCursorPos.X;
                    int deltaY = Cursor.Position.Y - lastCursorPos.Y;

                    ActiveForm.Left += deltaX;
                    ActiveForm.Top += deltaY;

                    lastCursorPos = Cursor.Position;
                }
            };
            Controls.Add(FormBorder);
            FormBorder.Paint += FormBorder_Paint;

            CloseButton = new CustomButton(disableOnHover: true)
            {
                Size = new Size(50, 40),
                BackColor = Color.IndianRed,
                ForeColor = Color.FromArgb(255, 20, 20, 20),
                Text = "X",
            };
            CloseButton.Location = new Point(ClientSize.Width - CloseButton.Width, 0);
            CloseButton.MouseClick += (s, ev) => { Application.Exit(); };
            FormBorder.Controls.Add(CloseButton);
        }

        private void FormBorder_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            using (Brush barBrush = new SolidBrush(FormBorder.BackColor))
            {
                g.FillRectangle(barBrush, 0, 0, ClientSize.Width, 40);
            }

            int shadowOffset = 1;
            using (Font textFont = new Font("Bombardier", 30f))
            using (Brush shadowBrush = new SolidBrush(Color.FromArgb(255, random.Next(256), random.Next(256), random.Next(256))))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(255, 235, 235, 235)))
            {
                g.DrawString(this.Text, textFont, shadowBrush, new Point(shadowOffset, shadowOffset));
                g.DrawString(this.Text, textFont, textBrush, new Point(0, 0));
            }
        }

        private void AddGameArea()
        {
            GameArea = new CustomPanel
            {
                Location = new Point(20, FormBorder.Height + 20),
                Size = new Size(
                    Width - 40,
                    Height - FormBorder.Height - 40),
            };
            Controls.Add(GameArea);

            for (int i = 0; i < random.Next(1, 4); i++)
            {
                Walls.Add(RandomWallSpawn());
            }

            shape = new Shape
                (
                type: "Square",
                rectangle: new Rectangle
                (new Point(Width / 2 - 25, Height / 2 - 25),
                new Size(50, 50)),
                fillColor: Color.CornflowerBlue,
                borderColor: Color.RoyalBlue,
                isDragging: false,
                lastCursorPoint: Point.Empty
                );
            GameArea.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left && shape.Rectangle.Contains(e.Location) && ActiveForm.ClientRectangle.Contains(e.Location))
                {
                    shape.IsDragging = true;
                    shape.LastCursorPoint = Cursor.Position;
                }
            };
            GameArea.MouseUp += (s, e) =>
            {
                shape.IsDragging = false;
            };
            GameArea.MouseMove += (s, e) =>
            {
                if (shape.IsDragging)
                {
                    int deltaX = Cursor.Position.X - shape.LastCursorPoint.X;
                    int deltaY = Cursor.Position.Y - shape.LastCursorPoint.Y;

                    int shapeX = shape.Rectangle.Location.X;
                    int shapeY = shape.Rectangle.Location.Y;

                    shapeX += deltaX;
                    shapeY += deltaY;

                    //if (shapeX <= 6)
                    //    shapeX = Math.Max(6, shapeX);
                    //if (shapeX >= GameArea.Width - shape.Rectangle.Width - 6)
                    //    shapeX = Math.Min(GameArea.Width - shape.Rectangle.Width - 6, shapeX);
                    //if (shapeY <= 6)
                    //    shapeY = Math.Max(6, shapeY);
                    //if (shapeY >= GameArea.Height - shape.Rectangle.Height - 6)
                    //    shapeY = Math.Min(GameArea.Height - shape.Rectangle.Height - 6, shapeY);

                    shapeX = Math.Max(6, Math.Min(shapeX, GameArea.Width - shape.Rectangle.Width - 6));
                    shapeY = Math.Max(6, Math.Min(shapeY, GameArea.Height - shape.Rectangle.Height - 6));


                    shape.Rectangle = new Rectangle(new Point(shapeX, shapeY), shape.Rectangle.Size);
                    shape.LastCursorPoint = Cursor.Position;

                    GameArea.Invalidate();
                }
            };

            var DeleteSpawnCollisions = new Timer { Interval = 1 };
            DeleteSpawnCollisions.Tick += (s, ev) =>
            {
                DeleteSpawnCollisions.Stop();

                Walls.RemoveAll(x => x.Bounds.IntersectsWith(shape.Rectangle));
                GameArea.Invalidate();
            };
            DeleteSpawnCollisions.Start();

            this.Invalidate();
            GameArea.Paint += (s, ev) =>
            {
                Graphics g = ev.Graphics;

                foreach (Wall wall in Walls)
                {
                    using (Brush fillWallBrush = new SolidBrush((Color)wall.Color))
                    {
                        g.FillRectangle(fillWallBrush, wall.Bounds);
                    }
                    g.DrawRectangle(new Pen(Color.Black), wall.Bounds);
                }

                using (Pen borderPen = new Pen(Color.FromArgb(255, 225, 225, 225), 5f))
                {
                    double halfThickness = (double)borderPen.Width / 2;
                    int inset = (int)Math.Round(halfThickness, MidpointRounding.AwayFromZero);
                    g.DrawRectangle(borderPen, inset, inset, GameArea.ClientSize.Width - (inset * 2) - 1, GameArea.ClientSize.Height - (inset * 2) - 1);
                }

                using (Brush fillBrush = new SolidBrush(shape.FillColor))
                { g.FillRectangle(fillBrush, shape.Rectangle); }

                using (Pen borderPen = new Pen(shape.BorderColor))
                { g.DrawRectangle(borderPen, shape.Rectangle); }
            };
        }

        private Wall RandomWallSpawn()
        {
            Rectangle spawnBounds = new Rectangle(new Point(7, 7), new Size(914, 628)); // 928, 642

            return new Wall(new Rectangle(
                new Point(
                    random.Next(spawnBounds.Location.X, spawnBounds.Size.Width + 1),
                    random.Next(spawnBounds.Location.Y, spawnBounds.Size.Height + 1)),
                new Size(
                    random.Next(80, 200),
                    random.Next(60, 180))));
        }
    }
}
