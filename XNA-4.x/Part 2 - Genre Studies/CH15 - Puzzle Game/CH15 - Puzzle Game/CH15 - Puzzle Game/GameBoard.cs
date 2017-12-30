using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace CH15___Puzzle_Game
{
    class GameBoard
    {
        // the game states
        public enum GameState
        {
            Playing,
            GameOver,
        }

        // the current game state
        private GameState m_CurrentState = GameState.GameOver;
        public GameState CurrentState
        {
            get { return m_CurrentState; }
            set { m_CurrentState = value; }
        }

        // score for the current game
        private long m_Score = 0;
        public long Score
        {
            get { return m_Score; }
            set
            {
                m_Score = value;
                m_ScoreSprite.Value = value;
                if (Score > HighScore)
                {
                    // we have a new high score!
                    HighScore = Score;
                }
            }
        }

        // best score seen so far
        private long m_HighScore = 0;
        public long HighScore
        {
            get { return m_HighScore; }
            set
            {
                m_HighScore = value;
                m_HighScoreSprite.Value = value;
            }
        }

        // score 
        private long m_BaseScore = 5;
        public long BaseScore
        {
            get { return m_BaseScore; }
            set { m_BaseScore = value; }
        }

        // size of each cell
        public static readonly int CELL_SIZE = Cell.CELL_SIZE;

        // space between each cell
        public static readonly int CELL_PADDING = Cell.CELL_PADDING;

        // the default constructor
        public GameBoard()
        {
            InitCells();
            Score = 0;
            HighScore = 0;
        }

        // the game board's width
        private int m_BoardWidth = 9;
        public int BoardWidth
        {
            get { return m_BoardWidth; }
            set
            {
                m_BoardWidth = value;
                InitCells();
            }
        }

        // the game board's height
        private int m_BoardHeight = 15;
        public int BoardHeight
        {
            get { return m_BoardHeight; }
            set
            {
                m_BoardHeight = value;
                InitCells();
            }
        }

        // index into color array for each cell on the board
        private Cell[,] m_Cells;

        private void InitCells()
        {
            // init array
            m_Cells = new Cell[BoardWidth, BoardHeight];

            // assign new cell to each element
            for (int y = 0; y < BoardHeight; y++)
            {
                for (int x = 0; x < BoardWidth; x++)
                {
                    m_Cells[x, y] = new Cell();
                }
            }

            // clear the game board
            Clear();
        }

        // number of seconds since a new row was added
        private double m_SecondsSinceLastRow = 0.0f;

        // dealy (in seconds) before adding a new row
        private double m_SecondsBeforeNewRow = 8.0f;
        public double SecondsBeforeNewRow
        {
            get { return m_SecondsBeforeNewRow; }
            set { m_SecondsBeforeNewRow = value; }
        }

        // current state of the cursor pulse
        private double m_CursorAge = 0.0f;

        // cursor pulse rate
        private double m_CursorPulseInSeconds = 1.0f;
        public double CursorPulseInSeconds
        {
            get { return m_CursorPulseInSeconds; }
            set { m_CursorPulseInSeconds = value; }
        }

        // how many rows does a new game start with?
        private int m_NewGameRowCount = 6;
        public int NewGameRowCount
        {
            get { return m_NewGameRowCount; }
            set { m_NewGameRowCount = value; }
        }

        // add a new row to the board, moving existing rows up
        public bool NewRow(int numRows)
        {
            // handy temp variable to note lasst row
            int LastRow = BoardHeight - 1;

            // repeat add operation numRows times
            for (int row = 0; row < numRows; row++)
            {
                // scan top row to see if there is room for a new row
                for (int x = 0; x < BoardWidth; x++)
                {
                    if (m_Cells[x, 0].ColorIndex != CellColors.EmptyCellColorIndex)
                    {
                        // nope. game over!
                        CurrentState = GameState.GameOver;
                        return false;
                    }
                }

                // move all rows up one
                for (int y = 0; y < LastRow; y++) // for each (but last) row
                {
                    for (int x = 0; x < BoardWidth; x++) // for each column
                    {
                        // copy next row to the current row
                        m_Cells[x, y].Copy(m_Cells[x, y + 1]);
                    }
                }

                // fill last row with new random cells
                for (int x = 0; x < BoardWidth; x++)
                {
                    // pick a new cell color from our array, randomly
                    m_Cells[x, LastRow].Reset();
                    m_Cells[x, LastRow].ColorIndex = CellColors.RandomIndex();

                    // if this cell matches the previous cell, try again
                    if (x > 0)
                    {
                        int cell1 = m_Cells[x - 1, LastRow].ColorIndex;
                        while (m_Cells[x, LastRow].ColorIndex == cell1)
                        {
                            m_Cells[x, LastRow].ColorIndex = CellColors.RandomIndex();
                        }
                    }
                }

                // move cursor up a row
                MoveCursor(CursorDirection.Up);
            }

            // reset vertical offset
            m_SecondsSinceLastRow = 0.0f;

            // see if there are any matches
            ScanForMatches();

            // success
            return true;
        }

        // reset game to initial state
        public void Clear()
        {
            // fill board with empty cells
            for (int y = 0; y < BoardHeight; y++) // for each row
            {
                for (int x = 0; x < BoardWidth; x++) // for each column
                {
                    // empty the cell
                    m_Cells[x, y].Reset();
                }
            }

            // create NewGameRowCount rows now that board is clear
            for (int i = 0; i < NewGameRowCount; i++)
            {
                NewRow(1);
            }

            // center the cursor on the game board
            CenterCursor();

            // reset the score
            Score = 0;
        }

        // handy "constants" for the MoveCursor method
        public struct CursorDirection
        {
            public static readonly Vector2 Left = new Vector2(-1, 0);
            public static readonly Vector2 Right = new Vector2(1, 0);
            public static readonly Vector2 Up = new Vector2(0, -1);
            public static readonly Vector2 Down = new Vector2(0, 1);
        };

        // the current cursor position, or index into the cells array
        private Vector2 m_CursorPosition = Vector2.Zero;
        public Vector2 CursorPosition
        {
            get { return m_CursorPosition; }
            set { m_CursorPosition = value; }
        }

        // move the cursor
        public void MoveCursor(Vector2 delta)
        {
            CursorPosition += delta;

            // check horizontal bounds
            if (CursorPosition.X < 0)
            {
                m_CursorPosition.X = 0;
            }
            else if (CursorPosition.X > BoardWidth - 2)
            {
                // width minus two since cursor is two cells wide
                m_CursorPosition.X = BoardWidth - 2;
            }

            // check vertical bounds
            if (CursorPosition.Y < 0)
            {
                m_CursorPosition.Y = 0;
            }
            else if (CursorPosition.Y > BoardHeight - 2)
            {
                // height minus two since last row is not playable
                m_CursorPosition.Y = BoardHeight - 2;
            }
        }

        // center the cursor on the game board
        public void CenterCursor()
        {
            m_CursorPosition.X = (float)Math.Round(BoardWidth / 2.0) - 1;
            m_CursorPosition.Y = (float)Math.Floor(BoardHeight / 2.0);
        }

        // the (virtual) buttons that we're actually interested in
        private enum GameButton
        {
            Up,
            Down,
            Left,
            Right,
            Fire,
            Pause,
            // -------------
            Count
        };

        // track how long each button has been held down
        private double[] ButtonPressDuration = new double[(int)GameButton.Count];

        // rate at which to repeat presses when button is held down
        private const double BUTTON_REPEAT_RATE = 0.2;

        // update the scrolling rows and the pulsing cursor
        public void Update(double elapsed)
        {
            // process player input
            CheckButtons(elapsed);

            // only update the counters if the game is active
            if (CurrentState == GameState.Playing)
            {
                // scrolling row counter
                m_SecondsSinceLastRow += elapsed;
                if (m_SecondsSinceLastRow > m_SecondsBeforeNewRow)
                {
                    // time for a new row
                    NewRow(1);
                }

                // pulsing cursor counter
                m_CursorAge += elapsed;
                while (m_CursorAge > CursorPulseInSeconds)
                {
                    // try to keep counter between 0 and max seconds
                    // not entirely necessary since the math functions will handle 
                    // angles greater than 360 degrees, but I like to do it.
                    m_CursorAge -= CursorPulseInSeconds;
                }
            }

            // eliminate vertical gaps in the cells
            Gravity();

            // have each cell update itself
            foreach (Cell cell in m_Cells)
            {
                cell.Update(elapsed);
            }

            // animate "game over" text
            m_GameOverHover += elapsed;
        }

        // process player input
        private void CheckButtons(double elapsed)
        {
            // poll the current states
            GamePadState pad1 = GamePad.GetState(PlayerIndex.One);
            KeyboardState key1 = Keyboard.GetState();

            // helper to merge keyboard and gamepad inputs
            bool pressed = false;

            // UP Button
            pressed = key1.IsKeyDown(Keys.Up);
            pressed |= pad1.DPad.Up == ButtonState.Pressed;
            pressed |= pad1.ThumbSticks.Left.Y > 0;
            ProcessButton(GameButton.Up, pressed, elapsed);

            // DOWN Button
            pressed = key1.IsKeyDown(Keys.Down);
            pressed |= pad1.DPad.Down == ButtonState.Pressed;
            pressed |= pad1.ThumbSticks.Left.Y < 0;
            ProcessButton(GameButton.Down, pressed, elapsed);

            // LEFT Button
            pressed = key1.IsKeyDown(Keys.Left);
            pressed |= pad1.DPad.Left == ButtonState.Pressed;
            pressed |= pad1.ThumbSticks.Left.X < 0;
            ProcessButton(GameButton.Left, pressed, elapsed);

            // RIGHT Button
            pressed = key1.IsKeyDown(Keys.Right);
            pressed |= pad1.DPad.Right == ButtonState.Pressed;
            pressed |= pad1.ThumbSticks.Left.X > 0;
            ProcessButton(GameButton.Right, pressed, elapsed);

            // FIRE Button
            pressed = key1.IsKeyDown(Keys.Space);
            pressed |= pad1.Buttons.A == ButtonState.Pressed;
            ProcessButton(GameButton.Fire, pressed, elapsed);
        }

        // eliminate redundant button press code by generalizing it here
        private void ProcessButton(GameButton button, bool pressed, double elapsed)
        {
            if (pressed)
            {
                // manage button repeats by tracking how long
                // this button has been pressed
                ButtonPressDuration[(int)button] += elapsed;

                // if the repeat rate has been exceeded, treat
                // this as a new button press
                if (ButtonPressDuration[(int)button] > BUTTON_REPEAT_RATE)
                {
                    ButtonPressDuration[(int)button] = elapsed;
                }

                // button was just pressed or has repeated
                if (ButtonPressDuration[(int)button] == elapsed)
                {
                    switch (button)
                    {
                        case GameButton.Up:
                            MoveCursor(CursorDirection.Up);
                            break;
                        case GameButton.Down:
                            MoveCursor(CursorDirection.Down);
                            break;
                        case GameButton.Left:
                            MoveCursor(CursorDirection.Left);
                            break;
                        case GameButton.Right:
                            MoveCursor(CursorDirection.Right);
                            break;
                        case GameButton.Fire:
                            if (CurrentState == GameState.GameOver)
                            {
                                // start a new game
                                Clear();
                                CenterCursor();
                                CurrentState = GameState.Playing;
                            }
                            else
                            {
                                // swap the currently-selected cells
                                SwapCells();
                            }
                            break;
                    }
                }
            }
            else
            {
                // button was released, reset counter
                ButtonPressDuration[(int)button] = 0;
            }
        }

        // swap the two cells that lie beneath the cursor
        private void SwapCells()
        {
            // temp variables to save some typing
            int x = (int)CursorPosition.X;
            int y = (int)CursorPosition.Y;

            // can't swap a cell that's being cleared ...
            bool OkToSwap = !m_Cells[x, y].IsClearing;
            OkToSwap &= !m_Cells[x + 1, y].IsClearing;

            // ... or falling ...
            OkToSwap &= !m_Cells[x, y].IsFalling;
            OkToSwap &= !m_Cells[x + 1, y].IsFalling;

            // ... or has a falling cell above it ...
            OkToSwap &= !(y > 0 && m_Cells[x, y - 1].IsFalling);
            OkToSwap &= !(y > 0 && m_Cells[x + 1, y - 1].IsFalling);

            // if conditions are met, swap it
            if (OkToSwap)
            {
                // swap cells that lie beneath the cursor
                Cell swap = m_Cells[x + 1, y];
                m_Cells[x + 1, y] = m_Cells[x, y];
                m_Cells[x, y] = swap;

                // check gravity right away so that player can't swap
                // a cell over an empty cell for a match.
                Gravity();

                // check for matches after a valid swap
                ScanForMatches();
            }
        }

        private void ScanForMatches()
        {
            // temp variables to save some typing
            bool valid = false;
            int cell1, cell2, cell3;

            // PASS ONE - Scan Horizontal
            for (int y = 0; y < BoardHeight - 1; y++) // for each row
            {
                for (int x = 2; x < BoardWidth; x++) // for each column
                {
                    // grab the ColorIndex for the next 3 cells
                    cell1 = m_Cells[x - 2, y].ColorIndex;
                    cell2 = m_Cells[x - 1, y].ColorIndex;
                    cell3 = m_Cells[x - 0, y].ColorIndex;

                    // can't match if a cell is empty ...
                    valid = cell1 != CellColors.EmptyCellColorIndex;

                    // ... or is alredy being cleared ...
                    valid &= !m_Cells[x - 2, y].IsClearing;
                    valid &= !m_Cells[x - 1, y].IsClearing;
                    valid &= !m_Cells[x - 0, y].IsClearing;

                    // ... or is falling
                    valid &= !m_Cells[x - 2, y].IsFalling;
                    valid &= !m_Cells[x - 1, y].IsFalling;
                    valid &= !m_Cells[x - 0, y].IsFalling;

                    // if the conditions are met, mark the cells as
                    // "to be cleared", and scan for more matches
                    if (valid && cell1 == cell2 && cell2 == cell3)
                    {
                        m_Cells[x - 2, y].ColorIndex = -cell1;
                        m_Cells[x - 1, y].ColorIndex = -cell1;
                        m_Cells[x - 0, y].ColorIndex = -cell1;
                        x++;
                        while (x < BoardWidth && cell1 == m_Cells[x, y].ColorIndex)
                        {
                            m_Cells[x++, y].ColorIndex = -cell1;
                        }
                    }
                }
            }

            // PASS TWO - Scan Vertical
            // in this pass, we need to check for the absolute value 
            // of the ColorIndex since we marked matched cells with
            // a negative sign
            for (int x = 0; x < BoardWidth; x++) // for each column
            {
                for (int y = 2; y < BoardHeight - 1; y++) // for each row
                {
                    // grab the ColorIndex for the next 3 cells
                    cell1 = Math.Abs(m_Cells[x, y - 2].ColorIndex);
                    cell2 = Math.Abs(m_Cells[x, y - 1].ColorIndex);
                    cell3 = Math.Abs(m_Cells[x, y - 0].ColorIndex);

                    // can't match if a cell is empty ...
                    valid = cell1 != CellColors.EmptyCellColorIndex;

                    // ... or is alredy being cleared ...
                    valid &= !m_Cells[x, y - 2].IsClearing;
                    valid &= !m_Cells[x, y - 1].IsClearing;
                    valid &= !m_Cells[x, y - 0].IsClearing;

                    // ... or is falling
                    valid &= !m_Cells[x, y - 2].IsFalling;
                    valid &= !m_Cells[x, y - 1].IsFalling;
                    valid &= !m_Cells[x, y - 0].IsFalling;

                    // if the conditions are met, mark the cells as
                    // "to be cleared", and scan for more matches
                    if (valid && cell1 == cell2 && cell2 == cell3)
                    {
                        m_Cells[x, y - 2].ColorIndex = -cell1;
                        m_Cells[x, y - 1].ColorIndex = -cell1;
                        m_Cells[x, y - 0].ColorIndex = -cell1;
                        y++;
                        while (y < BoardHeight - 1 &&
                            cell1 == Math.Abs(m_Cells[x, y].ColorIndex))
                        {
                            m_Cells[x, y++].ColorIndex = -cell1;
                        }
                    }
                }
            }

            // PASS THREE - Scan for Marked Matches
            // the reason we do this in a seperate pass is that immediately 
            // flagging a horizontal match would make those same cells 
            // unavailble for a vertical match

            // track the number of unique colors matched
            List<int> matchedColors = new List<int>();

            // score this round of matches seperately
            long points = 0;

            // scan for cells that have been flagged as matched
            for (int y = 0; y < BoardHeight - 1; y++) // for each row
            {
                for (int x = 0; x < BoardWidth; x++) // for each column
                {
                    // negative color indicies are matches
                    if (m_Cells[x, y].ColorIndex < 0)
                    {
                        // reset color index so that it's valid, then 
                        // actually mark this cell as clearing
                        int color = m_Cells[x, y].ColorIndex;
                        m_Cells[x, y].ColorIndex = -color;
                        m_Cells[x, y].IsClearing = true;

                        // if this is a newly-visited index, remember it
                        if (!matchedColors.Contains(color))
                        {
                            matchedColors.Add(color);
                        }

                        // update the local score
                        points += BaseScore;
                    }
                }
            }

            // multiply new score by the number of unique colors as 
            // a simple combo bonus, then add it to the real score
            Score += points * matchedColors.Count;
        }

        // remove vertical gaps between cells
        private void Gravity()
        {
            // when a cell is done falling, it may trigger a match
            bool CheckForMatches = false;

            // scan each column
            for (int x = 0; x < BoardWidth; x++)
            {
                // scan reach row, from the 3rd-to-last row to the first.
                // cells on the last row can't fall, they're not even playable.
                // cells on the 2nd-to-last row can't fall, they're resting 
                // on the last row.
                for (int y = BoardHeight - 3; y >= 0; y--)
                {
                    // a handy temp variable to save some typing
                    Cell cell1 = m_Cells[x, y + 0];

                    // See if cell has fallen far enough to merit moving to 
                    // the next row
                    if (cell1.FallAge >= cell1.FallRate)
                    {
                        // mark cell as done falling
                        cell1.IsFalling = false;

                        // when a cell is done falling, it may trigger a match
                        CheckForMatches = true;

                        // actually move the cell to the next row
                        m_Cells[x, y + 1].Copy(cell1);
                        m_Cells[x, y + 0].Reset();

                        // process this row again, just to make sure we're not
                        // triggering any false hits -- since we set the IsFalling 
                        // flag to false, this cell is technically available for 
                        // a match, but it may not be completely done falling.
                        // add two since we've already moved the cell down a row
                        y += 2;
                    }

                    // is cell falling?
                    if (cell1.ColorIndex != CellColors.EmptyCellColorIndex)
                    {
                        // a handy temp variable to save some typing.
                        // cell immediately below the cell we're processing
                        Cell cell2 = m_Cells[x, y + 1];

                        // can fall if the cell bellow is empty ...
                        bool flag = cell2.ColorIndex ==
                            CellColors.EmptyCellColorIndex;

                        // ... or falling
                        flag |= cell2.IsFalling;

                        // but don't set a cell to fall if it's already 
                        // falling since it will reset it's state 
                        // information (FallingAge, ...)
                        flag &= !cell1.IsFalling;

                        // ok, flag to fall if conditions are met
                        if (flag)
                        {
                            cell1.IsFalling = true;
                        }
                    }
                }
            }

            if (CheckForMatches)
            {
                ScanForMatches();
            }
        }

        // the main Draw method
        public void Draw(SpriteBatch batch)
        {
            // no matter what we draw, it will have a background
            DrawBoardBackground(batch);
            switch (CurrentState)
            {
                // player is actively playing the game
                case GameState.Playing:
                    DrawActiveGameBoard(batch);
                    DrawCursor(batch);
                    break;

                // the game is paused or over
                // case GameState.Paused:
                case GameState.GameOver:
                    DrawInactiveGameBoard(batch);
                    break;
            }
        }

        // useful for the DrawXXX methods, allows us to move the 
        // board by setting one variable
        public Vector2 BoardTopLeft = new Vector2(100, 75);

        // when active, draw actual cells
        private void DrawActiveGameBoard(SpriteBatch batch)
        {
            // the last row is only partially visible until it's fully on-screen
            Rectangle LastRowTextureRect = new Rectangle(
                Cell.TextureRectCell.Left,
                Cell.TextureRectCell.Top,
                Cell.TextureRectCell.Width,
                (int)Math.Round((CELL_SIZE + CELL_PADDING) *
                    m_SecondsSinceLastRow / m_SecondsBeforeNewRow)
                );

            // temp variable to hold the top / left of each cell
            // since new rows aren't fully visible until they're playable,
            // the Y component of this vector is adjusted to account for 
            // the scrolling cells.
            Vector2 CellTopLeft = new Vector2(0, Cell.TextureRectCell.Height -
                LastRowTextureRect.Height);

            for (int y = 0; y < BoardHeight; y++) // for each row
            {
                // start with the left-most pixel of this cell
                CellTopLeft.X = 0;

                // draw playable rows in their normal color
                if (y != BoardHeight - 1)
                {
                    for (int x = 0; x < BoardWidth; x++) // for each column
                    {
                        // draw the cell
                        m_Cells[x, y].Draw(
                            batch,
                            BoardTopLeft + CellTopLeft,
                            Cell.TextureRectCell,
                            false);

                        // advance to the next cell
                        CellTopLeft.X += CELL_SIZE + CELL_PADDING;
                    }
                }

                // draw last row a little darker
                else
                {
                    // draw last row, a little darker
                    for (int x = 0; x < BoardWidth; x++) // for each column
                    {
                        // draw the cell
                        m_Cells[x, y].Draw(
                            batch,
                            BoardTopLeft + CellTopLeft,
                            LastRowTextureRect,
                            true);

                        // advance to the next cell
                        CellTopLeft.X += CELL_SIZE + CELL_PADDING;
                    }
                }

                // advance to the next row
                CellTopLeft.Y += CELL_SIZE + CELL_PADDING;
            }
        }

        // fill the board with solid cells when not active
        private void DrawInactiveGameBoard(SpriteBatch batch)
        {
            // temp variable to hold the top / left of each cell
            Vector2 CellTopLeft = Vector2.Zero;

            for (int y = 0; y < BoardHeight; y++) // for each row
            {
                for (int x = 0; x < BoardWidth; x++) // for each column
                {
                    // determine the top, left of this cell
                    CellTopLeft.X = x * (CELL_SIZE + CELL_PADDING);
                    CellTopLeft.Y = y * (CELL_SIZE + CELL_PADDING);

                    // draw the cell
                    m_Cells[x, y].Draw(
                        batch,
                        BoardTopLeft + CellTopLeft,
                        1);
                }
            }

            // reuse cell vector to draw centered game over text
            CellTopLeft.X = BoardWidth * (CELL_SIZE + CELL_PADDING) / 2 -
                m_GameOverRect.Width / 2;
            CellTopLeft.Y = BoardHeight * (CELL_SIZE + CELL_PADDING) / 2 -
                m_GameOverRect.Height / 2;

            // animate "game over" text
            CellTopLeft.Y += (float)Math.Sin(m_GameOverHover) *
                m_GameOverRect.Height;

            // draw "game over" text over the game board
            batch.Draw(
                m_TextureText,
                BoardTopLeft + CellTopLeft,
                m_GameOverRect,
                Color.White);
        }

        // fill board with background color
        private void DrawBoardBackground(SpriteBatch batch)
        {
            // encompass the cells and add a CELL_PADDING border
            Rectangle BoardRect = new Rectangle(
                (int)BoardTopLeft.X - CELL_PADDING,
                (int)BoardTopLeft.Y - CELL_PADDING,
                BoardWidth * (CELL_SIZE + CELL_PADDING) + CELL_PADDING,
                BoardHeight * (CELL_SIZE + CELL_PADDING) + CELL_PADDING
                );

            // actually draw the scaled rectangle
            batch.Draw(
                Cell.Texture,
                BoardRect,
                Cell.TextureRectCell,
                CellColors.Normal[CellColors.EmptyCellColorIndex]);

            // draw score
            batch.Draw(m_TextureText, m_ScoreLabelLoc,
                m_ScoreRect, Color.White);
            m_ScoreSprite.Draw(batch, m_ScoreValueLoc);

            // draw high score
            batch.Draw(m_TextureText, m_HighScoreLabelLoc,
                m_HighScoreRect, Color.White);
            m_HighScoreSprite.Draw(batch, m_HighScoreValueLoc);
        }

        // draw the cursor around the currently-selected cells
        private void DrawCursor(SpriteBatch batch)
        {
            // temp variable that we can tweak
            // send X as raw index into array,
            // send Y as screen coordinates.
            Vector2 Position = CursorPosition;

            // convert Y index to screen coordinate
            Position.Y *= (CELL_SIZE + CELL_PADDING);
            // vertically center cursor on cell 
            Position.Y += CELL_SIZE / 2 -
                Cell.TextureRectCursorLeft.Height / 2 - CELL_PADDING;
            // move cusor up as the cells move up
            Position.Y += CELL_SIZE + CELL_PADDING -
                (float)((CELL_SIZE + CELL_PADDING) *
                m_SecondsSinceLastRow / SecondsBeforeNewRow);

            // since texture is stored in Cell class, have it
            // draw the cursor for us
            Cell.DrawCursor(batch, BoardTopLeft, Position,
                m_CursorAge / CursorPulseInSeconds);
        }

        // texture for "game over" text and score labels
        private Texture2D m_TextureText;
        private Rectangle m_ScoreRect = new Rectangle(0, 16, 64, 16);
        private Rectangle m_HighScoreRect = new Rectangle(128, 0, 112, 24);
        private Rectangle m_GameOverRect = new Rectangle(0, 32, 160, 96);

        // support for displaying score
        private NumberSprite m_ScoreSprite = new NumberSprite();
        private Vector2 m_ScoreLabelLoc = Vector2.Zero;
        private Vector2 m_ScoreValueLoc = Vector2.Zero;

        // support for displaying high score
        private NumberSprite m_HighScoreSprite = new NumberSprite();
        private Vector2 m_HighScoreLabelLoc = Vector2.Zero;
        private Vector2 m_HighScoreValueLoc = Vector2.Zero;

        // "game over" hover cycle
        private double m_GameOverHover = 0;

        // load game textures
        public void LoadMedia(ContentManager content)
        {
            // cells and cursor
            Cell.Texture = content.Load<Texture2D>(@"media\cell");
            Cell.TextureRectCell = new Rectangle(1, 33, CELL_SIZE, CELL_SIZE);
            Cell.TextureRectCursorLeft = new Rectangle(0, 0, 5, 28);
            Cell.TextureRectCursorRight = new Rectangle(32, 0, 5, 28);

            // numbers for score
            NumberSprite.Texture = content.Load<Texture2D>(@"media\numbers");

            // miscellaneous text
            m_TextureText = content.Load<Texture2D>(@"media\text");

            // vertical center of game board, for locations of scores
            int BoardCenterY = (BoardHeight * (CELL_SIZE + CELL_PADDING)) / 2;

            // set locations for score labels
            m_ScoreLabelLoc.X = BoardTopLeft.X + BoardWidth * (CELL_SIZE + CELL_PADDING) + 64;
            m_ScoreLabelLoc.Y = BoardTopLeft.Y + BoardCenterY - m_ScoreRect.Height - 8;
            m_HighScoreLabelLoc.X = m_ScoreLabelLoc.X;
            m_HighScoreLabelLoc.Y = BoardTopLeft.Y + BoardCenterY + 8;

            // set locations for score values
            m_HighScoreValueLoc.X = m_HighScoreLabelLoc.X + m_HighScoreRect.Width + 8;
            m_HighScoreValueLoc.Y = m_HighScoreLabelLoc.Y;
            m_ScoreValueLoc.X = m_HighScoreValueLoc.X;
            m_ScoreValueLoc.Y = m_ScoreLabelLoc.Y;
        }
    }
}
