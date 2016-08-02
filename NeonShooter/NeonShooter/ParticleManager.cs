using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading.Tasks;

namespace NeonShooter
{
    public static class ParticleManager
    {
        public static List<Particle> ParticleList = new List<Particle>();
        public static List<Particle> ParticleList2 = new List<Particle>();
        public static List<Particle> LaserBeamParticles = new List<Particle>();
    
        public static void Clear()
        {
            ParticleList = new List<Particle>();
            ParticleList2 = new List<Particle>();
            LaserBeamParticles = new List<Particle>();
        }

        public static void Update()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Task.Factory.StartNew(() => Update1());
            Task.Factory.StartNew(() => Update2());
            Task.Factory.StartNew(() => UpdateBeam());
        }

        public static void Update1()
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            lock (ParticleList)
            {
                for (int i = 0; i < ParticleList.Count; i++)
                {
                    try
                    {
                        if (i < ParticleList.Count && ParticleList[i] != null)
                        {
                            ParticleList[i].Update();

                            if (ParticleList[i].LifeTime > 255 || ParticleList[i].WayOfDisappearing == Disappearing.Slowly && ParticleList[i].LifeTime > 250)
                            {
                                ParticleList.Remove(ParticleList[i]);
                            }
                        }
                    }
                    catch { }
                }
            }
        }
        public static void Update2()
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            lock (ParticleList2)
            {
                for (int i = 0; i < ParticleList2.Count; i++)
                {
                    if (i < ParticleList2.Count && ParticleList2[i] != null)
                    {
                        try
                        {
                            ParticleList2[i].Update();

                            if (ParticleList2[i].LifeTime > 255 || ParticleList2[i].WayOfDisappearing == Disappearing.Slowly && ParticleList2[i].LifeTime > 250)
                            {
                                ParticleList2.Remove(ParticleList2[i]);
                            }
                        }
                        catch { }
                    }
                }
            }
        }
        public static void UpdateBeam()
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            lock (LaserBeamParticles)
            {
                for (int i = 0; i < LaserBeamParticles.Count; i++)
                {
                    if (i < LaserBeamParticles.Count && LaserBeamParticles[i] != null)
                    {
                        try
                        {
                            LaserBeamParticles[i].Update();

                            lock (GameManager.EnemyList)
                            {
                                for (int i2 = 0; i2 < GameManager.EnemyList.Count; i2++)
                                {
                                    if (LaserBeamParticles[i].IsColidingWith(GameManager.EnemyList[i2]) && LaserBeamParticles[i].Vel.LengthSquared() > 45 * 45 && 
                                        LaserBeamParticles[i].LifeTime < 100)
                                    {
                                        //CreateParticleExplosionFromEntityTexture(GameManager.EnemyList[i2], LaserBeamParticles[i].Vel, 0.8f);
                                        //if (Save.Default.Sound)
                                        //    ImportedFiles.Explosion.Play(0.03f, 0.3f, 0);
                                        GameManager.EnemyList[i2].OnDeath();
                                        GameManager.EnemyList.RemoveAt(i2);
                                        LaserBeamParticles[i].LifeTime = 101;
                                    }
                                }
                            }

                            if (LaserBeamParticles[i].LifeTime > 100)
                            {
                                LaserBeamParticles.RemoveAt(i);
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        public static void CreateParticleExplosionFromEntityTexture(Entity E, float DriftSpeed)
        {
            Color[] Col = new Color[E.Tex.Width * E.Tex.Height];
            E.Tex.GetData(Col);

            Color[,] Col2D = new Color[E.Tex.Width, E.Tex.Height];
            for (int i = 0; i < Col.Length; i++)
            {
                Col2D[i % E.Tex.Width, i / E.Tex.Width] = Col[i];
            }
            
            for (int ix = 0; ix < Col2D.GetLength(0); ix++)
            {
                for (int iy = 0; iy < Col2D.GetLength(1); iy++)
                {
                    // Only create a Particle if it's a visible Texel
                    if (Col2D[ix, iy].A != 0 || Col2D[ix, iy].ToVector3() != Vector3.Zero)
                    {
                        float s = (float)Math.Sin(-E.Rotation + 89.5f);
                        float c = (float)Math.Cos(-E.Rotation + 89.5f);

                        // Moving the Origin to the Middle of the Texture
                        float x = ix - E.Tex.Width / 2;
                        float y = iy - E.Tex.Height / 2;

                        // Rotating the Point around the Origin
                        Vector2 RotatedPoint = new Vector2( (x * c) - (y * s),
                                                            (x * s) + (y * c));

                        // Construktor                Adding the Entity Position to the Rotated Point
                        ParticleList.Add(new Particle(RotatedPoint + E.Pos,
                            // Lifetime         /
                            Data.RDM.Next(100, 256), 0,
                            // Texture               Color
                            ImportedFiles.PureWhite, Color.Lerp(Col2D[ix, iy], E.Color, 0.5f),
                            // Velocity
                            new Vector2((RotatedPoint.X - E.Tex.Width / 2 + Data.RDM.Next(7)) / 10f * DriftSpeed,
                                        (RotatedPoint.Y - E.Tex.Height / 2 + Data.RDM.Next(7)) / 10f * DriftSpeed) + E.Vel, 0.2f, Disappearing.Instant, true, true, new Vector2(1.04f, 1.04f)));
                    }
                }
            }
        }
        public static void CreateParticleExplosionFromEntityTexture(Entity E, Vector2 StartVel, float DriftSpeed)
        {
            Color[] Col = new Color[E.Tex.Width * E.Tex.Height];
            E.Tex.GetData(Col);

            Color[,] Col2D = new Color[E.Tex.Width, E.Tex.Height];
            for (int i = 0; i < Col.Length; i++)
            {
                Col2D[i % E.Tex.Width, i / E.Tex.Width] = Col[i];
            }

            for (int ix = 0; ix < Col2D.GetLength(0); ix++)
            {
                for (int iy = 0; iy < Col2D.GetLength(1); iy++)
                {
                    // Only create a Particle if it's a visible Texel
                    if (Col2D[ix, iy].A != 0 || Col2D[ix, iy].ToVector3() != Vector3.Zero)
                    {
                        float s = (float)Math.Sin(-E.Rotation + 89.5f);
                        float c = (float)Math.Cos(-E.Rotation + 89.5f);

                        // Moving the Origin to the Middle of the Texture
                        float x = ix - E.Tex.Width / 2;
                        float y = iy - E.Tex.Height / 2;

                        // Rotating the Point around the Origin
                        Vector2 RotatedPoint = new Vector2((x * c) - (y * s),
                                                            (x * s) + (y * c));

                        // Construktor                Adding the Entity Position to the Rotated Point
                        ParticleList.Add(new Particle(RotatedPoint + E.Pos,
                            // Lifetime         /
                            Data.RDM.Next(100, 256), 0,
                            // Texture               Color
                            ImportedFiles.PureWhite, Color.Lerp(Col2D[ix, iy], E.Color, 0.5f),
                            // Velocity
                            new Vector2((RotatedPoint.X - E.Tex.Width / 2 + Data.RDM.Next(7)) / 10f * DriftSpeed,
                                        (RotatedPoint.Y - E.Tex.Height / 2 + Data.RDM.Next(7)) / 10f * DriftSpeed) + E.Vel + StartVel, 0.2f, Disappearing.Instant, true, true, new Vector2(1.04f, 1.04f)));
                    }
                }
            }
        }
        public static void CreateExplosion(Vector2 Pos, Vector2 Vel, Color Col, int AmountOfParticles, int MaxStrength)
        {
            for (int ip = 0; ip < AmountOfParticles; ip++)
            {
                float Angle = Data.RDM.Next(361);
                float Strength = Data.RDM.Next(0, MaxStrength + (100));
                Strength /= 100;
                ParticleList.Add(new Particle(Pos, new Vector2((float)Math.Sin(Angle) * Strength, (float)Math.Cos(Angle) * Strength) + Vel, Col));
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            lock (ParticleList)
            {
                for (int i = 0; i < ParticleList.Count; i++)
                {
                    if (i < ParticleList.Count && ParticleList[i] != null)
                        ParticleList[i].Draw(spriteBatch);
                }
            }

            lock (ParticleList2)
            {
                for (int i = 0; i < ParticleList2.Count; i++)
                {
                    if (i < ParticleList2.Count && ParticleList2[i] != null)
                        ParticleList2[i].Draw(spriteBatch);
                }
            }

            lock (LaserBeamParticles)
            {
                for (int i = 0; i < LaserBeamParticles.Count; i++)
                {
                    if (i < LaserBeamParticles.Count && LaserBeamParticles[i] != null)
                        LaserBeamParticles[i].Draw(spriteBatch);
                }
            }
        }
    }
}
