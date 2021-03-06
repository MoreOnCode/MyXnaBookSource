// VirtualKeyboard.cs
#region Using Statements
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Codetopia.Graphics;
#endregion

namespace Codetopia.Input
{
    public class VirtualKeyboard
    {
        // is the virtual keyboard visible?
        protected bool m_Visible = false;
        public bool Visible
        {
            get { return m_Visible; }
            set { m_Visible = value; }
        }

        // the text that we're editing
        protected string m_Text = "";
        public Vector2 TextSize = Vector2.Zero;
        public string Text
        {
            get { return m_Text; }
            set
            {
                m_Text = value;
                if (Font != null)
                {
                    TextSize = Font.MeasureString(Text);
                }
                else
                {
                    TextSize = Vector2.Zero;
                }
            }
        }

        // caption for the virtual keyboard dialog
        protected string m_Caption = "Dialog Caption";
        public string Caption
        {
            get { return m_Caption; }
            set { m_Caption = value; }
        }

        // location of the dialog on the screen
        protected Vector2 m_Location = Vector2.Zero;
        public Vector2 Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }
        
        // simple way to set location without creating a new Vector2
        public void SetLocation(int x, int y)
        {
            m_Location.X = x;
            m_Location.Y = y;
        }
        
        // center the dialog within the specified rectangle
        public void CenterIn(Rectangle rect)
        {
            SetLocation(
                rect.Left + rect.Width / 2 - m_RectBackground.Width / 2,
                rect.Top + rect.Height / 2 - m_RectBackground.Height / 2);
        }

        // the texture for the dialog and buttons
        protected static Texture2D m_Texture = null;
        public Texture2D Texture
        {
            get { return m_Texture; }
            set { m_Texture = value; }
        }

        // texture coordinates for the various dialog components
        protected static readonly Rectangle 
            m_RectBackground = new Rectangle(0, 183, 360, 216);
        protected static readonly Rectangle 
            m_RectButtons = new Rectangle(0, 0, 357, 181);
        protected static readonly Rectangle
            m_RectCaption = new Rectangle(2, 2, 357, 30);
        protected static readonly Rectangle
            m_RectText = new Rectangle(7, 36, 346, 26);
        protected static readonly Rectangle
            m_RectCursor = new Rectangle(7, 4, 1, 20);

        // texture coordinates for the two empty buttons (small and large)
        protected static readonly Rectangle
            m_RectButtonLarge = new Rectangle(24, 400, 48, 28);
        protected static readonly Rectangle
            m_RectButtonSmall = new Rectangle(0, 400, 23, 28);

        // gather player input, and call the overloaded Update
        public void Update(double elapsed)
        {
            // don't update if the dialog isn't visible
            if (Visible)
            {
                Update(elapsed,
                    GamePad.GetState(PlayerIndex.One),
                    Keyboard.GetState());
            }
        }

        // enforce delay before repeating a button press
        protected double m_TimeSinceLastProcessInput = ProcessInputDelay;
        protected const double ProcessInputDelay = 0.2;

        // the blinking cursor for the text editor
        protected bool m_CursorBlink = true;
        protected const double CursorBlinkRate = 0.75;
        protected double m_CursorBlinkCounter = 0;

        // update state and process player input
        public void Update(double elapsed, 
            GamePadState pad1, KeyboardState key1)
        {
            // don't update if the dialog isn't visible
            if (Visible)
            {
                // increment repeat delay counter
                m_TimeSinceLastProcessInput += elapsed;

                // increment cursor flash counter, toggle blink
                m_CursorBlinkCounter += elapsed;
                if (m_CursorBlinkCounter >= CursorBlinkRate)
                {
                    m_CursorBlinkCounter = 0;
                    m_CursorBlink = !m_CursorBlink;
                }

                // process player input
                ProcessInput(pad1, key1);
            }
        }

        // simple method to determine when no gamepad buttons are pressed
        protected bool NonePressed(GamePadState pad1)
        {
            return
                pad1.Buttons.ToString() == "{Buttons:None}" &&
                pad1.DPad.ToString() == "{DPad:None}" &&
                pad1.Triggers.ToString() == "{Left:0 Right:0}" &&
                pad1.ThumbSticks.ToString() ==
                    "{Left:{X:0 Y:0} Right:{X:0 Y:0}}";
        }

