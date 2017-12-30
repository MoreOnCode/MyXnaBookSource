using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Codetopia.Input
{
    public class KAGamePad
    {
        public static KAGamePadState GetState(PlayerIndex player)
        {
            KAGamePadState state = new KAGamePadState(GamePad.GetState(player));
            KeyMapHelper.ProcessKeyboardInput(player, state);
            return state;
        }

        public static GamePadCapabilities GetCapabilities(PlayerIndex index)
        {
            return GamePad.GetCapabilities(index);
        }

        public static bool SetVibration(PlayerIndex index, float left, float right)
        {
            return GamePad.SetVibration(index, left, right);
        }
    }
}
