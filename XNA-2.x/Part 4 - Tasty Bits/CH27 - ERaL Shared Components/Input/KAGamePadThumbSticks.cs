using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Codetopia.Input
{
    public class KAGamePadThumbSticks
    {
        public static bool operator !=(KAGamePadThumbSticks state1, KAGamePadThumbSticks state2)
        {
            return !AreSame(state1, state2);
        }

        public static bool operator ==(KAGamePadThumbSticks state1, KAGamePadThumbSticks state2)
        {
            return AreSame(state1, state2);
        }

        public Vector2 Left;
        public Vector2 Right;

        public override bool Equals(object obj)
        {
            bool same = false;

            if (obj != null && obj is KAGamePadThumbSticks)
            {
                same = AreSame(this, (KAGamePadThumbSticks)obj);
            }

            return same;
        }

        public static bool AreSame(KAGamePadThumbSticks state1, KAGamePadThumbSticks state2)
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
