using System;
using System.Drawing;

namespace Moving_Square
{
    public class Effect
    {
        public string EffectName { get; set; }
        public Rectangle EffectHitBox { get; set; }

        public System.Windows.Forms.Timer EffectDuration { get; set; }
        public System.Windows.Forms.Timer EffectDespawnTimer { get; set; }

        public int? SpawnChance { get; set; }
        public int? Ticks { get; set; }

        public Image EffectImage { get; set; }

        public Color? NewSquareBorderColor { get; set; }
        public Color? NewSquareFillColor { get; set; }
        public int? NewSquareSpeed { get; set; }

        public DateTime? EffectStartTime { get; set; }
        public DateTime? EffectSpawnTime { get; set; }

        public bool? EffectIsActive { get; set; }
        public bool? CanSpawn { get; set; }
        public bool? CanBeIncremented { get; set; }
        public bool? CanDrawEffect { get; set; }

        public Effect(string effectName, Rectangle effectHitBox,
            System.Windows.Forms.Timer effectDuration,
            System.Windows.Forms.Timer effectDespawnTimer = null,
            int? spawnChance = null,
            bool? canDrawEffect = null, Image effectImage = null,
            Color? newSquareBorderColor = null, Color? newSquareFillColor = null,
            int? newSquareSpeed = null,
            bool? effectIsActive = null, bool? canSpawn = null, bool? canBeIncremented = null,
            DateTime? effectStartTime = null, DateTime? effectSpawnTime = null
            )
        {
            EffectName = effectName;
            EffectHitBox = effectHitBox; // Effect's spawnpoint which is later declared random

            EffectDuration = effectDuration; // Effect length
            EffectDespawnTimer = effectDespawnTimer; // Effect despawn timer

            SpawnChance = spawnChance ?? 625; // Default is 1 spawn per 625 ticks (~1/10s)
            Ticks = 0; // Stores ticks to access spawn timer

            EffectImage = effectImage; // Visuals for the effect

            NewSquareBorderColor = newSquareBorderColor ?? Color.RoyalBlue; // Square's colors while effect is in effect
            NewSquareFillColor = newSquareFillColor ?? Color.CornflowerBlue;
            NewSquareSpeed = newSquareSpeed ?? 5; // Square's speed while affected

            EffectIsActive = effectIsActive ?? false;
            CanSpawn = canSpawn ?? true;
            CanBeIncremented = canBeIncremented ?? true; // Used to randomize spawn chance
            CanDrawEffect = canDrawEffect ?? false;

            EffectStartTime = effectStartTime ?? DateTime.MinValue; // Exact time the buff is obtained
            EffectSpawnTime = effectSpawnTime ?? DateTime.MinValue; // Tracks the moment of the effect's first appearance
        }
    }
}
