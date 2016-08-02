using System;
using Microsoft.Xna.Framework;

namespace NeonShooter
{
    public class Bullet : Entity
    {
        private float PullLength;
        private float ForceMagnitude;
        private float ForceAngle;
        private Vector2 PullVektor;
        public bool FromPlayer;

        //private int GridPullTimer;

        public Bullet(Vector2 Pos, float Direction, bool FromPlayer)
        {
            this.Pos = Pos;
            
            Tex = ImportedFiles.Bullet;
            Rotation = Direction;
            Radius = 10;
            this.FromPlayer = FromPlayer;

            if (FromPlayer)
            {
                Color = Color.LightBlue;
                Vel = new Vector2((float)Math.Sin(Direction) * Stats.PlayerBulletSpeed, (float)Math.Cos(Direction) * Stats.PlayerBulletSpeed);
            }
            else
            {
                Color = Color.Orange;
                Vel = new Vector2((float)Math.Sin(Direction) * Stats.EnemyBulletSpeed, (float)Math.Cos(Direction) * Stats.EnemyBulletSpeed);
            }
        }

        public void GetPulledBy(Vector2 Puller, bool Direction)
        {
            PullLength = Vector2.Distance(Puller, Pos);
            PullVektor = Puller - Pos;
            ForceMagnitude = Data.ParticleForceMultiplier / PullLength;
            ForceAngle = (float)Math.Atan2(PullVektor.X, PullVektor.Y);

            if (ForceMagnitude > 0.7f)
            {
                ForceMagnitude = 0.7f;
            }

            if (Direction)
            {
                Vel.X += ForceMagnitude * (float)Math.Sin(ForceAngle);
                Vel.Y += ForceMagnitude * (float)Math.Cos(ForceAngle);
            }
            else
            {
                Vel.X -= ForceMagnitude * (float)Math.Sin(ForceAngle);
                Vel.Y -= ForceMagnitude * (float)Math.Cos(ForceAngle);
            }
        }
        public void GetPulledBy(Vector2 Puller, bool Direction, float Strength)
        {
            PullLength = Vector2.Distance(Puller, Pos);
            PullVektor = Puller - Pos;
            ForceMagnitude = Data.ParticleForceMultiplier / PullLength;
            ForceAngle = (float)Math.Atan2(PullVektor.X, PullVektor.Y);

            if (ForceMagnitude > 0.7f)
            {
                ForceMagnitude = 0.7f;
            }

            ForceMagnitude *= Strength;

            if (Direction)
            {
                Vel.X += ForceMagnitude * (float)Math.Sin(ForceAngle);
                Vel.Y += ForceMagnitude * (float)Math.Cos(ForceAngle);
            }
            else
            {
                Vel.X -= ForceMagnitude * (float)Math.Sin(ForceAngle);
                Vel.Y -= ForceMagnitude * (float)Math.Cos(ForceAngle);
            }
        }

        public new void Update()
        {
            Pos += Vel;

            //if (GridPullTimer > 25 && FromPlayer)
            //    GameManager.BackgroundGrid.ApplyForce(Pos, -4f);

            //GridPullTimer++;
        }
    }
}