        // process player input
        protected void ProcessInput(GamePadState pad1, KeyboardState key1)
        {
            // forego delay if no keys are pressed
            if (NonePressed(pad1) && key1.GetPressedKeys().Length == 0)
            {
                m_TimeSinceLastProcessInput = ProcessInputDelay;
            }

            // don't repeat same press on every frame, delay between repeats
            if (m_TimeSinceLastProcessInput >= ProcessInputDelay)
            {
                // capture state before updates to detect changes
                Vector2 prevSelectedKey = m_SelectedCharButton;
                string prevText = Text;
                int prevCursor = Cursor;
                bool prevShiftPressed = ShiftPressed;
                int prevVKB = CurrentVKB;

                // process input from game pad and keyboard
                ProcessInput(pad1);
                ProcessInput(key1);

                // detect changes
                bool changed =                 
                    (prevSelectedKey  != m_SelectedCharButton) ||
                    (prevText         != Text)          ||
                    (prevCursor       != Cursor)        ||
                    (prevShiftPressed != ShiftPressed)  ||
                    (prevVKB          != CurrentVKB);

                // state changed
                if (changed)
                {
                    // reset button delay
                    m_TimeSinceLastProcessInput = 0;

                    // reset cursor blink
                    m_CursorBlink = true;
                    m_CursorBlinkCounter = 0;
                }
            }
        }

        // insert a character at the current cursor position
        protected void InsertChar(char c)
        {
            if (c != '\0')
            {
                Text = Text.Insert(Cursor, c.ToString());
                Cursor++;
            }
        }

        // delete the character just before the current cursor position
        protected void BackSpace()
        {
            if (Cursor > 0)
            {
                Text = Text.Remove(Cursor - 1, 1);
                Cursor--;
            }
        }

        // process player input from the gamepad
        protected void ProcessInput(GamePadState pad1)
        {
            // change in selected virtual keyboard button
            int dx = 0;
            int dy = 0;

            // select button to the left or right
            if (pad1.ThumbSticks.Left.X < 0)
            {
                dx = -1;
            }
            else if (pad1.ThumbSticks.Left.X > 0)
            {
                dx = 1;
            }

            // select button up or down
            if (pad1.ThumbSticks.Left.Y > 0)
            {
                dy = -1;
            }
            else if (pad1.ThumbSticks.Left.Y < 0)
            {
                dy = 1;
            }

            // actually change the selected button
            ChangeSelectedCharButton(dx, dy);

            // shift shortcut
            if (pad1.Buttons.LeftStick == ButtonState.Pressed)
            {
                ShiftPressed = !ShiftPressed;
            }

            // press the selected virtual keyboard button
            if (pad1.Buttons.A == ButtonState.Pressed)
            {
                if (SelectedChar != '\0')
                {
                    // selected button is a character, add to text
                    InsertChar(SelectedChar);
                }
                else if (m_SelectedCharButton.Y == 4)
                {
                    if (m_SelectedCharButton.X == 10)
                    {
                        // selected done button
                        Done();
                        return;
                    }
                    else if (m_SelectedCharButton.X == 11)
                    {
                        // selected cancel button
                        Cancel();
                        return;
                    }
                }
                else if (m_SelectedCharButton.X == 0)
                {
                    // button in first column
                    switch ((int)m_SelectedCharButton.Y)
                    {
                        case 0: // Alpha
                            CurrentVKB = VKB_ALPHAL;
                            break;
                        case 1: // Symbol
                            CurrentVKB = VKB_SYMBOL;
                            break;
                        case 2: // Accent
                            CurrentVKB = VKB_ACCNTL;
                            break;
                        case 3: // Shift
                            ShiftPressed = !ShiftPressed;
                            break;
                    }
                }
                else if (m_SelectedCharButton.X == 11)
                {
                    // button in last column
                    switch ((int)m_SelectedCharButton.Y)
                    {
                        case 0: // Left
                            Cursor--;
                            break;
                        case 1: // Right
                            Cursor++;
                            break;
                        case 2: // Back Space
                            BackSpace();
                            break;
                        case 3: // Space
                            InsertChar(' ');
                            break;
                    }
                }
            }

            // process shortcut gamepad buttons
            if (pad1.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                Cursor--; // Left
            }
            else if (pad1.Buttons.RightShoulder == ButtonState.Pressed)
            {
                Cursor++; // Right
            }

            if (pad1.Buttons.Start == ButtonState.Pressed)
            {
                Done(); // Done
                return;
            }
            else if (pad1.Buttons.Back == ButtonState.Pressed)
            {
                Cancel(); // Cancel
                return;
            }
            else if (pad1.Buttons.B == ButtonState.Pressed)
            {
                // Back Space
                BackSpace();
            }
            else if (pad1.Buttons.X == ButtonState.Pressed)
            {
                // Space
                InsertChar(' ');
            }
            else if (pad1.Buttons.Y == ButtonState.Pressed)
            {
                // toggle character mappings (alpha, symbol, accent)
                switch (CurrentVKB)
                {
                    case VKB_ACCNTL:
                    case VKB_ACCNTU:
                        CurrentVKB = VKB_ALPHAL;
                        break;
                    case VKB_SYMBOL:
                        CurrentVKB = VKB_ACCNTL;
                        break;
                    case VKB_ALPHAL:
                    case VKB_ALPHAU:
                        CurrentVKB = VKB_SYMBOL;
                        break;
                }
            }

            // preserve shift state
            if (CurrentVKB == VKB_ACCNTL || CurrentVKB == VKB_ACCNTU)
            {
                CurrentVKB = (ShiftPressed ? VKB_ACCNTU : VKB_ACCNTL);
            }
            else if (CurrentVKB == VKB_ALPHAL || CurrentVKB == VKB_ALPHAU)
            {
                CurrentVKB = (ShiftPressed ? VKB_ALPHAU : VKB_ALPHAL);
            }
        }

