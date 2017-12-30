using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Codetopia.Input
{
    // a drop-in replacement for the GamePadDPad class
    public class KAGamePadDPad
    {
        // public members to represent the state of each of the 
        // directional buttons on a standard Xbox 360 gamepad
        public ButtonState Down;
        public ButtonState Left;
        public ButtonState Right;
        public ButtonState Up;

        // overridden Equals method; are the current and 
        // specified instances the same?
        public override bool Equals(object obj)
        {
            bool same = false;

            if (obj != null && obj is KAGamePadDPad)
            {
                same = KAGamePadDPad.AreSame(this, (KAGamePadDPad)obj);
            }

            return same;
        }

        // overridden "not equals" operator
        public static bool operator !=
            (KAGamePadDPad state1, KAGamePadDPad state2)
        {
            return !KAGamePadDPad.AreSame(state1, state2);
        }

        // overridden "equals" operator
        public static bool operator ==
            (KAGamePadDPad state1, KAGamePadDPad state2)
        {
            return KAGamePadDPad.AreSame(state1, state2);
        }

        // overridden GetHashCode method
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // are the two specified instances the same?
        public static bool AreSame(KAGamePadDPad state1, KAGamePadDPad state2)
        {
            return
                state1.Down == state2.Down &&
                state1.Left == state2.Left &&
                state1.Right == state2.Right &&
                state1.Up == state2.Up;
        }

        // a string representaion of this instance
        public override string ToString()
        {
            return
                Down.ToString() +
                Left.ToString() +
                Right.ToString() +
                Up.ToString();
        }
    }
}
