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
    public static class Data
    {
        public static Vector2 ScreenSize = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        
        public static int PlayerRespawnTime = 180;
        public static float BaseWaveLength = 15.2f;
        public static float WaveLengthFaktor = 1.2f;
        public static float BloomStrength = 1.5f;
        public static bool BloomCharging = true;
        public static bool BootWithBloom = true;
        public static float ChromaticAbberationAmount = 0.002f;
        public static bool ChromaticCharging;

        public static int ParticleForceMultiplier = 5000;
        public static Random RDM = new Random();

        public static void TriggerFullScreenShaderEffects()
        {
            BloomStrength = 1.51f;
            ChromaticAbberationAmount = 0.00201f;
        }

        public static void Update()
        {
            // Bloom
            if (BloomStrength > 1.5)
            {
                if (BloomCharging)
                {
                    BloomStrength += 0.25f;
                }
                else
                {
                    BloomStrength -= 0.25f;
                }
                if (BloomStrength > 2.5f)
                {
                    BloomCharging = false;
                }
            }
            else { BloomCharging = true; }

            // CA Effect
            if (ChromaticAbberationAmount > 0.002)
            {
                if (ChromaticCharging)
                {
                    ChromaticAbberationAmount += 0.0006f;
                }
                else
                {
                    ChromaticAbberationAmount -= 0.0006f;
                }
                if (ChromaticAbberationAmount > 0.007)
                {
                    ChromaticCharging = false;
                }
            }
            else { ChromaticCharging = true; }

            //// CounterUpdate
            //if (ExplosionDisortionCounter > 0)
            //    ExplosionDisortionCounter--;
        }

        // Will rape your performance
        public static Texture2D TurnAllBlackPixelsTransparent(Texture2D texture)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].R == 0 && data[i].G == 0 && data[i].B == 0)
                {
                    data[i].A = 0;
                }

                //data[i] = Color.FromNonPremultiplied(data[i].R, data[i].G, data[i].B, 255 - Math.Max(Math.Max(data[i].R, data[i].G), data[i].B));
            }
            texture.SetData(data);
            return texture;
        }
    }

    public static class Stats
    {
        public static int PlayerBulletSpeed = 25;
        public static float PlayerMaxSpeed = 9;
        public static float PlayerAcceleration = 3;
        public static int PlayerShootCooldown = 5;

        public static float EnemySpeed = 8f;
        public static int EnemyHitpoints = 4;
        public static int EnemySpawnTime = 60;
        public static int EnemyBulletSpeed = 6;

        public static float SeekerResistance = 0.8f;
        public static float BlackHoleResistance = 3;
        public static float ShooterResistance = 1;

        public static int SeekerDashChargeTime = 20;
        public static int SeekerDashTime = 600;
        public static int ShooterShotgunCooldown = 60;
        public static int ShooterAllDirCooldown = 100;
        public static float BlackHoleSpeed = 2;
        
        public static void Reset()
        {
            PlayerBulletSpeed = 25;
            EnemyBulletSpeed = 6;
            PlayerMaxSpeed = 9;
            EnemySpeed = 8f;
            EnemyHitpoints = 4;
            SeekerResistance = 0.8f;
            BlackHoleResistance = 3;
            ShooterResistance = 1;
            SeekerDashChargeTime = 20;
            SeekerDashTime = 600;
            ShooterShotgunCooldown = 60;
            ShooterAllDirCooldown = 100;
            BlackHoleSpeed = 2;
            EnemySpawnTime = 60;
            PlayerShootCooldown = 5;
        }
    }
}
