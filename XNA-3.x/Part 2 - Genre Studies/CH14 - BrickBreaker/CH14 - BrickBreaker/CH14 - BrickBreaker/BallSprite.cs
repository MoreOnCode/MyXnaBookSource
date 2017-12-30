using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelPerfect2D;

namespace CH14___BrickBreaker
{
    public class BallSprite : IPixelPerfectSprite
    {
        #region IPixelPerfectSprite Members

        // a reference to the texture where the ball image can be found
        public Texture2D TextureData
        {
            get { return Game1.GameTexture; }
            set { }
        }

        // the rectangular bounds of the ball sprite within the texture
        public Rectangle TextureRect
        {
            get { return Game1.BallRect; }
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
            get { return Game1.BallOpaqueData; }
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

        // the change in location, in pixels per second
        protected Vector2 m_Movement = Vector2.Zero;
        public Vector2 Movement
        {
            get { return m_Movement; }
            set { m_Movement = value; }
        }

        // simple helper to assign a movement vector without having 
        // to create a new Vector2 object
        public void SetMovement(float dx, float dy)
        {
            m_Movement.X = dx;
            m_Movement.Y = dy;
        }

        // update the location of the sprite, check for wall collisions
        public bool Update(double elapsed)
        {
            // is ball still in playing field?
            bool InBounds = true;

            // update the ball's location
            float seconds = (float)elapsed;
            m_Location += seconds * m_Movement;

            // did ball leave playing field to the left?
            if (m_Location.X < Game1.PlayableRect.Left)
            {
                // bring ball back into field, reverse X movement
                m_Location.X = Game1.PlayableRect.Left;
                m_Movement.X *= -1;
            }
            // did ball leave playing field to the right?
            else if (m_Location.X >
                Game1.PlayableRect.Right - TextureRect.Width)
            {
                // bring ball back into field, reverse X movement
                m_Location.X = Game1.PlayableRect.Right - TextureRect.Width;
                m_Movement.X *= -1;
            }

            // did ball leave playing field at the top?
            if (m_Location.Y < Game1.PlayableRect.Top)
            {
                // bring ball back into field, reverse Y movement
                m_Location.Y = Game1.PlayableRect.Top;
                m_Movement.Y *= -1;
            }
            // did ball leave playing field at the bottom?
            else if (m_Location.Y >
                Game1.PlayableRect.Bottom - TextureRect.Height)
            {
                // tell teh main game class that the player messed up
                InBounds = false;
            }

            // return game state: true = ok, false = oops
            return InBounds;
        }
    }
}
