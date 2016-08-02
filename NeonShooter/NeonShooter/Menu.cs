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
    public class Menu
    {
        public List<Button> ButtonList = new List<Button>();

        public float Angle;
        public float Strength;

        private int SpawnDistance = (int)(Data.ScreenSize / 2).Length();
        private float AblenkungsWinkel = 0.99f;
        private float Force = 0.6f;

        public void Update(GameTime GT)
        {
            if (Save.Default.Particles)
            {
                //for (int i = 0; i < 10; i++) { ParticleManager.ParticleList2.Add(new Particle(new Vector2(Data.RDM.Next((int)Data.ScreenSize.X), 0), MenuManager.MenuColor, 0.25f)); }
                //for (int i = 0; i < 10; i++) { ParticleManager.ParticleList2.Add(new Particle(new Vector2(Data.RDM.Next((int)Data.ScreenSize.X), Data.ScreenSize.Y), MenuManager.MenuColor, 0.25f)); }
                //for (int i = 0; i < 5; i++) { ParticleManager.ParticleList2.Add(new Particle(new Vector2((int)Data.ScreenSize.X, Data.RDM.Next((int)Data.ScreenSize.Y)), MenuManager.MenuColor, 0.25f)); }
                //for (int i = 0; i < 5; i++) { ParticleManager.ParticleList2.Add(new Particle(new Vector2(0, Data.RDM.Next((int)Data.ScreenSize.Y)), MenuManager.MenuColor, 0.25f)); }
                lock (ParticleManager.ParticleList2)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        float Angle = Data.RDM.Next(360);
                        //int SpawnDistance = (int)(Data.ScreenSize / 2).Length();
                        ParticleManager.ParticleList2.Add(new Particle(new Vector2(Data.ScreenSize.X / 2 + (float)Math.Sin(Angle) * SpawnDistance, 
                            Data.ScreenSize.Y / 2 + (float)Math.Cos(Angle) * SpawnDistance), MenuManager.MenuColor, 0f));
                    }
                }

                lock (ParticleManager.ParticleList)
                {
                    for (int i = 0; i < ParticleManager.ParticleList.Count; i++)
                    {
                        if (i < ParticleManager.ParticleList.Count && ParticleManager.ParticleList[i] != null)
                        {
                            ParticleManager.ParticleList[i].OrbitAround(Data.ScreenSize / 2, Force, AblenkungsWinkel);
                        }
                    }
                }

                lock (ParticleManager.ParticleList2)
                {
                    for (int i = 0; i < ParticleManager.ParticleList2.Count; i++)
                    {
                        if (i < ParticleManager.ParticleList2.Count && ParticleManager.ParticleList2[i] != null)
                        {
                            ParticleManager.ParticleList2[i].OrbitAround(Data.ScreenSize / 2, Force, AblenkungsWinkel);
                        }
                    }
                }
            }

            if (Controls.CurMS.LeftButton == ButtonState.Pressed && Controls.LastMS.LeftButton == ButtonState.Released)
            {
                if (Save.Default.Particles)
                {
                    for (int ip = 0; ip < 50; ip++)
                    {
                        Angle = Data.RDM.Next(361);
                        Strength = Data.RDM.Next(2, 15);
                        ParticleManager.ParticleList.Add(new Particle(new Vector2(Controls.CurMS.X, Controls.CurMS.Y), new Vector2((float)Math.Sin(Angle) * Strength, (float)Math.Cos(Angle) * Strength), Color.LightBlue, 0, 0));
                    }
                }
            }

            for (int i = 0; i < ButtonList.Count; i++)
            {
                ButtonList[i].Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < ButtonList.Count; i++)
            {
                ButtonList[i].Draw(spriteBatch);
            }
        }
    }
}
