using System;
using System.Collections.Generic;
using System.Threading;
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
    public class HP_Bar
    {
        public Vector2 Pos;
        public int Width;
        public int Heigth;
        public int GrayMargin;
        public float Percentage;
        private float LastPercentage;

        public HP_Bar(Vector2 Position, int Width, int Heigth, int Margin, float Percentage)
        {
            this.Pos = Position;
            this.Width = Width;
            this.Heigth = Heigth;
            this.GrayMargin = Margin;
            this.Percentage = Percentage;
            LastPercentage = Percentage;
        }

        public void Draw(SpriteBatch SB)
        {
            if (LastPercentage > Percentage)
            {
                LastPercentage -= 0.003f;
            }
            else
            {
                LastPercentage = Percentage;
            }

            SB.Draw(ImportedFiles.PureWhite, new Rectangle((int)Pos.X - Width / 2, (int)Pos.Y - Heigth / 2, Width, Heigth), Color.DarkGray * 0.5f);

            SB.Draw(ImportedFiles.PureWhite, new Rectangle((int)Pos.X - Width / 2 + GrayMargin, (int)Pos.Y - Heigth / 2 + GrayMargin,
                Width - GrayMargin * 2, Heigth - GrayMargin * 2), Color.FromNonPremultiplied(200, 200, 200, 255) * 0.5f);

            SB.Draw(ImportedFiles.PureWhite, new Rectangle((int)Pos.X - Width / 2 + GrayMargin, (int)Pos.Y - Heigth / 2 + GrayMargin,
                (int)((Width - GrayMargin * 2) * LastPercentage), Heigth - GrayMargin * 2), Color.DarkGreen * 0.5f);

            SB.Draw(ImportedFiles.PureWhite, new Rectangle((int)Pos.X - Width / 2 + GrayMargin, (int)Pos.Y - Heigth / 2 + GrayMargin, 
                (int)((Width - GrayMargin * 2) * Percentage), Heigth - GrayMargin * 2), Color.Green);
        }
    }
}
