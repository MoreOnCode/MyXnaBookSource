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
    class Cell
    {
        // is this cell falling?
        private bool m_IsFalling = false;
        public bool IsFalling
        {
            get { return m_IsFalling; }
            set
            {
                m_IsFalling = value;
                m_FallAge = 0;
            }
        }

        // time (in seconds) this cell has been falling
        private double m_FallAge = 0;
        public double FallAge
        {
            get { return m_FallAge; }
            set { m_FallAge = value; }
        }

        // time (in seconds) it takes to fall a single row
        private double m_FallRate = 0.25;
        public double FallRate
        {
            get { return m_FallRate; }
            set { m_FallRate = value; }
        }

        // is this cell clearing?
        private bool m_IsClearing = false;
        public bool IsClearing
        {
            get { return m_IsClearing; }
            set
            {
                m_IsClearing = value;
                m_ClearAge = 0;
            }
        }

        // initialize this cell (empty, not falling, not clearing)
        public void Reset()
        {
            ColorIndex = CellColors.EmptyCellColorIndex;
            IsClearing = false;
            IsFalling = false;
        }

        // copy another cell's state to this cell
        public void Copy(Cell cell)
        {
            // copy public properties
            ColorIndex = cell.ColorIndex;
            IsClearing = cell.IsClearing;
            IsFalling = cell.IsFalling;

            // also need to copy state data since Cell.Copy() may have 
            // been called from GameBoard.NewRow() and reseting these 
            // values will make the gravity animation seem choppy
            m_ClearAge = cell.m_ClearAge;
            m_FallAge = cell.m_FallAge;
        }

        // time (in seconds) this cell has been clearing
        private double m_ClearAge = 0;

        // time between flashes (in seconds) while clearing
        private double m_ClearFlashRate = 0.125;
        public double ClearFlashRate
        {
            get { return m_ClearFlashRate; }
            set { m_ClearFlashRate = value; }
        }

        // time (in seconds) it takes a cell to clear
        private double m_ClearDuration = 1;
        public double ClearDuration
        {
            get { return m_ClearDuration; }
            set { m_ClearDuration = value; }
        }

        // the index into CellColors.Normal and CellColors.Dark for this cell
        private int m_ColorIndex = CellColors.EmptyCellColorIndex;
        public int ColorIndex
        {
            get { return m_ColorIndex; }
            set { m_ColorIndex = value; }
        }

        // the source texture
        private static Texture2D m_Texture;
        public static Texture2D Texture
        {
            get { return m_Texture; }
            set { m_Texture = value; }
        }

        // location within the texture of our left cursor
        private static Rectangle m_TextureRectCursorLeft = Rectangle.Empty;
        public static Rectangle TextureRectCursorLeft
        {
            get { return m_TextureRectCursorLeft; }
            set { m_TextureRectCursorLeft = value; }
        }

        // location within the texture of our right cursor
        private static Rectangle m_TextureRectCursorRight = Rectangle.Empty;
        public static Rectangle TextureRectCursorRight
        {
            get { return m_TextureRectCursorRight; }
            set { m_TextureRectCursorRight = value; }
        }

        // location within the texture of our cell (pure white square)
        private static Rectangle m_TextureRectCell = Rectangle.Empty;
        public static Rectangle TextureRectCell
        {
            get { return m_TextureRectCell; }
            set { m_TextureRectCell = value; }
        }

        // size of each cell (assumes square cells)
        public const int CELL_SIZE = 20;

        // space between each cell
        public const int CELL_PADDING = 2;

        // update cell state, based on elapsed time (in seconds)
        public void Update(double elapsed)
        {
            // update clearing state
            if (IsClearing)
            {
                m_ClearAge += elapsed;
                if (m_ClearAge > ClearDuration)
                {
                    // we're done, this cell is officially gone
                    IsClearing = false;
                    IsFalling = false;
                    ColorIndex = CellColors.EmptyCellColorIndex;
                }
            }

            // update falling state
            if (IsFalling)
            {
                // falling is managed by GameBoard, just update the age here
                m_FallAge += elapsed;
            }
        }

        // draw this cell
        public void Draw(SpriteBatch batch, Vector2 location, Rectangle texRect, bool dark)
        {
            // don't bother drawing empty cells
            if (ColorIndex != CellColors.EmptyCellColorIndex)
            {
                // if the cell is falling, proportionally offset towards next row
                if (IsFalling)
                {
                    location.Y += (float)Math.Round((CELL_SIZE + CELL_PADDING) *
                        (FallAge / FallRate));
                }

                // assume this is a normal cell
                Color tint = CellColors.Normal[ColorIndex];

                // flash clearing cells
                if (IsClearing)
                {
                    // toggle color every time ClearFlashRate seconds pass by
                    bool flash = (int)(m_ClearAge / ClearFlashRate) % 2 == 1;
                    if (flash)
                    {
                        tint = Color.White;
                    }
                }
                else if (dark)
                {
                    // draw (unplayable) cells on the bottom row a little darker
                    tint = CellColors.Dark[ColorIndex];
                }

                // actually draw the cell
                batch.Draw(
                    Texture,
                    location,
                    texRect,
                    tint);
            }
        }

        // draw full cell in specified color, used by game over screen
        public void Draw(SpriteBatch batch, Vector2 location, int color)
        {
            batch.Draw(
                Texture,
                location,
                TextureRectCell,
                CellColors.Normal[color]);
        }

        // draw the cursor around the currently-selected cells
        public static void DrawCursor(SpriteBatch batch, Vector2 BoardTopLeft, Vector2 CursorPosition, double pulse)
        {
            // create local copy of position to play with, pulsing the X component
            Vector2 position = CursorPosition;

            // convert X index to screen coordinate
            position.X *= (CELL_SIZE + CELL_PADDING);
            // move cusor just to left of cell
            position.X -= TextureRectCursorLeft.Width;
            // pulse cursor
            position.X += (float)Math.Cos(2 * MathHelper.Pi * pulse);

            // round to the nearest pixel
            position.X = (int)Math.Round(position.X);
            position.Y = (int)Math.Round(position.Y);

            // draw left cursor
            batch.Draw(
                Texture,
                BoardTopLeft + position,
                TextureRectCursorLeft,
                Color.White);

            // reset X index
            position.X = CursorPosition.X + 2;
            // convert X index to screen coordinate
            position.X *= (CELL_SIZE + CELL_PADDING);
            // pulse cursor
            position.X -= (float)Math.Cos(2 * MathHelper.Pi * pulse) + CELL_PADDING;

            // round to the nearest pixel
            position.X = (int)Math.Round(position.X);
            position.Y = (int)Math.Round(position.Y);

            // draw right cursor
            batch.Draw(
                Texture,
                BoardTopLeft + position,
                TextureRectCursorRight,
                Color.White);
        }
    }
}
