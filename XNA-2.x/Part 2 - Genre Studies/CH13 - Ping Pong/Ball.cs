// Ball.cs
// A simple class to encapsulate the game ball

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Chapter13
{
    class Ball : GameObject 
    {
        protected float m_DX;
        public float DX
        {
            get { return m_DX; }
            set { m_DX = value; }
        }

        protected float m_DY;
        public float DY
        {
            get { return m_DY; }
            set { m_DY = value; }
        }
    }
}


