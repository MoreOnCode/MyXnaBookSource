// AI.cs
using System;
using System.Threading;
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
    public abstract class ArtificialIntelligence
    {
        // a default constructor
        public ArtificialIntelligence()
        {
        }

        // constructor with depth initialization
        public ArtificialIntelligence(int depth)
        {
            Depth = depth;
        }

        // used by UI to render progress bar
        protected double m_Status = 0;
        public double Status
        {
            get { return m_Status; }
        }
        
        // used by GameBoard to know when the AI is done taking its turn
        protected bool m_Done = true;
        public bool Done
        {
            get { return m_Done; }
        }

        // the move that the AI selected, as an index into GameBoard.ValidMoves[]
        protected int m_Move = 0;
        public int Move
        {
            get { return m_Move; }
        }

        // number of levels to recurse when searching all possible board combos
        protected int m_Depth = 4;
        public int Depth
        {
            get { return m_Depth; }
            set { m_Depth = value; }
        }

        // handy member to generate random values
        protected Random m_rand = new Random();

        // since the .NET Compact Framework doesn't support 
        // ParameterizedThreadStart, we need to handle passing
        // parameters on our own. (boooooo!)
        SelectMoveParams m_ThreadParam = new SelectMoveParams();

        // the public interface to other classes, kicks off threaded 
        // task, then returns to caller. caller polls AI.Done to see
        // if threaded task is done
        public void SelectMove(GameBoard board)
        {
            m_Done = false;
            m_Move = 0;
            m_Status = 0;
            m_ThreadParam.Board = board;
            m_ThreadParam.Depth = Depth;
            Thread task = new Thread(SelectMoveTask);
            task.Start();
        }

        // parameters for the threaded task
        public struct SelectMoveParams
        {
            public int Depth;
            public GameBoard Board;
        }

        // the method that is actually called when threading starts
        protected void SelectMoveTask()
        {
            m_Move = SelectMoveRecursive(m_ThreadParam.Board, m_ThreadParam.Depth);
            m_Status = 1;
            m_Done = true;
        }

        // helper method to generate a simple heuristic for a given GameBoard
        // add 1 for each piece owned by Player.One, subtract 1 for each piece 
        // owned by Player.Two. Larger sums favor Player.One, smaller sums
        // favor Player.Two.
        protected int Evaluate(GameBoard board)
        {
            return board.Score(Player.One) - board.Score(Player.Two);
        }

        // this method must be implemented by any actual AIs that derive 
        // from this base class. the threaded method calls this method, 
        // which only exists in derived classes. that way, this base
        // class can handle the nitty-gritty threading and synchronization
        // tasks, and leave the actual AI processing to the derived classes.
        protected abstract int SelectMoveRecursive(GameBoard board, int depth);
    }
}
