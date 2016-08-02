using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace NeonShooter
{
    public class Player : Entity
    {
        private bool KeysWerePressedX;
        private bool KeysWerePressedY;
        private int ShootCooldown;
        public int RespawnTimer;
        private int DashCooldownTimer;
        public int DashTimer;
        private Vector2 PulledVel;
        public float HealthPoints = 100.0f;
        public float Strength;
        public float Angle;
        public int BombThrowCooldown = 0;
        public int BeamChargeTimer = -1;
        public int BeamTimer = -180;
        public float BeamAngle;
        private float BeamSpreadAngle;

        public Player()
        {
            this.Pos = Data.ScreenSize / 2;
            this.Vel = Vector2.Zero;

            this.Radius = 20;
            this.Rotation = 0;

            this.Tex = ImportedFiles.Player;
            this.Color = Color.FromNonPremultiplied(50, 125, 200, 255);

            this.PulledVel = Vector2.Zero;
        }

        public void GetPulledBy(Vector2 Puller, bool Direction)
        {
            float PullLength;
            float ForceMagnitude;
            float ForceAngle;
            Vector2 PullVektor;
            PullLength = Vector2.Distance(Puller, Pos);
            PullVektor = Puller - Pos;
            ForceMagnitude = 3000 / (PullLength * PullLength) * 2;
            ForceAngle = (float)Math.Atan2(PullVektor.X, PullVektor.Y);

            if (ForceMagnitude > 0.7f)
            {
                ForceMagnitude = 0.7f;
            }

            if (Direction)
            {
                PulledVel.X += ForceMagnitude * (float)Math.Sin(ForceAngle);
                PulledVel.Y += ForceMagnitude * (float)Math.Cos(ForceAngle);
            }
            else
            {
                PulledVel.X -= ForceMagnitude * (float)Math.Sin(ForceAngle);
                PulledVel.Y -= ForceMagnitude * (float)Math.Cos(ForceAngle);
            }
        }

        public void GameOver()
        {
            // GameOver

            if (GameManager.Score > Save.Default.Highscore1)
            {
                Save.Default.Highscore3 = Save.Default.Highscore2;
                Save.Default.Highscore2 = Save.Default.Highscore1;
                Save.Default.Highscore1 = (int)GameManager.Score;
            }
            else
            {
                if (GameManager.Score > Save.Default.Highscore2)
                {
                    Save.Default.Highscore3 = Save.Default.Highscore2;
                    Save.Default.Highscore2 = (int)GameManager.Score;
                }
                else
                {
                    if (GameManager.Score > Save.Default.Highscore3)
                    {
                        Save.Default.Highscore3 = (int)GameManager.Score;
                    }
                }
            }
            Save.Default.Save();
            MenuManager.Highscore.ButtonList[0].Text = "1. Highscore: " + Save.Default.Highscore1.ToString();
            MenuManager.Highscore.ButtonList[1].Text = "2. Highscore: " + Save.Default.Highscore2.ToString();
            MenuManager.Highscore.ButtonList[2].Text = "3. Highscore: " + Save.Default.Highscore3.ToString();

            if (Save.Default.Particles)
            {
                for (int ip = 0; ip < 3000; ip++)
                {
                    GameManager.Angle = Data.RDM.Next(3610) / 10;
                    GameManager.Strength = Data.RDM.Next(0, 3000) / 100;
                    ParticleManager.ParticleList.Add(new Particle(GameManager.ThisPlayer.Pos, new Vector2((float)Math.Sin(GameManager.Angle) * GameManager.Strength, (float)Math.Cos(GameManager.Angle) * GameManager.Strength) + GameManager.ThisPlayer.Vel, GameManager.ThisPlayer.Color));
                }
            }

            GameManager.Reset();

            MenuManager.ThisGameState = GameState.Highscore;
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Play(ImportedFiles.TheGrid);
        }
        public void LostLife()
        {
            if (Save.Default.Sound)
            {
                ImportedFiles.PlayerExplosion.Play(0.3f, 0, 0);
            }
            RespawnTimer = Data.PlayerRespawnTime;
            Tex = null;
            Stats.Reset();
            MediaPlayer.Stop();
            GameManager.EnemyList.Clear();
            GameManager.BulletList.Clear();
            GameManager.Lives--;

            if (Save.Default.Particles)
            {
                for (int ip = 0; ip < 700; ip++)
                {
                    Angle = Data.RDM.Next(3610);
                    Strength = (float)Data.RDM.Next(0, 3000) / 100f;
                    ParticleManager.ParticleList.Add(new Particle(Pos, new Vector2((float)Math.Sin(Angle) * Strength, (float)Math.Cos(Angle) * Strength) + Vel, Color));
                }
            }

            if (GameManager.Lives < 1)
            {
                GameOver();
            }
        }

        public void MovementControls()
        {
            KeysWerePressedX = false;
            KeysWerePressedY = false;

            if (Controls.CurKS.IsKeyDown(Keys.W))
            {
                Vel.Y -= Stats.PlayerAcceleration;
                KeysWerePressedY = true;
            }

            if (Controls.CurKS.IsKeyDown(Keys.A))
            {
                Vel.X -= Stats.PlayerAcceleration;
                KeysWerePressedX = true;
            }

            if (Controls.CurKS.IsKeyDown(Keys.S))
            {
                Vel.Y += Stats.PlayerAcceleration;
                KeysWerePressedY = true;
            }

            if (Controls.CurKS.IsKeyDown(Keys.D))
            {
                Vel.X += Stats.PlayerAcceleration;
                KeysWerePressedX = true;
            }

            if (!KeysWerePressedX)
            {
                Vel.X /= 1.2f;
            }

            if (!KeysWerePressedY)
            {
                Vel.Y /= 1.2f;
            }

            if (Vel.X > Stats.PlayerMaxSpeed)
            {
                Vel.X = Stats.PlayerMaxSpeed;
            }

            if (Vel.Y > Stats.PlayerMaxSpeed)
            {
                Vel.Y = Stats.PlayerMaxSpeed;
            }

            if (Vel.X < -Stats.PlayerMaxSpeed)
            {
                Vel.X = -Stats.PlayerMaxSpeed;
            }

            if (Vel.Y < -Stats.PlayerMaxSpeed)
            {
                Vel.Y = -Stats.PlayerMaxSpeed;
            }
        }
        public void UpdateDash()
        {
            if (Controls.CurKS.IsKeyDown(Keys.Space) && DashCooldownTimer < 1 && Vel.X > 8 ||
                    Controls.CurKS.IsKeyDown(Keys.Space) && DashCooldownTimer < 1 && Vel.Y > 8 ||
                    Controls.CurKS.IsKeyDown(Keys.Space) && DashCooldownTimer < 1 && Vel.X < -8 ||
                    Controls.CurKS.IsKeyDown(Keys.Space) && DashCooldownTimer < 1 && Vel.X < -8)
            {
                DashTimer = 10;
                DashCooldownTimer = 30;

                if (Save.Default.Sound)
                {
                    ImportedFiles.Warp.Play(0.2f, 0, 0);
                }
            }
            DashCooldownTimer--;
            DashTimer--;

            if (DashTimer > 0)
            {
                Pos += Vel * 4;
                GameManager.BackgroundGrid.ApplyForce(Pos, -3f);

                if (DashTimer == 10 || DashTimer == 8 || DashTimer == 6 || DashTimer == 4 || DashTimer == 2)
                {
                    ParticleManager.ParticleList.Add(new Particle(this.Pos, 240 + DashTimer, this.Rotation, this.Tex, this.Color * 0.5f, this.Vel, 0, Disappearing.Instant, false, false, new Vector2(1.04f, 1.04f)));
                }
            }
        }
        public void UpdateBeam()
        {
            if (Controls.CurMS.RightButton == ButtonState.Pressed && Controls.LastMS.RightButton == ButtonState.Released && BeamChargeTimer < 0 && BeamTimer < -180)
            {
                BeamChargeTimer = 115;
                if (Save.Default.Sound)
                    ImportedFiles.Beam.Play();
            }

            if (BeamChargeTimer > 0)
            {
                Pos -= Vel * (BeamChargeTimer / 115);
                DashCooldownTimer = 30;
                for (int i = 0; i < 3; i++)
                {
                    float Angle = Data.RDM.Next(360);
                    int SpawnDistance = Data.RDM.Next(50, 150);
                    ParticleManager.LaserBeamParticles.Add(new Particle(new Vector2(Pos.X + (float)Math.Sin(Angle) * SpawnDistance, Pos.Y + (float)Math.Cos(Angle) * SpawnDistance), 
                        Vector2.Zero, Color));
                }
                GameManager.BackgroundGrid.ApplyForce(Pos, -1);
                lock (ParticleManager.LaserBeamParticles)
                {
                    for (int i = 0; i < ParticleManager.LaserBeamParticles.Count; i++)
                    {
                        if (ParticleManager.LaserBeamParticles[i].LifeTime < 40)
                            ParticleManager.LaserBeamParticles[i].OrbitAround(Pos, 5);
                    }
                }

                for (int i = 0; i < GameManager.EnemyList.Count; i++)
                {
                    GameManager.EnemyList[i].GetPulledBy(Pos, false);
                }
            }

            if (BeamChargeTimer == 0)
            {
                BeamTimer = 10;
                BeamAngle = (float)Math.Atan2(Controls.CurMS.X - Pos.X, Controls.CurMS.Y - Pos.Y);
            }

            if (BeamTimer > 0)
            {
                Pos -= Vel;
                for (int i = 0; i < 10; i++)
                {
                    BeamSpreadAngle = Data.RDM.Next(-3, 3) / 360f * (float)Math.PI;
                    ParticleManager.LaserBeamParticles.Add(new Particle(Pos + new Vector2(Data.RDM.Next(-20, 20), Data.RDM.Next(-20, 20)), 50, 0, ImportedFiles.Particle, Color, 
                        new Vector2((float)Math.Sin(BeamAngle + BeamSpreadAngle) * 50, (float)Math.Cos(BeamAngle + BeamSpreadAngle) * 50), 0, Disappearing.Instant, true, false, Vector2.One));
                }

                for (int i = 0; i < GameManager.EnemyList.Count; i++)
                {
                    GameManager.EnemyList[i].GetPulledBy(Pos, false);
                }
                Vel += -new Vector2((float)Math.Sin(BeamAngle) * 25, (float)Math.Cos(BeamAngle) * 25);
            }

            BeamTimer--;
            BeamChargeTimer--;
        }
        public void UpdateOtherWeapons()
        {
            if (Controls.CurMS.LeftButton == ButtonState.Pressed && ShootCooldown < 1 && BeamTimer < 0 && BeamChargeTimer < 0)
            {
                GameManager.BulletList.Add(new Bullet(new Vector2(this.Pos.X + (float)Math.Sin(Rotation + 89.5f) * 15, 
                    this.Pos.Y + (float)Math.Cos(Rotation + 89.5f) * 15), (float)Math.Atan2(Controls.CurMS.X - Pos.X, Controls.CurMS.Y - Pos.Y), true));
                GameManager.BulletList.Add(new Bullet(new Vector2(this.Pos.X + (float)Math.Sin(Rotation + 89.5f) * -15, 
                    this.Pos.Y + (float)Math.Cos(Rotation + 89.5f) * -15), (float)Math.Atan2(Controls.CurMS.X - Pos.X, Controls.CurMS.Y - Pos.Y), true));

                if (Save.Default.Sound)
                {
                    ImportedFiles.Shoot.Play(0.03f, 0, 0);
                }

                ShootCooldown = Stats.PlayerShootCooldown;
            }

            //if (Controls.CurKS.IsKeyDown(Keys.E) && Controls.LastKS.IsKeyUp(Keys.E) && BombThrowCooldown < 1)
            //{
            //    GameManager.BombList.Add(new Bomb(30));

            //    BombThrowCooldown = 180;
            //}
            //BombThrowCooldown--;
            ShootCooldown--;
        }

        public new void Update()
        {
            GameManager.HPBar.Percentage = HealthPoints / 100;

            if (RespawnTimer < 1)
            {
                if (DashTimer < 1)
                {
                    MovementControls();

                    PulledVel /= 1.3f;
                }

                UpdateDash();
                UpdateBeam();
                UpdateOtherWeapons();

                Rotation = (float)Math.Atan2(Controls.CurMS.X - Pos.X, Controls.CurMS.Y - Pos.Y);

                PreventFromMovingOutOfBounds();

                Pos += Vel;
                Pos += PulledVel;
                if (BeamChargeTimer > 0)
                {
                    lock (ParticleManager.LaserBeamParticles)
                    {
                        for (int i = 0; i < ParticleManager.LaserBeamParticles.Count; i++)
                        {
                            ParticleManager.LaserBeamParticles[i].Pos += Vel * 0.1f;
                        }
                    }
                }
            }
            else
            {
                RespawnTimer--;
            }

            if (RespawnTimer == 1)
            {
                Pos = Data.ScreenSize / 2;
                Vel = Vector2.Zero;
                HealthPoints = 100f;

                Tex = ImportedFiles.Player;
                MediaPlayer.Volume = 0.25f;
                MediaPlayer.Play(ImportedFiles.Derezzed);
            }
        }
    }
}
