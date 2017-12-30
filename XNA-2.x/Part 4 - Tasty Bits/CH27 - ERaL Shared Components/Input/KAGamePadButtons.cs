using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Codetopia.Input
{
    public class KAGamePadButtons
    {
        public static bool operator !=(KAGamePadButtons state1, KAGamePadButtons state2)
        {
            return !AreSame(state1, state2);
        }

        public static bool operator ==(KAGamePadButtons state1, KAGamePadButtons state2)
        {
            return AreSame(state1, state2);
        }

        public ButtonState A;
        public ButtonState B;
        public ButtonState Back;
        public ButtonState LeftShoulder;
        public ButtonState LeftStick;
        public ButtonState RightShoulder;
        public ButtonState RightStick;
        public ButtonState Start;
        public ButtonState X;
        public ButtonState Y;

        public override bool Equals(object obj)
        {
            bool same = false;

            if (obj != null && obj is KAGamePadButtons)
            {
                same = AreSame(this, (KAGamePadButtons)obj);
            }

            return same;
        }

        public static bool AreSame(KAGamePadButtons state1, KAGamePadButtons state2)
        {
            return
                state1.A == state2.A &&
                state1.B == state2.B &&
                state1.Back == state2.Back &&
                state1.LeftShoulder == state2.LeftShoulder &&
                state1.LeftStick == state2.LeftStick &&
                state1.RightShoulder == state2.RightShoulder &&
                state1.RightStick == state2.RightStick &&
                state1.Start == state2.Start &&
                state1.X == state2.X &&
                state1.Y == state2.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return
                A.ToString() +
                B.ToString() +
                Back.ToString() +
                LeftShoulder.ToString() +
                LeftStick.ToString() +
                RightShoulder.ToString() +
                RightStick.ToString() +
                Start.ToString() +
                X.ToString() +
                Y.ToString();
        }
    }
}
