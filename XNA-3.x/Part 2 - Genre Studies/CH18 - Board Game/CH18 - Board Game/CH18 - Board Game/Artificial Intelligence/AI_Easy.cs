using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH18___Board_Game
{
    class EasyAI : ArtificialIntelligence
    {
        // constructor with depth initializer
        public EasyAI(int depth) : base(depth) { }

        // called by base class from seperate thread
        protected override int SelectMoveRecursive(GameBoard board, int depth)
        {
            // assume first move is the best
            int move = 0;

            // best score, based on player
            if (board.CurrentPlayer == Player.One)
            {
                move = FindBestMove(board, true, int.MinValue);
            }
            else if (board.CurrentPlayer == Player.Two)
            {
                move = FindBestMove(board, false, int.MaxValue);
            }

            // return selected move
            return move;
        }

        protected int FindBestMove(GameBoard board, bool isPlayerOne, int score)
        {
            // assume first move is the best
            int move = 0;

            // scan valid moves, looking for best score
            for (int index = 0; index < board.ValidMoves.Count; index++)
            {
                // create copy of current board to play with
                GameBoard board2 = new GameBoard(board);

                // make the next valid move
                board2.MakeMove(index);

                // what's the score?
                int val = Evaluate(board2);

                // best score for player one is positive,
                // best score for player two is negative
                if (isPlayerOne)
                {
                    if (val > score)
                    {
                        score = val;
                        move = index;
                    }
                }
                else
                {
                    if (val < score)
                    {
                        score = val;
                        move = index;
                    }
                }
            }

            // report findings to the caller
            return move;
        }
    }
}
