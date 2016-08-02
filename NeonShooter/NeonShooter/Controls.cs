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
    public static class Controls
    {
        public static MouseState CurMS;
        public static MouseState LastMS;
        public static KeyboardState CurKS;
        public static KeyboardState LastKS;

        public static void Update()
        {
            LastKS = CurKS;
            LastMS = CurMS;

            CurMS = Mouse.GetState();
            CurKS = Keyboard.GetState();
        }

        public static bool WasKeyJustPressed(Keys K) { return CurKS.IsKeyDown(K) && LastKS.IsKeyUp(K); }
        public static Vector2 GetMouseVector() { return new Vector2(CurMS.X, CurMS.Y); }
    }
}
