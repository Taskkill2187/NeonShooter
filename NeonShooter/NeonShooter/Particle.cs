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
    public enum Disappearing
    {
        Slowly,
        Instant
    }

    public class Particle : Entity
    {
        private float PullLength;
        private float ForceMagnitude;
        public float GravForce;
        public int LifeTime;
        private Vector2 PullVektor;
        private float Angle;
        public bool MarkAsNoLongerNeeded;
        public Disappearing WayOfDisappearing = Disappearing.Slowly;
        public Vector2 Friction = new Vector2(1.04f, 1.04f);
        public bool AffectedByBlackHoles = true;
        public bool Disortion = true;
        public Vector2 Size = new Vector2(1, 1);
        
        public Particle(Vector2 Pos, Vector2 Vel, Color Color)
        {
            this.Pos = Pos;
            this.Vel = Vel;

            Friction = new Vector2(1.05f, 1.05f);
            this.Tex = ImportedFiles.Particle;
            this.Color = Color.Lerp(Color, Color.FromNonPremultiplied(Data.RDM.Next(100, 255), Data.RDM.Next(100, 255), Data.RDM.Next(100, 255), 255), 0.1f);
            this.Radius = 0;
            this.Rotation = 0;
            this.LifeTime = -10 + Data.RDM.Next(20);
            GravForce = 0.5f;
        }
        public Particle(Vector2 Pos, Color BaseColor, float ColorVariation)
        {
            this.Pos = Pos;
            this.Vel = Vector2.Zero;

            this.Tex = ImportedFiles.Particle;
            this.Color = Color.Lerp(BaseColor, Color.FromNonPremultiplied(Data.RDM.Next(256), Data.RDM.Next(256), Data.RDM.Next(256), 255), ColorVariation);
            this.Radius = 0;
            this.Rotation = 0;
            this.LifeTime = -(int)(Vector2.Distance(Pos, (Data.ScreenSize / 2)) / 3 - 105);
            Disortion = true;
        }
        public Particle(Vector2 Pos, Vector2 Vel, Color Color, Vector2 Friction, bool Disortion)
        {
            this.Pos = Pos;
            this.Vel = Vel;

            this.Friction = Friction;
            this.Tex = ImportedFiles.Particle;
            this.Color = Color.Lerp(Color, Color.FromNonPremultiplied(Data.RDM.Next(100, 255), Data.RDM.Next(100, 255), Data.RDM.Next(100, 255), 255), 0.1f);
            this.Radius = 0;
            this.Rotation = 0;
            this.LifeTime = -10 + Data.RDM.Next(20);
            AffectedByBlackHoles = false;
            this.Disortion = Disortion;
        }
        public Particle(Vector2 Pos, Vector2 Vel, Color Color, int LifeTime, float GravitiationForce)
        {
            this.Pos = Pos;
            this.Vel = Vel;

            this.Tex = ImportedFiles.Particle;
            this.Color = Color.Lerp(Color, Color.FromNonPremultiplied(Data.RDM.Next(100, 255), Data.RDM.Next(100, 255), Data.RDM.Next(100, 255), 255), 0.1f);
            this.Radius = 0;
            this.Rotation = 0;
            this.LifeTime = LifeTime;
            this.GravForce = GravitiationForce;
        }
        public Particle(Vector2 Pos, int LifeTime, float Rotation, Texture2D Tex, Color Col, Vector2 Vel, float GravitiationForce, Disappearing WayOfDisappearing, bool Disortion, 
            bool AffectedByBlackHoles, Vector2 Friction)
        {
            this.Pos = Pos;
            this.Vel = Vel;

            this.Tex = Tex;
            this.Color = Col;
            this.Radius = 0;
            this.Rotation = 0;
            this.LifeTime = LifeTime;
            this.WayOfDisappearing = WayOfDisappearing;
            this.GravForce = GravitiationForce;
            this.Disortion = Disortion;
            this.AffectedByBlackHoles = AffectedByBlackHoles;
            this.Friction = Friction;
        }

        public void GetPulledBy(Vector2 Puller, bool Direction)
        {
            PullLength = Vector2.Distance(Puller, Pos);
            PullVektor = Puller - Pos;
            ForceMagnitude = Data.ParticleForceMultiplier / PullLength;
            Angle = (float)Math.Atan2(PullVektor.X, PullVektor.Y);

            if (ForceMagnitude > 0.7f)
            {
                ForceMagnitude = 0.7f;
            }

            ForceMagnitude /= 10;

            if (Direction)
            {
                Vel.X += ForceMagnitude * (float)Math.Sin(Angle);
                Vel.Y += ForceMagnitude * (float)Math.Cos(Angle);
            }
            else
            {
                Vel.X -= ForceMagnitude * (float)Math.Sin(Angle);
                Vel.Y -= ForceMagnitude * (float)Math.Cos(Angle);
            }
        }
        public void GetPulledBy(Vector2 Puller, bool Direction, float Strength)
        {
            PullLength = Vector2.Distance(Puller, Pos);
            PullVektor = Puller - Pos;
            ForceMagnitude = Data.ParticleForceMultiplier / PullLength;
            Angle = (float)Math.Atan2(PullVektor.X, PullVektor.Y);

            if (ForceMagnitude > 0.7f)
            {
                ForceMagnitude = 0.7f;
            }

            ForceMagnitude /= 10;

            ForceMagnitude *= Strength;

            if (Direction)
            {
                Vel.X += ForceMagnitude * (float)Math.Sin(Angle);
                Vel.Y += ForceMagnitude * (float)Math.Cos(Angle);
            }
            else
            {
                Vel.X -= ForceMagnitude * (float)Math.Sin(Angle);
                Vel.Y -= ForceMagnitude * (float)Math.Cos(Angle);
            }
        }
        public void GetPulledBy(Vector2 Puller, bool Direction, float Strength, float ForceFieldSize)
        {
            PullLength = Vector2.Distance(Puller, Pos);
            PullVektor = Puller - Pos;
            ForceMagnitude = Data.ParticleForceMultiplier / (PullLength * ForceFieldSize);
            Angle = (float)Math.Atan2(PullVektor.X, PullVektor.Y);

            if (ForceMagnitude > 0.7f)
            {
                ForceMagnitude = 0.7f;
            }

            ForceMagnitude /= 10;

            ForceMagnitude *= Strength;

            if (Direction)
            {
                Vel.X += ForceMagnitude * (float)Math.Sin(Angle);
                Vel.Y += ForceMagnitude * (float)Math.Cos(Angle);
            }
            else
            {
                Vel.X -= ForceMagnitude * (float)Math.Sin(Angle);
                Vel.Y -= ForceMagnitude * (float)Math.Cos(Angle);
            }
        }

        public void OrbitAround(Vector2 Point)
        {
            PullLength = Vector2.Distance(Point, Pos);
            Vector2 PullVektor = Point - Pos;
            ForceMagnitude = Data.ParticleForceMultiplier / PullLength;
            Angle = (float)Math.Atan2(PullVektor.X, PullVektor.Y) + 3.1f;

            if (ForceMagnitude > 0.7f)
            {
                ForceMagnitude = 0.7f;
            }

            ForceMagnitude /= 2;

            Vel.X -= ForceMagnitude * (float)Math.Sin(Angle);
            Vel.Y -= ForceMagnitude * (float)Math.Cos(Angle);
        }
        public void OrbitAround(Vector2 Point, float Strength)
        {
            PullLength = Vector2.Distance(Point, Pos);
            Vector2 PullVektor = Point - Pos;
            ForceMagnitude = Data.ParticleForceMultiplier / PullLength;
            Angle = (float)Math.Atan2(PullVektor.X, PullVektor.Y) + 3.1f;

            if (ForceMagnitude > 0.7f)
            {
                ForceMagnitude = 0.7f;
            }

            ForceMagnitude /= 2;
            ForceMagnitude *= Strength;

            Vel.X -= ForceMagnitude * (float)Math.Sin(Angle);
            Vel.Y -= ForceMagnitude * (float)Math.Cos(Angle);
        }
        public void OrbitAround(Vector2 Point, float Strength, float DistractionAngle)
        {
            PullLength = Vector2.Distance(Point, Pos);
            Vector2 PullVektor = Point - Pos;
            ForceMagnitude = Data.ParticleForceMultiplier / PullLength;
            Angle = (float)Math.Atan2(PullVektor.X, PullVektor.Y) + (float)Math.PI - DistractionAngle;

            if (ForceMagnitude > 0.7f)
            {
                ForceMagnitude = 0.7f;
            }

            ForceMagnitude /= 2;
            ForceMagnitude *= Strength;

            Vel.X -= ForceMagnitude * (float)Math.Sin(Angle);
            Vel.Y -= ForceMagnitude * (float)Math.Cos(Angle);
        }

        public new void Update()
        {
            //if (Pos.X < 0)
            //{
            //    Vel.X *= -1;
            //    Vel.X += 0.2f;
            //}

            //if (Pos.X > Data.ScreenSize.X)
            //{
            //    Vel.X *= -1;
            //    Vel.X -= 0.2f;
            //}

            //if (Pos.Y < 0)
            //{
            //    Vel.Y *= -1;
            //    Vel.Y += 0.2f;
            //}

            //if (Pos.Y > Data.ScreenSize.Y)
            //{
            //    Vel.Y *= -1;
            //    Vel.Y -= 0.2f;
            //}

            Vel.X /= Friction.X;
            Vel.Y /= Friction.Y;

            LifeTime++;

            Rotation = (float)Math.Atan2(Vel.X, Vel.Y);

            //Vel.Y += GravForce;

            Pos += Vel;
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            if (Disortion) { Size.X = Vel.Length() / 2; }

            if (Tex != null)
            {
                if (WayOfDisappearing == Disappearing.Slowly)
                {
                    spriteBatch.Draw(Tex, Pos, null, Color.FromNonPremultiplied(this.Color.R, this.Color.G, this.Color.B, 255 - LifeTime), -Rotation + 89.5f, 
                        new Vector2(Tex.Width / 2, Tex.Height / 2), Size, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(Tex, Pos, null, Color.FromNonPremultiplied(this.Color.R, this.Color.G, this.Color.B, 255), -Rotation + 89.5f, 
                        new Vector2(Tex.Width / 2, Tex.Height / 2), Size, SpriteEffects.None, 0);
                }
            }
        }
    }
}
