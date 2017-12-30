using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CH17___Solitare
{
    public static class CardBack
    {
        // list of all available card backs
        private static List<Rectangle> m_TextureRects = new List<Rectangle>();
        public static void ClearTextureRects() { m_TextureRects.Clear(); }
        public static void AddTextureRect(Rectangle rect) { m_TextureRects.Add(rect); }

        // which card back images is active?
        private static int m_BackIndex = 0;
        public static int BackIndex
        {
            get { return m_BackIndex; }
            set { m_BackIndex = value; }
        }

        // select the next image
        public static void NextCardBack()
        {
            BackIndex += 1;
            if (BackIndex >= m_TextureRects.Count)
            {
                BackIndex = 0;
            }
        }

        // select the previous image
        public static void PreviousCardBack()
        {
            BackIndex -= 1;
            if (BackIndex < 0)
            {
                BackIndex = m_TextureRects.Count - 1;
            }
        }

        // useful for card back animations, unused in this game
        public static void Update(double elapsed)
        {
        }

        // actually draw the card back
        public static void Draw(SpriteBatch batch, Vector2 position)
        {
            batch.Draw(GameLogic.Texture, position, m_TextureRects[BackIndex], Color.White);
        }
    }
}
