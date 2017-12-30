using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH18___Board_Game
{
    class MinMaxAI : ArtificialIntelligence
    {
        // constructor with depth initializer
        public MinMaxAI(int depth) : base(depth) { }

        // called by base class from seperate thread
        protected override int SelectMoveRecursive(GameBoard board, int depth)
        {
            // default to the first valid move
            int move = 0;

            // select min or max method based on player
            if (board.CurrentPlayer == Player.One)
            {
                int score = Max(board, Depth, ref move);
            }
            else if (board.CurrentPlayer == Player.Two)
            {
                int score = Min(board, Depth, ref move);
            }

            // return selected move (may have been updated by Min or Max)
            return move;
        }

        // examine every valid move, trying to maximize Player.One's score
        private int Max(GameBoard board, int depth, ref int move)
        {
            // have we recursed far enough?
            if (depth <= 0)
            {
                return Evaluate(board);
            }

            // start with rediculously low score so that first 
            // inspection will result in a match
            int score = int.MinValue;

            // for each ValidMove ...
            for (int index = 0; index < board.ValidMoves.Count; index++)
            {
                // report status if we're at top-most recursion depth
                if (depth == Depth)
                {
                    m_Status = (double)index / (double)board.ValidMoves.Count;
                }
                // copy current board
                GameBoard board2 = new GameBoard(board);
                // make move based on ValidMoves[index]
                board2.MakeMove(index);

                // get best move that opponent can make
                int val = Min(board2, depth - 1, ref move);
                if (val > score)
                {
                    // found a better score!
                    score = val;
                    if (depth == Depth)
                    {
                        // udpate move if this is top-level
                        move = index;
                    }
                }
            }

            // return best score found
            return score;
        }

        // examine every valid move, trying to minimize Player.Two's score
        // (remember, negative numbers are good for Player.Two)
        private int Min(GameBoard board, int depth, ref int move)
        {
            // have we recursed far enough?
            if (depth <= 0)
            {
                return Evaluate(board);
            }

            // start with rediculously high score so that first 
            // inspection will result in a match
            int score = int.MaxValue;

            // for each ValidMove ...
            for (int index = 0; index < board.ValidMoves.Count; index++)
            {
                // report status if we're at top-most recursion depth
                if (depth == Depth)
                {
                    m_Status = (double)index / (double)board.ValidMoves.Count;
                }
                // copy current board
                GameBoard board2 = new GameBoard(board);
                // make move based on ValidMoves[index]
                board2.MakeMove(index);

                // get best move that opponent can make
                int val = Max(board2, depth - 1, ref move);
                if (val < score)
                {
                    // found a better score!
                    score = val;
                    if (depth == Depth)
                    {
                        // udpate move if this is top-level
                        move = index;
                    }
                }
            }

            // return best score found
            return score;
        }
    }
}
