// Player.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chapter16
{
    public class Player : GameSprite 
    {
        // pixel-perfect collision data
        protected static bool[,] m_OpaqueData = null;
        public override bool[,] OpaqueData
        {
            get { return m_OpaqueData; }
            set { m_OpaqueData = value; }
        }

        // standard constructor
        public Player()
        {
            // location of ship image in master game texture
            TextureRect = new Rectangle(640, 288, 32, 32);

            // player movement is restricted to the bottom 2/3 of screen
            ScreenBounds = new Rectangle(32, 200, 640 - 64, 480 - 232);

        }

        // location to draw the player's score and stats
        protected Vector2 m_HudPosition = Vector2.Zero;
        public Vector2 HudPosition
        {
            get { return m_HudPosition; }
            set { m_HudPosition = value; }
        }

        // location of player ship when new game starts
        protected Vector2 m_StartPosition = Vector2.Zero;
        public Vector2 StartPosition
        {
            get { return m_StartPosition; }
            set { m_StartPosition = value; }
        }

        // player's health, 0 = one more hit will kill you, 3 = full health
        protected int m_Health = 0;
        public int Health
        {
            get { return m_Health; }
            set { m_Health = value > 3 ? 3 : value; }
        }

        // player's shield, 0 = no protection, 3 = full protection
        protected int m_Shield = 0;
        public int Shield
        {
            get { return m_Shield; }
            set { m_Shield = value > 3 ? 3 : value; }
        }

        // class to hold and draw the current score
        public NumberSprite NumberSprite = new NumberSprite();
        public long Score
        {
            get { return NumberSprite.Value; }
            set { NumberSprite.Value = value; }
        }

        // number of seconds to wait between shots
        protected double m_FireDelay = 1.5;
        public double FireDelay
        {
            get { return m_FireDelay; }
            set { m_FireDelay = value; }
        }

        // player is starting a new game, reset properties
        public void Init()
        {
            // set initial ship location
            Location = StartPosition;
            
            // reset score and other stats
            Score = 0;
            Health = 3;
            Shield = 3;
            FireDelay = 1.5;

            // if needed, generate pixel-perfect data for the player
            if (OpaqueData == null)
            {
                OpaqueData = PixelPerfectHelper.GetOpaqueData(this);
            }
        }

        // allow player to fire another bullet immediately
        public void ResetFireDelay()
        {
            TotalElapsed = FireDelay;
        }

        // can this player fire a bullet yet?
        public bool Fire()
        {
            bool retval = false;
            // have FireDelay seconds passed since last shot?
            if (TotalElapsed > FireDelay)
            {
                // reset counter, and allow shot
                TotalElapsed = 0;
                retval = true;
            }
            return retval;
        }

        // player was hit, process it
        public void TakeHit()
        {
            if (Shield > 0)
            {
                // take from shield first, if available
                Shield -= 1;
            }
            else if (Health > 0)
            {
                // take from shield next, if available
                Health -= 1;
            }
            else
            {
                // otherwise, game is over for this player
                IsActive = false;
            }
        }
    }
}
