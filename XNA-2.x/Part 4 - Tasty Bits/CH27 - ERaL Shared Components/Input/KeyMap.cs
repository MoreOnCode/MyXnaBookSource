using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Codetopia.Input
{
    public class KeyMap
    {
        // store the list of button-to-key mappings
        protected Dictionary<PadButtons, Keys> m_KeyMap = new Dictionary<PadButtons, Keys>();

        public KeyMap() { }

        public bool HasMappings
        {
            get { return m_KeyMap.Count > 0; }
        }

        public void AddMapping(PadButtons button, Keys key)
        {
            m_KeyMap.Add(button,key);
        }

        // given a game pad button, return the keyboard
        // key to which it maps (if any). in the case of
        // PlayerIndex.Three and PlayerIndex.Four, this
        // method will always return a zero.
        public Keys GetKey(PadButtons button)
        {
            Keys key = Keys.None;

            if (m_KeyMap.ContainsKey(button))
            {
                key = m_KeyMap[button];
            }
            return key;
        }

        public ButtonState GetButtonState(
            KeyboardState keyState, 
            ButtonState btnState,
            PadButtons button)
        {
            bool pressed = keyState.IsKeyDown(GetKey(button));
            return pressed || btnState == ButtonState.Pressed ? 
                ButtonState.Pressed : 
                ButtonState.Released;
        }

        public float GetTriggerState(
            KeyboardState keyState,
            float btnState,
            PadButtons button)
        {
            if (btnState == 0.0f)
            {
                if (keyState.IsKeyDown(GetKey(button)))
                {
                    btnState = KeyMapHelper.TRIGGER_MAX;
                }
            }
            return btnState;
        }

        public float GetStickState(
            KeyboardState keyState,
            float btnState,
            PadButtons btnMin,
            PadButtons btnMax)
        {
            if (btnState == 0.0f)
            {
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
