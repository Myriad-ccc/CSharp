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
        public string RecentEffectLength { get; set; }
        public string RecentEffectDespawn { get; set; }
        public int TotalEffectsCollected { get; set; }
        public int CurrentOutOfBoundMoves { get; set; }

        public Statistics(Point playerPosition, int playerSpeed,
            string recentEffectLength, string recentEffectDespawn, 
            int totalEffectsCollected, int currentOutOfBoundMoves)
        {
            PlayerPosition = playerPosition;
            PlayerSpeed = playerSpeed;
            RecentEffectLength = recentEffectLength;
            RecentEffectDespawn = recentEffectDespawn;
            TotalEffectsCollected = totalEffectsCollected;
            CurrentOutOfBoundMoves = currentOutOfBoundMoves;
        }
    }
}
