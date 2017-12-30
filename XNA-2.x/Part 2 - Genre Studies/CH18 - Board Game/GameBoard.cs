// GameBoard.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Chapter18
{
    // state of the current game
    public enum GameState
    {
        GameOver,
        Playing,
    }

    // enum to denote current player
    public enum Player
    {
        None,
        One,
        Two,
    }

    // type of player two
    public enum PlayerType
    {
        None,
        CPU_Random,
        CPU_Easy,
        CPU_MinMax,
        CPU_AlphaBeta,
        Human,
    }

    // the game board
    public class GameBoard
    {
        // default constructor
        public GameBoard()
        {
            InitGrid();
            ToggleCurrentPlayer();
        }

        // copy constructor
        public GameBoard(GameBoard board)
        {
            Copy(board);
        }

        // the current player
        private Player m_CurrentPlayer = Player.None;
        public Player CurrentPlayer
        {
            get { return m_CurrentPlayer; }
            set { m_CurrentPlayer = value; }
        }

        // type of the current player
        public PlayerType CurrentPlayerType()
        {
            return PlayerTypes[(int)CurrentPlayer];
        }

        // select previous player type for player two
        public void PreviousPlayerType()
        {
            int index = (int)PlayerTypes[(int)Player.Two] - 1;
            if (index <= 0) index = (int)PlayerType.Human;
            PlayerTypes[(int)Player.Two] = (PlayerType)index;
        }

        // select next player type for player two
        public void NextPlayerType()
        {
            int index = (int)PlayerTypes[(int)Player.Two] + 1;
            if (index > (int)PlayerType.Human) index = 1;
            PlayerTypes[(int)Player.Two] = (PlayerType)index;
        }

        // helper to know that: 1) player may have switched to a 
        // CPU player, 2) AI has not been fired yet
        private bool CpuMoveHasBeenStarted = false;
        
        // switch between Player.One and Player.Two
        public void ToggleCurrentPlayer()
        {
            // toggle player, then regenerate list of valid moves
            CurrentPlayer = OtherPlayer();
            UpdateListOfValidMoves();

            // if there aren't any valid moves ...
            if (ValidMoves.Count == 0)
            {
                // ... switch back to the original player
                CurrentPlayer = OtherPlayer();
                UpdateListOfValidMoves();

                // if there still aren't any valid moves, the game is over
                if (ValidMoves.Count == 0)
                {
                    State = GameState.GameOver;
                }
            }

            // if this player is a CPU player, note that it's ready
            // to kick off its AI processing thread
            CpuMoveHasBeenStarted = false;
        }

        // get the score for the specified player
        // rather than store the scores in member variables and try
        // to keep them in sync with the display, pull Value from 
        // the NumberSprite directly
        public int Score(Player player)
        {
            long score = 0;
            switch (player)
            {
                case Player.One:
                    score = m_ScoreOne.Value;
                    break;
                case Player.Two:
                    score = m_ScoreTwo.Value;
                    break;
            }
            return (int)score;
        }

        // the current game state
        private GameState m_State = GameState.Playing;
        public GameState State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        // helper used to rotate the gear (selection) graphics
        private double m_EffectAge = 0;

        // copy another board's state data to this board
        public void Copy(GameBoard board)
        {
            InitGrid(board.GridWidth, board.GridHeight);
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    Grid[x, y] = board.Grid[x, y];
                }
            }
            CurrentPlayer = board.CurrentPlayer;
            UpdateListOfValidMoves();
        }

        // single place to denote the top, left of the game grid
        private Vector2 TopLeft = new Vector2(48, 48);

        // the four AI types, and their recursion depths
        private ArtificialIntelligence[] m_ComputerOpponents = 
        {
            new RandomAI(1),
            new EasyAI(1),
            new MinMaxAI(5),
            new AlphaBetaAI(5),
        };

        // the current AI player (if any)
        public ArtificialIntelligence CurrentAI
        {
            get
            {
                ArtificialIntelligence ai = null;
                int indexAI = CurrentAiIndex;
                if (indexAI >= 0)
                {
                    ai = m_ComputerOpponents[indexAI];
                }
                return ai;
            }
        }

        // the current AI player's index (if any)
        protected int CurrentAiIndex
        {
            get
            {
                int indexAI = (int)CurrentPlayerType() - (int)PlayerType.CPU_Random;
                if (indexAI < 0 || indexAI >= m_ComputerOpponents.Length)
                {
                    indexAI = -1;
                }
                return indexAI;
            }
        }

        // texture rects for display of current player two
        protected Rectangle[] m_PlayerTypeRects =
        {
            Rectangle.Empty,              // none
            new Rectangle(64,  0, 64, 16), // cpu rand
            new Rectangle(64, 16, 64, 16), // cpu easy
            new Rectangle(64, 32, 64, 16), // cpu hard
            new Rectangle(64, 48, 64, 16), // cpu fast
            new Rectangle( 0, 48, 64, 16), // human
        };

        // select texture rect for current player two
        protected Rectangle PlayerTypeRect
        {
            get 
            {
                int index = (int)PlayerTypes[(int)Player.Two];
                return m_PlayerTypeRects[index];
            }
        }

        // available player types; only Player.Two may be a CPU player
        public PlayerType[] PlayerTypes = {
            PlayerType.None,       // placeholder
            PlayerType.Human,      // player 1
            PlayerType.CPU_MinMax, // player 2
        };

        // the game texture
        private static Texture2D m_Texture;
        public static Texture2D Texture
        {
            get { return m_Texture; }
            set { m_Texture = value; }
        }

        // gear graphic and a single, white pixel
        private static readonly Rectangle CursorRect = new Rectangle(32, 0, 32, 32);
        private static readonly Rectangle PixelRect = new Rectangle(0, 33, 1, 1);

        // width of the game board, in grid cells
        private int m_GridWidth = 12;
        public int GridWidth
        {
            get { return m_GridWidth; }
            set
            {
                if (value > 2 && value != GridWidth)
                {
                    m_GridWidth = value;
                    InitGrid();
                }
            }
        }

        // height of the game board, in grid cells
        private int m_GridHeight = 12;
        public int GridHeight
        {
            get { return m_GridHeight; }
            set
            {
                if (value > 2 && value != GridHeight)
                {
                    m_GridHeight = value;
                    InitGrid();
                }
            }
        }

        // the game grid
        private GamePiece[,] Grid;

        // init game grid with existing grid width and height
        private void InitGrid()
        {
            InitGrid(GridWidth, GridHeight);
        }

        // init game grid with specific grid width and height
        private void InitGrid(int width, int height)
        {
            // redimension array
            Grid = new GamePiece[width, height];
            
            // bypass setter logic
            m_GridWidth = width;
            m_GridHeight = height;

            // create new instance of a game piece for each cell in grid
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Grid[x, y] = new GamePiece();
                }
            }

            // calculate the center of the grid
            int cx = width / 2;
            int cy = height / 2;

            // set the starting game pieces
            Grid[cx - 1, cy - 1].Owner = Player.One;
            Grid[cx - 1, cy - 0].Owner = Player.Two;
            Grid[cx - 0, cy - 1].Owner = Player.Two;
            Grid[cx - 0, cy - 0].Owner = Player.One;

            // init CurrentPlayer so that next call to TogglePlayer will
            // pick Player.One
            CurrentPlayer = Player.None;
        }

        // move cursor to the next valid move
        public void NextValidMove()
        {
            Point current = new Point((int)m_Cursor.X, (int)m_Cursor.Y);
            int index = ValidMoves.IndexOf(current);
            index++;
            if (index >= ValidMoves.Count) index = 0;
            MoveCursorTo(index);
        }

        // move cursor to the previous valid move
        public void PreviousValidMove()
        {
            Point current = new Point((int)m_Cursor.X, (int)m_Cursor.Y);
            int index = ValidMoves.IndexOf(current);
            index--;
            if (index < 0) index = ValidMoves.Count - 1;
            MoveCursorTo(index);
        }

        // the current cursor locaion in grid units
        private Vector2 m_Cursor = Vector2.Zero;

        // move cursor to specified location
        public void MoveCursorTo(Point location)
        {
            m_Cursor = new Vector2(location.X, location.Y);
        }

        // move cursor to location indicated by index into ValidMoves array
        public void MoveCursorTo(int index)
        {
            if (index >= 0 && index < ValidMoves.Count)
            {
                m_Cursor = new Vector2(ValidMoves[index].X, ValidMoves[index].Y);
            }
        }

        // the heartbeat of the class
        public void Update(double elapsed)
        {
            // spin the gear icons
            m_EffectAge += elapsed;

            // call update on every game piece in the grid
            foreach (GamePiece piece in Grid)
            {
                piece.Update(elapsed);
            }

            // see if this player is a CPU player
            ArtificialIntelligence ai = CurrentAI;

            // if it is a CPU player ...
            if (ai != null)
            {
                /// ... if ready to start processing, kick off worker thread
                if (ai.Done && !CpuMoveHasBeenStarted )
                {
                    CpuMoveHasBeenStarted = true;
                    ai.SelectMove(this);
                }
                // otherwise, make the suggested move
                else if (ai.Done)
                {
                    MakeMove(ai.Move);
                }
            }
        }

        // draw all the components on the screen
        public void Draw(SpriteBatch batch)
        {
            DrawGrid(batch);
            DrawProgress(batch);
            DrawCursor(batch);
            DrawPieces(batch);
            DrawHUD(batch);
        }

        // draw the game board
        private void DrawGrid(SpriteBatch batch)
        {
            // big rect, draw grid border
            Rectangle borderRect = new Rectangle(
                (int)TopLeft.X - 2, 
                (int)TopLeft.Y - 2, 
                GridWidth * 32 + 4, 
                GridHeight * 32 + 4);
            batch.Draw(Texture, borderRect, PixelRect, Color.Black);

            // size of a single grid cell
            Rectangle cellRect = new Rectangle(0, 0, 32, 32);
            
            // alternating colors for grid cells
            Color[] colors = {
                Color.CornflowerBlue,
                Color.RoyalBlue,
                };

            for (int y = 0; y < GridHeight; y++) // for each row
            {
                cellRect.Y = y * 32 + (int)TopLeft.Y;
                for (int x = 0; x < GridWidth; x++) // for each column
                {
                    cellRect.X = x * 32 + (int)TopLeft.X;
                    batch.Draw(Texture, cellRect, PixelRect, colors[(x + y) % 2]);
                }
            }
        }

        // if CPU player is working, show progress on screen
        private void DrawProgress(SpriteBatch batch)
        {
            if (State == GameState.Playing)
            {
                // assume zero progress
                double progress = 0;

                // grab progress from current CPU player, if any
                ArtificialIntelligence ai = CurrentAI;
                if (ai != null)
                {
                    progress = ai.Status;
                }

                // actually draw progress bar
                Rectangle rect = new Rectangle(
                    (int)TopLeft.X,
                    (int)TopLeft.Y + GridHeight * 32,
                    (int)Math.Round(progress * (GridWidth * 32)),
                    2);
                batch.Draw(Texture, rect, PixelRect, Color.White);
            }
        }

        // draw any existing game pieces
        private void DrawPieces(SpriteBatch batch)
        {
            Vector2 position = Vector2.Zero;
            for (int y = 0; y < GridHeight; y++) // for each row
            {
                position.Y = TopLeft.Y + y * 32;
                for (int x = 0; x < GridWidth; x++) // for each column
                {
                    position.X = TopLeft.X + x * 32;
                    Grid[x, y].Draw(batch, position);
                }
            }
        }

        // render valid move hints in blue, and current selection in white
        private void DrawCursor(SpriteBatch batch)
        {
            Point current = Point.Zero;
            Vector2 position = Vector2.Zero;
            Vector2 center = new Vector2(16, 16);

            for (int y = 0; y < GridHeight; y++) // for each row
            {
                current.Y = y;
                position.Y =
                    TopLeft.Y + // top of the grid
                    y * 32 +    // grid position of this cell
                    center.Y;   // plus rotation offset
                
                for (int x = 0; x < GridWidth; x++) // for each column
                {
                    current.X = x;
                    position.X = 
                        TopLeft.X + // left of the grid
                        x * 32 +    // grid position of this cell
                        center.X;   // plus rotation offset

                    // highlight this cell if it's currently selected by human player
                    bool highlight = current.X == m_Cursor.X;
                    highlight &= current.Y == m_Cursor.Y;
                    highlight &= CurrentPlayerType() == PlayerType.Human;
                    
                    if (highlight)
                    {
                        batch.Draw(
                            Texture,            // cursor texture
                            position,           // cursor x, y
                            CursorRect,         // cursor source rect
                            Color.White,        // color
                            (float)m_EffectAge, // cursor rotation
                            center,             // center of cursor
                            1.0f,               // don't scale
                            SpriteEffects.None, // no effect
                            0.0f);              // topmost layer
                    }
                    else if(ValidMoves.Contains(current))
                    {
                        batch.Draw(
                            Texture,            // cursor texture
                            position,           // cursor x, y
                            CursorRect,         // cursor source rect
                            Color.Navy,         // color
                            (float)m_EffectAge, // cursor rotation
                            center,             // center of cursor
                            1.0f,               // don't scale
                            SpriteEffects.None, // no effect
                            0.0f);              // topmost layer
                    }
                }
            }
        }

        // score for player one and player two
        NumberSprite m_ScoreOne = new NumberSprite();
        NumberSprite m_ScoreTwo = new NumberSprite();

        // extra pieces for HUD
        GamePiece m_PieceOne = new GamePiece(Player.One);
        GamePiece m_PieceTwo = new GamePiece(Player.Two);

        // render the heads up display
        private void DrawHUD(SpriteBatch batch)
        {
            Vector2 position = Vector2.Zero;
            Vector2 center = new Vector2(16, 16);

            position.X = 
                TopLeft.X +      // left of grid
                GridWidth * 32 + // right of grid
                2 +              // plus border
                32;              // plus some space
            position.Y = 
                TopLeft.Y +      // top of grid
                GridWidth * 16 - // middle of grid
                32;              // minus one row (since HUD takes up two rows)

            // draw gear around player one's HUD piece?
            if (State == GameState.Playing && CurrentPlayer == Player.One)
            {
                position += center - Vector2.One;
                batch.Draw(
                    Texture,            // cursor texture
                    position,           // cursor x, y
                    CursorRect,         // cursor source rect
                    Color.White,        // color
                    (float)m_EffectAge, // cursor rotation
                    center,             // center of cursor
                    1.0f,               // don't scale
                    SpriteEffects.None, // no effect
                    0.0f);              // topmost layer
                position -= center - Vector2.One;
            }
            // draw player one's HUD piece?
            batch.Draw(Texture, position, GamePiece.PieceRect, GamePiece.Colors[1]);

            // draw gear around player two's HUD piece?
            position.Y += 32;
            if (State == GameState.Playing && CurrentPlayer == Player.Two)
            {
                position += center;
                batch.Draw(
                    Texture,            // cursor texture
                    position,           // cursor x, y
                    CursorRect,         // cursor source rect
                    Color.White,        // color
                    (float)m_EffectAge, // cursor rotation
                    center,             // center of cursor
                    1.0f,               // don't scale
                    SpriteEffects.None, // no effect
                    0.0f);              // topmost layer
                position -= center;
            }
            // draw player two's HUD piece?
            batch.Draw(Texture, position, GamePiece.PieceRect, GamePiece.Colors[2]);

            // draw scores
            position.Y -= 26;
            position.X += 32;
            m_ScoreOne.Draw(batch, position);
            position.Y += 32;
            m_ScoreTwo.Draw(batch, position);

            if (PlayerTypeRect != Rectangle.Empty)
            {
                position.X -= 16;
                position.Y += 32;
                batch.Draw(Texture, position, PlayerTypeRect, Color.White);
            }
        }

        // list of valid moves for this player and board
        public List<Point> ValidMoves = new List<Point>();
        public void UpdateListOfValidMoves()
        {
            // off-grid, start with invalid cursor position
            m_Cursor = -Vector2.One;

            // clear current valid moves
            Vector2 position = Vector2.Zero;
            ValidMoves.Clear();

            // don't bother scanning if no one's playing
            if (CurrentPlayer == Player.None) return;

            // reset scores
            int score1 = 0;
            int score2 = 0;

            for (int y = 0; y < GridHeight; y++) // for each row
            {
                position.Y = y;
                for (int x = 0; x < GridWidth; x++) // for each column
                {
                    position.X = x;
                    if (Grid[x, y].Owner == Player.None)
                    {
                        // rules for valid move: 1) empty cell, 2) next to 
                        // opponent's piece, 3) scan straight-line reveals
                        // piece owned by current player
                        if (Scan(position, CursorDirection.North) ||
                            Scan(position, CursorDirection.NorthEast) ||
                            Scan(position, CursorDirection.East) ||
                            Scan(position, CursorDirection.SouthEast) ||
                            Scan(position, CursorDirection.South) ||
                            Scan(position, CursorDirection.SouthWest) ||
                            Scan(position, CursorDirection.West) ||
                            Scan(position, CursorDirection.NorthWest))
                        {
                            ValidMoves.Add(new Point(x, y));
                        }
                    }
                    else if (Grid[x, y].Owner == Player.One)
                    {
                        score1++;
                    }
                    else
                    {
                        score2++;
                    }
                }
            }

            // if there's at least one valid move, locate the cursor on
            // the first valid move in the list
            if (ValidMoves.Count > 0)
            {
                MoveCursorTo(ValidMoves[0]);
            }

            // update the NumberSprites with the new scores
            m_ScoreOne.Value = score1;
            m_ScoreTwo.Value = score2;
        }

        // scan without marking
        private bool Scan(Vector2 position, Vector2 direction)
        {
            return Scan(position, direction, false);
        }

        // scan, possibly commiting move to game state
        private bool Scan(Vector2 position, Vector2 direction, bool mark)
        {
            // assume caller verified that first cell is empty
            // move to next cell in scan ...
            Player other = OtherPlayer();
            position += direction;
            int count = 0;

            // ... and start matching opponent cells
            while (InBounds(position) && GetPiece(position).Owner == other)
            {
                if (mark)
                {
                    Grid[(int)position.X,(int)position.Y].Owner = CurrentPlayer;
                }
                count++;
                position += direction;
            }

            // return result as boolean
            return
                count > 0 &&
                InBounds(position) &&
                GetPiece(position).Owner == CurrentPlayer;
        }

        // commit currently-selected cell as player's move
        public void MakeMove()
        {
            Point current = new Point((int)m_Cursor.X, (int)m_Cursor.Y);
            int index = ValidMoves.IndexOf(current);
            MakeMove(index);
        }

        // select a move from the ValidMove list, useful for CPU players
        public void MakeMove(int index)
        {
            if (index >= 0 && index < ValidMoves.Count)
            {
                Point move = ValidMoves[index];
                Vector2 position = new Vector2(move.X,move.Y);

                // mark current cell as belonging to the current player
                Grid[move.X, move.Y].Owner = CurrentPlayer;

                // scan each direction, if valid, claim it
                if (Scan(position, CursorDirection.North))
                {
                    Scan(position, CursorDirection.North, true);
                }
                if (Scan(position, CursorDirection.NorthEast))
                {
                    Scan(position, CursorDirection.NorthEast, true);
                }
                if (Scan(position, CursorDirection.East))
                {
                    Scan(position, CursorDirection.East, true);
                }
                if (Scan(position, CursorDirection.SouthEast))
                {
                    Scan(position, CursorDirection.SouthEast, true);
                }
                if (Scan(position, CursorDirection.South))
                {
                    Scan(position, CursorDirection.South, true);
                }
                if (Scan(position, CursorDirection.SouthWest))
                {
                    Scan(position, CursorDirection.SouthWest, true);
                }
                if (Scan(position, CursorDirection.West))
                {
                    Scan(position, CursorDirection.West, true);
                }
                if (Scan(position, CursorDirection.NorthWest))
                {
                    Scan(position, CursorDirection.NorthWest, true);
                }

                // move has been committed, switch players
                ToggleCurrentPlayer();
            }
        }

        // if the specified cell within the grid?
        private bool InBounds(Vector2 position)
        {
            return
                position.X >= 0 &&
                position.X < GridWidth &&
                position.Y >= 0 &&
                position.Y < GridHeight;
        }

        // get a reference to CurrentPlayer's opponent
        private Player OtherPlayer()
        {
            Player player = Player.None;
            if (CurrentPlayer == Player.One)
            {
                player = Player.Two;
            }
            else 
            {
                player = Player.One;
            }
            return player;
        }

        // get a game piece, given a vector (rather than [x,y])
        public GamePiece GetPiece(Vector2 position)
        {
            return Grid[(int)position.X, (int)position.Y];
        }
    }

    // simple "constants" for scanning grid cells
    public sealed class CursorDirection
    {
        public static readonly Vector2 North     = new Vector2(  0, -1);
        public static readonly Vector2 NorthEast = new Vector2(  1, -1);
        public static readonly Vector2 East      = new Vector2(  1,  0);
        public static readonly Vector2 SouthEast = new Vector2(  1,  1);
        public static readonly Vector2 South     = new Vector2(  0,  1);
        public static readonly Vector2 SouthWest = new Vector2( -1,  1);
        public static readonly Vector2 West      = new Vector2( -1,  0);
        public static readonly Vector2 NorthWest = new Vector2( -1, -1);
    }
}