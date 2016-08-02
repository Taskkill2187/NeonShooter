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
    public class GridSpring
    {
        public GridPoint End1;
        public GridPoint End2;

        public GridSpring(GridPoint End1, GridPoint End2)
        {
            this.End1 = End1;
            this.End2 = End2;
        }

        public void Update()
        {
            End1.ApplyForce((End2.Pos - End1.Pos) / 25);
            End2.ApplyForce((End1.Pos - End2.Pos) / 25);
        }

        public void Draw(SpriteBatch SB)
        {
            ImportedFiles.DrawLine(End1.Pos, End2.Pos, 1, Color.OrangeRed * 0.2f, SB);
        }
    }
}
