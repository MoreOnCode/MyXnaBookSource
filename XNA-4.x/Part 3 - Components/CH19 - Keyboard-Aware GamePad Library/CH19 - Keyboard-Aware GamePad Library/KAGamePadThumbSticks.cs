using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Codetopia.Input
{
    // a drop-in replacement for the GamePadThumbsticks class
    public class KAGamePadThumbSticks
    {
        // public members to represent the state of each of the 
        // thumbsticks on a standard Xbox 360 gamepad
        public Vector2 Left;
        public Vector2 Right;

        // overridden "not equals" operator
        public static bool operator !=
            (KAGamePadThumbSticks state1, KAGamePadThumbSticks state2)
        {
            return !AreSame(state1, state2);
        }

        // overridden "equals" operator
        public static bool operator ==
            (KAGamePadThumbSticks state1, KAGamePadThumbSticks state2)
        {
            return AreSame(state1, state2);
        }

        // overridden Equals method; are the current and 
        // specified instances the same?
        public override bool Equals(object obj)
        {
            bool same = false;

            if (obj != null && obj is KAGamePadThumbSticks)
            {
                same = AreSame(this, (KAGamePadThumbSticks)obj);
            }

            return same;
        }

        // are the two specified instances the same?
        public static bool AreSame(
            KAGamePadThumbSticks state1, KAGamePadThumbSticks state2)
        {
            return
                state1.Left == state2.Left &&
                state1.Right == state2.Right;
        }

        // overridden GetHashCode method
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // a string representaion of this instance
        public override string ToString()
        {
            return
                Left.ToString() +
                Right.ToString();
        }
    }
}
