// GameSprite.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Codetopia.Graphics
{
    public class GameSprite : IPixelPerfectSprite 
    {
        // there's only one game texture, return it
        Texture2D m_TextureData = null;
        public Texture2D TextureData
        {
            get { return m_TextureData; }
            set { m_TextureData = value; }
        }

        // there's only one texture rect, return it
        protected static Rectangle m_TextureRect = new Rectangle(0, 0, 32, 32);
        public Rectangle TextureRect
        {
            get { return m_TextureRect; }
            set { m_TextureRect = value; }
        }

        // location of this sprite
        protected Vector2 m_Location = Vector2.Zero;
        public Vector2 Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        // opaque pixel data for this sprite
        protected bool[,] m_OpaqueData;
        public bool[,] OpaqueData
        {
            get { return m_OpaqueData; }
            set { m_OpaqueData = value; }
        }

        // draw this sprite, using current settings, and specified tint
        public void Draw(SpriteBatch batch, Color color)
        {
            batch.Draw(TextureData, Location, TextureRect, color);
        }
    }
}
