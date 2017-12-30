// KeyMapper.cs
// Maintain a list of key mappings for use when
// emulating a game pad

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Codetopia.Input
{
    public class KeyMapHelper
    {
        public const float TRIGGER_MAX = 1.00f;
        public const float TRIGGER_MIN = 0.00f;
        public const float THUMBSTICK_MAX = 1.00f;
        public const float THUMBSTICK_MIN = -1.00f;

        protected static Dictionary<PlayerIndex, KeyMap> m_KeyMaps = null;
        protected static Dictionary<PlayerIndex, KeyMap> KeyMaps
        {
            get
            {
                if (m_KeyMaps == null)
                {
                    UseDefaultMappings();
                }
                return m_KeyMaps;
            }
        }

        protected static void InitKeyMaps()
        {
            m_KeyMaps = new Dictionary<PlayerIndex, KeyMap>();
            m_KeyMaps.Add(PlayerIndex.One, new KeyMap());
            m_KeyMaps.Add(PlayerIndex.Two, new KeyMap());
        }

        public const Keys UNKNOWN_KEY = Keys.None;
        public const PadButtons UNKNOWN_BUTTON = PadButtons.Unknown;

        public static bool IsConnected(PlayerIndex player)
        {
            bool isConnected = GamePad.GetState(player).IsConnected;

            if (player == PlayerIndex.One || player == PlayerIndex.Two)
            {
                isConnected |= KeyMaps[player].HasMappings;
            }

            return isConnected;
        }

        public static bool HasMappings(PlayerIndex player)
        {
            bool hasMappings = false;

            if (player == PlayerIndex.One || player == PlayerIndex.Two)
            {
                hasMappings = KeyMaps[player].HasMappings;
            }

            return hasMappings;
        }

        public static void ProcessMappingFile(string filename)
        {
            ProcessMappingFile(filename, 0);
            if (KeyMaps.Count == 0)
            {
                UseDefaultMappings();
            }
        }

        protected static void ProcessMappingFile(string filename,int depth)
        {
            // check for null filename
            if (filename == null) { return; }

            // prepend the app's path
            string path = StorageContainer.TitleLocation + "\\" + filename;

            // if the file exists, process it
            if (File.Exists(path))
            {
                try
                {
                    // local variable to read file, line-by-line
                    string line = null;

                    // open the file
                    StreamReader reader =
                        new StreamReader(
                            new FileStream(path, FileMode.Open));

                    if (depth == 0)
                    {
                        InitKeyMaps();
                    }

                    PlayerIndex player = PlayerIndex.One;

                    // for every line in the file, read it in,
                    // create a level from the data, and add the
                    // newly-created level to our collection
                    while ((line = reader.ReadLine()) != null)
                    {
                        ProcessMappingLine(ref player, line, depth);
                    }

                    // after reading all the lines, close the file
                    reader.Close();
                }
                catch { }
            }
        }

        protected static void ProcessMappingLine(ref PlayerIndex player, string line,int depth)
        {
            line = line.Trim();
            string lower = line.ToLower();
            if (lower.StartsWith("#include"))
            {
                string include =
                    line.Substring("#include".Length).Trim();
                ProcessMappingFile(include, depth + 1);
            }
            else if (
                line.StartsWith("#") ||
                line.StartsWith("//") ||
                line.Length == 0)
            {
                // ignore line, it's blank or a comment
            }
            else if (lower.StartsWith("playerone"))
            {
                player = PlayerIndex.One;
            }
            else if (lower.StartsWith("playertwo"))
            {
                player = PlayerIndex.Two;
            }
            else
            {
                int index = line.IndexOf("=");
                if (index > 0)
                {
                    string btnName = line.Substring(0, index).Trim();
                    string keyName = line.Substring(index + 1).Trim();

                    Keys key = UNKNOWN_KEY;
                    PadButtons button = UNKNOWN_BUTTON;

                    try
                    {
                        if (Enum.IsDefined(typeof(Keys), keyName))
                        {
                            key = (Keys)Enum.Parse(typeof(Keys), keyName, true);
                        }
                        if (Enum.IsDefined(typeof(PadButtons), btnName))
                        {
                            button = (PadButtons)Enum.Parse(typeof(PadButtons), btnName, true);
                        }
                    }
                    catch { }

                    if (
                        key != UNKNOWN_KEY &&
                        button != UNKNOWN_BUTTON)
                    {
                        KeyMapHelper.KeyMaps[player].AddMapping(button, key);
                    }
                }
            }
        }

        public static void UseDefaultMappings()
        {
            InitKeyMaps();

            PlayerIndex player = PlayerIndex.One;
            ProcessMappingLine(ref player, "PlayerOne", 0);
            ProcessMappingLine(ref player, "X=F", 0);
            ProcessMappingLine(ref player, "Y=T", 0);
            ProcessMappingLine(ref player, "A=V", 0);
            ProcessMappingLine(ref player, "B=G", 0);
            ProcessMappingLine(ref player, "ShoulderLeft=Q", 0);
            ProcessMappingLine(ref player, "ShoulderRight=E", 0);
            ProcessMappingLine(ref player, "TriggerLeft=Z", 0);
            ProcessMappingLine(ref player, "TriggerRight=C", 0);
            ProcessMappingLine(ref player, "ThumbLeft=X", 0);
            ProcessMappingLine(ref player, "Start=LeftControl", 0);
            ProcessMappingLine(ref player, "Back=LeftShift", 0);
            ProcessMappingLine(ref player, "DPadUp=W", 0);
            ProcessMappingLine(ref player, "DPadLeft=A", 0);
            ProcessMappingLine(ref player, "DPadDown=S", 0);
            ProcessMappingLine(ref player, "DPadRight=D", 0);
            ProcessMappingLine(ref player, "ThumbLeftDown=W", 0);
            ProcessMappingLine(ref player, "ThumbLeftLeft=A", 0);
            ProcessMappingLine(ref player, "ThumbLeftUp=S", 0);
            ProcessMappingLine(ref player, "ThumbLeftRight=D", 0);
            ProcessMappingLine(ref player, "PlayerTwo", 0);
            ProcessMappingLine(ref player, "X=NumPad4", 0);
            ProcessMappingLine(ref player, "Y=NumPad8", 0);
            ProcessMappingLine(ref player, "A=NumPad5", 0);
            ProcessMappingLine(ref player, "B=NumPad6", 0);
            ProcessMappingLine(ref player, "ShoulderLeft=NumPad7", 0);
            ProcessMappingLine(ref player, "ShoulderRight=NumPad9", 0);
            ProcessMappingLine(ref player, "TriggerLeft=Delete", 0);
            ProcessMappingLine(ref player, "TriggerRight=PageDown", 0);
            ProcessMappingLine(ref player, "ThumbLeft=End", 0);
            ProcessMappingLine(ref player, "Start=RightControl", 0);
            ProcessMappingLine(ref player, "Back=RightShift", 0);
            ProcessMappingLine(ref player, "DPadUp=Up", 0);
            ProcessMappingLine(ref player, "DPadLeft=Left", 0);
            ProcessMappingLine(ref player, "DPadDown=Down", 0);
            ProcessMappingLine(ref player, "DPadRight=Right", 0);
            ProcessMappingLine(ref player, "ThumbLeftUp=Up", 0);
            ProcessMappingLine(ref player, "ThumbLeftLeft=Left", 0);
            ProcessMappingLine(ref player, "ThumbLeftDown=Down", 0);
            ProcessMappingLine(ref player, "ThumbLeftRight=Right", 0);
        }

        public static void ProcessKeyboardInput(PlayerIndex player, KAGamePadState state)
        {
            state.IsConnected = IsConnected(player);
            if (HasMappings(player))
            {
                KeyboardState keyState = Keyboard.GetState();
                KeyMap mapper = KeyMaps[player];
                state.Buttons.A = mapper.GetButtonState(
                    keyState,
                    state.Buttons.A,
                    PadButtons.A);
                state.Buttons.B = mapper.GetButtonState(
                    keyState,
                    state.Buttons.B,
                    PadButtons.B);
                state.Buttons.Back = mapper.GetButtonState(
                    keyState,
                    state.Buttons.Back,
                    PadButtons.Back);
                state.Buttons.LeftShoulder = mapper.GetButtonState(
                    keyState,
                    state.Buttons.LeftShoulder,
                    PadButtons.ShoulderLeft);
                state.Buttons.LeftStick = mapper.GetButtonState(
                    keyState,
                    state.Buttons.LeftStick,
                    PadButtons.ThumbLeft);
                state.Buttons.RightShoulder = mapper.GetButtonState(
                    keyState,
                    state.Buttons.RightShoulder,
                    PadButtons.ShoulderRight);
                state.Buttons.RightStick = mapper.GetButtonState(
                    keyState,
                    state.Buttons.RightStick,
                    PadButtons.ThumbRight);
                state.Buttons.Start = mapper.GetButtonState(
                    keyState,
                    state.Buttons.Start,
                    PadButtons.Start);
                state.Buttons.X = mapper.GetButtonState(
                    keyState,
                    state.Buttons.X,
                    PadButtons.X);
                state.Buttons.Y = mapper.GetButtonState(
                    keyState,
                    state.Buttons.Y,
                    PadButtons.Y);

                state.DPad.Down = mapper.GetButtonState(
                    keyState,
                    state.DPad.Down,
                    PadButtons.DPadDown);
                state.DPad.Left = mapper.GetButtonState(
                    keyState,
                    state.DPad.Left,
                    PadButtons.DPadLeft);
                state.DPad.Right = mapper.GetButtonState(
                    keyState,
                    state.DPad.Right,
                    PadButtons.DPadRight);
                state.DPad.Up = mapper.GetButtonState(
                    keyState,
                    state.DPad.Up,
                    PadButtons.DPadUp);

                state.Triggers.Left = mapper.GetTriggerState(
                    keyState,
                    state.Triggers.Left,
                    PadButtons.TriggerLeft);
                state.Triggers.Right = mapper.GetTriggerState(
                    keyState,
                    state.Triggers.Right,
                    PadButtons.TriggerRight);

                Vector2 stick = state.ThumbSticks.Left;
                stick.X = mapper.GetStickState(
                    keyState,
                    stick.X,
                    PadButtons.ThumbLeftLeft,
                    PadButtons.ThumbLeftRight);
                stick.Y = mapper.GetStickState(
                    keyState,
                    stick.Y,
                    PadButtons.ThumbLeftDown,
                    PadButtons.ThumbLeftUp);
                state.ThumbSticks.Left = stick;

                stick = state.ThumbSticks.Right;
                stick.X = mapper.GetStickState(
                    keyState,
                    stick.X,
                    PadButtons.ThumbRightLeft,
                    PadButtons.ThumbRightRight);
                stick.Y = mapper.GetStickState(
                    keyState,
                    stick.Y,
                    PadButtons.ThumbRightDown,
                    PadButtons.ThumbRightUp);
                state.ThumbSticks.Right = stick;
            }
        }

        //// given a game pad button, return the keyboard
        //// key to which it maps (if any). in the case of
        //// PlayerIndex.Three and PlayerIndex.Four, this
        //// method will always return a zero.
        //public static Keys GetKey(PadButtons key)
        //{
        //    if (m_KeyMap.ContainsKey(key))
        //    {
        //        return m_KeyMap[key];
        //    }
        //    return 0;
        //}

        //public ButtonState GetButtonState(
        //    KeyboardState keyState, 
        //    ButtonState btnState, 
        //    PadButtons button)
        //{
        //    bool pressed = keyState.IsKeyDown(GetKey(button));
        //    return pressed || btnState == ButtonState.Pressed ? 
        //        ButtonState.Pressed : 
        //        ButtonState.Released;
        //}

        //public float GetTriggerState(
        //    KeyboardState keyState,
        //    float btnState,
        //    GamePadButtons button)
        //{
        //    if (btnState == 0.0f)
        //    {
        //        if (keyState.IsKeyDown(GetKey(button)))
        //        {
        //            btnState = TRIGGER_MAX;
        //        }
        //    }
        //    return btnState;
        //}

        //public float GetStickState(
        //    KeyboardState keyState,
        //    float btnState,
        //    PadButtons btnMin,
        //    PadButtons btnMax)
        //{
        //    if (btnState == 0.0f)
        //    {
        //        if (keyState.IsKeyDown(GetKey(btnMin)))
        //        {
        //            btnState = THUMBSTICK_MIN;
        //        }
        //        else if (keyState.IsKeyDown(GetKey(btnMax)))
        //        {
        //            btnState = THUMBSTICK_MAX;
        //        }
        //    }
        //    return btnState;
        //}
    }

    // simple enum for available controller buttons
    public enum PadButtons
    {
        Unknown,         // placeholder for new enum
        Start,           // white start button
        Back,            // white back button
        A,               // green A button
        B,               // red B button
        X,               // blue X button
        Y,               // yellow Y button
        DPadUp,          // D-Pad directions
        DPadDown,
        DPadLeft,
        DPadRight,
        ShoulderLeft,    // left shoulder button
        ShoulderRight,   // right shoulder button
        ThumbLeft,       // left thumbstick press
        ThumbLeftLeft,   // left thumbstick directions
        ThumbLeftRight,
        ThumbLeftUp,
        ThumbLeftDown,
        ThumbRight,      // right thumbstick press
        ThumbRightLeft,  // right thumbstick directions
        ThumbRightRight,
        ThumbRightUp,
        ThumbRightDown,
        TriggerLeft,     // left trigger press
        TriggerRight,    // right trigger press
    };
}
