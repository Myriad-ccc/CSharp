using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moving_Square
{
    public class Statistics
    {
        public Point PlayerPosition { get; set; }
        public int PlayerSpeed { get; set; }
        public string RecentBuffLength { get; set; }
        public string RecentBuffDespawn { get; set; }
        public int TotalBuffsCollected { get; set; }
        public int CurrentOutOfBoundMoves { get; set; }

        public Statistics(Point playerPosition, int playerSpeed,
            string recentBuffLength, string recentBuffDespawn, 
            int totalBuffsCollected, int currentOutOfBoundMoves)
        {
            PlayerPosition = playerPosition;
            PlayerSpeed = playerSpeed;
            RecentBuffLength = recentBuffLength;
            RecentBuffDespawn = recentBuffDespawn;
            TotalBuffsCollected = totalBuffsCollected;
            CurrentOutOfBoundMoves = currentOutOfBoundMoves;
        }
    }
}
