// KAGamePad.cs
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Codetopia.Input
{
    // a drop-in replacement for the GamePad class 
    // which maps keyboard keys to gamepad buttons
    public class KAGamePad
    {
        // private constructor to prevent caller from 
        // creating their own instances
        protected KAGamePad() { }

        // get the state of the virtual gamepad
        public static KAGamePadState GetState(PlayerIndex player)
        {
            // create the gamepad state instance
            KAGamePadState state = 
                new KAGamePadState(GamePad.GetState(player));
            
            // append the state of mapped keyboard keys (if any)
            KeyMapHelper.ProcessKeyboardInput(player, state);

            // return the composite state
            return state;
        }

        // pass through for querrying the underlying gamepad
        public static GamePadCapabilities GetCapabilities(PlayerIndex index)
        {
            return GamePad.GetCapabilities(index);
        }

        // pass through for controlling the rumble 
        // motors of the underlying gamepad
        public static bool SetVibration(
            PlayerIndex index, float left, float right)
        {
            return GamePad.SetVibration(index, left, right);
        }
    }
}
