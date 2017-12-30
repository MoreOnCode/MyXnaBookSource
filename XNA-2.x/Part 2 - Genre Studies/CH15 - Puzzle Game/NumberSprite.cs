// NumberSprite.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Chapter15
{
    class NumberSprite
    {
        // texture is shared across all instances (static)
        private static Texture2D m_Texture;
        private static Rectangle[] TextureRects = new Rectangle[10];
        public static Texture2D Texture
        {
            get { return m_Texture; }
            set
            {
                // set texture
                m_Texture = value;

                // texture is 10 evenly-spaced numbers
                int widthChar = Texture.Width / 10;
                int heightChar = Texture.Height;
                for (int i = 0; i < 10; i++)
                {
                    TextureRects[i] = new Rectangle(
                        i * widthChar,
                        0,
                        widthChar,
                        heightChar );
                }
            }
        }

        // cache a copy of the last value to save some CPU cycles
        private char[] m_ValueAsText = "0".ToCharArray();
        private long m_Value = 0;
        public long Value
        {
            get { return m_Value; }
            set
            {
                if (m_Value != value)
                {
                    m_Value = value;
                    m_ValueAsText = value.ToString().ToCharArray();
                }
            }
        }

        // actually draw the number (using default White tint)
        public void Draw(SpriteBatch batch, Vector2 position)
        {
            Draw(batch, position, Color.White);
        }

        // actually draw the number
        public void Draw(SpriteBatch batch, Vector2 position, Color tint)
        {
            // draw the number, char-by-char, from cache
            for (int i = 0; i < m_ValueAsText.Length; i++)
            {
                int c = m_ValueAsText[i] - '0';
                batch.Draw(Texture, position, TextureRects[c], tint);
                position.X += TextureRects[c].Width;
            }
        }
    }
}
