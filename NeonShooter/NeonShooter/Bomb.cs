using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace NeonShooter
{
    public class Bomb : Entity
    {
        const int ExplosionTime = 15;
        public int ExplosionTimer;
        bool IsExploding;
        int TimeUntilExplosion;

        public Bomb(int TimeUntilExplosion)
        {
            Pos = GameManager.ThisPlayer.Pos;
            this.TimeUntilExplosion = TimeUntilExplosion;
            IsExploding = false;
            ExplosionTimer = ExplosionTime;
            Radius = 180;
            Vel = new Vector2((Controls.CurMS.X - GameManager.ThisPlayer.Pos.X) / TimeUntilExplosion, (Controls.CurMS.Y - GameManager.ThisPlayer.Pos.Y) / TimeUntilExplosion);
        }

        public new void Update()
        {
            TimeUntilExplosion--;
            if (TimeUntilExplosion < 1 && IsExploding == false)
            {
                IsExploding = true;
                ImportedFiles.SeismicBomb.Play(0.2f, 0, 0);
                for (int i = 0; i < GameManager.EnemyList.Count; i++)
                {
                    if (GameManager.EnemyList[i].IsColidingWith(this))
                    {
                        GameManager.EnemyList[i].Hitpoints = 0;
                    }
                }
            }

            if (IsExploding)
                ExplosionTimer--;
            else
                Pos += Vel;
        }

        public new void Draw(SpriteBatch SB)
        {
            if (IsExploding)
            {
                SB.Draw(ImportedFiles.BombExplosionAnim, new Vector2(Pos.X - Radius, Pos.Y - Radius), new Rectangle(((int)((float)ExplosionTimer / (float)ExplosionTime * 9f) * 250), 0, 250, 250), Color.White, 0, Vector2.Zero, Radius / 125, SpriteEffects.None, 1);
            }
            else
            {
                SB.Draw(ImportedFiles.Bomb, new Rectangle((int)Pos.X - 25, (int)Pos.Y - 25, 50, 50), Color.DarkGray);
            }
        }
    }
}