        // map Input.Keys to chars, by range
        // an array of an array of int -- first four int values are always
        // { starting Keys enum, ending Keys enum, first char, shift flag }
        // inner array may also have a list of chars to map the Key enum
        protected int[][] m_KeyMap = {
            new int[] {(int)Keys.A,        (int)Keys.Z,       'a', 0},
            new int[] {(int)Keys.A,        (int)Keys.Z,       'A', 1},
            new int[] {(int)Keys.NumPad0,  (int)Keys.NumPad9, '0', 0},
            new int[] {(int)Keys.D0,       (int)Keys.D9,      '0', 0},
            new int[] {(int)Keys.D0,       (int)Keys.D9,      '0', 1, 
                ')', '!', '@', '#', '$', '%', '^', '&', '*', '(', },
            new int[] {(int)Keys.OemSemicolon, (int)Keys.OemTilde, ';', 0,
                ';', '=', ',', '-', '.', '/', '`', },
            new int[] {(int)Keys.OemSemicolon, (int)Keys.OemTilde, ';', 1,
                ':', '+', '<', '_', '>', '?', '\0', },
            new int[] {(int)Keys.OemOpenBrackets, (int)Keys.OemQuotes, ';', 0,
                '[', '\\', ']', '\'', 'z', '\\', },
            new int[] {(int)Keys.OemOpenBrackets, (int)Keys.OemQuotes, ';', 1,
                '{', '|', '}', '"', 'Z', '|', },
            new int[] {(int)Keys.OemBackslash, (int)Keys.OemBackslash, ';', 0,
                '\\', },
            new int[] {(int)Keys.OemBackslash, (int)Keys.OemBackslash, ';', 1,
                '|', },
        };

