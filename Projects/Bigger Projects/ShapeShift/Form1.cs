using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShapeShift
{
    public partial class Form1 : Form
    {
        private readonly Random random = new Random();

        private bool isFormDragging = false;
        private Point lastCursorPos;
        private Panel FormBorder;
        private CustomButton CloseButton;

        private CustomPanel CreationPanel;
        private CustomButton DeleteShape;
        private CustomButton AddShape;
        private CustomButton RefreshShape;
        private CustomComboBox ShapeComboBox;

        private CustomPanel PropertyPanel;
        private CustomTextBox ShapeWidth;
        private CustomTextBox ShapeHeight;
        private CustomTextBox ShapeSides;
        private CustomButton ShapeToFill;

        private CustomComboBox FillColors;
        private CustomComboBox BorderColors;
        private CustomButton ResetProperties;

        private CustomPanel GameArea;
        private Shape defaultShape;
        private Shape shape;
        private bool FillButton = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureForm(); // sets the form's default properties
            AddFormBorder(); // adds a custom form border
            AddGameArea(); // defines an area accessible to the shape
            DefineShape(); // adds a template for the shape's default state
            ShapeConsole(); // adds controls used for modifying the shape
        }

        private void ConfigureForm()
        {
            Width = 1000;
            Height = 800;
            Width += Width - ClientSize.Width;
            Height += 2 * (Height - ClientSize.Height);
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.FromArgb(255, 27, 27, 27);
            DoubleBuffered = true;
            KeyPreview = true;

            KeyDown += Form1_KeyDown;
            Paint += Form1_Paint;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // does not work
            if (e.KeyCode == Keys.Escape)
            {
                Control focusedControl = this.ActiveControl;
                if (focusedControl != null)
                {
                    this.Focus();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawString("Create", new Font("Bombardier", 32f), Brushes.CornflowerBlue, new Point(51, 56));
            using (Brush textBrush = new SolidBrush(Color.FromArgb(255, 225, 225, 225)))
            { g.DrawString("Create", new Font("Bombardier", 32f), textBrush, new Point(50, 55)); }

            g.DrawString("Properties", new Font("Bombardier", 32f), Brushes.RoyalBlue, new Point(31, 224));
            using (Brush textBrush = new SolidBrush(Color.FromArgb(255, 225, 225, 225)))
            { g.DrawString("Properties", new Font("Bombardier", 32f), textBrush, new Point(30, 223)); }
        }

        private void AddFormBorder()
        {
            FormBorder = new Panel()
            {
                BackColor = Color.FromArgb(255, 20, 20, 20),
                Location = new Point(0, 0),
                Size = new Size(ClientSize.Width, 39)
            };
            FormBorder.MouseDown += (object s, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Left && e.Y <= 39 && e.X < ClientSize.Width - 50)
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
                Size = new Size(50, 39),
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
                g.FillRectangle(barBrush, 0, 0, ClientSize.Width, 39);
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

        private void DefineShape()
        {
            defaultShape = new Shape
                (
                type: "",
                rectangle: new Rectangle
                (new Point(GameArea.ClientRectangle.Location.X + GameArea.Width / 2 - 100, GameArea.ClientRectangle.Location.Y + GameArea.Height / 2 - 100),
                new Size(200, 200)),
                points: null,
                fillColor: Color.FromArgb(255, 153, 153, 153),
                borderColor: Color.FromArgb(255, 225, 225, 225),
                borderThickness: 3f,
                toFill: false,
                size: Size.Empty,
                sides: 5
                );
            shape = defaultShape;
        }

        private void ShapeConsole()
        {
            CreationPanel = new CustomPanel()
            {
                Width = 200,
                Height = 130,
                Top = GameArea.Top + 50,
                Left = 20,
                BackColor = Color.Transparent
            };
            Controls.Add(CreationPanel);

            ShapeComboBox = new CustomComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 200,
                BackColor = Color.FromArgb(255, 25, 25, 25),
                ForeColor = Color.FromArgb(255, 225, 225, 225),
                Location = Point.Empty,
            };
            //ShapeComboBox.Items.Add("Base");
            ShapeComboBox.Items.Add("Rectangle");
            ShapeComboBox.Items.Add("Polygon");
            ShapeComboBox.Items.Add("Ellipse");
            ShapeComboBox.Items.Add("Triangle");
            ShapeComboBox.Items.Add("!Polygon!");
            ShapeComboBox.Items.Add("Shape");
            ShapeComboBox.SelectedIndex = ShapeComboBox.Items.Count - 1;
            ShapeComboBox.SelectedIndexChanged += ShapeComboBox_SelectedIndexChanged;
            CreationPanel.Controls.Add(ShapeComboBox);

            DeleteShape = new CustomButton()
            {
                Width = 60,
                Height = 60,
                Location = new Point(10, ShapeComboBox.Bottom + 5),
                BackColor = Color.FromArgb(255, 25, 25, 25),
                ForeColor = Color.IndianRed,
                Text = "-"
            };

            DeleteShape.Click += DeleteShape_Click;
            DeleteShape.Click += (s, ev) => { GameArea.Invalidate(); };
            CreationPanel.Controls.Add(DeleteShape);

            AddShape = new CustomButton()
            {
                Width = 60,
                Height = 60,
                Location = new Point(70, DeleteShape.Location.Y),
                BackColor = Color.FromArgb(255, 25, 25, 25),
                ForeColor = Color.PaleGreen,
                Text = "+"
            };
            AddShape.Click += ShapeComboBox_SelectedIndexChanged;
            AddShape.Click += (s, ev) => { GameArea.Invalidate(); };
            CreationPanel.Controls.Add(AddShape);

            RefreshShape = new CustomButton()
            {
                Width = 60,
                Height = 60,
                BackColor = Color.FromArgb(255, 25, 25, 25),
                Font = new Font("Courier New", 30f),
                ForeColor = Color.White,
                Text = "↻",
                Location = new Point(130, DeleteShape.Location.Y)
            };
            RefreshShape.Click += (s, ev) => { GameArea.Invalidate(); };
            CreationPanel.Controls.Add(RefreshShape);



            PropertyPanel = new CustomPanel()
            {
                Width = 500,
                Height = 400,
                Top = CreationPanel.Location.Y + CreationPanel.Height + 35,
                Left = CreationPanel.Left - 10,
                BackColor = Color.Transparent
            };
            Controls.Add(PropertyPanel);

            PropertyPanel.Paint += (s, ev) =>
            {
                using (Font textFont = new Font("Gilroy-Medium", 20f))
                using (Brush textBrush = new SolidBrush(Color.FromArgb(255, 225, 225, 225)))
                {
                    ev.Graphics.DrawString("Width: ", textFont, textBrush,
                        new Point(85, 0));

                    ev.Graphics.DrawString("Height: ", textFont, textBrush,
                        new Point(85, 40));

                    ev.Graphics.DrawString("Sides: ", textFont, textBrush,
                        new Point(85, 85));
                }
            };

            Color textForeColor = Color.FromArgb(255, 225, 225, 225);
            ShapeWidth = new CustomTextBox()
            {
                Width = 50,
                ForeColor = textForeColor,
                Font = new Font("Gilroy-Medium", 18f),
                Text = "200",
                Location = new Point(180, 0)
            };
            ShapeWidth.TextChanged += (s, ev) =>
            {
                try
                {
                    int customWidth = int.Parse(ShapeWidth.Text);
                    if (customWidth <= 0 || customWidth >= GameArea.Width)
                        ShapeWidth.ForeColor = Color.IndianRed;
                    else
                    {
                        ShapeWidth.ForeColor = textForeColor;

                        int shapeWidth = shape.W;
                        if (customWidth != defaultShape.W && customWidth > 0 && customWidth < GameArea.Width)
                            shapeWidth = customWidth;
                        shape.Rectangle = new Rectangle(shape.Rectangle.Location, new Size(shapeWidth, shape.Rectangle.Size.Height));
                    }
                    GameArea.Invalidate();
                }
                catch (Exception) { }
                ;
            };
            PropertyPanel.Controls.Add(ShapeWidth);

            ShapeHeight = new CustomTextBox()
            {
                Width = 50,
                ForeColor = Color.FromArgb(255, 225, 225, 225),
                Font = new Font("Gilroy-Medium", 18f),
                Text = "200",
                Location = new Point(180, 40)
            };
            ShapeHeight.TextChanged += (s, ev) =>
            {
                try
                {
                    int customHeight = int.Parse(ShapeHeight.Text);
                    if (customHeight <= 0 || customHeight >= GameArea.Height)
                        ShapeHeight.ForeColor = Color.IndianRed;
                    else
                    {
                        ShapeHeight.ForeColor = textForeColor;

                        int shapeHeight = shape.H;
                        if (customHeight != defaultShape.H && customHeight > 0 && customHeight < GameArea.Height)
                            shapeHeight = customHeight;
                        shape.Rectangle = new Rectangle(shape.Rectangle.Location, new Size(shape.Rectangle.Size.Width, shapeHeight));
                    }
                    GameArea.Invalidate();
                }
                catch (Exception) { }
                ;
            };
            PropertyPanel.Controls.Add(ShapeHeight);

            ShapeSides = new CustomTextBox()
            {
                Width = 50,
                ForeColor = textForeColor,
                Text = "5",
                Location = new Point(180, 80)
            };
            ShapeSides.TextChanged += (s, ev) =>
            {
                try
                {
                    int customSides = int.Parse(ShapeSides.Text);
                    if (customSides < 3 || customSides > 99) // remove upper cap for pc melt, lower cap for crash
                        ShapeSides.ForeColor = Color.IndianRed;
                    else
                    {
                        ShapeSides.ForeColor = textForeColor;
                        shape.Sides = customSides;
                    }
                    GameArea.Invalidate();
                }
                catch (Exception) { }
                ;
            };
            PropertyPanel.Controls.Add(ShapeSides);
            ShapeSides.Enabled = false;

            ShapeComboBox.SelectedIndexChanged += (s, ev) =>
            {
                string selectedIndex = ShapeComboBox.SelectedItem?.ToString();
                if (selectedIndex == "Polygon" || selectedIndex == "!Polygon!")
                    ShapeSides.Enabled = true;
                else
                {
                    ShapeSides.Enabled = false;
                    shape.Sides = 5;
                }
            };

            ShapeToFill = new CustomButton(disableOnHover: true, disableOnPress: true)
            {
                Width = 75,
                Height = 75,
                BackColor = Color.FromArgb(255, 25, 25, 25),
                ForeColor = Color.FromArgb(255, 225, 225, 225),
                Text = "Fill",
                Location = new Point(0, ShapeSides.Bottom - 75)
            };
            ShapeToFill.Click += (s, ev) =>
            {
                FillButton = !FillButton;
                UpdateFillButton();
            };
            PropertyPanel.Controls.Add(ShapeToFill);

            FillColors = new CustomComboBox(tryFontSize: 18f)
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 230,
                BackColor = Color.FromArgb(255, 25, 25, 25),
                ForeColor = Color.FromArgb(255, 225, 225, 225),
                Location = new Point(0, 130)
            };
            FillColors.Items.Add("Fill Color");
            AddWebColors(FillColors);
            FillColors.SelectedIndex = 0;
            FillColors.SelectedIndexChanged += (s, ev) =>
            {
                if (FillColors.SelectedIndex > 0
                && FillColors.SelectedIndex <= FillColors.Items.Count
                && FillColors.SelectedItem is KnownColor selectedKnownColor
                && shape != null)
                {
                    Color selectedColor = Color.FromKnownColor(selectedKnownColor);
                    shape.FillColor = selectedColor;
                }
                else shape.FillColor = Color.FromArgb(255, 225, 225, 225);
                GameArea.Invalidate();
            };
            PropertyPanel.Controls.Add(FillColors);

            BorderColors = new CustomComboBox(tryFontSize: 18f)
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 230,
                BackColor = Color.FromArgb(255, 25, 25, 25),
                ForeColor = Color.FromArgb(255, 225, 225, 225),
                Location = new Point(0, FillColors.Bottom + 5)
            };
            BorderColors.Items.Add("Border Color");
            AddWebColors(BorderColors);
            BorderColors.SelectedIndex = 0;
            BorderColors.SelectedIndexChanged += (s, ev) =>
            {
                if (BorderColors.SelectedIndex > 0
                && BorderColors.SelectedIndex <= BorderColors.Items.Count
                && BorderColors.SelectedItem is KnownColor selectedKnownColor
                && shape != null)
                {
                    Color selectedColor = Color.FromKnownColor(selectedKnownColor);
                    shape.BorderColor = selectedColor;
                }
                else shape.BorderColor = Color.FromArgb(255, 225, 225, 225);
                GameArea.Invalidate();
            };
            PropertyPanel.Controls.Add(BorderColors);

            ResetProperties = new CustomButton(tryFontSize: 18f, disableOnHover: true, disableOnPress: true)
            {
                Width = 75,
                Height = 40,
                BackColor = Color.FromArgb(255, 25, 25, 25),
                ForeColor = Color.IndianRed,
                Text = "Reset",
                Location = new Point(0, 0)
            };
            ResetProperties.Click += (s, ev) =>
            {
                ShapeWidth.Text = 200.ToString();
                ShapeHeight.Text = 200.ToString();
                ShapeSides.Text = defaultShape.Sides.ToString();
                FillColors.SelectedIndex = 0;
                BorderColors.SelectedIndex = 0; FillButton = false;
                UpdateFillButton(); // contains GameArea.Invalidate()
            };
            PropertyPanel.Controls.Add(ResetProperties);
        }

        private void UpdateFillButton()
        {
            if (FillButton)
            {
                ShapeToFill.BackColor = Color.FromArgb(255, 225, 225, 225);
                ShapeToFill.ForeColor = Color.FromArgb(255, 25, 25, 25);
                if (shape != null)
                    shape.ToFill = true;
            }
            else
            {
                ShapeToFill.BackColor = Color.FromArgb(255, 25, 25, 25);
                ShapeToFill.ForeColor = Color.FromArgb(255, 225, 225, 225);
                if (shape != null)
                    shape.ToFill = false;
            }
            GameArea.Invalidate();
        }

        private void AddWebColors(ComboBox list)
        {
            foreach (KnownColor knownColor in Enum.GetValues(typeof(KnownColor)))
            {
                Color color = Color.FromKnownColor(knownColor);
                if (!color.IsSystemColor)
                    list.Items.Add(knownColor);
            }
        }

        private void DrawShape(Graphics g)
        {
            if (shape != null)
            {
                switch (shape.Type)
                {
                    //case "Base":
                    //    using (Brush fillBrush = new SolidBrush(shape.FillColor))
                    //    { g.FillRectangle(fillBrush, shape.Rectangle); }

                    //    using (Pen borderPen = new Pen(shape.BorderColor, shape.BorderThickness))
                    //    { g.DrawRectangle(borderPen, shape.Rectangle); }
                    //    break;
                    case "Rectangle":
                        if (shape.ToFill)
                        {
                            using (Brush fillBrush = new SolidBrush(shape.FillColor))
                            { g.FillRectangle(fillBrush, shape.Rectangle); }
                        }
                        using (Pen borderPen = new Pen(shape.BorderColor, shape.BorderThickness))
                        { g.DrawRectangle(borderPen, shape.Rectangle); }
                        break;
                    case "Polygon":
                        if (shape.ToFill)
                        {
                            using (Brush fillBrush = new SolidBrush(shape.FillColor))
                            { g.FillPolygon(fillBrush, shape.Points); }
                        }
                        using (Pen borderPen = new Pen(shape.BorderColor, shape.BorderThickness))
                        { g.DrawPolygon(borderPen, shape.Points); }
                        break;
                    case "Ellipse":
                        if (shape.ToFill)
                        {
                            using (Brush fillBrush = new SolidBrush(shape.FillColor))
                            { g.FillEllipse(fillBrush, shape.Rectangle); }
                        }
                        using (Pen borderPen = new Pen(shape.BorderColor, shape.BorderThickness))
                        { g.DrawEllipse(borderPen, shape.Rectangle); }
                        break;
                    case "Triangle":
                        GraphicsState data = g.Save();
                        try
                        {
                            g.TranslateTransform(shape.Rectangle.Location.X, shape.Rectangle.Location.Y);
                            if (shape.ToFill)
                            {
                                using (Brush fillBrush = new SolidBrush(shape.FillColor))
                                { g.FillPolygon(fillBrush, shape.Points); }
                            }
                            using (Pen borderPen = new Pen(shape.BorderColor, shape.BorderThickness))
                            { g.DrawPolygon(borderPen, shape.Points); }
                        }
                        finally
                        {
                            g.Restore(data);
                        }
                        break;
                    case "Undefined Polygon":
                        if (shape.ToFill)
                        {
                            using (Brush fillBrush = new SolidBrush(shape.FillColor))
                            { g.FillPolygon(fillBrush, shape.Points); }
                        }
                        using (Pen borderPen = new Pen(shape.BorderColor, shape.BorderThickness))
                        { g.DrawPolygon(borderPen, shape.Points); }
                        break;
                }
            }
        }

        private void ShapeDelete() { shape = null; }

        private void DeleteShape_Click(object sender, EventArgs e)
        { ShapeDelete(); }

        private PointF[] PolygonVertices(Rectangle bounds, int sides)
        {
            if (sides < 3) sides = 3;

            PointF[] vertices = new PointF[sides];
            float h = bounds.X + bounds.Width / 2f;
            float k = bounds.Y + bounds.Height / 2f;
            float radius = Math.Min(bounds.Width / 2f, bounds.Height / 2f);

            double startAngle = -Math.PI / 2.0;
            double angleStep = (2 * Math.PI) / sides;

            for (int i = 0; i < sides; i++)
            {
                double currentAngle = startAngle + i * angleStep;
                float offsetX = h + (float)(radius * Math.Sin(currentAngle));
                float offsetY = k + (float)(radius * Math.Cos(currentAngle));
                vertices[i] = new PointF(offsetX, offsetY);
            }
            return vertices;
        }

        private PointF[] RandomPolygon(Rectangle bounds, int sides)
        {
            PointF[] vertices = new PointF[sides];
            float offsetX = shape.Rectangle.Location.X;
            float offsetY = shape.Rectangle.Location.Y;

            for (int i = 0; i < shape.Sides; i++)
            {
                vertices[i] = new PointF(
                    random.Next(shape.Rectangle.Size.Width + 1) + offsetX,
                    random.Next(shape.Rectangle.Size.Height + 1) + offsetY);
            }
            return vertices;
        }

        private void SetShape()
        {
            if (ShapeComboBox.Items.Contains("Shape"))
                ShapeComboBox.Items.Remove("Shape");

            shape = defaultShape;
            if (ShapeComboBox.SelectedIndex >= 0 && ShapeComboBox.SelectedIndex <= ShapeComboBox.Items.Count)
            {
                switch (ShapeComboBox.SelectedIndex)
                {
                    case 0:
                        shape.Type = "Rectangle";
                        break;
                    case 1:
                        shape.Type = "Polygon";
                        shape.Points = PolygonVertices(shape.Rectangle, shape.Sides);
                        break;
                    case 2:
                        shape.Type = "Ellipse";
                        break;
                    case 3:
                        shape.Type = "Triangle";
                        shape.Points = new PointF[]
                        {
                            new Point(shape.Rectangle.Width / 2, 0),
                            new Point(0, shape.Rectangle.Height),
                            new Point(shape.Rectangle.Width, shape.Rectangle.Height)
                        };
                        break;
                    case 4:
                        shape.Type = "Undefined Polygon";
                        shape.Points = RandomPolygon(shape.Rectangle, shape.Sides);
                        break;
                }
            }
        }

        private void ShapeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShapeDelete();
            SetShape();
        }

        private void AddGameArea()
        {
            GameArea = new CustomPanel
            {
                Width = 500,
                Height = 757,
                Left = 250,
                Top = 60
            };
            Controls.Add(GameArea);

            GameArea.Paint += (s, ev) =>
            {
                Graphics g = ev.Graphics;

                // Game Area's border
                using (Pen borderPen = new Pen(Color.FromArgb(255, 225, 225, 225), 3f))
                {
                    double halfThickness = (double)borderPen.Width / 2;
                    int inset = (int)Math.Round(halfThickness, MidpointRounding.AwayFromZero);
                    g.DrawRectangle(borderPen, inset, inset, GameArea.ClientSize.Width - (inset * 2) - 1, GameArea.ClientSize.Height - (inset * 2) - 1);
                }

                DrawShape(g);
            };

            GameArea.MouseDown += GameArea_MouseDown;
            GameArea.MouseUp += GameArea_MouseUp;
            GameArea.MouseMove += GameArea_MouseMove;
        }

        private bool CheckEllipseClick(Rectangle bounds, Point click)
        {
            float h = bounds.X + bounds.Width / 2.0f; // center X h
            float k = bounds.Y + bounds.Height / 2.0f; // center Y k
            float a = bounds.Width / 2; // semi-major axis
            float b = bounds.Height / 2; // semi-minor axis

            if (a <= 0 || b <= 0)
                return false;

            // displacement of click from center
            float dx = click.X - h;
            float dy = click.Y - k;

            double V = (dx * dx) / (a * a) + (dy * dy) / (b * b);

            return V <= 1;
        }

        private void GameArea_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left && shape != null)
            {
                bool hitsShape = false;

                if (shape.Type == "Ellipse")
                {
                    hitsShape = CheckEllipseClick(shape.Rectangle, e.Location);
                }
                else
                {
                    hitsShape = shape.Rectangle.Contains(e.Location);
                }

                if (hitsShape && ActiveForm.ClientRectangle.Contains(e.Location))
                {
                    shape.IsDragging = true;
                    shape.LastCursorPoint = Cursor.Position;
                }
            }
        }
        private void GameArea_MouseUp(object sender, MouseEventArgs e)
        {
            shape.IsDragging = false;
        }
        private void GameArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (shape != null && shape.IsDragging)
            {
                int deltaX = Cursor.Position.X - shape.LastCursorPoint.X;
                int deltaY = Cursor.Position.Y - shape.LastCursorPoint.Y;

                int shapeX = shape.Rectangle.Location.X;
                int shapeY = shape.Rectangle.Location.Y;

                shapeX += deltaX;
                shapeY += deltaY;

                //if (shapeX <= 6)
                //    shapeX = Math.Max(6, shapeX);
                //if (shapeX >= GameArea.Width - shape.Rectangle.Width - 7)
                //    shapeX = Math.Min(GameArea.Width - shape.Rectangle.Width - 6, shapeX);
                //if (shapeY <= 6)
                //    shapeY = Math.Max(6, shapeY);
                //if (shapeY >= GameArea.Height - shape.Rectangle.Height - 7)
                //    shapeY = Math.Min(GameArea.Height - shape.Rectangle.Height - 6, shapeY);

                shapeX = Math.Max(6, Math.Min(shapeX, GameArea.Width - shape.Rectangle.Width - 7));
                shapeY = Math.Max(6, Math.Min(shapeY, GameArea.Height - shape.Rectangle.Height - 7));


                shape.Rectangle = new Rectangle(new Point(shapeX, shapeY), shape.Rectangle.Size);
                shape.LastCursorPoint = Cursor.Position;

                if (shape.Type == "Polygon")
                    shape.Points = PolygonVertices(shape.Rectangle, shape.Sides);
                else if (shape.Type == "Undefined Polygon")
                    shape.Points = RandomPolygon(shape.Rectangle, shape.Sides);

                GameArea.Invalidate();
            }
        }
    }
}
