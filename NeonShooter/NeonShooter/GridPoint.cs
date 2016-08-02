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
    public class GridPoint
    {
        public Vector2 Pos, Vel;
        public bool Moveable = true;
        private float Damping = 0.99f;

        public GridPoint(Vector2 Position, bool Moveable)
        {
            Pos = Position;
            Vel = Vector2.Zero;
            this.Moveable = Moveable;
        }

        public void ApplyForce(Vector2 Force) { Vel += Force; }
        public void GetPulledBy(Vector2 Puller, float Force)
        {
            float PullLength = Vector2.Distance(Puller, Pos);
            Vector2 PullVektor = Puller - Pos;
            float ForceMagnitude = 1000 / (PullLength * 600);
            float Angle = (float)Math.Atan2(PullVektor.X, PullVektor.Y);

            if (ForceMagnitude > 0.7f)
            {
                ForceMagnitude = 0.7f;
            }

            ForceMagnitude *= 0.1f * Force;

            Vel.X += ForceMagnitude * (float)Math.Sin(Angle);
            Vel.Y += ForceMagnitude * (float)Math.Cos(Angle);
        }

        public void Update()
        {
            if (Moveable)
            {
                if (Vel.X > 5)
                    Vel.X = 5;

                if (Vel.Y > 5)
                    Vel.Y = 5;

                if (Vel.X < -5)
                    Vel.X = -5;

                if (Vel.Y < -5)
                    Vel.Y = -5;

                Pos += Vel;
                Vel *= Damping;
            }
        }
    }
}
