// AI_Random.cs
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter18
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
