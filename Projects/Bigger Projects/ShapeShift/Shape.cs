using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeShift
{
    public class Shape
    {
        public string Type { get; set; }
        public Rectangle Rectangle { get; set; }
        public PointF[] Points { get; set; }
        public Color FillColor { get; set; }
        public Color BorderColor { get; set; }
        public float BorderThickness { get; set; }
        public bool ToFill { get; set; }
        public Size Size { get; set; }
        public int Sides { get; set; }

        public bool IsOnScreen { get; set; }
        public bool IsDragging { get; set; }
        public Point LastCursorPoint { get; set; }

        public int X => Rectangle.Location.X;
        public int Y => Rectangle.Location.Y;
        public int W => Rectangle.Size.Width;
        public int H => Rectangle.Size.Height;

        public Shape(string type, Rectangle rectangle, PointF[] points,
            Color fillColor, Color borderColor, float borderThickness,
            bool toFill, Size size, int sides)
        {
            Type = type;
            Rectangle = rectangle;
            Points = points;
            FillColor = fillColor;
            BorderColor = borderColor;
            BorderThickness = borderThickness; 
            ToFill = toFill;
            Size = size;
            Sides = sides;
        }
    }
}
