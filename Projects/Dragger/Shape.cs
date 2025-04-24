using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dragging
{
    public class Shape
    {
        public string Type { get; set; }
        public Rectangle Rectangle { get; set; }
        public Color FillColor { get; set; }
        public Color BorderColor { get; set; }
        public bool IsDragging { get; set; }
        public Point LastCursorPoint { get; set; }

        public Shape(string type, Rectangle rectangle, Color fillColor, Color borderColor, bool isDragging, Point lastCursorPoint)
        {
            Type = type;
            Rectangle = rectangle;
            FillColor = fillColor;
            BorderColor = borderColor;
            IsDragging = isDragging;
            LastCursorPoint = lastCursorPoint;
        }
    }
}
