// KeyMapHelper.cs
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Codetopia.Input
{
    // maintain a list of key mappings for use when emulating 
    // a game pad; also combine the state of the physical 
    // gamepad with any mapped keys
    public class KeyMapHelper
    {
        // extreme values for the analog gamepad buttons
        public const float TRIGGER_MAX = 1.00f;
        public const float TRIGGER_MIN = 0.00f;
        public const float THUMBSTICK_MAX = 1.00f;
        public const float THUMBSTICK_MIN = -1.00f;

        // collection of key mappings, indexed by player
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

        // reserve space in the list to map 
        // keys for players one and two
        protected static void InitKeyMaps()
        {
            // initialize the list
            KeyMapHelper.m_KeyMaps = new Dictionary<PlayerIndex, KeyMap>();
            
            // populate the list with empty mappings
            KeyMapHelper.m_KeyMaps.Add(PlayerIndex.One, new KeyMap());
            KeyMapHelper.m_KeyMaps.Add(PlayerIndex.Two, new KeyMap());
        }

        // an unmapped keyboard key
        public const Keys UNKNOWN_KEY = Keys.None;

        // an unrecognized gamepad button
        public const PadButtons UNKNOWN_BUTTON = PadButtons.Unknown;

        // true if there is an active, physical gamepad or there are 
        // mapped keyboard keys for the specified player; assumes that
        // there is an attached keyboard
        public static bool IsConnected(PlayerIndex player)
        {
            bool isConnected = GamePad.GetState(player).IsConnected;

            if (player == PlayerIndex.One || player == PlayerIndex.Two)
            {
                isConnected |= KeyMapHelper.KeyMaps[player].HasMappings;
            }

            return isConnected;
        }

        // does the specified player have any keyboard mappings?
        public static bool HasMappings(PlayerIndex player)
        {
            bool hasMappings = false;

            if (player == PlayerIndex.One || player == PlayerIndex.Two)
            {
                hasMappings = KeyMapHelper.KeyMaps[player].HasMappings;
            }

            return hasMappings;
        }

        // read keyboard mappings from a data file
        public static void ProcessMappingFile(string filename)
        {
            // try to process the file
            KeyMapHelper.ProcessMappingFile(filename, 0);

            // if the mappings failed, just use our hard-coded defaults
            if (KeyMapHelper.KeyMaps.Count == 0)
            {
                KeyMapHelper.UseDefaultMappings();
            }
        }

        // read keyboard mappings from a data file
        protected static void ProcessMappingFile(string filename, int depth)
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

                    // if this is the first call in the chain, initialize 
                    // our key mapping data, clearing any existing mappings
                    if (depth == 0)
                    {
                        KeyMapHelper.InitKeyMaps();
                    }

                    // always need a valid player, assume player one, just
                    // in case the data file forgot to specify a player
                    PlayerIndex player = PlayerIndex.One;

                    // for every line in the file, read it in and process it
                    while ((line = reader.ReadLine()) != null)
                    {
                        KeyMapHelper
                            .ProcessMappingLine(ref player, line, depth);
                    }

                    // after reading all the lines, close the file
                    reader.Close();
                }
                catch { }
            }
        }

        // process a single line of data from a mapping file
        protected static void ProcessMappingLine(
            ref PlayerIndex player, string line,int depth)
        {
            // trim leading and trailing whitespace from the line
            line = line.Trim();

            // convert to lower case for case-insensitive comparisons
            string lower = line.ToLower();
            if (lower.StartsWith("#include"))
            {
                // process the speficied file as if its contents had 
                // been embedded within the current file
                string include =
                    line.Substring("#include".Length).Trim();
                KeyMapHelper.ProcessMappingFile(include, depth + 1);
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
                // start applying mappings for PlayerIndex.One
                player = PlayerIndex.One;
            }
            else if (lower.StartsWith("playertwo"))
            {
                // start applying mappings for PlayerIndex.Two
                player = PlayerIndex.Two;
            }
            else
            {
                // actual mapping line is in the form "button=key"
                int index = line.IndexOf("=");
                if (index > 0)
                {
                    // extract key and button names
                    string btnName = line.Substring(0, index).Trim();
                    string keyName = line.Substring(index + 1).Trim();

                    // assume failure (we don't recognize the names)
                    Keys key = UNKNOWN_KEY;
                    PadButtons button = UNKNOWN_BUTTON;

                    try
                    {
                        if (Enum.IsDefined(typeof(Keys), keyName))
                        {
                            // the string to the left of the equals 
                            // sign should be the name of a valid
                            // member of the Keys enumeration
                            key = (Keys)Enum.Parse(
                                typeof(Keys), keyName, true);
                        }
                        if (Enum.IsDefined(typeof(PadButtons), btnName))
                        {
                            // the string to the right of the equals 
                            // sign should be the name of a valid
                            // member of the PadButtons enumeration
                            button = (PadButtons)Enum.Parse(
                                typeof(PadButtons), btnName, true);
                        }
                    }
                    catch { }

                    if (key != UNKNOWN_KEY && button != UNKNOWN_BUTTON)
                    {
                        // looks valid, go ahead and apply the 
                        // mapping between the button and the key
                        KeyMapHelper.KeyMaps[player]
                            .AddMapping(button, key);
                    }
                }
            }
        }

        // clear the current mappings and use our hard-coded defaults
        public static void UseDefaultMappings()
        {
            // clear any existing mappings
            KeyMapHelper.InitKeyMaps();

            // keep track of which player's keys are being mapped
            PlayerIndex player = PlayerIndex.One;

            // map defaults for player one
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

            // map defaults for player two
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

        // augment state data from the physical gamepad (if any) with
        // mapped keyboard state data (if any)
        public static void ProcessKeyboardInput(
            PlayerIndex player, KAGamePadState state)
        {
            // set the connection state for the virtual controller
            state.IsConnected = KeyMapHelper.IsConnected(player);

            // if the specified player has keyboard mappings, process them
            if (HasMappings(player))
            {
                // get the current state of the keyboard
                KeyboardState keyState = Keyboard.GetState();

                // get the key mappings (if any) for the specified player
                KeyMap mapper = KeyMapHelper.KeyMaps[player];

                // update digital button state data
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

                // update directional button state data
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

                // update trigger state data
                state.Triggers.Left = mapper.GetTriggerState(
                    keyState,
                    state.Triggers.Left,
                    PadButtons.TriggerLeft);
                state.Triggers.Right = mapper.GetTriggerState(
                    keyState,
                    state.Triggers.Right,
                    PadButtons.TriggerRight);

                // update thumbtick state data (left)
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

                // update thumbtick state data (right)
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
