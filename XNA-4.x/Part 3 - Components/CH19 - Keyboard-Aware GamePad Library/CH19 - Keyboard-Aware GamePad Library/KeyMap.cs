using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Codetopia.Input
{
    // used by the KeyMapHelper to map keyboard keys to 
    // gamepad buttons for a given player
    public class KeyMap
    {
        // store the list of button-to-key mappings
        protected Dictionary<PadButtons, Keys> m_KeyMap = 
            new Dictionary<PadButtons, Keys>();

        // default, empty constructor
        public KeyMap() { }

        // does this instance of the KeyMap class contain
        // any keyboard mappings? In the case of Player.Three
        // and Player.Four, the answer is always "no" (false).
        public bool HasMappings
        {
            get { return m_KeyMap.Count > 0; }
        }

        // map a keyboard key to a gamepad button
        public void AddMapping(PadButtons button, Keys key)
        {
            // make sure the button isn't already mapped
            if (m_KeyMap.ContainsKey(button))
            {
                m_KeyMap.Remove(button);
            }

            // map the button to the key
            m_KeyMap.Add(button,key);
        }

        // given a gamepad button, return the keyboard
        // key to which it maps (if any). in the case of
        // PlayerIndex.Three and PlayerIndex.Four, this
        // method will always return a zero (Keys.None).
        public Keys GetKey(PadButtons button)
        {
            Keys key = Keys.None;

            if (m_KeyMap.ContainsKey(button))
            {
                key = m_KeyMap[button];
            }
            return key;
        }

        // get the state of a mapped digital button
        public ButtonState GetButtonState(
            KeyboardState keyState, 
            ButtonState btnState,
            PadButtons button)
        {
            // is the mapped key pressed?
            bool keyPressed = keyState.IsKeyDown(GetKey(button));
            // is the actual gamepad button pressed?
            bool btnPressed = (btnState == ButtonState.Pressed);

            // if either is pressed, treat the virtual button as 
            // pressed; otherwise, report the state as released
            return keyPressed || btnPressed ? 
                ButtonState.Pressed : 
                ButtonState.Released;
        }

        // get the state of a mapped trigger
        public float GetTriggerState(
            KeyboardState keyState,
            float btnState,
            PadButtons button)
        {
            // if the trigger on the controller isn't 
            // being used, check the keyboard
            if (btnState == 0.0f)
            {
                // since the keyboard is digital, and the 
                // trigger is analog, map pressed keys to 
                // the extreme trigger value
                if (keyState.IsKeyDown(GetKey(button)))
                {
                    btnState = KeyMapHelper.TRIGGER_MAX;
                }
            }
            return btnState;
        }

        // get the state of a mapped thumbstick
        public float GetStickState(
            KeyboardState keyState,
            float btnState,
            PadButtons btnMin,
            PadButtons btnMax)
        {
            // if the thumbstick on the controller isn't 
            // being used, check the keyboard
            if (btnState == 0.0f)
            {
                // since the keyboard is digital, and the 
                // thumbstick is analog, map pressed keys 
                // to one of the extreme thumbstick values
                if (keyState.IsKeyDown(GetKey(btnMin)))
                {
                    btnState = KeyMapHelper.THUMBSTICK_MIN;
                }
                else if (keyState.IsKeyDown(GetKey(btnMax)))
                {
                    btnState = KeyMapHelper.THUMBSTICK_MAX;
                }
            }
            return btnState;
        }
    }
}
