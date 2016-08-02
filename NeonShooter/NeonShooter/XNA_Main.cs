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
    public enum GameState
    {
        MainMenu,
        Highscore,
        Options,
        Game
    }

    public class XNA_Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BloomComponent Bloom;

        RenderTarget2D renderTarget;
        Texture2D renderTargetSave;

        public XNA_Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = (int)Data.ScreenSize.X;
            graphics.PreferredBackBufferHeight = (int)Data.ScreenSize.Y;
            
            if (Data.BootWithBloom)
            {
                Bloom = new BloomComponent(this);
                Components.Add(Bloom);
                Bloom.Settings = new BloomSettings(null, 0.01f, 1.5f, 1.5f, 1, 2f, 2);
            }

            //Highscore zurücksetzten

            //Save.Default.Highscore1 = 0;
            //Save.Default.Highscore2 = 0;
            //Save.Default.Highscore3 = 0;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            MenuManager.InitializeMenus();

            #region Prepare Particles
            //if (Save.Default.Particles)
            //{
            //    for (int h = 0; h < 200; h++)
            //    {
            //        lock (ParticleManager.ParticleList2)
            //        {
            //            for (int i = 0; i < 10; i++)
            //            {
            //                ParticleManager.ParticleList2.Add(new Particle(new Vector2(Data.RDM.Next((int)Data.ScreenSize.X), 0), MenuManager.MenuColor, 0.25f));
            //            }

            //            for (int i = 0; i < 10; i++)
            //            {
            //                ParticleManager.ParticleList2.Add(new Particle(new Vector2(Data.RDM.Next((int)Data.ScreenSize.X), Data.ScreenSize.Y), MenuManager.MenuColor, 0.25f));
            //            }

            //            for (int i = 0; i < 5; i++)
            //            {
            //                ParticleManager.ParticleList2.Add(new Particle(new Vector2((int)Data.ScreenSize.X, Data.RDM.Next((int)Data.ScreenSize.Y)), MenuManager.MenuColor, 0.25f));
            //            }

            //            for (int i = 0; i < 5; i++)
            //            {
            //                ParticleManager.ParticleList2.Add(new Particle(new Vector2(0, Data.RDM.Next((int)Data.ScreenSize.Y)), MenuManager.MenuColor, 0.25f));
            //            }
            //        }

            //        lock (ParticleManager.ParticleList)
            //        {
            //            for (int i = 0; i < ParticleManager.ParticleList.Count; i++)
            //            {
            //                if (i < ParticleManager.ParticleList.Count && ParticleManager.ParticleList[i] != null)
            //                {
            //                    ParticleManager.ParticleList[i].GetPulledBy(Data.ScreenSize / 2, true, 1, 0);
            //                }
            //            }
            //        }

            //        lock (ParticleManager.ParticleList2)
            //        {
            //            for (int i = 0; i < ParticleManager.ParticleList2.Count; i++)
            //            {
            //                if (i < ParticleManager.ParticleList2.Count && ParticleManager.ParticleList2[i] != null)
            //                {
            //                    ParticleManager.ParticleList2[i].GetPulledBy(Data.ScreenSize / 2, true, 1, 0);
            //                }
            //            }
            //        }
            //        ParticleManager.Update();
            //    }
            //}
            #endregion
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            renderTarget = new RenderTarget2D(GraphicsDevice, (int)Data.ScreenSize.X, (int)Data.ScreenSize.Y, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            ImportedFiles.Load(Content, GraphicsDevice);
            MediaPlayer.Volume = 0.4f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(ImportedFiles.TheGrid);
        }

        protected override void Update(GameTime gameTime)
        {
            Controls.Update();
            FPSCounter.Update(gameTime);

            #region Effect Updater
            if (Data.BootWithBloom)
            {
                Bloom.Settings = new BloomSettings(null, 0.01f, Data.BloomStrength, Data.BloomStrength, 1, 1.5f, 1);
            }
            Data.Update();
            #endregion

            #region Fullscreen State Updater
            if (graphics.IsFullScreen && !Save.Default.Fullscreen)
            {
                graphics.IsFullScreen = false;
                graphics.ApplyChanges();
            }

            if (!graphics.IsFullScreen && Save.Default.Fullscreen)
            {
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
            }
            #endregion

            #region Menu Control
            if (Controls.CurKS.IsKeyDown(Keys.Escape) && Controls.LastKS.IsKeyUp(Keys.Escape) && MenuManager.ThisGameState == GameState.MainMenu)
            {
                Save.Default.Save();
                this.Exit();
            }

            if (Controls.CurKS.IsKeyDown(Keys.Escape) && Controls.LastKS.IsKeyUp(Keys.Escape) && MenuManager.ThisGameState == GameState.Game)
            {
                MenuManager.ThisGameState = GameState.MainMenu;
                GameManager.Reset();
                MediaPlayer.Volume = 0.4f;
                MediaPlayer.Play(ImportedFiles.TheGrid);
            }

            if (Controls.CurKS.IsKeyDown(Keys.Escape) && Controls.LastKS.IsKeyUp(Keys.Escape) && MenuManager.ThisGameState == GameState.Options)
            {
                MenuManager.ThisGameState = GameState.MainMenu;
                if (Save.Default.Sound)
                    ImportedFiles.Spawn.Play(0.2f, -0.4f, 0);
                Data.BloomStrength = 3.01f;
            }

            if (Controls.CurKS.IsKeyDown(Keys.Escape) && Controls.LastKS.IsKeyUp(Keys.Escape) && MenuManager.ThisGameState == GameState.Highscore)
            {
                MenuManager.ThisGameState = GameState.MainMenu;
                if (Save.Default.Sound)
                    ImportedFiles.Spawn.Play(0.2f, -0.4f, 0);
                Data.BloomStrength = 3.01f;
            }
            #endregion

            #region Music
            if (Save.Default.Music)
            {
                MediaPlayer.IsMuted = false;
            }
            else
            {
                MediaPlayer.IsMuted = true;
            }
            #endregion

            #region GameState Updater
            if (GameManager.NotPaused && MenuManager.ThisGameState == GameState.Game || MenuManager.ThisGameState != GameState.Game)
            {
                ParticleManager.Update();
            }

            switch (MenuManager.ThisGameState)
            {
                case GameState.MainMenu:
                    MenuManager.MainMenu.Update(gameTime);
                    break;

                case GameState.Options:
                    MenuManager.Options.Update(gameTime);
                    break;

                case GameState.Highscore:
                    MenuManager.Highscore.Update(gameTime);
                    break;

                case GameState.Game:
                    GameManager.Update(gameTime);
                    break;
            }
            #endregion
        }

        protected override void Draw(GameTime gameTime)
        {
            #region RenderTarget
            GraphicsDevice.SetRenderTarget(renderTarget);

            if (Data.BootWithBloom)
            {
                if (Save.Default.PixelShader == 1)
                {
                    Bloom.Visible = true;
                }
                else
                {
                    Bloom.Visible = false;
                }
                Bloom.BeginDraw();
            }

            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            //FPSCounter.Draw(spriteBatch);
            ParticleManager.Draw(spriteBatch);
            switch (MenuManager.ThisGameState)
            {
                case GameState.MainMenu:
                    MenuManager.MainMenu.Draw(spriteBatch);
                    break;

                case GameState.Options:
                    MenuManager.Options.Draw(spriteBatch);
                    break;

                case GameState.Highscore:
                    MenuManager.Highscore.Draw(spriteBatch);
                    break;

                case GameState.Game:
                    GameManager.Draw(spriteBatch);
                    break;
            }
            spriteBatch.Draw(ImportedFiles.Pointer, new Vector2(Controls.CurMS.X, Controls.CurMS.Y), Color.LightBlue);
            spriteBatch.End();

            renderTargetSave = renderTarget;
            #endregion

            #region Render to screen
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            switch (Save.Default.PixelShader)
            {
                case 0:
                    spriteBatch.Draw(renderTargetSave, new Rectangle(0, 0, (int)Data.ScreenSize.X, (int)Data.ScreenSize.Y), Color.White);
                    break;

                case 1:
                    for (int i = 0; i < 3; i++)
                    {
                        ImportedFiles.ChromaticAberration.Parameters["AbberationAmount"].SetValue(Data.ChromaticAbberationAmount);
                        ImportedFiles.ChromaticAberration.Parameters["RadialBlurAmount"].SetValue(5);
                        ImportedFiles.ChromaticAberration.CurrentTechnique.Passes[0].Apply();
                        spriteBatch.Draw(renderTargetSave, new Rectangle(0, 0, (int)Data.ScreenSize.X, (int)Data.ScreenSize.Y), Color.White * (0.5f / 3f));
                    }
                    spriteBatch.Draw(renderTargetSave, new Rectangle(0, 0, (int)Data.ScreenSize.X, (int)Data.ScreenSize.Y), Color.White * 0.5f);
                    break;

                case 2:
                    ImportedFiles.ChromaticAberration.Parameters["AbberationAmount"].SetValue(Data.ChromaticAbberationAmount);
                    ImportedFiles.ChromaticAberration.Parameters["RadialBlurAmount"].SetValue(5);
                    ImportedFiles.ChromaticAberration.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(renderTargetSave, new Rectangle(0, 0, (int)Data.ScreenSize.X, (int)Data.ScreenSize.Y), Color.White * 0.5f);
                    spriteBatch.Draw(renderTargetSave, new Rectangle(0, 0, (int)Data.ScreenSize.X, (int)Data.ScreenSize.Y), Color.White * 0.5f);
                    break;

                case 3:
                    ImportedFiles.SimpleBlur.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(renderTargetSave, new Rectangle(0, 0, (int)Data.ScreenSize.X, (int)Data.ScreenSize.Y), Color.White * 0.5f);
                    spriteBatch.Draw(renderTargetSave, new Rectangle(0, 0, (int)Data.ScreenSize.X, (int)Data.ScreenSize.Y), Color.White * 0.5f);
                    break;
            }
            spriteBatch.End();
            #endregion

            base.Draw(gameTime);
        }
    }
}
