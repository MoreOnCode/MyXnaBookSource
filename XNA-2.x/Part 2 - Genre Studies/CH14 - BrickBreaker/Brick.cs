// Brick.cs
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter14
{
    public class Brick
    {
        // used by the editor to know when level data has changed
        protected bool m_Changed = false;
        public bool Changed
        {
            get { return m_Changed; }
            set { m_Changed = value; }
        }

        // the left-most coordinate of the brick
        protected int m_X = 0;
        public int X
        {
            get { return m_X; }
            set
            {
                Changed |= (m_X == value);
                m_X = value;
            }
        }

        // the top-most coordinate of the brick
        protected int m_Y = 0;
        public int Y
        {
            get { return m_Y; }
            set
            {
                Changed |= (m_Y == value);
                m_Y = value;
            }
        }

        // number of times this brick must be hit to disappear
        protected int m_HitsToClear = 1;
        public int HitsToClear
        {
            get { return m_HitsToClear; }
            set
            {
                Changed |= (m_HitsToClear == value);
                m_HitsToClear = value;
            }
        }

        // ball hit brick, register the hit
        public void RegisterHit()
        {
            if (HitsToClear > 0)
            {
                HitsToClear--;
                Changed = true;
            }
        }

        // simple property to save some typing
        public bool Active
        {
            get { return HitsToClear > 0; }
        }

        // override to serialize brick data to a file stream
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(X).Append("|")
              .Append(Y).Append("|")
              .Append(HitsToClear).Append("|");
            return sb.ToString();
        }
    }
}
