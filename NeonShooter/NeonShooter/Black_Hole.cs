using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BloomPostprocess;
using System.Threading.Tasks;
using System.Reflection;

namespace NeonShooter
{
    public class Black_Hole : Enemy
    {
        public Black_Hole(Vector2 Pos) : base()
        {
            this.Color = Color.Red;
            this.Spawning = 60;
            this.Pos = Pos;
            this.Pos.Y = 0;
            this.Radius = 30;
            this.Vel = Vector2.Zero;
            this.Rotation = 0;
            this.Hitpoints = Stats.EnemyHitpoints;
            this.Tex = ImportedFiles.BlackHole;
            this.ScoreSteal = 60;
            Resistance = Stats.BlackHoleResistance;
            OnHPLoss += OnHPLossEvent;
        }

        public void OnHPLossEvent(object sender, EventArgs e)
        {
            ParticleManager.CreateExplosion(Pos, Vel, Color, 250, 1500);
        }

        public override void Update()
        {
            Attract();
            StealScore();

            Vel.Y = Stats.BlackHoleSpeed;
            Pos += Vel;
            Task.Factory.StartNew(() => GameManager.BackgroundGrid.ApplyForce(Pos, 1.5f));

            base.Update();
        }
    }
}
