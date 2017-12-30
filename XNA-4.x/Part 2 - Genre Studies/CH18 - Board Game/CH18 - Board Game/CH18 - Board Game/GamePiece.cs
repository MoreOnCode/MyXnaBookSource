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

namespace CH18___Board_Game
{
    public struct GamePiece
    {
        // simple copy constructor
        public GamePiece(Player owner)
        {
            Owner = owner;
        }

        // texture rectangle
        public static readonly Rectangle PieceRect = new Rectangle(0, 0, 32, 32);

        // color, based on owner
        public static readonly Color[] Colors = {
            Color.Transparent,
            Color.Goldenrod,
            Color.OrangeRed,
        };

        // owner of this piece
        public Player Owner;

        // update this piece
        public void Update(double elapsed)
        {
        }

        // render this piece at the specified position
        public void Draw(SpriteBatch batch, Vector2 position)
        {
            if (Owner == Player.None) return;
            batch.Draw(GameBoard.Texture, position, PieceRect, Colors[(int)Owner]);
        }
    }
}
