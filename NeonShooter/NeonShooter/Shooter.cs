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
    public enum ShooterType
    {
        Shotgun,
        Star
    }

    public class Shooter : Enemy
    {
        ShooterType Type;

        public Shooter()
        {
            Pos = GetRandomPosAtTheEndOfTheScreen();
            Color = Color.Red;
            Spawning = 60;
            Radius = 30;
            Rotation = (float)Math.Atan2(GameManager.ThisPlayer.Pos.X - Pos.X, GameManager.ThisPlayer.Pos.Y - Pos.Y);
            Vel = new Vector2((float)Math.Sin(Rotation) * Stats.EnemySpeed, (float)Math.Cos(Rotation) * Stats.EnemySpeed);
            Rotation = -Rotation + 89.5f;
            Hitpoints = Stats.EnemyHitpoints;
            Tex = ImportedFiles.Wanderer;
            Resistance = Stats.ShooterResistance;
            Type = (ShooterType)Data.RDM.Next(2);
        }

        public override void Update()
        {
            Rotation = ForceAngle;
            Vel = new Vector2((float)Math.Sin(Rotation) * Stats.EnemySpeed + (0.03f * ((float)Math.Sqrt(GameManager.Wave))), 
                                   (float)Math.Cos(Rotation) * Stats.EnemySpeed + (0.03f * ((float)Math.Sqrt(GameManager.Wave))));
            Vel /= 1.3f;
            Rotation = -Rotation + 89.5f;

            if (ChangeMovementTimer < 1)
            {
                ForceAngle = Data.RDM.Next(361);
                ChangeMovementTimer = 60;
            }
            ChangeMovementTimer--;

            if (ShootCooldown < 1 && Spawning < 1)
            {
                switch ((int)Type)
                {
                    case 0:
                        ShotGunShootAtPlayer();
                        ShootCooldown = Stats.ShooterShotgunCooldown;
                        break;

                    case 1:
                        ShootInAllDirections();
                        ShootCooldown = Stats.ShooterAllDirCooldown;
                        break;
                }
            }
            ShootCooldown--;

            if (Pos.X < 0 && Vel.X < 0)
            {
                //Vel.X *= -1;
                ForceAngle = (float)Math.Atan2(Data.ScreenSize.X - Pos.X, Data.ScreenSize.Y - Pos.Y);
            }

            if (Pos.X > Data.ScreenSize.X && Vel.X > 0)
            {
                //Vel.X *= -1;
                ForceAngle = (float)Math.Atan2(Data.ScreenSize.X - Pos.X, Data.ScreenSize.Y - Pos.Y);
            }

            if (Pos.Y < 0 && Vel.Y < 0)
            {
                //Vel.Y *= -1;
                ForceAngle = (float)Math.Atan2(Data.ScreenSize.X - Pos.X, Data.ScreenSize.Y - Pos.Y);
            }

            if (Pos.Y > Data.ScreenSize.Y && Vel.Y > 0)
            {
                //Vel.Y *= -1;
                ForceAngle = (float)Math.Atan2(Data.ScreenSize.X - Pos.X, Data.ScreenSize.Y - Pos.Y);
            }

            Pos += Vel;

            EnemyBar.Percentage = Hitpoints / Stats.EnemyHitpoints;
            EnemyBar.Pos = new Vector2(Pos.X, Pos.Y - (int)Radius - 30);

            InvincibilityCooldown--;

            Spawning--;
        }
    }
}
