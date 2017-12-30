using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CH16___Top_Down_Scroller
{
    public class Bullet : GameSprite
    {
        // pixel-perfect collision data
        protected static bool[,] m_OpaqueData = null;
        public override bool[,] OpaqueData
        {
            get { return m_OpaqueData; }
            set { m_OpaqueData = value; }
        }

        // track bullets that players shoot, so that they
        // get credit for hits
        protected Player m_Shooter = null;
        public Player Shooter
        {
            get { return m_Shooter; }
            set { m_Shooter = value; }
        }

        // standard constructor
        public Bullet()
        {
            // location of ship image in master game texture
            TextureRect = new Rectangle(704, 288, 32, 32);

            // bullet velocity
            MovePixelsPerSecond = 200;
        }

        // reset this bullet's properties
        public void Init()
        {
            // if needed, generate pixel-perfect data
            if (OpaqueData == null)
            {
                OpaqueData = PixelPerfectHelper.GetOpaqueData(this);
            }

            // assume this bullet didn't originate from a player
            Shooter = null;
        }

        // direction of bullet, up from player, down from enemy
        protected bool m_MoveUp = true;
        public bool MoveUp
        {
            get { return m_MoveUp; }
            set { m_MoveUp = value; }
        }
    }
}
