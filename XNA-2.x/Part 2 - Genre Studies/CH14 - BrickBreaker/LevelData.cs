// LevelData.cs
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter14
{
    public class LevelData
    {
        // has the data changed? (know when to save)
        protected bool m_Changed = false;
        public bool Changed
        {
            get 
            {
                // check level data
                bool changed = m_Changed;

                // check each brick in this level
                foreach (Brick b in Bricks)
                {
                    // stop looking if we detect any changes
                    changed |= b.Changed;
                    if (changed) { break; }
                }
                return changed; 
            }
            set
            {
                m_Changed = value;

                if (!m_Changed)
                {
                    // if the editor is reseting the changed flag,
                    // reset all of the bricks in the level as well
                    foreach (Brick b in Bricks)
                    {
                        b.Changed = false;
                    }
                }
            }
        }

        // width of a brick (in pixels) for this level
        protected int m_BrickWidth = 40;
        public int BrickWidth
        {
            get { return m_BrickWidth; }
            set
            {
                Changed |= (m_BrickHeight == value);
                m_BrickWidth = value;
            }
        }

        // half width is useful for grid functions
        public int HalfWidth
        {
            get { return m_BrickWidth / 2; }
        }

        // height of a brick (in pixels) for this level
        protected int m_BrickHeight = 20;
        public int BrickHeight
        {
            get { return m_BrickHeight; }
            set
            {
                Changed |= (m_BrickHeight == value);
                m_BrickHeight = value;
            }
        }

        // half height is useful for grid functions
        public int HalfHeight
        {
            get { return m_BrickHeight / 2; }
        }

        // the collection of bricks for this level
        protected List<Brick> m_Bricks = new List<Brick>();
        public List<Brick> Bricks
        {
            get { return m_Bricks; }
        }

        // given a point on the play area, identiry the 
        // brick at that location, if any
        public Brick FindBrick(int x, int y)
        {
            Brick brick = null;
            foreach (Brick b in Bricks)
            {
                if (
                    x >= b.X && 
                    x < b.X + BrickWidth && 
                    y >= b.Y && 
                    y < b.Y + BrickHeight)
                {
                    // given point lies within this brick
                    brick = b;
                    break;
                }
            }
            return brick;
        }

        // add a new brick to the level, given the coordinate 
        // of the a top, left corner, knowing the width and
        // height of all bricks on this level
        public Brick AddBrick(int x, int y, int hits)
        {
            Brick brick = null;
            if (x >= 0 && y >= 0)
            {
                brick = new Brick();
                brick.X = x;
                brick.Y = y;
                brick.HitsToClear = hits;
                Bricks.Add(brick);
                Changed = true;
            }
            return brick;
        }

        // remove the specified brick from this level's collection
        public void DeleteBrick(Brick brick)
        {
            if (Bricks.Contains(brick))
            {
                Bricks.Remove(brick);
                Changed = true;
            }
        }

        // override used to serialize this level to a file stream
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(BrickWidth).Append("|")
              .Append(BrickHeight).Append("|");
            foreach(Brick brick in Bricks){
              sb.Append(brick.ToString());
            }
            return sb.ToString();
        }

        // accept defaults: bricks are 40x20, no bricks in the level
        // this might be useful for creating levels from code
        public LevelData() : base()
        {
        }

        // overload used to deserialize level data from a file
        public LevelData(string data)
            : base()
        {
            // the first two tokens are the width and height
            string[] tokens = data.Split("|".ToCharArray());
            if (tokens.Length > 1)
            {
                BrickWidth = int.Parse(tokens[0]);
                BrickHeight = int.Parse(tokens[1]);
            }

            // the remaining tokens are brick triplets
            // skip first two tokens, we've already extracted them
            int index = 2; 

            // as long as more tokens remiain, process them
            while (index < tokens.Length)
            {
                // brick data is stored as a triplet: 
                // Width, Height, and NumHitsToClear
                if (index + 2 < tokens.Length)
                {
                    Brick brick = AddBrick(
                        int.Parse(tokens[index + 0]),
                        int.Parse(tokens[index + 1]),
                        int.Parse(tokens[index + 2]));
                    brick.Changed = false;
                }

                // move on to the next three tokens
                index += 3;
            }

            // reset the changed flag
            Changed = false;
        }
    }
}
