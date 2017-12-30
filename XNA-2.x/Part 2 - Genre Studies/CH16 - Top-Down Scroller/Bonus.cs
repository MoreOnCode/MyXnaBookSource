// Bonus.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chapter16
{
    public class Bonus : GameSprite 
    {
        // pixel-perfect data, populated by PixelPerfectHelper
        protected static bool[,] m_OpaqueData = null;
        public override bool[,] OpaqueData
        {
            get { return m_OpaqueData; }
            set { m_OpaqueData = value; }
        }

        // the type of bonus this object represents
        public enum Type
        {
            Health,
            Shield,
            Weapon,
            Score,
            // unused, easy way to count enums
            Count,
        }

        // the type of bonus that this instance represents
        protected Type m_BonusType = Type.Score;
        public Type BonusType
        {
            get { return m_BonusType; }
            set { m_BonusType = value; }
        }

        // generic constructor
        public Bonus()
        {
            TextureRect = new Rectangle(608,288,32,32);
            MovePixelsPerSecond = 50;
        }

        // initialize this bonus' properties using a random bonus type
        public void Init()
        {
            if (m_rand.NextDouble() < 0.75)
            {
                // 75% of the time, just throw out a score bonus
                Init(Type.Score);
            }
            else
            {
                // 25% of the time, randomly select a bonus
                int next = m_rand.Next((int)Type.Count);
                Init((Type)next);
            }
        }

        // initialize this bonus' properties using the specified bonus type
        public void Init(Type type)
        {
            // remember the type
            BonusType = type;

            // set the tint color for this type
            switch (type)
            {
                case Type.Health:
                    Color = Color.OrangeRed;
                    break;
                case Type.Shield:
                    Color = Color.LawnGreen;
                    break;
                case Type.Weapon:
                    Color = Color.Gold;
                    break;
                case Type.Score:
                    Color = Color.RoyalBlue;
                    break;
            }

            // if needed, generate pixel-perfect data
            if (OpaqueData == null)
            {
                OpaqueData = PixelPerfectHelper.GetOpaqueData(this);
            }
        }
    }
}
