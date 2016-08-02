using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace NeonShooter
{
    public static class FPSCounter
    {
        public static int frameRate = 0;
        public static int frameCounter = 0;
        public static TimeSpan elapsedTime = TimeSpan.Zero;

        public static void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            frameCounter++;

            string fps = string.Format("FPS: " + frameRate);

            if (ImportedFiles.SmallFont != null)
                spriteBatch.DrawString(ImportedFiles.SmallFont, fps, new Vector2(Data.ScreenSize.X - 12 - ImportedFiles.SmallFont.MeasureString(fps).X, 12), Color.White);
        }
    }
}
