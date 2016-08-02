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
    public delegate void ClickEventHandler();

    public class Button
    {
        public event EventHandler OnClick;

        public Texture2D Tex;
        public Texture2D TexPressed;

        public Rectangle Rect;
        public Color Color;
        private string text;
        public string Text
        {
            set { text = value; UpdatePos(); }
            get { return text; }
        }
        public SpriteFont Font = ImportedFiles.BigFont;
        public Vector2 TopRight;
        public Vector2 Center;

        public Button(string Text, Rectangle rectangle, Color color)
        {
            Tex = null;
            TexPressed = null;
            this.Text = Text;
            Rect = rectangle;
            Color = color;
            Font = ImportedFiles.BigFont;
            TopRight = new Vector2(Rect.X, Rect.Y);
            Center = TopRight + Font.MeasureString(Text) / 2;
            UpdatePos();
        }
        public Button(string Text, Rectangle rectangle, Texture2D Texture, Texture2D TexturePressed)
        {
            Tex = Texture;
            TexPressed = TexturePressed;
            this.Text = Text;
            Rect = rectangle;
            Font = ImportedFiles.BigFont;
            TopRight = new Vector2(Rect.X, Rect.Y);
            Center = TopRight + Font.MeasureString(Text) / 2;
            UpdatePos();
        }
        public Button(string Text, Vector2 Center, Color color, SpriteFont Font)
        {
            Tex = null;
            TexPressed = null;
            this.Text = Text;
            Color = color;
            this.Font = Font;
            this.Center = Center;
            UpdatePos();
        }

        public void UpdatePos()
        {
            TopRight = new Vector2(Center.X - Font.MeasureString(Text).X / 2, Center.Y - Font.MeasureString(Text).Y / 2);
            Rect = new Rectangle((int)TopRight.X, (int)TopRight.Y, (int)Font.MeasureString(Text).X, (int)Font.MeasureString(Text).Y);
        }
        public void ResetOnClickEvent()
        {
            OnClick = null;
        }

        public void Update()
        {
            if (Controls.CurMS.LeftButton == ButtonState.Released && Controls.LastMS.LeftButton == ButtonState.Pressed && Rect.Intersects(new Rectangle(Controls.CurMS.X, Controls.CurMS.Y, 1, 1)))
            {
                if (OnClick != null)
                {
                    if (Save.Default.Sound)
                    {
                        ImportedFiles.Spawn.Play(0.2f, -0.4f, 0);
                    }
                    Data.TriggerFullScreenShaderEffects();
                    OnClick(this, EventArgs.Empty);
                }
            }
            TopRight = new Vector2(Center.X - Font.MeasureString(Text).X / 2, Center.Y - Font.MeasureString(Text).Y / 2);
            Rect = new Rectangle((int)TopRight.X, (int)TopRight.Y, (int)Font.MeasureString(Text).X, (int)Font.MeasureString(Text).Y);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Controls.CurMS.LeftButton == ButtonState.Pressed && Rect.Intersects(new Rectangle(Controls.CurMS.X, Controls.CurMS.Y, 1, 1)))
            {
                if (TexPressed != null)
                {
                    spriteBatch.Draw(TexPressed, Rect, Color.White);
                }
                spriteBatch.DrawString(Font, Text, TopRight, Color);
            }
            else
            {
                if (Tex != null)
                {
                    spriteBatch.Draw(Tex, Rect, Color.White);
                }
                spriteBatch.DrawString(Font, Text, TopRight, Color);
            }
        }
    }
}
