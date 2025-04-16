using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Moving_Square
{
    public class Effect
    {
        public bool EffectIsActive { get; set; }
        public bool CanSpawn { get; set; }

        public Rectangle EffectHitBox { get; set; }

        public System.Windows.Forms.Timer EffectDuration { get; set; }
        public System.Windows.Forms.Timer EffectDespawnTimer { get; set; }

        public int? SpawnChance { get; set; }

        public PictureBox EffectPictureBox { get; set; }
        public Image EffectPictureBoxImage { get; set; }

        public Color? NewSquareBorderColor { get; set; }
        public Color? NewSquareFillColor { get; set; }

        public int? NewSquareSpeed { get; set; }

        public Effect(bool effectIsActive, bool canSpawn,
            Rectangle effectHitBox, 
            System.Windows.Forms.Timer effectDuration, System.Windows.Forms.Timer effectDespawnTimer = null,
            int? spawnChance = null,
            PictureBox effectPictureBox = null, Image effectPictureBoxImage = null ,
            Color? newSquareBorderColor = null, Color? newSquareFillColor = null, 
            int? newSquareSpeed = null)
        {
            EffectIsActive = effectIsActive;
            CanSpawn = canSpawn;

            EffectHitBox = effectHitBox; // Power up has random spawnpoint

            EffectDuration = effectDuration; // Effect length
            EffectDespawnTimer = effectDespawnTimer; // Power up despawn timer

            SpawnChance = spawnChance ?? 625; // default is 1 spawn per 625 ticks (~1/10s)

            EffectPictureBox = effectPictureBox;
            EffectPictureBoxImage = effectPictureBoxImage; // Optional visuals for the power up

            NewSquareBorderColor = newSquareBorderColor; // Square's colors while power up is in effect
            NewSquareFillColor = newSquareFillColor;

            NewSquareSpeed = newSquareSpeed; // Square's speed while buffed
        }
    }
}
