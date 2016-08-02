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
    public class ScoreSign : Entity
    {
        public int LifeTime;
        public string Text;

        public ScoreSign(Vector2 Pos, string Text, Color color)
        {
            LifeTime = 0;
            this.Pos = Pos;
            this.Text = Text;
            this.Color = color;
        }

        public new void Update()
        {
            LifeTime++;

            Pos.Y -= 3;
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(ImportedFiles.SmallFont, Text, Pos - ImportedFiles.SmallFont.MeasureString(Text) / 2, Color * (1f - (float)LifeTime / 255f));
        }
    }
}
