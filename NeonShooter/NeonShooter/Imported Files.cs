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
    public static class ImportedFiles
    {
        public static Texture2D Player;
        public static Texture2D Bullet;
        public static Texture2D Pointer;
        public static Texture2D Seeker;
        public static Texture2D Wanderer;
        public static Texture2D Particle;
        public static Texture2D BlackHole;
        public static Texture2D PureWhite;
        public static Texture2D BombExplosionAnim;
        public static Texture2D DMG_Indikator;
        public static Texture2D Bomb;

        public static Song Derezzed;
        public static Song TheGrid;

        public static SoundEffect Shoot;
        public static SoundEffect Spawn;
        public static SoundEffect Explosion;
        public static SoundEffect PlayerExplosion;
        public static SoundEffect Warp;
        public static SoundEffect SeismicBomb;
        public static SoundEffect Beam;

        public static SpriteFont BigFont;
        public static SpriteFont SmallFont;

        public static Effect ChromaticAberration;
        public static Effect SimpleBlur;

        public static void Load(ContentManager Content, GraphicsDevice graphics)
        {
            Player = Content.Load<Texture2D>("Player");
            Bullet = Content.Load<Texture2D>("Bullet");
            Pointer = Content.Load<Texture2D>("Pointer");
            Seeker = Content.Load<Texture2D>("Seeker");
            Wanderer = Content.Load<Texture2D>("Wanderer");
            Derezzed = Content.Load<Song>("InGameMusic");
            Shoot = Content.Load<SoundEffect>("shoot-04");
            Spawn = Content.Load<SoundEffect>("spawn-08");
            Explosion = Content.Load<SoundEffect>("explosion-05");
            BigFont = Content.Load<SpriteFont>("BigFont");
            SmallFont = Content.Load<SpriteFont>("SmallFont");
            PlayerExplosion = Content.Load<SoundEffect>("explosion-04");
            TheGrid = Content.Load<Song>("MenuMusic");
            Warp = Content.Load<SoundEffect>("spawn-01");
            BlackHole = Content.Load<Texture2D>("Black Hole");
            ChromaticAberration = Content.Load<Effect>("ChromaticAberration");
            SimpleBlur = Content.Load<Effect>("SimpleBlur");
            BombExplosionAnim = Content.Load<Texture2D>("BombExplosionAnim");
            DMG_Indikator = Content.Load<Texture2D>("DMG_Indikator");
            SeismicBomb = Content.Load<SoundEffect>("SeismicBombInSPAAACE");
            Bomb = Content.Load<Texture2D>("Bomb");
            Beam = Content.Load<SoundEffect>("Beam");

            Particle = new Texture2D(graphics, 4, 2, false, SurfaceFormat.Color);
            Color[] Col = new Color[8];
            Col[0] = new Color(255, 255, 255, 255);
            Col[1] = new Color(255, 255, 255, 255);
            Col[2] = new Color(255, 255, 255, 255);
            Col[3] = new Color(255, 255, 255, 255);
            Col[4] = new Color(255, 255, 255, 255);
            Col[5] = new Color(255, 255, 255, 255);
            Col[6] = new Color(255, 255, 255, 255);
            Col[7] = new Color(255, 255, 255, 255);
            Particle.SetData<Color>(Col);

            PureWhite = new Texture2D(graphics, 1, 1, false, SurfaceFormat.Color);
            Col = new Color[1];
            Col[0] = new Color(255, 255, 255, 255);
            PureWhite.SetData<Color>(Col);
        }

        public static void DrawLine(Vector2 End1, Vector2 End2, int Thickness, Color Col, SpriteBatch SB)
        {
            Vector2 Line = End1 - End2;
            SB.Draw(Particle, End1, null, Col, -(float)Math.Atan2(Line.X, Line.Y) - (float)Math.PI / 2, new Vector2(0, 0.5f), new Vector2(Line.Length(), Thickness), SpriteEffects.None, 0f);
        }

        public static void DrawCircle(Vector2 Pos, float Radius, Color Col, SpriteBatch SB)
        {
            for (int i = -(int)Radius; i < (int)Radius; i++)
            {
                int HalfHeight = (int)Math.Sqrt(Radius * Radius - i * i);
                SB.Draw(Particle, new Rectangle((int)Pos.X + i, (int)Pos.Y - HalfHeight, 1, HalfHeight * 2), Col);
            }
        }

        public static void DrawCircleCoolDownTimer(Vector2 Pos, float Radius, Color Col, SpriteBatch SB, float Percentage)
        {
            for (int i = 0; i < (int)Radius; i++)
            {
                int HalfHeight = (int)Math.Sqrt(Radius * Radius - i * i);
                int StripHeight = HalfHeight * 2;
                Vector2 Gradient = new Vector2((float)Math.Cos((Math.PI * 2) / Percentage), (float)Math.Sin((Math.PI * 2) / Percentage));
                SB.Draw(Particle, new Rectangle((int)Pos.X + i, (int)Pos.Y + HalfHeight, 1, StripHeight), Col);
            }

            if (Percentage > 0.5f)
            {
                for (int i = -(int)Radius; i < 0; i++)
                {
                    int HalfHeight = (int)Math.Sqrt(Radius * Radius - i * i);
                    int StripHeight = HalfHeight * 2;
                    SB.Draw(Particle, new Rectangle((int)Pos.X + i, (int)Pos.Y - HalfHeight, 1, StripHeight), Col);
                }
            }
        }
    }
}
