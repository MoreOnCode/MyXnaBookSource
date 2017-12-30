using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelPerfect2D;

namespace CH14___BrickBreaker
{
    public class BrickSprite : IPixelPerfectSprite
    {

        // create references for each of our brick colors
        public static readonly Color[] m_Tint = 
        {
            new Color(255,128,128), // 1 hit
            new Color(128,255,128), // 2 hits
            new Color(128,128,255), // 3 hits
            new Color(255,128,255), // 4 hits
            new Color(255,255,128), // 5 hits
            new Color(255,194,129), // 6 hits
            new Color(192,192,192), // 7 hits
            new Color(255,192,192), // 8 hits
            new Color(192,255,255), // 9 hits
        };

        // the color of the brick, based on the remaining number of hits
        public Color Tint
        {
            get { return m_Tint[m_Brick.HitsToClear - 1]; }
            set { }
        }

        // the instance of the Brick class to which this sprite is associated
        protected Brick m_Brick = null;
        public Brick Brick
        {
            get { return m_Brick; }
            set
            {
                // make a copy of the brick; preserving the original
                // brick data so that it can be rused if this level 
                // is played again
                m_Brick = new Brick();
                m_Brick.X = value.X;
                m_Brick.Y = value.Y;
                m_Brick.HitsToClear = value.HitsToClear;
                m_Brick.Changed = false;

                // update the sprite location, based on brick location
                m_Location.X = Game1.PlayableRect.Left + m_Brick.X;
                m_Location.Y = Game1.PlayableRect.Top + m_Brick.Y;
            }
        }

        // is the associated brick active?
        public bool Active
        {
            get { return m_Brick != null && m_Brick.Active; }
            set { }
        }

        #region IPixelPerfectSprite Members

        // a reference to the texture where the brick image can be found
        public Texture2D TextureData
        {
            get { return Game1.GameTexture; }
            set { }
        }

        // the rectangular bounds of the brick sprite within the texture
        public Rectangle TextureRect
        {
            get { return Game1.BrickRect; }
            set { }
        }

        // the current location of this sprite
        protected Vector2 m_Location = Vector2.Zero;
        public Vector2 Location
        {
            get
            {
                return m_Location;
            }
            set { }
        }

        // pixel-perfect collision detection data
        public bool[,] OpaqueData
        {
            get { return Game1.BrickOpaqueData; }
            set { }
        }

        #endregion
    }
}
