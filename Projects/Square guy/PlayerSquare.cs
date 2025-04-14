using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moving_Square
{
    public class PlayerSquare
    {
        public Rectangle Rectangle { get; set; }
        public int Speed { get; set; }
        public Color FillColor { get; set; }
        public Color BorderColor { get; set; }

        public PlayerSquare(Rectangle rectangle, int speed, Color fillColor, Color borderColor)
        {
            Rectangle = rectangle;
            Speed = speed;
            FillColor = fillColor;
            BorderColor = borderColor;
        }
    }
}
