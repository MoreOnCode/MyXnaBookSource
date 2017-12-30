using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CH17___Solitare
{
    public class GameLogic
    {
        // all 52 cards are created once in this container.
        // shuffling happens here. this container is never
        // rendered. cards are distributed to the other
        // containers from here, and returned back whever
        // the game is reset.
        CardContainer m_Deck = new CardContainer(0, 0);

        // the solitare "stock" pile
        CardContainer m_Stock = new CardContainer(150, 70);

        // the solitare "waste" pile
        CardContainer m_Waste = new CardContainer(200, 70);

        // the four solitare "home" piles
        CardContainer m_HomeHeart = new CardContainer(300, 70);
        CardContainer m_HomeDiamond = new CardContainer(350, 70);
        CardContainer m_HomeSpade = new CardContainer(400, 70);
        CardContainer m_HomeClub = new CardContainer(450, 70);

        // the seven "chute" and "stack stock" piles
        CardContainer[] m_StackStock = new CardContainer[7];
        CardContainer[] m_StackChute = new CardContainer[7];

        // the one and only game texture
        // static so that all classes can reference it
        protected static Texture2D m_Texture;
        public static Texture2D Texture
        {
            get { return m_Texture; }
            set { m_Texture = value; }
        }

        // called once, after the GameLogic object is created
        public void Init()
        {
            ClearDecks();
            FillMainDeck();
        }

        // empty all containers, including the main deck
        public void ClearDecks()
        {
            m_Deck.ClearCards();
            m_Stock.ClearCards();
            m_Waste.ClearCards();
            m_HomeHeart.ClearCards();
            m_HomeDiamond.ClearCards();
            m_HomeSpade.ClearCards();
            m_HomeClub.ClearCards();

            if (m_StackStock[0] == null)
            {
                // the chutes haven't been created yet, do it now
                for (int i = 0; i < m_StackStock.Length; i++)
                {
                    m_StackStock[i] = new CardContainer(150 + i * 50, 140);
                    m_StackChute[i] = new CardContainer(150 + i * 50, 150);
                }
            }
            else
            {
                // empty the existing chutes
                for (int i = 0; i < m_StackStock.Length; i++)
                {
                    m_StackStock[i].ClearCards();
                    m_StackChute[i].ClearCards();
                }
            }
        }

        // populate the main deck
        public void FillMainDeck()
        {
            if (Texture == null)
            {
                // the texture MUST be loaded before this method is called
                // this method inspects the texture to determine the locations
                // of each card, card back, and the cursor images
                throw new Exception("Card.Texture must be set before SolitareGame.Init is called.");
            }

            // helper variables to save some typing
            int w = Texture.Width;
            int h = Texture.Height;
            float dx = Card.CardSize.X;
            float dy = Card.CardSize.Y;

            // current position in the texture
            float x = 0;
            float y = 0;

            // for each suit
            for (int suit = 0; suit < 4; suit++)
            {
                // for each rank
                for (int rank = 0; rank < 13; rank++)
                {
                    // create and configure a new card instance
                    Card card = new Card();
                    card.TextureRect = new Rectangle((int)x, (int)y, (int)dx, (int)dy);
                    card.IsFaceUp = false;
                    card.Rank = (Card.RANKS)rank;
                    card.Suit = (Card.SUITS)suit;

                    // add the new card to the deck
                    m_Deck.AcceptCard(card);

                    // determine where the next card image will be
                    x += dx;
                    if (x > w || x + dx > w)
                    {
                        y += dy;
                        x = 0;
                    }
                }
            }

            // beyond the 52 standard cards, there are 14 other "card" images
            for (int back = -2; back < 12; back++)
            {
                if (back == -2)
                {
                    // the black joker image
                    Card.TextureRectBlackJoker = new Rectangle((int)x, (int)y, (int)dx, (int)dy);
                }
                else if (back == -1)
                {
                    // the red joker image
                    Card.TextureRectRedJoker = new Rectangle((int)x, (int)y, (int)dx, (int)dy);
                }
                else if (back == 10)
                {
                    // the "no card here" image
                    Card.TextureRectEmpty = new Rectangle((int)x, (int)y, (int)dx, (int)dy);
                }
                else if (back == 11)
                {
                    // the card cursor, split into four seperate images
                    int cw = Card.CURSOR_SIZE;
                    int ch = Card.CURSOR_SIZE;
                    Card.TextureRectCursorNW = new Rectangle((int)x, (int)y, cw, ch);
                    Card.TextureRectCursorNE = new Rectangle((int)x + (int)dx - Card.CURSOR_SIZE, (int)y, cw, ch);
                    Card.TextureRectCursorSE = new Rectangle((int)x + (int)dx - Card.CURSOR_SIZE, (int)y + (int)dy - Card.CURSOR_SIZE, cw, ch);
                    Card.TextureRectCursorSW = new Rectangle((int)x, (int)y + (int)dy - Card.CURSOR_SIZE, cw, ch);
                }
                else
                {
                    // one of the 10 card back images
                    CardBack.AddTextureRect(new Rectangle((int)x, (int)y, (int)dx, (int)dy));
                }

                // determine the location of the next card image
                x += dx;
                if (x > w || x + dx > w)
                {
                    y += dy;
                    x = 0;
                }
            }
        }

        // used to animate the cursor
        protected double m_CursorCycle = 0;

        // index to the currently-selected chute
        protected int m_Cursor = 0;
        public int Cursor
        {
            get { return m_Cursor; }
            set
            {
                // enforce upper array bounds
                if (value > m_StackStock.Length - 1)
                {
                    value = m_StackStock.Length - 1;
                }

                // enforce lower array bounds
                if (value < 0)
                {
                    value = 0;
                }

                // set value
                m_Cursor = value;
            }
        }

        // index of the "selected" cute
        protected int m_Selected = -1;
        public int Selected
        {
            get { return m_Selected; }
            set
            {
                // enforce cute array bounds
                if (value >= 0 && value < m_StackChute.Length && m_StackChute[value].TopCard != null)
                {
                    m_Selected = value;
                }
                else
                {
                    m_Selected = -1;
                }
            }
        }

        // select a chute, or move waste card to chute
        public void Select()
        {
            // default case is to just select a chute
            bool doSelect = true;

            // helper variables to save some typing
            Card waste = m_Waste.TopCard;
            Card chute = m_StackChute[Cursor].TopCard;

            if (waste != null && chute != null)
            {
                // see if waste card can be moved to the current chute
                int rankWaste = (int)waste.Rank;
                int rankChute = (int)chute.Rank;
                if (rankWaste == rankChute - 1 && waste.IsRedCard != chute.IsRedCard)
                {
                    m_Waste.MoveTowards = m_StackChute[Cursor];
                    doSelect = false;
                }
            }
            else if (waste != null && chute == null && waste.Rank == Card.RANKS.King)
            {
                // move king from waste to chute
                m_Waste.MoveTowards = m_StackChute[Cursor];
                doSelect = false;
            }

            // nothing else seems to have worked, just select the current chute
            if (doSelect)
            {
                Selected = Cursor;
            }
        }

        // prep the board for a new game
        public void NewGame()
        {
            Reset();
            m_Deck.Shuffle(3);
            DealChutes();
        }

        // move all cards back to the main deck
        public void Reset()
        {
            m_Deck.AcceptAllCards(m_Stock);
            m_Deck.AcceptAllCards(m_Waste);
            m_Deck.AcceptAllCards(m_HomeHeart);
            m_Deck.AcceptAllCards(m_HomeDiamond);
            m_Deck.AcceptAllCards(m_HomeSpade);
            m_Deck.AcceptAllCards(m_HomeClub);
            for (int i = 0; i < m_StackStock.Length; i++)
            {
                m_Deck.AcceptAllCards(m_StackStock[i]);
                m_Deck.AcceptAllCards(m_StackChute[i]);
            }
            m_Deck.Orient(false);
        }

        // deal the cards out of the deck for a new game
        public void DealChutes()
        {
            // helper variable to save some typing
            Card card;

            // deal cards to the chutes
            for (int iChute = 0; iChute < m_StackChute.Length; iChute++)
            {
                card = m_Deck.DrawCard();
                card.IsFaceUp = true;
                m_StackChute[iChute].AcceptCard(card);
                for (int iStock = iChute + 1; iStock < m_StackStock.Length; iStock++)
                {
                    card = m_Deck.DrawCard();
                    card.IsFaceUp = false;
                    m_StackStock[iStock].AcceptCard(card);
                }
            }

            // put the remaining cards into the stock pile
            m_Stock.AcceptAllCards(m_Deck);
        }

        // move the top stock card to the waste pile
        public void DrawCardFromStock()
        {
            Card card = m_Stock.TopCard;
            if (card != null)
            {
                // typical scenario, just move the card
                card.IsFaceUp = true;
                m_Stock.MoveTowards = m_Waste;
            }
            else
            {
                // the stock pile has no cards, move waste cards back over
                m_Stock.AcceptAllCards(m_Waste);

                // waste cards are face up, flip them back over
                m_Stock.Orient(false);

                // waste cards are in reverse order from the stock pile
                m_Stock.Reverse();

                // now we can draw the top card from the stock pile
                card = m_Stock.TopCard;
                if (card != null)
                {
                    card.IsFaceUp = true;
                    m_Stock.MoveTowards = m_Waste;
                }
            }
        }

        // move card(s) from one chute to another
        public void MoveCards()
        {
            if (Selected >= 0 && Selected != Cursor)
            {
                // player is trying to move card(s) between chutes, validate

                // helper variables to save some typing
                CardContainer moveTo = m_StackChute[Cursor];
                CardContainer moveFrom = m_StackChute[Selected];
                if (moveTo.TopCard == null && moveFrom.BottomCard.Rank == Card.RANKS.King)
                {
                    // player is moving a king to an empty chute
                    moveFrom.MoveTowards = moveTo;
                    moveFrom.MoveStartingWith = 0;
                    Selected = -1;
                }
                else if (moveTo.TopCard != null)
                {
                    // player is moving non-king card(s) between chutes

                    // get the rank and suit color of the "bottom" card 
                    // (the top card in a chute container is the "bottom"
                    // card, as viewed on the screen -- it sits on top of
                    // the other cards)
                    int rankTo = (int)moveTo.TopCard.Rank;
                    bool redTo = moveTo.TopCard.IsRedCard;

                    // scan the source chute for the (only) card that 
                    // satisfies the moving rules of the game
                    for (int i = 0; i < moveFrom.CardCount; i++)
                    {
                        // the current card
                        Card card = moveFrom.GetCard(i);
                        if ((int)card.Rank == rankTo - 1 && card.IsRedCard != redTo)
                        {
                            // we have a match! stop looking.
                            moveFrom.MoveTowards = moveTo;
                            moveFrom.MoveStartingWith = i;
                            Selected = -1;
                            break;
                        }
                    }
                }

                if (Selected >= 0)
                {
                    // this wasn't a valid move, or the player was
                    // de-selecting the currently-selected chute
                    Selected = Cursor;
                }
            }
        }

        // try to move the top card in the waste pile home or
        // try to move the last card in the current chute home
        public void GoHome()
        {
            // see if top waste card can move home
            CardContainer home = FindHome(m_Waste.TopCard);
            if (home != null)
            {
                m_Waste.MoveTowards = home;
            }
            else
            {
                // waste card failed, see if current chute has a card
                home = FindHome(m_StackChute[Cursor].TopCard);
                if (home != null)
                {
                    m_StackChute[Cursor].MoveTowards = home;
                }
            }
        }

        // given a card, see if any of the home piles will accept it
        protected CardContainer FindHome(Card cardToMove)
        {
            // the home that will allow the card
            CardContainer home = null;
            if (cardToMove != null)
            {
                // determine potential home, based on suit
                switch (cardToMove.Suit)
                {
                    case Card.SUITS.Spade:
                        home = m_HomeSpade;
                        break;
                    case Card.SUITS.Club:
                        home = m_HomeClub;
                        break;
                    case Card.SUITS.Diamond:
                        home = m_HomeDiamond;
                        break;
                    case Card.SUITS.Heart:
                        home = m_HomeHeart;
                        break;
                }

                // the top card in the selected home pile
                Card cardToHost = home.TopCard;

                // the rank of the card that is moving
                int rankMove = (int)cardToMove.Rank;

                if (cardToHost != null)
                {
                    // see if the cards have the proper ranks
                    int rankHost = (int)cardToHost.Rank;
                    if (rankHost != rankMove - 1)
                    {
                        home = null;
                    }
                }
                else
                {
                    // player may be trying to move an ace home
                    if (rankMove != (int)Card.RANKS.Ace)
                    {
                        home = null;
                    }
                }
            }

            // return our findings
            return home;
        }

        // update each container (for moving card animations),
        // animate cursor pulse
        public bool Update(double elapsed)
        {
            // as long as any deck is animating a card, we need to 
            // ignore player input
            bool animating = false;

            // update each container, noting whether it was animating
            animating |= m_Deck.Update(elapsed);
            animating |= m_Stock.Update(elapsed);
            animating |= m_Waste.Update(elapsed);
            animating |= m_HomeHeart.Update(elapsed);
            animating |= m_HomeDiamond.Update(elapsed);
            animating |= m_HomeSpade.Update(elapsed);
            animating |= m_HomeClub.Update(elapsed);
            for (int i = 0; i < m_StackStock.Length; i++)
            {
                animating |= m_StackStock[i].Update(elapsed);
                animating |= m_StackChute[i].Update(elapsed);

                // if a chute is empty, and its stock isn't, deal the 
                // top card to the chute
                if (m_StackChute[i].TopCard == null &&
                    m_StackStock[i].TopCard != null &&
                    m_StackStock[i].MoveTowards == null)
                {
                    m_StackStock[i].TopCard.IsFaceUp = true;
                    m_StackStock[i].MoveTowards = m_StackChute[i];
                    animating = true;
                }
            }
            // update cursor pulse
            m_CursorCycle += elapsed;

            // return our findings
            return animating;
        }

        // render the cards to the screen
        public void Draw(SpriteBatch batch)
        {
            // draw the home piles
            m_HomeHeart.Draw(batch, false);
            m_HomeDiamond.Draw(batch, false);
            m_HomeSpade.Draw(batch, false);
            m_HomeClub.Draw(batch, false);

            // draw the chutes
            for (int i = m_StackStock.Length - 1; i >= 0; i--)
            {
                m_StackStock[i].Draw(batch, false);
                m_StackChute[i].Draw(batch, true, Selected == i);
                if (i == Cursor)
                {
                    Card.DrawCursor(batch, m_StackChute[i].Position, m_CursorCycle);
                }
            }

            // draw the waste and stock piles
            m_Waste.Draw(batch, false);
            m_Stock.Draw(batch, false);
        }
    }
}
