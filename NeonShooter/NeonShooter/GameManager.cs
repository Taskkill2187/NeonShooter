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
    public enum BackgroundType { Stars, Grid }
    public static class GameManager
    {
        public static Player ThisPlayer = new Player();
        public static List<Bullet> BulletList = new List<Bullet>();
        public static List<Enemy> EnemyList = new List<Enemy>();
        public static List<ScoreSign> ScoreSignList = new List<ScoreSign>();
        public static List<Bomb> BombList = new List<Bomb>();
        public static HP_Bar HPBar = new HP_Bar(new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y - 50), 800, 50, 8, 1f);
        public static List<Type> EnemyTypes;
        public static DynamicGrid BackgroundGrid = new DynamicGrid(new Rectangle(-20, -20, (int)Data.ScreenSize.X, (int)Data.ScreenSize.Y), 15);
        public static BackgroundType BType = BackgroundType.Grid;

        private static int EnemySpawnTimer = 180;
        public static float Score = 0;
        public static int Lives = 3;
        public static int Wave = 0;
        public static int WaveKills;
        public static float Strength;
        public static float Angle;
        public static bool NotPaused = true;
        public static bool DebugMode = false;
        private static bool FoundBlackHole = false;
        private static bool FoundShooters = false;
        public static long DiagonalWindowSize = (long)Data.ScreenSize.X * (long)Data.ScreenSize.X + (long)Data.ScreenSize.Y * (long)Data.ScreenSize.Y;
        public static float DMG_IndikatorState = 0;
        public static bool InUpgradeMenu;
        public static bool LimitersActivated = false;

        public static void Reset()
        {
            ThisPlayer = new Player();
            BulletList = new List<Bullet>();
            EnemyList = new List<Enemy>();
            BombList = new List<Bomb>();
            ParticleManager.Clear();
            ScoreSignList = new List<ScoreSign>();

            if (EnemyTypes == null) { GetEnemyTypes(); }

            EnemySpawnTimer = 180;
            Score = 0;
            Lives = 3;
            Strength = 0;
            WaveKills = 0;
            Wave = 0;
            Angle = 0;
        }
        public static void GetEnemyTypes()
        {
            Type[] AType = Assembly.GetAssembly(typeof(Enemy)).GetTypes();
            EnemyTypes = new List<Type>();
            for (int i = 0; i < AType.Length; i++)
            {
                if (AType[i].IsSubclassOf(typeof(Enemy)))
                {
                    EnemyTypes.Add(AType[i]);
                }
            }
        }
        public static void UpdateAllEntities()
        {
            ThisPlayer.Update();

            for (int i = 0; i < BulletList.Count; i++)
            {
                BulletList[i].Update();

                if (((long)BulletList[i].Pos.X * (long)BulletList[i].Pos.X) + ((long)BulletList[i].Pos.Y * (long)BulletList[i].Pos.Y) > DiagonalWindowSize)
                {
                    BulletList.Remove(BulletList[i]);
                }
            }

            lock (EnemyList)
            {
                for (int i = 0; i < EnemyList.Count; i++)
                {
                    EnemyList[i].Update();

                    if (EnemyList[i].GetType() == typeof(Black_Hole))
                    {
                        if (EnemyList[i].Pos.Y > Data.ScreenSize.Y - 1 - EnemyList[i].Radius)
                        {
                            Score -= 50;
                            ThisPlayer.HealthPoints -= 20f;
                            ScoreSignList.Add(new ScoreSign(EnemyList[i].Pos, "-50 Points!", Color.Red));
                            ParticleManager.CreateExplosion(EnemyList[i].Pos + new Vector2(0, -10), Vector2.Zero, EnemyList[i].Color, 500, 3000);
                            if (Save.Default.Sound)
                            {
                                ImportedFiles.PlayerExplosion.Play(0.3f, 0, 0);
                            }
                            EnemyList.RemoveAt(i);
                        }
                    }
                }
            }

            for (int i = 0; i < BombList.Count; i++)
            {
                BombList[i].Update();

                if (BombList[i].ExplosionTimer < 1)
                    BombList.Remove(BombList[i]);
            }

            for (int i = 0; i < ScoreSignList.Count; i++)
            {
                ScoreSignList[i].Update();

                if (ScoreSignList[i].LifeTime > 255)
                {
                    ScoreSignList.RemoveAt(i);
                }
            }
        }
        public static void SpawnNewStars()
        {
            lock (ParticleManager.ParticleList2)
            {
                ParticleManager.ParticleList2.Add(new Particle(new Vector2(Data.RDM.Next((int)Data.ScreenSize.X), 0), new Vector2(0, 5 + Data.RDM.Next(5)), Color.Orange, new Vector2(1000, 1), false));
                ParticleManager.ParticleList2.Add(new Particle(new Vector2(Data.RDM.Next((int)Data.ScreenSize.X), 0), new Vector2(0, 5 + Data.RDM.Next(5)), Color.Orange, new Vector2(1000, 1), false));
            }
        }
        public static void EnemySpawner()
        {
            lock (EnemyList)
            {
                if (EnemySpawnTimer < 1 && EnemyList.Count < 5 && ThisPlayer.RespawnTimer < 1)
                {
                    if (Wave == 0)
                    {
                        EnemyList.Add(new Seeker());
                    }

                    if (Wave > 0 && Wave < 3)
                    {
                        if (Data.RDM.Next(7) == 6)
                        {
                            FoundBlackHole = false;
                            for (int i = 0; i < EnemyList.Count; i++)
                            {
                                if (EnemyList[i].GetType() == typeof(Black_Hole))
                                    FoundBlackHole = true;
                            }
                            if (FoundBlackHole) { EnemyList.Add(new Seeker()); }
                            else { EnemyList.Add(new Black_Hole(new Vector2(Data.RDM.Next((int)Data.ScreenSize.X), Data.RDM.Next((int)Data.ScreenSize.Y)))); }
                        }
                        else
                        {
                            EnemyList.Add(new Seeker());
                        }
                    }

                    if (Wave > 2)
                    {
                        switch (Data.RDM.Next(7))
                        {
                            case 1:
                                EnemyList.Add(new Seeker());
                                break;
                            case 2:
                                EnemyList.Add(new Seeker());
                                break;
                            case 3:
                                EnemyList.Add(new Seeker());
                                break;
                            case 4:
                                FoundBlackHole = false;
                                for (int i = 0; i < EnemyList.Count; i++)
                                {
                                    if (EnemyList[i].GetType() == typeof(Black_Hole))
                                        FoundBlackHole = true;
                                }
                                if (FoundBlackHole) { EnemyList.Add(new Seeker()); }
                                else { EnemyList.Add(new Black_Hole(new Vector2(Data.RDM.Next((int)Data.ScreenSize.X), Data.RDM.Next((int)Data.ScreenSize.Y)))); }
                                break;
                            case 5:
                                FoundBlackHole = false;
                                for (int i = 0; i < EnemyList.Count; i++)
                                {
                                    if (EnemyList[i].GetType() == typeof(Black_Hole))
                                        FoundBlackHole = true;
                                }
                                if (FoundBlackHole) { EnemyList.Add(new Seeker()); }
                                else { EnemyList.Add(new Black_Hole(new Vector2(Data.RDM.Next((int)Data.ScreenSize.X), Data.RDM.Next((int)Data.ScreenSize.Y)))); }
                                break;
                            case 6:
                                FoundShooters = false;
                                for (int i = 0; i < EnemyList.Count; i++)
                                {
                                    if (EnemyList[i].GetType() == typeof(Shooter))
                                        FoundShooters = true;
                                }
                                if (FoundShooters) { EnemyList.Add(new Seeker()); }
                                else { EnemyList.Add(new Shooter()); }
                                break;
                        }
                    }

                    if (Save.Default.Sound)
                    {
                        ImportedFiles.Spawn.Play(0.1f, 0, 0);
                    }
                    EnemySpawnTimer = Stats.EnemySpawnTime;
                }
            }
            EnemySpawnTimer--;
        }

        public static void CheckingForEnemyDeathAndDamage()
        {
            lock (EnemyList)
            {
                for (int i = 0; i < EnemyList.Count; i++)
                {
                    for (int ib = 0; ib < BulletList.Count; ib++)
                    {
                        try
                        {
                            if (EnemyList[i].IsColidingWith(BulletList[ib]) && BulletList[ib].FromPlayer)
                            {
                                // Damage
                                EnemyList[i].GetDMG(1, BulletList[ib]);
                                BulletList.Remove(BulletList[ib]);
                                ib--;

                                if (ib != 0)
                                    ib--;

                                if (ib > BulletList.Count)
                                    ib = BulletList.Count;
                            }
                        }
                        catch { }
                    }

                    if (EnemyList[i].Hitpoints < 1)
                    {
                        // Death
                        EnemyList[i].OnDeath();
                        EnemyList.Remove(EnemyList[i]);
                        i--;
                    }
                }
            }
        }
        public static void CheckingForPlayerDeathAndDamage()
        {
            if (!DebugMode)
            {
                lock (EnemyList)
                {
                    for (int i = 0; i < EnemyList.Count; i++)
                    {
                        if (ThisPlayer.IsColidingWith(EnemyList[i]) && ThisPlayer.DashTimer < 1 && EnemyList[i].Spawning < 1)
                        {
                            ThisPlayer.HealthPoints -= 20f;
                            DMG_IndikatorState = 1.5f;
                            if (Save.Default.Particles)
                            {
                                if (EnemyList[i].GetType() == typeof(Shooter))
                                {
                                    for (int ip = 0; ip < 300; ip++)
                                    {
                                        Angle = Data.RDM.Next(361);
                                        Strength = Data.RDM.Next(0, 2000 + (100));
                                        Strength /= 100;
                                        ParticleManager.ParticleList.Add(new Particle(EnemyList[i].Pos, new Vector2((float)Math.Sin(Angle) * Strength, (float)Math.Cos(Angle) * Strength) + EnemyList[i].Vel, Color.OrangeRed));
                                    }
                                }
                                else
                                {
                                    for (int ip = 0; ip < 300; ip++)
                                    {
                                        Angle = Data.RDM.Next(361);
                                        Strength = Data.RDM.Next(0, 2000 + (100));
                                        Strength /= 100;
                                        ParticleManager.ParticleList.Add(new Particle(EnemyList[i].Pos, new Vector2((float)Math.Sin(Angle) * Strength, (float)Math.Cos(Angle) * Strength) + EnemyList[i].Vel, EnemyList[i].Color));
                                    }
                                }
                            }
                            if (Save.Default.Sound)
                            {
                                ImportedFiles.Explosion.Play(0.1f, 0, 0);
                            }
                            EnemyList.Remove(EnemyList[i]);
                            if (Save.Default.Particles)
                            {
                                for (int ip = 0; ip < 100; ip++)
                                {
                                    Angle = Data.RDM.Next(3610);
                                    Strength = (float)Data.RDM.Next(0, 500) / 100f;
                                    ParticleManager.ParticleList.Add(new Particle(ThisPlayer.Pos, new Vector2((float)Math.Sin(Angle) * Strength, (float)Math.Cos(Angle) * Strength) + ThisPlayer.Vel, ThisPlayer.Color));
                                }
                            }
                            if (Save.Default.Sound)
                            {
                                ImportedFiles.PlayerExplosion.Play(0.1f, 0, 0);
                            }
                            if (ThisPlayer.HealthPoints <= 0)
                            {
                                ThisPlayer.LostLife();
                            }
                        }
                    }
                }

                for (int ib = 0; ib < BulletList.Count; ib++)
                {
                    if (ThisPlayer.IsColidingWith(BulletList[ib]) && !BulletList[ib].FromPlayer)
                    {
                        ThisPlayer.HealthPoints -= 5f;
                        BulletList.Remove(BulletList[ib]);
                        if (Save.Default.Particles)
                        {
                            for (int ip = 0; ip < 100; ip++)
                            {
                                Angle = Data.RDM.Next(3610);
                                Strength = (float)Data.RDM.Next(0, 500) / 100f;
                                ParticleManager.ParticleList.Add(new Particle(ThisPlayer.Pos, new Vector2((float)Math.Sin(Angle) * Strength, (float)Math.Cos(Angle) * Strength) + ThisPlayer.Vel, ThisPlayer.Color));
                            }
                        }
                        if (Save.Default.Sound)
                        {
                            ImportedFiles.PlayerExplosion.Play(0.1f, -0.4f, 0);
                        }
                        if (ThisPlayer.HealthPoints <= 0)
                        {
                            ThisPlayer.LostLife();
                        }
                    }
                }
            }
        }

        public static void Update(GameTime GT)
        {
            if (InUpgradeMenu)
            {
                MenuManager.UpgradeMenu.Update(GT);
            }
            else
            {
                if (Controls.CurKS.IsKeyDown(Keys.P) && Controls.LastKS.IsKeyUp(Keys.P))
                    NotPaused = !NotPaused;

                if (NotPaused)
                {
                    UpdateAllEntities();

                    CheckingForEnemyDeathAndDamage();
                    CheckingForPlayerDeathAndDamage();

                    EnemySpawner();

                    // Waves
                    if (Data.BaseWaveLength + (Wave * Data.WaveLengthFaktor) - WaveKills < 1)
                    {
                        EnemySpawnTimer = 120;
                        Wave++;
                        WaveKills = 0;
                        if (LimitersActivated)
                        {
                            InUpgradeMenu = true;
                            MenuManager.CreateNewUpgradeMenu();
                        }
                    }

                    if (BType == BackgroundType.Grid)
                        Task.Factory.StartNew(() => BackgroundGrid.Update(null));
                    else
                        SpawnNewStars();

                    // Let the DMG Indkator Texture slowly fade
                    DMG_IndikatorState -= 0.01f;
                }
            }
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (InUpgradeMenu)
            {
                MenuManager.UpgradeMenu.Draw(spriteBatch);
            }
            else
            {
                if (BType == BackgroundType.Grid)
                    BackgroundGrid.Draw(spriteBatch);

                HPBar.Draw(spriteBatch);
                ThisPlayer.Draw(spriteBatch);

                for (int i = 0; i < BulletList.Count; i++)
                {
                    BulletList[i].Draw(spriteBatch);
                }

                lock (EnemyList)
                {
                    for (int i = 0; i < EnemyList.Count; i++)
                    {
                        EnemyList[i].Draw(spriteBatch);
                    }
                }

                for (int i = 0; i < ScoreSignList.Count; i++)
                {
                    ScoreSignList[i].Draw(spriteBatch);
                }

                for (int i = 0; i < BombList.Count; i++)
                {
                    BombList[i].Draw(spriteBatch);
                }

                spriteBatch.Draw(ImportedFiles.DMG_Indikator, new Rectangle(0, 0, (int)Data.ScreenSize.X, (int)Data.ScreenSize.Y), Color.White * DMG_IndikatorState);

                if (ThisPlayer.RespawnTimer > 0)
                {
                    spriteBatch.DrawString(ImportedFiles.BigFont, "Respawn in: " + (ThisPlayer.RespawnTimer / 60).ToString(),
                        Data.ScreenSize / 2 - ImportedFiles.BigFont.MeasureString("Respawn in: " + (ThisPlayer.RespawnTimer / 60).ToString()) / 2, Color.LightBlue, 0,
                        new Vector2(0, 0), 1, SpriteEffects.None, 0);
                }
                if (!NotPaused)
                {
                    spriteBatch.DrawString(ImportedFiles.BigFont, "Paused", Data.ScreenSize / 2 - ImportedFiles.BigFont.MeasureString("Paused") / 2, Color.LightBlue, 0,
                        new Vector2(0, 0), 1, SpriteEffects.None, 0);
                }
                spriteBatch.DrawString(ImportedFiles.SmallFont, "Score: " + Score.ToString(),
                    new Vector2(Data.ScreenSize.X - ImportedFiles.SmallFont.MeasureString("Score: " + Score.ToString()).X - 12, 12), Color.LightBlue);

                spriteBatch.DrawString(ImportedFiles.SmallFont, "Lives: " + Lives.ToString(),
                    new Vector2(12, 12), Color.LightBlue);

                spriteBatch.DrawString(ImportedFiles.SmallFont, "Level: " + (Wave + 1).ToString(),
                    new Vector2(Data.ScreenSize.X / 2 - ImportedFiles.SmallFont.MeasureString("Level: " + Wave.ToString()).X / 2, 12), Color.LightBlue, 0,
                    new Vector2(0, 0), 1, SpriteEffects.None, 0);
            }
        }
    }
}
