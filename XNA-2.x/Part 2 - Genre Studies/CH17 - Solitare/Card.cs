// Card.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chapter17
{
    public class Card
    {
        // the texture rect for this card instance
        protected Rectangle m_TextureRect;
        public Rectangle TextureRect
        {
            get { return m_TextureRect; }
            set { m_TextureRect = value; }
        }

        // the graphic to display when container holds no cards
        protected static Rectangle m_TextureRectEmpty;
        public static Rectangle TextureRectEmpty
        {
            get { return m_TextureRectEmpty; }
            set { m_TextureRectEmpty = value; }
        }

        // texture rect for the red joker (not used in this game)
        protected static Rectangle m_TextureRectRedJoker;
        public static Rectangle TextureRectRedJoker
        {
            get { return m_TextureRectRedJoker; }
            set { m_TextureRectRedJoker = value; }
        }

        // texture rect for the black joker (not used in this game)
        protected static Rectangle m_TextureRectBlackJoker;
        public static Rectangle TextureRectBlackJoker
        {
            get { return m_TextureRectBlackJoker; }
            set { m_TextureRectBlackJoker = value; }
        }

        // top left of cursor
        protected static Rectangle m_TextureRectCursorNW;
        public static Rectangle TextureRectCursorNW
        {
            get { return m_TextureRectCursorNW; }
            set { m_TextureRectCursorNW = value; }
        }

        // top right of cursor
        protected static Rectangle m_TextureRectCursorNE;
        public static Rectangle TextureRectCursorNE
        {
            get { return m_TextureRectCursorNE; }
            set { m_TextureRectCursorNE = value; }
        }

        // bottom right of cursor
        protected static Rectangle m_TextureRectCursorSE;
        public static Rectangle TextureRectCursorSE
        {
            get { return m_TextureRectCursorSE; }
            set { m_TextureRectCursorSE = value; }
        }

        // bottom left of cursor
        protected static Rectangle m_TextureRectCursorSW;
        public static Rectangle TextureRectCursorSW
        {
            get { return m_TextureRectCursorSW; }
            set { m_TextureRectCursorSW = value; }
        }

        // the size of a single card
        protected static Vector2 m_CardSize = new Vector2(47, 64);
        public static Vector2 CardSize
        {
            get { return m_CardSize; }
            set { m_CardSize = value; }
        }

        // display the chutes so that you can read each card
        protected static Vector2 m_CascadeOffset = new Vector2(0, 18);
        public static Vector2 CascadeOffset
        {
            get { return m_CascadeOffset; }
            set { m_CascadeOffset = value; }
        }

        // the face values of the standard cards
        public enum RANKS
        {
            Ace,
            Deuce,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Joker,
        }

        // the rank of this card instance
        protected RANKS m_Rank;
        public RANKS Rank
        {
            get { return m_Rank; }
            set { m_Rank = value; }
        }

        // the suits of the standard cards
        public enum SUITS
        {
            Spade,
            Club,
            Diamond,
            Heart,
        }

        // the suit of this card instance
        protected SUITS m_Suit;
        public SUITS Suit
        {
            get { return m_Suit; }
            set { m_Suit = value; }
        }

        // true when this card intance has a suit of heart or diamond
        // useful for Solitare's game rules
        public bool IsRedCard
        {
            get { return Suit == SUITS.Diamond || Suit == SUITS.Heart; }
        }

        // can the player see this card?
        protected bool m_IsFaceUp = false;
        public bool IsFaceUp
        {
            get { return m_IsFaceUp; }
            set { m_IsFaceUp = value; }
        }

        // useful for card-level animations, unused for this game
        public void Update(double elapsed)
        {
        }

        // draw this card, normal
        public void Draw(SpriteBatch batch, Vector2 position)
        {
            Draw(batch, position, false);
        }

        // draw this card, optionally highlighted as "selected"
        public void Draw(SpriteBatch batch,Vector2 position,bool isSelected)
        {
            if (IsFaceUp)
            {
                if (isSelected)
                {
                    // face up and selected, highlight in yellow
                    batch.Draw(GameLogic.Texture, position, TextureRect, Color.Goldenrod);
                }
                else
                {
                    // face up and not selected, draw normally
                    batch.Draw(GameLogic.Texture, position, TextureRect, Color.White);
                }
            }
            else
            {
                // not face up, draw the back of the card
                CardBack.Draw(batch, position);
            }
        }

        // draw the "no card here" placeholder image
        public static void DrawNoCard(SpriteBatch batch, Vector2 position)
        {
            batch.Draw(GameLogic.Texture, position, TextureRectEmpty, Color.White);
        }

        // helpful const for loading and drawing the card cursor 
        public const int CURSOR_SIZE = 12;

        // draw a pulsing cursor around the card
        public static void DrawCursor(SpriteBatch batch, Vector2 position, double cycle)
        {
            // pulsing effect is based on simple Sin function
            float delta = (int)Math.Round(Math.Sin(cycle * 4) * 2.0f - 3.0f);

            // location of the four cursor images, plus pulsing offset
            Vector2 vNW = position + new Vector2(delta, delta);
            position.X += CardSize.X - CURSOR_SIZE;
            Vector2 vNE = position + new Vector2(-delta, delta);
            position.Y += CardSize.Y - CURSOR_SIZE;
            Vector2 vSE = position + new Vector2(-delta, -delta);
            position.X -= CardSize.X - CURSOR_SIZE;
            Vector2 vSW = position + new Vector2(delta, -delta);

            // actually draw the cursor
            batch.Draw(GameLogic.Texture, vNW, TextureRectCursorNW, Color.White);
            batch.Draw(GameLogic.Texture, vNE, TextureRectCursorNE, Color.White);
            batch.Draw(GameLogic.Texture, vSE, TextureRectCursorSE, Color.White);
            batch.Draw(GameLogic.Texture, vSW, TextureRectCursorSW, Color.White);
        }
    }
}