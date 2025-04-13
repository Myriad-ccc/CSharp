using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
        public Color Color { get; set; }

        public PlayerSquare(Rectangle rectangle, int speed, Color color)
        {
            Rectangle = rectangle;
            Speed = speed;
            Color = color;
        }
    }
}
