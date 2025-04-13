using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cubeRace
{
    public class Player
    {
        public Rectangle Rectangle { get; set; }
        public Color Color { get; set; }
        public float Speed{ get; set; }
        public int Number { get; set; }

        public Player(Rectangle rectangle, Color color, float speed, int number)
        {
            Rectangle = rectangle;
            Color = color;
            Speed = speed;
            Number = number;
        }
    }
}