        // process player input from the keyboard
        protected void ProcessInput(KeyboardState key1)
        {
            // get a list of pressed keys
            Keys[] keys = key1.GetPressedKeys();

            // get the state of the shift key
            bool shift = 
                key1.IsKeyDown(Keys.LeftShift) || 
                key1.IsKeyDown(Keys.RightShift);

            // process each pressed key
            foreach (Keys key in keys)
            {
                // map pressed key and shift state to int values
                int keyInt = (int)key;
                int shiftInt = (shift ? 1 : 0);

                // keep track of selected char (if any)
                char c = '\0';

                // see if this key is listed in our char map
                foreach (int[] map in m_KeyMap)
                {
                    if (
                        shiftInt == map[3] && 
                        keyInt >= map[0] && 
                        keyInt <= map[1])
                    {
                        // is in map, standard entry
                        if (map.Length == 4)
                        {
                            c = (char)(map[2] + (keyInt - map[0]));
                        }
                        // is in map, with list of chars to map
                        else
                        {
                            c = (char)(map[4 + (keyInt - map[0])]);
                        }

                        // stop looking, we mapped it
                        break;
                    }
                }

                // if we didn't identify the key in our mapping, 
                // see if it's another key that we're interested in
                if (c == '\0')
                {
                    switch (key)
                    {
                        case Keys.Space:
                            c = ' ';
                            break;
                        case Keys.Enter:
                            Done();
                            return;
                        case Keys.Escape:
                            Cancel();
                            return;
                        case Keys.End:
                            Cursor = Text.Length;
                            break;
                        case Keys.Home:
                            Cursor = 0;
                            break;
                        case Keys.Left:
                            Cursor--;
                            break;
                        case Keys.Right:
                            Cursor++;
                            break;
                        case Keys.Back:
                            BackSpace();
                            break;
                    }
                }

                // add char to text, if we mapped one
                InsertChar(c);
            }
        }

        // draw the virtual keyboard
        public void Draw(SpriteBatch batch)
        {
            // only draw when visible
            if (Visible)
            {
                DrawVirtualKeyboard(batch);
            }
        }

