using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Codetopia.Input
{
    public class KAGamePadTriggers
    {
        public static bool operator !=(KAGamePadTriggers state1, KAGamePadTriggers state2)
        {
            return !AreSame(state1, state2);
        }

        public static bool operator ==(KAGamePadTriggers state1, KAGamePadTriggers state2)
        {
            return AreSame(state1, state2);
        }

        public float Left;
        public float Right;

        public override bool Equals(object obj)
        {
            bool same = false;

            if (obj != null && obj is KAGamePadTriggers)
            {
                same = AreSame(this, (KAGamePadTriggers)obj);
            }

            return same;
        }

        public static bool AreSame(KAGamePadTriggers state1, KAGamePadTriggers state2)
        {
            return
                state1.Left == state2.Left &&
                state1.Right == state2.Right;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return
                Left.ToString() +
                Right.ToString();
        }
    }
}
