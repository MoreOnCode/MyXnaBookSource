// SpriteBase.cs
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Codetopia.Input;

namespace Chapter27
{
    public class SpriteBase
    {
        // the texture for this sprite
        protected Texture2D m_Texture = null;
        public Texture2D Texture
        {
            get { return m_Texture; }
            set { 
                m_Texture = value;
                if (TextureRect == Rectangle.Empty)
                {
                    TextureRect = 
                        new Rectangle(0, 0, Texture.Width, Texture.Height);
                }
            }
        }

        // the texture coordinates for this sprite
        protected Rectangle m_TextureRect = Rectangle.Empty;
        public Rectangle TextureRect
        {
            get { return m_TextureRect; }
            set { m_TextureRect = value; }
        }

        // on-screen location of this sprite
        protected Vector2 m_Location = Vector2.Zero;
        public Vector2 Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        // the color to tint this sprite with
        protected Color m_Tint = Color.White;
        public Color Tint
        {
            get { return m_Tint; }
            set { m_Tint = value; }
        }

        // number of seconds that the player hasn't used the controller
        protected double m_TimeIdle = 0;
        public double TimeIdle
        {
            get { return m_TimeIdle; }
            set { m_TimeIdle = value; }
        }

        // number of seconds that the player hasn't used the controller
        protected double m_TimeDelay = 0;
        public double TimeDelay
        {
            get { return m_TimeDelay; }
            set { m_TimeDelay = value; }
        }

        // the current state of the keyboard-aware game pad
        protected static KAGamePadState m_GamePadState = null;
        public static KAGamePadState GamePadState
        {
            get { return m_GamePadState; }
            set { m_GamePadState = value; }
        }

        // track controller idle time, used be some sprite subclasses
        public virtual void Update(float elapsed)
        {
            m_TimeIdle = GamePadState.IsIdle ? m_TimeIdle + elapsed : 0;
        }

        // actually draw the sprite
        public virtual void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture, Location, TextureRect, Tint);
        }

        // draw sprite with offset, used to shake avatar and flag
        public virtual void Draw(SpriteBatch batch, Vector2 delta)
        {
            batch.Draw(Texture, Location + delta, TextureRect, Tint);
        }
    }
}
