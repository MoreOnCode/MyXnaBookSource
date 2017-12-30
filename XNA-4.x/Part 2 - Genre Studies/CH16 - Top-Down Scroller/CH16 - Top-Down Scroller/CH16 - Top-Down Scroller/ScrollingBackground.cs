using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CH16___Top_Down_Scroller
{
    public class ScrollingBackground
    {
        // tile sets fall into three categories:
        //   Fill - tiles that are one type
        //   ToGrass - tiles whose top half is grass
        //   FromGrass - tiles whose bottom half is grass
        protected List<Rectangle[]> m_FillTiles = new List<Rectangle[]>();
        protected List<Rectangle[]> m_ToGrassTiles = new List<Rectangle[]>();
        protected List<Rectangle[]> m_FromGrassTiles = new List<Rectangle[]>();

        // initialize texture rects and fill tile array with grass tiles
        public void InitTiles()
        {
            // init texture rects for the tiles
            InitTilesHelper.InitFillTiles(m_FillTiles);
            InitTilesHelper.InitToGrassTiles(m_ToGrassTiles);
            InitTilesHelper.InitFromGrassTiles(m_FromGrassTiles);

            // fill screen with grass
            RowsOfCurrentTypeRemaining = 6;
            CurrentType = 0;
            NextRow();
            NextRow();
            NextRow();
            NextRow();
            NextRow();
            NextRow();
        }

        // helper to save a little typing
        public static readonly Rectangle EmptyRect = Rectangle.Empty;

        // index into m_TileRects array, first row
        protected int FirstRowIndex = 0;

        // the tiles that are currently on the screen
        protected Rectangle[,] m_TileRects = new Rectangle[5, 6]
        {
            {EmptyRect, EmptyRect, EmptyRect, EmptyRect, EmptyRect, EmptyRect },
            {EmptyRect, EmptyRect, EmptyRect, EmptyRect, EmptyRect, EmptyRect },
            {EmptyRect, EmptyRect, EmptyRect, EmptyRect, EmptyRect, EmptyRect },
            {EmptyRect, EmptyRect, EmptyRect, EmptyRect, EmptyRect, EmptyRect },
            {EmptyRect, EmptyRect, EmptyRect, EmptyRect, EmptyRect, EmptyRect },
        };

        // minimum number of rows that a tile type will be shown
        protected int m_MinTypeLength = 5;
        public int MinTypeLength
        {
            get { return m_MinTypeLength; }
            set { m_MinTypeLength = value; }
        }

        // maximum number of rows that a tile type will be shown
        protected int m_MaxTypeLength = 20;
        public int MaxTypeLength
        {
            get { return m_MaxTypeLength; }
            set { m_MaxTypeLength = value; }
        }

        // speed at which new rows are introduced (in seconds)
        protected double m_RowsPerSecond = 0.75;
        public double RowsPerSecond
        {
            get { return m_RowsPerSecond; }
            set { m_RowsPerSecond = value; }
        }

        // our "in-between" tile, all tiles transiton from and to this type
        protected const int GRASS_TILE = 0;

        // how many rows of the current type remain?
        protected int RowsOfCurrentTypeRemaining = 0;

        // start with grass tiles
        protected int CurrentType = GRASS_TILE;

        // helper member to generate random numbers
        Random m_rand = new Random();

        // introduce a new row of tiles
        protected void NextRow()
        {
            // will ultimately point to the textures for the next row's tiles
            Rectangle[] sourceRects;

            // is it time to pick a new tile type?
            if (RowsOfCurrentTypeRemaining == 0)
            {
                // randomly pick the next tile type
                int nextType = m_rand.Next(m_FillTiles.Count - 1) + 1;
                if (Game1.GameOver || CurrentType != GRASS_TILE)
                {
                    // if the game is over or the current tile isn't grass,
                    // the next tile type should be grass
                    nextType = GRASS_TILE;
                }

                if (!Game1.GameOver)
                {
                    // how many rows of this new type will we display?
                    RowsOfCurrentTypeRemaining = MinTypeLength +
                        m_rand.Next(MaxTypeLength - MinTypeLength);
                }

                if (CurrentType == GRASS_TILE)
                {
                    // we're transitioning away from grass tiles
                    sourceRects = m_FromGrassTiles[nextType];
                }
                else
                {
                    // we're transitioning to grass tiles
                    sourceRects = m_ToGrassTiles[CurrentType];
                }
                CurrentType = nextType;
            }
            else
            {
                // it's not time for a new tile, create next from from 
                // the existing tiles
                sourceRects = m_FillTiles[CurrentType];
            }

            // our "first row" index moves through the tile rects array
            // that way, we don't need to keep moving the existing tiles
            // in the array as they move down the screen, we just point 
            // to a new "first row".
            FirstRowIndex = (FirstRowIndex + 1) % 6;

            // fill the new row with our new tiles
            for (int x = 0; x < 5; x++)
            {
                m_TileRects[x, FirstRowIndex] =
                    // randomly select a tile from the available source tiles
                    sourceRects[m_rand.Next(sourceRects.Length)];
            }

            // decrement the "rows remaining" count
            RowsOfCurrentTypeRemaining -= 1;

            // if we're done with this tile type, get ready for a new type
            if (RowsOfCurrentTypeRemaining < 0)
            {
                RowsOfCurrentTypeRemaining = 0;
            }
        }

        // time in seconds spent on the current row, when this 
        // value exceeds RowsPerSecond, it's time for a new row
        protected double TimeSpentOnCurrentRow = 0;

        // keep track of how long we've been on the current row,
        // generate a new row when it's time
        public void Update(double elapsed)
        {
            TimeSpentOnCurrentRow += elapsed;
            if (TimeSpentOnCurrentRow >= RowsPerSecond)
            {
                TimeSpentOnCurrentRow -= RowsPerSecond;
                NextRow();
            }
        }

        // draw all the tiles
        public void Draw(SpriteBatch batch)
        {
            // position to draw each tile
            Vector2 pos = Vector2.Zero;

            // offset from top, based on time the 
            // current row has been on the screen
            double dy = (TimeSpentOnCurrentRow / RowsPerSecond) * 128.0 - 256.0;

            // for each row of tiles
            for (int y = 0; y < 6; y++)
            {
                // y location of this row, in pixels
                pos.Y = (float)((6 - y) * 128.0 + dy);

                // index into m_TileRects array for this row
                int row = (FirstRowIndex + y) % 6;

                // for each column of tiles
                for (int x = 0; x < 5; x++)
                {
                    // x location of this tile, in pixels
                    pos.X = x * 128;

                    // actually draw the current tile
                    batch.Draw(Game1.Texture, pos,
                        m_TileRects[x, row], Color.White);
                }
            }
        }
    }

    // simple helper class to note where each 
    // tile lives in the master game texture
    public class InitTilesHelper
    {
        public static void InitFillTiles(List<Rectangle[]> list)
        {
            list.Clear();
            list.Add(m_GrassRects);
            list.Add(m_TreeRects);
            list.Add(m_RockRects);
            list.Add(m_StoneRects);
        }

        public static void InitToGrassTiles(List<Rectangle[]> list)
        {
            list.Clear();
            list.Add(m_GrassRects); // cheat, same tiles
            list.Add(m_TreeRectsToGrass);
            list.Add(m_RockRectsToGrass);
            list.Add(m_StoneRectsToGrass);
        }

        public static void InitFromGrassTiles(List<Rectangle[]> list)
        {
            list.Clear();
            list.Add(m_GrassRects); // cheat, same tiles
            list.Add(m_TreeRectsFromGrass);
            list.Add(m_RockRectsFromGrass);
            list.Add(m_StoneRectsFromGrass);
        }

        private static Rectangle[] m_GrassRects = 
        {
            new Rectangle(000,000,128,128),
            new Rectangle(128,000,128,128),
            new Rectangle(256,000,128,128),
            new Rectangle(384,000,128,128),
        };

        private static Rectangle[] m_TreeRects = 
        {
            new Rectangle(000,128,128,128),
            new Rectangle(128,128,128,128),
            new Rectangle(256,128,128,128),
            new Rectangle(384,128,128,128),
            new Rectangle(512,128,128,128),
            new Rectangle(640,128,128,128),
            new Rectangle(768,128,128,128),
        };

        private static Rectangle[] m_TreeRectsToGrass = 
        {
            new Rectangle(512,000,128,128),
        };

        private static Rectangle[] m_TreeRectsFromGrass = 
        {
            new Rectangle(256,256,128,128),
        };

        private static Rectangle[] m_RockRects = 
        {
            new Rectangle(000,256,128,128),
            new Rectangle(128,256,128,128),
        };

        private static Rectangle[] m_RockRectsToGrass = 
        {
            new Rectangle(640,000,128,128),
            new Rectangle(768,000,128,128),
        };

        private static Rectangle[] m_RockRectsFromGrass = 
        {
            new Rectangle(384,256,128,128),
            new Rectangle(256,384,128,128),
        };

        private static Rectangle[] m_StoneRects = 
        {
            new Rectangle(000,384,128,128),
            new Rectangle(128,384,128,128),
        };

        private static Rectangle[] m_StoneRectsToGrass = 
        {
            new Rectangle(896,000,128,128),
        };

        private static Rectangle[] m_StoneRectsFromGrass = 
        {
            new Rectangle(384,384,128,128),
        };
    }
}
