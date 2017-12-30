using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH18___Board_Game
{
    class RandomAI : ArtificialIntelligence
    {
        // constructor with depth initializer
        public RandomAI(int depth) : base(depth) { }

        // as simple as AI gets -- random selection
        protected override int SelectMoveRecursive(GameBoard board, int depth)
        {
            // randomly select a move from the list of valid moves
            return m_rand.Next(board.ValidMoves.Count);
        }
    }
}
