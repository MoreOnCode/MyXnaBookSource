using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Codetopia.Input
{
    public class KAGamePadDPad
    {
        public static bool operator !=(KAGamePadDPad state1, KAGamePadDPad state2)
        {
            return !AreSame(state1, state2);
        }

        public static bool operator ==(KAGamePadDPad state1, KAGamePadDPad state2)
        {
            return AreSame(state1, state2);
        }

        public ButtonState Down;
        public ButtonState Left;
        public ButtonState Right;
        public ButtonState Up;

        public override bool Equals(object obj)
        {
            bool same = false;

            if (obj != null && obj is KAGamePadDPad)
            {
                same = AreSame(this, (KAGamePadDPad)obj);
            }

            return same;
        }

        public static bool AreSame(KAGamePadDPad state1, KAGamePadDPad state2)
        {
            return
                state1.Down == state2.Down &&
                state1.Left == state2.Left &&
                state1.Right == state2.Right &&
                state1.Up == state2.Up;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

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
