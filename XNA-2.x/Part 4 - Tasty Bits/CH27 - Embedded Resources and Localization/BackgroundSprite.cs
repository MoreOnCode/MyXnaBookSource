// BackgroundSprite.cs
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace Chapter27
{
    public class BackgroundSprite : SpriteBase 
    {
        // handy constants for scrolling background
        protected const float IDLE_DELAY = 3.0f;
        protected const float RATE_IDLE = (1280.0f - 640.0f) / 120.0f;
        protected const float RATE_MOVE = RATE_IDLE * 8.0f;
        
        // rate and direction of scroll, in pixels per second
        protected Vector2 m_Movement = new Vector2(-RATE_IDLE, 0);

        // initialize our source texture rectangle
        public BackgroundSprite()
        {
            TextureRect = new Rectangle(0, 0, 1280, 480);
        }

        // process player input and update object states
        public sealed override void Update(float elapsed)
        {
            // determine the change in position of the background sprite
            float deltaX = -RATE_MOVE * GamePadState.ThumbSticks.Left.X;
            m_TimeIdle = (deltaX == 0.0f) ? m_TimeIdle + elapsed : 0;

            // update the location
            if (TimeIdle > IDLE_DELAY)
            {
                Location += m_Movement * elapsed;
            }
            else
            {
                m_Location.X += deltaX * elapsed;
            }

            // reverse direction whenever we hit an edge
            if (Location.X > 0 || Location.X < -640)
            {
                m_Movement.X = -m_Movement.X;
                m_Location.X = Math.Min(m_Location.X, 0.0f);
                m_Location.X = Math.Max(m_Location.X, -640.0f);
            }
        }
    }
}
