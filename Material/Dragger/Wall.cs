using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragging
{
    public class Wall
    {
        public Rectangle Bounds { get; set; }
        public Color? Color { get; set; }

        public Wall(Rectangle bounds, Color? color = null)
        {
            Bounds = bounds;
            Color = color ?? System.Drawing.Color.DimGray;
        }
    }
}
