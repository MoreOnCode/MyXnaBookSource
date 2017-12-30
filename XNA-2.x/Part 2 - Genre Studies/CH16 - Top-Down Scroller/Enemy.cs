// Enemy.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chapter16
{
    public class Enemy : GameSprite
    {
        // pixel-perfect collision data
        protected static bool[,] m_OpaqueData = null;
        public override bool[,] OpaqueData
        {
            get { return m_OpaqueData; }
            set { m_OpaqueData = value; }
        }

        // standard constructor
        public Enemy()
        {
            TextureRect = new Rectangle(576,288,32,32);
            Color = Color.White;
            LastShot = 0;
        }

        // number of seconds between shots
        protected double FireDelay = 1;

        // number of shots fired (see the Update method)
        protected double LastShot = 0;

        // which pre-determined path is this enemy following?
        protected int Path = 0;

        // reset this enemy's properties, randomly choosing a path
        public void Init()
        {
            // start at {-32, -32} on screen
            Location = -32 * Vector2.One;

            // new object, no time has elapsed
            TotalElapsed = 0;

            // don't fire right away, wait for FireDelay seconds
            LastShot = 0;

            // randomly select a path from the six pre-sets
            Path = m_rand.Next(6);
            if (Path == 4 || Path == 5)
            {
                // special paths, shoot a little faster
                FireDelay = 0.66;
            }
            else
            {
                // typically wait one second between shots
                FireDelay = 1;
            }

            // if needed, generate pixel-perfect data
            if (OpaqueData == null)
            {
                OpaqueData = PixelPerfectHelper.GetOpaqueData(this);
            }
        }

        // move enemy along its path, firing every now and then
        public override void Update(double elapsed)
        {
            base.Update(elapsed);

            // since TotalElapsed keeps growing, we need to note each
            // time FireDelay seconds have passed
            double shoot = Math.Floor(TotalElapsed / FireDelay);
            if (shoot > LastShot)
            {
                // fire a shot!
                GameObjectManager.AddBullet(this, false);

                // remember how many shots have been fired
                LastShot = shoot;
            }

            // move enemy along his chosen path
            switch (Path)
            {
                case 0:
                    // swing left, then back right, always moving down
                    m_Location.X = 320 + (float)Math.Cos(TotalElapsed) * 300;
                    m_Location.Y = 
                        (float)(TotalElapsed * MovePixelsPerSecond - 30);
                    break;
                case 1:
                    // swing right, then back left, always moving down
                    m_Location.X = 320 - (float)Math.Cos(TotalElapsed) * 300;
                    m_Location.Y = 
                        (float)(TotalElapsed * MovePixelsPerSecond - 30);
                    break;
                case 2:
                    // move diagonally down and right
                    m_Location.X = 
                        (float)(TotalElapsed * MovePixelsPerSecond - 30);
                    m_Location.Y = 
                        (float)(TotalElapsed * MovePixelsPerSecond - 30);
                    break;
                case 3:
                    // move diagonally down and left
                    m_Location.X = 
                        610 - (float)(TotalElapsed * MovePixelsPerSecond - 30);
                    m_Location.Y = 
                        (float)(TotalElapsed * MovePixelsPerSecond - 30);
                    break;
                case 4:
                    // move horizontally left, near top of screen
                    m_Location.X = 
                        610 - (float)(TotalElapsed * MovePixelsPerSecond - 30);
                    m_Location.Y = 64;
                    break;
                case 5:
                    // move horizontally right, near top of screen
                    m_Location.X = 
                        (float)(TotalElapsed * MovePixelsPerSecond - 30);
                    m_Location.Y = 64;
                    break;
            }
        }
    }
}
