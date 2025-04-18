using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Moving_Square
{
    public class Effect
    {
        public string EffectName { get; set; }
        public bool EffectIsActive { get; set; }
        public bool CanSpawn { get; set; }
        public bool CanBeIncremented { get; set; }

        public Rectangle EffectHitBox { get; set; }

        public System.Windows.Forms.Timer EffectDuration { get; set; }
        public DateTime EffectStartTime { get; set; }

        public System.Windows.Forms.Timer EffectDespawnTimer { get; set; }
        public DateTime? EffectSpawnTime { get; set; }

        public int? SpawnChance { get; set; }
        public int? Ticks { get; set; }

        public bool? CanDrawPowerUp { get; set; }
        public Image EffectImage { get; set; }

        public Color? NewSquareBorderColor { get; set; }
        public Color? NewSquareFillColor { get; set; }

        public int? NewSquareSpeed { get; set; }

        public Effect(string effectName,
            bool effectIsActive, bool canSpawn, bool canBeIncremented,
            Rectangle effectHitBox, 
            System.Windows.Forms.Timer effectDuration, DateTime effectStartTime, 
            System.Windows.Forms.Timer effectDespawnTimer = null, DateTime? effectSpawnTime = null,
            int? spawnChance = null,
            bool? canDrawPowerUp = null, Image effectImage = null ,
            Color? newSquareBorderColor = null, Color? newSquareFillColor = null, 
            int? newSquareSpeed = null)
        {
            EffectName = effectName;

            EffectIsActive = effectIsActive;
            CanSpawn = canSpawn;
            CanBeIncremented = canBeIncremented; // Used to randomize spawn chance

            EffectHitBox = effectHitBox; // Effect has random spawnpoint

            EffectDuration = effectDuration; // Effect length
            EffectStartTime = effectStartTime; // Exact time the buff is obtained

            EffectDespawnTimer = effectDespawnTimer; // Effect despawn timer
            EffectSpawnTime = effectSpawnTime; // Tracks the moment of the effect's first appearance

            SpawnChance = spawnChance ?? 625; // Default is 1 spawn per 625 ticks (~1/10s)
            Ticks = 0; // Stores ticks to access spawn timer

            CanDrawPowerUp = canDrawPowerUp ?? false;
            EffectImage = effectImage; // Visuals for the effect

            NewSquareBorderColor = newSquareBorderColor; // Square's colors while effect is in effect
            NewSquareFillColor = newSquareFillColor;

            NewSquareSpeed = newSquareSpeed; // Square's speed while affected
        }
    }
}
