using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH18___Board_Game
{
    class AlphaBetaAI : ArtificialIntelligence
    {
        // constructor with depth initializer
        public AlphaBetaAI(int depth) : base(depth) { }

        // called by base class from seperate thread
        protected override int SelectMoveRecursive(GameBoard board, int depth)
        {
            // default to the first valid move
            int move = 0;

            // select min or max method based on player
            if (board.CurrentPlayer == Player.One)
            {
                // start with rediculously low alpha and high beta so that first 
                // inspection will result in a match
                int score = Max(board, Depth, int.MinValue, int.MaxValue, ref move);
            }
            else if (board.CurrentPlayer == Player.Two)
            {
                // start with rediculously low alpha and high beta so that first 
                // inspection will result in a match
                int score = Min(board, Depth, int.MinValue, int.MaxValue, ref move);
            }

            // return selected move (may have been updated by Min or Max)
            return move;
        }

        // examine every valid move, trying to maximize Player.One's score
        private int Max(GameBoard board, int depth, int alpha, int beta, ref int move)
        {
            // have we recursed far enough?
            if (depth <= 0)
            {
                return Evaluate(board);
            }

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
                int val = Min(board2, depth - 1, alpha, beta, ref move);

                if (val >= beta)
                {
                    // already found a better branch than this can possibly be
                    return beta;
                }

                if (val > alpha)
                {
                    // found a better score!
                    alpha = val;
                    if (depth == Depth)
                    {
                        // udpate move if this is top-level
                        move = index;
                    }
                }
            }

            // return best score found
            return alpha;
        }

        // examine every valid move, trying to minimize Player.Two's score
        // (remember, negative numbers are good for Player.Two)
        private int Min(GameBoard board, int depth, int alpha, int beta, ref int move)
        {
            // have we recursed far enough?
            if (depth <= 0)
            {
                return Evaluate(board);
            }

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
                int val = Max(board2, depth - 1, alpha, beta, ref move);

                if (val <= alpha)
                {
                    // already found a better branch than this can possibly be
                    return alpha;
                }

                if (val < beta)
                {
                    // found a better score!
                    beta = val;
                    if (depth == Depth)
                    {
                        // udpate move if this is top-level
                        move = index;
                    }
                }
            }

            // return best score found
            return beta;
        }
    }
}
