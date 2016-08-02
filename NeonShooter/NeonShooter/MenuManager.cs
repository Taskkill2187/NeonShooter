using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace NeonShooter
{
    public static class MenuManager
    {
        public static GameState ThisGameState = GameState.MainMenu;
        public static Menu MainMenu = new Menu();
        public static Menu Options = new Menu();
        public static Menu Highscore = new Menu();
        public static Menu UpgradeMenu = new Menu();

        public static Color MenuColor = Color.LightBlue;

        public static void InitializeMenus()
        {
            MainMenu.ButtonList.Add(new Button("Play", new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2 - 75), MenuColor, ImportedFiles.BigFont));
            MainMenu.ButtonList.Add(new Button("Highscore", new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2), MenuColor, ImportedFiles.BigFont));
            MainMenu.ButtonList.Add(new Button("Options", new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2 + 75), MenuColor, ImportedFiles.BigFont));

            MainMenu.ButtonList[0].OnClick += OnClick_Play;
            MainMenu.ButtonList[1].OnClick += OnClick_Highscore;
            MainMenu.ButtonList[2].OnClick += OnClick_Options;

            Options.ButtonList.Add(new Button("Music: " + Save.Default.Music.ToString(), new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2 - 225), MenuColor, ImportedFiles.BigFont));
            Options.ButtonList.Add(new Button("Sound: " + Save.Default.Sound.ToString(), new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2 - 150), MenuColor, ImportedFiles.BigFont));
            Options.ButtonList.Add(new Button("ParticleEffects: " + Save.Default.Particles.ToString(), new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2 - 75), MenuColor, ImportedFiles.BigFont));
            Options.ButtonList.Add(new Button("PixelShader: " + Save.Default.PixelShader.ToString(), new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2), MenuColor, ImportedFiles.BigFont));
            Options.ButtonList.Add(new Button("Fullscreen: " + Save.Default.Fullscreen.ToString(), new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2 + 75), MenuColor, ImportedFiles.BigFont));
            Options.ButtonList.Add(new Button("Game-Background: " + GameManager.BType.ToString(), new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2 + 150), MenuColor, ImportedFiles.BigFont));
            Options.ButtonList.Add(new Button("Limiters [EXPERIMENTAL]: " + GameManager.LimitersActivated.ToString(), new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2 + 225), MenuColor, ImportedFiles.BigFont));

            Options.ButtonList[0].OnClick += OnClick_Music;
            Options.ButtonList[1].OnClick += OnClick_Sound;
            Options.ButtonList[2].OnClick += OnClick_Particles;
            Options.ButtonList[3].OnClick += OnClick_PixelShader;
            Options.ButtonList[4].OnClick += OnClick_Fullscreen;
            Options.ButtonList[5].OnClick += OnClick_BType;
            Options.ButtonList[6].OnClick += (object sender, EventArgs e) => { GameManager.LimitersActivated = !GameManager.LimitersActivated;
                Options.ButtonList[6].Text = "Limiters [EXPERIMENTAL]: " + GameManager.LimitersActivated.ToString(); };
            switch (Save.Default.PixelShader)
            {
                case 0:
                    Options.ButtonList[3].Text = "PixelShader: None";
                    break;

                case 1:
                    Options.ButtonList[3].Text = "PixelShader: Bloom";
                    break;

                case 2:
                    Options.ButtonList[3].Text = "PixelShader: Cromatic-Abberation";
                    break;

                case 3:
                    Options.ButtonList[3].Text = "PixelShader: Simple-Blur";
                    break;
            }

            Highscore.ButtonList.Add(new Button("1. Highscore: " + Save.Default.Highscore1.ToString(), new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2 - 75), MenuColor, ImportedFiles.BigFont));
            Highscore.ButtonList.Add(new Button("2. Highscore: " + Save.Default.Highscore2.ToString(), new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2), MenuColor, ImportedFiles.BigFont));
            Highscore.ButtonList.Add(new Button("3. Highscore: " + Save.Default.Highscore3.ToString(), new Vector2(Data.ScreenSize.X / 2, Data.ScreenSize.Y / 2 + 75), MenuColor, ImportedFiles.BigFont));

            UpgradeMenu.ButtonList.Add(new Button("1", new Vector2(Data.ScreenSize.X / 3, Data.ScreenSize.Y / 2), MenuColor, ImportedFiles.SmallFont));
            UpgradeMenu.ButtonList.Add(new Button("2", new Vector2(Data.ScreenSize.X / 3 * 2, Data.ScreenSize.Y / 2), MenuColor, ImportedFiles.SmallFont));
        }
        public static void CreateNewUpgradeMenu()
        {
            UpgradeMenu.ButtonList[0].ResetOnClickEvent(); UpgradeMenu.ButtonList[1].ResetOnClickEvent();

            switch(GameManager.Wave - 1)
            {
                case 0:
                    UpgradeMenu.ButtonList[0].Text = "-50% Acceleration";
                    UpgradeMenu.ButtonList[1].Text = "-25% Seeker Charge Time";
                    UpgradeMenu.ButtonList[0].OnClick += (object sender, EventArgs e) => { Stats.PlayerAcceleration += (-Stats.PlayerAcceleration * 0.50f); GameManager.InUpgradeMenu = false; ParticleManager.Clear(); };
                    UpgradeMenu.ButtonList[1].OnClick += (object sender, EventArgs e) => { Stats.SeekerDashChargeTime += (int)(-Stats.SeekerDashChargeTime * 0.25f); GameManager.InUpgradeMenu = false; ParticleManager.Clear(); };
                    break;

                case 1:
                    UpgradeMenu.ButtonList[0].Text = "-10% Shooting-Frequenzy";
                    UpgradeMenu.ButtonList[1].Text = "+15% BlackHole-Speed";
                    UpgradeMenu.ButtonList[0].OnClick += (object sender, EventArgs e) => { Stats.PlayerShootCooldown += (int)(-Stats.PlayerShootCooldown * 0.10f); GameManager.InUpgradeMenu = false; ParticleManager.Clear(); };
                    UpgradeMenu.ButtonList[1].OnClick += (object sender, EventArgs e) => { Stats.BlackHoleSpeed += (Stats.BlackHoleSpeed * 0.15f); GameManager.InUpgradeMenu = false; ParticleManager.Clear(); };
                    break;

                case 2:
                    UpgradeMenu.ButtonList[0].Text = "+10% Enemy-HP";
                    UpgradeMenu.ButtonList[1].Text = "-50% EnemyBullet-Speed";
                    UpgradeMenu.ButtonList[0].OnClick += (object sender, EventArgs e) => { Stats.EnemyHitpoints += (int)(Stats.EnemyHitpoints * 0.10f); GameManager.InUpgradeMenu = false; ParticleManager.Clear(); };
                    UpgradeMenu.ButtonList[1].OnClick += (object sender, EventArgs e) => { Stats.EnemyBulletSpeed += (int)(-Stats.EnemyBulletSpeed * 0.50f); GameManager.InUpgradeMenu = false; ParticleManager.Clear(); };
                    break;

                case 3:
                    UpgradeMenu.ButtonList[0].Text = "+10% Enemy-HP";
                    UpgradeMenu.ButtonList[1].Text = "-50% EnemyBullet-Speed";
                    UpgradeMenu.ButtonList[0].OnClick += (object sender, EventArgs e) => { Stats.EnemyHitpoints += (int)(Stats.EnemyHitpoints * 0.10f); GameManager.InUpgradeMenu = false; };
                    UpgradeMenu.ButtonList[1].OnClick += (object sender, EventArgs e) => { Stats.EnemyBulletSpeed += (int)(-Stats.EnemyBulletSpeed * 0.50f); GameManager.InUpgradeMenu = false; };
                    break;

                default:
                    UpgradeMenu.ButtonList[0].Text = "Continue";
                    UpgradeMenu.ButtonList[1].Text = "Continue";
                    UpgradeMenu.ButtonList[0].OnClick += (object sender, EventArgs e) => { GameManager.InUpgradeMenu = false; };
                    UpgradeMenu.ButtonList[1].OnClick += (object sender, EventArgs e) => { GameManager.InUpgradeMenu = false; };
                    break;
            }
        }

        private static void OnClick_Play(object sender, EventArgs e)
        {
            ThisGameState = GameState.Game;
            GameManager.Reset();
            MediaPlayer.Volume = 0.3f;
            MediaPlayer.Play(ImportedFiles.Derezzed);
            ParticleManager.Clear();
        }
        private static void OnClick_Highscore(object sender, EventArgs e)
        {
            MenuManager.ThisGameState = GameState.Highscore;
        }
        private static void OnClick_Options(object sender, EventArgs e)
        {
            MenuManager.ThisGameState = GameState.Options;
        }

        private static void OnClick_Music(object sender, EventArgs e)
        {
            switch (Save.Default.Music)
            {
                case true:
                    Save.Default.Music = false;
                    MediaPlayer.IsMuted = true;
                    MenuManager.Options.ButtonList[0].Text = "Music: " + Save.Default.Music.ToString();
                    break;

                case false:
                    Save.Default.Music = true;
                    MediaPlayer.IsMuted = false;
                    MenuManager.Options.ButtonList[0].Text = "Music: " + Save.Default.Music.ToString();
                    break;
            }
        }
        private static void OnClick_Sound(object sender, EventArgs e)
        {
            switch (Save.Default.Sound)
            {
                case true:
                    Save.Default.Sound = false;
                    MenuManager.Options.ButtonList[1].Text = "Sound: " + Save.Default.Sound.ToString();
                    break;

                case false:
                    Save.Default.Sound = true;
                    MenuManager.Options.ButtonList[1].Text = "Sound: " + Save.Default.Sound.ToString();
                    break;
            }
        }
        private static void OnClick_Particles(object sender, EventArgs e)
        {
            switch (Save.Default.Particles)
            {
                case true:
                    Save.Default.Particles = false;
                    MenuManager.Options.ButtonList[2].Text = "ParticleEffects: " + Save.Default.Particles.ToString();
                    break;

                case false:
                    Save.Default.Particles = true;
                    MenuManager.Options.ButtonList[2].Text = "ParticleEffects: " + Save.Default.Particles.ToString();
                    break;
            }
        }
        private static void OnClick_PixelShader(object sender, EventArgs e)
        {
            switch (Save.Default.PixelShader)
            {
                case 0:
                    Save.Default.PixelShader = 1;
                    break;

                case 1:
                    Save.Default.PixelShader = 2;
                    break;

                case 2:
                    Save.Default.PixelShader = 3;
                    break;

                case 3:
                    Save.Default.PixelShader = 0;
                    break;
            }

            switch (Save.Default.PixelShader)
            {
                case 0:
                    Options.ButtonList[3].Text = "PixelShader: None";
                    break;

                case 1:
                    Options.ButtonList[3].Text = "PixelShader: Bloom";
                    break;

                case 2:
                    Options.ButtonList[3].Text = "PixelShader: Cromatic-Abberation";
                    break;

                case 3:
                    Options.ButtonList[3].Text = "PixelShader: Simple-Blur";
                    break;
            }
        }
        private static void OnClick_Fullscreen(object sender, EventArgs e)
        {
            switch (Save.Default.Fullscreen)
            {
                case true:
                    Save.Default.Fullscreen = false;
                    MenuManager.Options.ButtonList[4].Text = "Fullscreen: " + Save.Default.Fullscreen.ToString();
                    break;

                case false:
                    Save.Default.Fullscreen = true;
                    MenuManager.Options.ButtonList[4].Text = "Fullscreen: " + Save.Default.Fullscreen.ToString();
                    break;
            }
        }
        private static void OnClick_BType(object sender, EventArgs e)
        {
            if (GameManager.BType == BackgroundType.Grid)
                GameManager.BType = BackgroundType.Stars;
            else
                GameManager.BType = BackgroundType.Grid;

            Options.ButtonList[5].Text = "Game-Background: " + GameManager.BType.ToString();
        }
    }
}
