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
    public class Entity
    {
        public Vector2 Pos, Vel;
        public float Radius;
        public float Rotation;

        public Texture2D Tex;
        public Color Color;

        public bool IsColidingWith(Entity OtherEntity)
        {
            if (((this.Pos.X - OtherEntity.Pos.X) * (this.Pos.X - OtherEntity.Pos.X)) + (this.Pos.Y - OtherEntity.Pos.Y) * (this.Pos.Y - OtherEntity.Pos.Y) < (this.Radius + OtherEntity.Radius) * (this.Radius + OtherEntity.Radius))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void PreventFromMovingOutOfBounds()
        {
            if (Pos.X - Radius < 0 && Vel.X < 0)
            {
                Vel.X = 0;
                Pos.X = Radius;
            }

            if (Pos.X + Radius > Data.ScreenSize.X && Vel.X > 0)
            {
                Vel.X = 0;
                Pos.X = Data.ScreenSize.X - Radius;
            }

            if (Pos.Y - Radius < 0 && Vel.Y < 0)
            {
                Vel.Y = 0;
                Pos.Y = Radius;
            }

            if (Pos.Y + Radius > Data.ScreenSize.Y && Vel.Y > 0)
            {
                Vel.Y = 0;
                Pos.Y = Data.ScreenSize.Y - Radius;
            }
        }

        public virtual void Update() { }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Tex != null)
            {
                spriteBatch.Draw(Tex, Pos, null, Color, -Rotation +89.5f, new Vector2(Tex.Width / 2, Tex.Height / 2), 1f, SpriteEffects.None, 0);
            }
        }
    }
}
