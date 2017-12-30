using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelPerfect2D;

namespace Chapter14
{
    public class PaddleSprite : IPixelPerfectSprite
    {
        #region IPixelPerfectSprite Members

        // a reference to the texture where the paddle image can be found
        public Texture2D TextureData
        {
            get { return Game1.GameTexture; }
            set { }
        }

        // the rectangular bounds of the paddle sprite within the texture
        public Rectangle TextureRect
        {
            get { return Game1.PaddleRect; }
            set { }
        }

        // the current location of this sprite
        protected Vector2 m_Location = Vector2.Zero;
        public Vector2 Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        // pixel-perfect collision detection data
        protected bool[,] m_OpaqueData = null;
        public bool[,] OpaqueData
        {
            get { return Game1.PaddleOpaqueData; }
            set { }
        }

        // the tint color for this sprite
        public Color Tint
        {
            get { return Color.White; }
            set { }
        }

        // is the ball sprite active? always true
        public bool Active
        {
            get { return true; }
            set { }
        }

        #endregion
    }
}
