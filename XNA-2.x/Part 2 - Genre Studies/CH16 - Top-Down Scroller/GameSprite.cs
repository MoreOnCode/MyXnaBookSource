// GameSprite.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chapter16
{
    // implements IPixelPerfectSprite for use with PixelPerfectHelper methods
    public abstract class GameSprite : IPixelPerfectSprite 
    {
        // declare this property abstract so that
        // the derived game sprites can store their
        // own pixel-perfect data as a static member
        // (shared across instances of the same class)
        // if we declare the static member here, it's
        // shared across all subclasses, if we declare
        // the member here as non-static, there will be
        // a copy of the data (and a call to the helper)
        // for each instance of a game sprite (hundreds)
        public abstract bool[,] OpaqueData
        {
            get;
            set;
        }

        // point back to the one and only game texture
        public Texture2D TextureData
        {
            get { return Game1.Texture; }
            set { }
        }

        // define the bounds of the game sprite instance, 
        // Rectangle.Empty indicates no bounds checking
        protected Rectangle m_ScreenBounds = Rectangle.Empty;
        public Rectangle ScreenBounds
        {
            get { return m_ScreenBounds; }
            set { m_ScreenBounds = value; }
        }

        // get and set the on-screen location of the game sprite
        protected Vector2 m_Location = Vector2.Zero;
        public Vector2 Location
        {
            get { return m_Location; }
            set
            {
                // account for ScreenBounds, if any
                m_Location = value;
                if (ScreenBounds != Rectangle.Empty)
                {
                    if (m_Location.X < ScreenBounds.Left)
                    {
                        m_Location.X = ScreenBounds.Left;
                    }
                    else if (
                        m_Location.X + TextureRect.Width > ScreenBounds.Right)
                    {
                        m_Location.X = ScreenBounds.Right - TextureRect.Width;
                    }
                    if (m_Location.Y < ScreenBounds.Top)
                    {
                        m_Location.Y = ScreenBounds.Top;
                    }
                    else if (
                        m_Location.Y + TextureRect.Height > ScreenBounds.Bottom)
                    {
                        m_Location.Y = ScreenBounds.Bottom - TextureRect.Height;
                    }
                }
            }
        }

        // the texture rectangle for this sprite's image
        protected Rectangle m_TextureRect = Rectangle.Empty;
        public Rectangle TextureRect
        {
            get { return m_TextureRect; }
            set { m_TextureRect = value; }
        }

        // the tint for this sprite
        protected Color m_Color = Color.White;
        public Color Color
        {
            get { return m_Color; }
            set { m_Color = value; }
        }

        // indicates whether this sprite is on the screen or not
        protected bool m_IsActive = false;
        public bool IsActive
        {
            get { return m_IsActive; }
            set { m_IsActive = value; }
        }

        // velocity of this sprite
        protected double m_MovePixelsPerSecond = 100;
        public double MovePixelsPerSecond
        {
            get { return m_MovePixelsPerSecond; }
            set { m_MovePixelsPerSecond = value; }
        }

        // helper for sprites to generate random numbers
        protected static Random m_rand = new Random();

        protected double TotalElapsed = 0;
        public virtual void Update(double elapsed)
        {
            TotalElapsed += elapsed;
        }

        // draw this sprite on the screen, if it's active
        public virtual void Draw(SpriteBatch batch)
        {
            if (IsActive)
            {
                batch.Draw(Game1.Texture, Location, TextureRect, Color);
            }
        }
    }
}
