using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace NeonShooter
{
    public class Seeker : Enemy
    {
        private bool Charging;
        private int DashCooldownTimer;
        private int DashTimer;

        public Seeker()
        {
            this.Pos.X = 500 + Data.RDM.Next((int)Data.ScreenSize.X - 1000);
            this.Pos.Y = 0;
            
            Rotation = 0;
            Spawning = 20;
            Hitpoints = Stats.EnemyHitpoints;
            InvincibilityCooldown = 0;
            Radius = 20;
            DashTimer = 5;
            Charging = true;
            Vel = Vector2.Zero;

            this.Tex = ImportedFiles.Seeker;
            this.Color = Color.Orange;
            Resistance = Stats.SeekerResistance;
        }

        public void Dash()
        {
            if ((this.Pos.X - GameManager.ThisPlayer.Pos.X) * (this.Pos.X - GameManager.ThisPlayer.Pos.X) + (this.Pos.Y - GameManager.ThisPlayer.Pos.Y) * (this.Pos.Y - GameManager.ThisPlayer.Pos.Y) < 62500 && DashCooldownTimer < 1)
            {
                // Start the Charge
                DashChargeTimer = Stats.SeekerDashChargeTime;
                DashCooldownTimer = Stats.SeekerDashTime;
                Charging = true;
            }

            if (DashChargeTimer == 0 && Charging)
            {
                // Start the DashTimer
                if (DashTimer < 1) { DashTimer = 20; }
                if (Save.Default.Sound)
                    ImportedFiles.Warp.Play(0.09f, -0.1f, 0);
                Charging = false;
            }

            if (DashTimer > 0)
            {
                // Dash Anim
                if (DashTimer == 10 || DashTimer == 8 || DashTimer == 6 || DashTimer == 4 || DashTimer == 2)
                {
                    ParticleManager.ParticleList.Add(new Particle(this.Pos, 240 + DashTimer, this.Rotation, this.Tex, this.Color * 0.5f, this.Vel / 10, 0, Disappearing.Instant, false, false, new Vector2(1.04f, 1.04f)));
                }

                GameManager.BackgroundGrid.ApplyForce(Pos, -3f);

                // Dash
                float Speed = Stats.EnemySpeed + (0.05f * (float)Math.Sqrt(GameManager.Wave));
                Vel = new Vector2((float)Math.Sin(Rotation) * Speed, (float)Math.Cos(Rotation) * Speed) * 5;
            }

            if (Charging)
                Vel = Vector2.Zero;

            DashCooldownTimer--;
            DashTimer--;
            DashChargeTimer--;
        }

        public override void Update()
        {
            Dash();

            if (DashTimer < 0)
                Rotation = (float)Math.Atan2(GameManager.ThisPlayer.Pos.X - Pos.X, GameManager.ThisPlayer.Pos.Y - Pos.Y);

            float Speed = Stats.EnemySpeed + (0.04f * (float)Math.Sqrt(GameManager.Wave)) * 1.2f;
            //if (Speed > 7.5f)
            //    Speed = 7.5f;

            if (!Charging && DashTimer < 0)
                Vel = new Vector2((float)Math.Sin(Rotation) * Speed, (float)Math.Cos(Rotation) * Speed);

            Vel /= 1.03f;
            Pos += Vel;

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (DashChargeTimer < 1)
            {
                spriteBatch.Draw(Tex, Pos, null, Color * (1 - (float)Spawning / 60), -Rotation + 89.5f, new Vector2(Tex.Width / 2, Tex.Height / 2), 1, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(Tex, Pos, null, Color.FromNonPremultiplied(255, (int)(165f * DashChargeTimer / 30f),
                    (int)(255f * (float)DashChargeTimer / 30f), 255) * (1 - (float)Spawning / 60), -Rotation + 89.5f, new Vector2(Tex.Width / 2, Tex.Height / 2),
                    1, SpriteEffects.None, 0);
            }

            EnemyBar.Draw(spriteBatch);
        }
    }
}