        // draw the virtual keyboard
        protected void DrawVirtualKeyboard(SpriteBatch batch)
        {
            // offset to account for dialog caption
            Vector2 btnOffset = new Vector2(0, 32);

            // draw background and keys
            batch.Draw(Texture, Location, m_RectBackground, Color.White);
            batch.Draw(Texture, Location + btnOffset, m_RectButtons, 
                Color.White);
            HiliteSelectedButton(batch);

            // draw caption
            Vector2 size = Font.MeasureString(Caption);
            float x = Location.X + m_RectCaption.Left + 
                m_RectCaption.Width / 2 - size.X / 2;
            float y = Location.Y + m_RectCaption.Top + 
                m_RectCaption.Height / 2 - size.Y / 2;
            Font.DrawString(
                batch, Caption, (int)x + 1, (int)y + 1, Color.Black);
            Font.DrawString(
                batch, Caption, (int)x, (int)y, Color.White);

            // draw text
            x = Location.X + m_RectText.Left;
            y = Location.Y + m_RectText.Top + 
                m_RectText.Height / 2 - Font.FontHeight / 2;
            Font.DrawString(batch, Text, (int)x, (int)y, Color.Black);

            // draw cursor
            if (m_CursorBlink)
            {
                y = Location.Y + m_RectCursor.Top + 35;
                x += Font.MeasureString(Text.Substring(0, Cursor)).X;
                batch.Draw(
                    Texture, new Vector2(x, y), m_RectCursor, Color.Navy);
            }

            // draw button text
            Vector2 loc = Vector2.Zero;
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    // calc location
                    loc = Location + btnOffset + 
                          m_CharButtonLoc[row,col] +
                          m_CharLoc[CurrentVKB,row,col];

                    // draw character
                    m_Font.DrawString(
                        batch,
                        m_CharGrids[CurrentVKB][row][col],
                        (int)loc.X,
                        (int)loc.Y,
                        Color.Black);
                }
            }
        }

        // highlight the selected virtual keyboard button
        protected void HiliteSelectedButton(SpriteBatch batch)
        {
            // offset to account for dialog caption
            Vector2 btnOffset = new Vector2(0, 32);

            // texture rect and screen location for button
            Rectangle rect = Rectangle.Empty;
            Vector2 loc = Vector2.Zero;

            // indicate shift state
            if (ShiftPressed)
            {
                rect = m_RectButtonLarge;
                rect.X = 3;
                rect.Y = 33 + 30 * 3;

                loc.X = Location.X + rect.X + btnOffset.X;
                loc.Y = Location.Y + rect.Y + btnOffset.Y;

                batch.Draw(Texture, loc, rect, Color.PaleGoldenrod);
            }

            // small button is active?
            if (SelectedChar != '\0')
            {
                rect = m_RectButtonSmall;
                loc = Location + btnOffset +
                    m_CharButtonLoc[
                        (int)m_SelectedCharButton.Y,
                        (int)m_SelectedCharButton.X - 1];
            }
            // large button is active?
            else
            {
                // first or last column
                rect = m_RectButtonLarge;
                rect.X = (m_SelectedCharButton.X == 0 ? 3 : 309);
                rect.Y = 33 + 30 * (int)m_SelectedCharButton.Y;
                
                // Done button is special case
                if (m_SelectedCharButton.Y == 4 && m_SelectedCharButton.X == 10)
                {
                    rect.X = 259;
                }

                loc.X = Location.X + rect.X + btnOffset.X;
                loc.Y = Location.Y + rect.Y + btnOffset.Y;
            }

            // draw the hightlighted button
            batch.Draw(Texture, loc, rect, Color.LightGreen);
        }

        // the bitmap font used to draw Text and button glyphs
        protected GameFont m_Font = null;
        public GameFont Font
        {
            get { return m_Font; }
            set
            {
                m_Font = value;
                UpdateCharLocations();
                Text = Text;
            }
        }

        // virtual keyboard character button counts
        protected const int COUNT_VKBS = 5;  // number of character sets
        protected const int COUNT_ROWS = 4;  // number of character rows
        protected const int COUNT_COLS = 10; // number of character per row

        // top, left location of each character button
        protected static Vector2[,] m_CharButtonLoc = 
            new Vector2[COUNT_ROWS, COUNT_COLS];

        // location from top, left of button for each character
        protected Vector2[, ,] m_CharLoc = 
            new Vector2[COUNT_VKBS, COUNT_ROWS, COUNT_COLS];

        // calculate the position of each glyph, centered in its own button
        protected void UpdateCharLocations()
        {
            // for each set of virtual keyboard characters
            for (int vkb = 0; vkb < COUNT_VKBS; vkb++)
            {
                // for each row
                for (int row = 0; row < COUNT_ROWS; row++)
                {
                    // track top, left for each button
                    int btnLocX = 30;
                    int btnLocY = 33 + 30 * row;

                    // for each column
                    for (int col = 0; col < COUNT_COLS; col++)
                    {
                        // button locations aren't indexed by vkb,
                        // only calculate these values on the first
                        // pass of the outer loop.
                        if (vkb == 0)
                        {
                            // calculate button location
                            btnLocX += (col == 7 ? 27 : 25);
                            m_CharButtonLoc[row,col].X = btnLocX;
                            m_CharButtonLoc[row,col].Y = btnLocY;
                        }

                        // calculate character location, within the button
                        Vector2 size = 
                            Font.MeasureString(m_CharGrids[vkb][row][col]);
                        m_CharLoc[vkb,row,col].X =
                            m_RectButtonSmall.Width / 2 - size.X / 2;
                        m_CharLoc[vkb,row,col].Y =
                            m_RectButtonSmall.Height / 2 - size.Y / 2;
                    }
                }
            }
        }

        // virtual keyboard indicies
        protected const int VKB_ALPHAL = 0; // Alphabet (lower)
        protected const int VKB_ALPHAU = 1; // Alphabet (upper)
        protected const int VKB_SYMBOL = 2; // Symbols
        protected const int VKB_ACCNTL = 3; // Accents  (lower)
        protected const int VKB_ACCNTU = 4; // Accents  (upper)

        // currently-selected virtual keyboard
        protected int m_CurrentVKB = VKB_ALPHAL;
        public int CurrentVKB
        {
            get { return m_CurrentVKB; }
            set { m_CurrentVKB = value; }
        }

        // cursor (as index into Text characters)
        protected int m_Cursor = 0;
        public int Cursor
        {
            get { return m_Cursor; }
            set
            {
                m_Cursor = value;
                m_Cursor = Math.Max(0, m_Cursor);
                m_Cursor = Math.Min(Text.Length, m_Cursor);
            }
        }

        // state of the shift modifier for characters
        protected bool m_ShiftPressed = false;
        public bool ShiftPressed
        {
            get { return m_ShiftPressed; }
            set { m_ShiftPressed = value; }
        }

        // selected character button
        protected Vector2 m_SelectedCharButton = new Vector2(1, 0);

        // if selected button is a character, return it
        public char SelectedChar
        {
            get
            {
                // assume failure
                char selected = '\0';

                // get indices into virtual keyboard char array
                int col = (int)m_SelectedCharButton.X - 1;
                int row = (int)m_SelectedCharButton.Y;

                // check bounds of indices
                if (col >= 0 && col < COUNT_COLS &&
                    row >= 0 && row < COUNT_ROWS)
                {
                    // extract selected character
                    selected = m_CharGrids[CurrentVKB][row][col];
                }

                // return our findings
                return selected;
            }
        }

        // move the button cursor within the virtual keyboard
        protected void ChangeSelectedCharButton(int dx, int dy)
        {
            // update location
            m_SelectedCharButton.X += dx;
            m_SelectedCharButton.Y += dy;

            // check bounds
            m_SelectedCharButton.X = Math.Max(m_SelectedCharButton.X, 0);
            m_SelectedCharButton.X = Math.Min(m_SelectedCharButton.X, 11);
            m_SelectedCharButton.Y = Math.Max(m_SelectedCharButton.Y, 0);
            m_SelectedCharButton.Y = Math.Min(m_SelectedCharButton.Y, 4);

            // last row is special case; two large buttons
            if (m_SelectedCharButton.Y == 4)
            {
                m_SelectedCharButton.X = Math.Max(m_SelectedCharButton.X, 10);
            }
        }

        // chars for each virtual keyboard
        protected static readonly char[][][] m_CharGrids = 
        {
            // Alphabet - lower case
            new char[][] {
                "abcdefg123".ToCharArray(),
                "hijklmn456".ToCharArray(),
                "opqrstu789".ToCharArray(),
                "vwxyz-@_0.".ToCharArray(),
            },
            // Alphabet - upper case
            new char[][] {
                "ABCDEFG123".ToCharArray(),
                "HIJKLMN456".ToCharArray(),
                "OPQRSTU789".ToCharArray(),
                "VWXYZ-@_0.".ToCharArray(),
            },
            // Symbols
            new char[][] {
                ",;:'\"!?¡¿%".ToCharArray(),
                "[]{}`$£«»#" .ToCharArray(),
                "<>()€¥™-^\\".ToCharArray(),
                "|=*/+-@_&." .ToCharArray(),
            },
            // Accents - lower case
            new char[][] {
                "àáâãäåñœæβ".ToCharArray(),
                "èéêëþçýÿ˚˚".ToCharArray(),
                "ìíîïùúûüμζ".ToCharArray(),
                "òóôõöō©_×.".ToCharArray(),
            },
            // Accents - upper case
            new char[][] {
                "ÀÁÂÃÄÅÑŒÆß".ToCharArray(),
                "ÈÉÊËÞÇÝŸ˚˚".ToCharArray(),
                "ÌÍÎÏÙÚÛÜΜΞ".ToCharArray(),
                "ÒÓÔÕÖŌ®_×.".ToCharArray(),
            },
        };

        // delegate to notify host when we're done editing
        public delegate void InputCompleteEvent(string text);
        protected InputCompleteEvent m_Callback = null;
        protected void OnInputComplete()
        {
            if (m_Callback != null)
            {
                m_Callback(Text);
            }
        }

        // show the virtual keyboard dialog
        public void Show()
        {
            Show(null);
        }

        // show the virtual keyboard dialog, register callback (delegate)
        public void Show(InputCompleteEvent callback)
        {
            // don't reset dialog if it's already visible
            if (!Visible)
            {
                // make visible
                Visible = true;

                // reset state variables
                m_TimeSinceLastProcessInput = 0;
                m_SelectedCharButton = new Vector2(1, 0);
                Cursor = Text.Length;

                // remember original text (in case player decides to cancel)
                m_OriginalText = Text;

                // register callback delegate
                m_Callback = callback;
            }
        }

        // store copy of text without edits (in case player decides to cancel)
        protected string m_OriginalText = "";

        // we're done editing, notify host
        protected void Done()
        {
            Visible = false;
            OnInputComplete();
        }

        // edit canceled, reset text and notify host
        protected void Cancel()
        {
            Text = m_OriginalText;
            Visible = false;
            OnInputComplete();
        }
    }
}
