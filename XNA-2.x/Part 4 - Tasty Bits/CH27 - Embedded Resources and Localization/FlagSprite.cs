// FlagSprite.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chapter27
{
    public class FlagSprite : SpriteBase 
    {
        public FlagSprite()
        {
            TextureRect = new Rectangle(0, 0, 27, 15);
        }
    }
}
