using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CH16___Top_Down_Scroller
{
    public class Splat : GameSprite
    {
        // pixel-perfect data for splat ... unused, but here to 
        // satisfy the IPixelPerfectSprite interface which was
        // inherited from our base class (GameSprite)
        protected static bool[,] m_OpaqueData = null;
        public override bool[,] OpaqueData
        {
            get { return m_OpaqueData; }
            set { m_OpaqueData = value; }
        }

        // the texture rectangle for the splat image
        public Splat()
        {
            TextureRect = new Rectangle(544, 288, 32, 32);
        }

        // splat's are only on-screen for a brief time
        protected double TimeOnScreen = 0.25;

        // reset this splat's properties
        public void Init()
        {
            TotalElapsed = 0;
            if (OpaqueData == null)
            {
                OpaqueData = PixelPerfectHelper.GetOpaqueData(this);
            }
        }

        // update the splat, manage it's short lifetime
        public override void Update(double elapsed)
        {
            base.Update(elapsed);
            if (TotalElapsed > TimeOnScreen)
            {
                IsActive = false;
            }
        }
    }
}
