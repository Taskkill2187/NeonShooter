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
    public class Enemy : Entity
    {
        private int FirstRandom;
        private int SecondRandom;

        public int Spawning;
        internal float Hitpoints;
        public float Resistance;
        public int InvincibilityCooldown;

        internal float PullLength;
        internal float ForceMagnitude;
        internal float ForceAngle;
        internal Vector2 PullVektor;

        internal int DashChargeTimer;
        private int AttractCooldown;
        internal int ChangeMovementTimer;
        internal int ShootCooldown;
        public int ScoreSteal;

        internal float ShootingDirection;

        public HP_Bar EnemyBar;

        public event EventHandler OnHPLoss;

        public void Attract()
        {
            if (AttractCooldown < 1)
            {
                lock (ParticleManager.ParticleList)
                {
                    for (int i = 0; i < ParticleManager.ParticleList.Count; i++)
                    {
                        if (i < ParticleManager.ParticleList.Count && ParticleManager.ParticleList[i] != null && ParticleManager.ParticleList[i].AffectedByBlackHoles)
                        {
                            ParticleManager.ParticleList[i].OrbitAround(Pos, 4);
                            ParticleManager.ParticleList[i].LifeTime -= 2;
                            //ParticleManager.ParticleList[i].Vel.Y -= ParticleManager.ParticleList[i].GravForce * 3f;
                        }
                    }
                }

                for (int i = 0; i < GameManager.BulletList.Count; i++)
                {
                    if (GameManager.BulletList[i].FromPlayer)
                        GameManager.BulletList[i].GetPulledBy(Pos, false);
                }
                
                AttractCooldown = 3;
            }
            AttractCooldown--;
        }
        public void StealScore()
        {
            if (ScoreSteal < 1)
            {
                GameManager.Score--;
                GameManager.ScoreSignList.Add(new ScoreSign(Pos, "-1 Point!", Color.Red));
                ScoreSteal = 60;
            }
            ScoreSteal--;
        }
        public void ShootAtPlayer()
        {
            GameManager.BulletList.Add(new Bullet(this.Pos, (float)Math.Atan2(GameManager.ThisPlayer.Pos.X - Pos.X, GameManager.ThisPlayer.Pos.Y - Pos.Y), false));

            if (Save.Default.Sound)
            {
                ImportedFiles.Shoot.Play(0.04f, -0.5f, 0);
            }
        }
        public void ShotGunShootAtPlayer()
        {
            GameManager.BulletList.Add(new Bullet(Pos, (float)Math.Atan2(GameManager.ThisPlayer.Pos.X - Pos.X, GameManager.ThisPlayer.Pos.Y - Pos.Y) + 0.1f, false));
            GameManager.BulletList.Add(new Bullet(Pos, (float)Math.Atan2(GameManager.ThisPlayer.Pos.X - Pos.X, GameManager.ThisPlayer.Pos.Y - Pos.Y), false));
            GameManager.BulletList.Add(new Bullet(Pos, (float)Math.Atan2(GameManager.ThisPlayer.Pos.X - Pos.X, GameManager.ThisPlayer.Pos.Y - Pos.Y) - 0.1f, false));

            if (Save.Default.Sound)
            {
                ImportedFiles.Shoot.Play(0.04f, -0.5f, 0);
            }
        }
        public void ShootEveryWhere()
        {
            GameManager.BulletList.Add(new Bullet(this.Pos, ShootingDirection, false));
            ShootingDirection += 0.4f;

            if (Save.Default.Sound)
            {
                ImportedFiles.Shoot.Play(0.04f, -0.5f, 0);
            }
        }
        public void ShootInAllDirections()
        {
            for (int i = 0; i < 16; i++)
            {
                GameManager.BulletList.Add(new Bullet(this.Pos, ShootingDirection, false));
                ShootingDirection += 0.4f;
            }

            if (Save.Default.Sound)
            {
                ImportedFiles.Shoot.Play(0.04f, -0.5f, 0);
            }
        }

        public Vector2 GetRandomPosAtTheEndOfTheScreen()
        {
            FirstRandom = Data.RDM.Next(5);

            if (FirstRandom == 1)
            {
                SecondRandom = Data.RDM.Next((int)Data.ScreenSize.X + 1);

                return new Vector2(SecondRandom, 0);
            }

            if (FirstRandom == 2)
            {
                SecondRandom = Data.RDM.Next((int)Data.ScreenSize.Y + 1);

                return new Vector2(0, SecondRandom);
            }

            if (FirstRandom == 3)
            {
                SecondRandom = Data.RDM.Next((int)Data.ScreenSize.X + 1);

                return new Vector2(SecondRandom, (int)Data.ScreenSize.Y);
            }

            if (FirstRandom == 4)
            {
                SecondRandom = Data.RDM.Next((int)Data.ScreenSize.Y + 1);

                return new Vector2(Data.ScreenSize.X, SecondRandom);
            }

            return Vector2.Zero;
        }

        public void GetPulledBy(Vector2 Puller, bool Direction)
        {
            PullLength = Vector2.Distance(Puller, Pos);
            PullVektor = Puller - Pos;
            ForceMagnitude = Data.ParticleForceMultiplier / (PullLength * PullLength) * 25;
            ForceAngle = (float)Math.Atan2(PullVektor.X, PullVektor.Y);

            //if (ForceMagnitude > 0.7f)
            //{
            //    ForceMagnitude = 0.7f;
            //}

            if (Direction)
            {
                Pos.X += ForceMagnitude * (float)Math.Sin(ForceAngle);
                Pos.Y += ForceMagnitude * (float)Math.Cos(ForceAngle);
            }
            else
            {
                Pos.X -= ForceMagnitude * (float)Math.Sin(ForceAngle);
                Pos.Y -= ForceMagnitude * (float)Math.Cos(ForceAngle);
            }


        }

        public void OnDeath()
        {
            if (GameManager.ThisPlayer.HealthPoints > 100) { GameManager.ThisPlayer.HealthPoints = 100; }
            GameManager.WaveKills++;
            Data.TriggerFullScreenShaderEffects();
            if (Save.Default.Particles)
            {
                ParticleManager.CreateParticleExplosionFromEntityTexture(this, 1);
                ParticleManager.CreateExplosion(Pos, Vel, Color, 30, 1000);
            }
            GameManager.ScoreSignList.Add(new ScoreSign(Pos, (1f * ((float)GameManager.Wave + 1) / 2f).ToString() + " Points!", Color.LightGreen));
            GameManager.Score += 1f * ((float)GameManager.Wave + 1) / 2f;
            if (Save.Default.Sound)
            {
                ImportedFiles.Explosion.Play(0.15f, 0.3f, 0);
            }
            Task.Factory.StartNew(() => GameManager.BackgroundGrid.ApplyForce(Pos, -25f));
        }
        public void GetDMG(float DMG, Entity Origin)
        {
            if (InvincibilityCooldown < 0)
            {
                Hitpoints -= DMG / Resistance;
                if (Hitpoints <= 0)
                {
                    OnDeath();
                }
                InvincibilityCooldown = 5;
                OnHPLoss.Invoke(this, EventArgs.Empty);
            }
            if (Save.Default.Sound)
                ImportedFiles.Explosion.Play(0.05f, 0.3f, 0);
        }

        public Enemy()
        {
            Radius = 20;
            EnemyBar = new HP_Bar(new Vector2(Pos.X, Pos.Y - (int)Radius - 30), (int)Radius * 2 + 25, 25, 5, 1);
        }

        public new virtual void Update()
        {
            EnemyBar.Percentage = Hitpoints / Stats.EnemyHitpoints;
            EnemyBar.Pos = new Vector2(Pos.X, Pos.Y - (int)Radius - 30);

            InvincibilityCooldown--;

            PreventFromMovingOutOfBounds();

            Spawning--;
        }

        public new virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Tex != null)
            {
                spriteBatch.Draw(Tex, Pos, null, Color * (1 - (float)Spawning / 60), Rotation, new Vector2(Tex.Width / 2, Tex.Height / 2), 1, SpriteEffects.None, 0);
            }

            EnemyBar.Draw(spriteBatch);
        }
    }
}
