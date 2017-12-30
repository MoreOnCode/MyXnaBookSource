// CardContainer.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chapter17
{
    public class CardContainer
    {
        // no default constructor, init with position
        public CardContainer(float x, float y) : this(new Vector2(x, y)) { }
        public CardContainer(Vector2 position)
        {
            Position = position;
        }

        // list of all cards in this container
        protected List<Card> m_Cards = new List<Card>();

        // empty this container
        public void ClearCards() { m_Cards.Clear(); }

        // receive a new card into this container
        public void AcceptCard(Card card)
        {
            if (card != null)
            {
                m_Cards.Add(card);
            }
        }

        // receive all cards from another container, empty the donor
        public void AcceptAllCards(CardContainer container)
        {
            if (container != null)
            {
                foreach (Card card in container.m_Cards)
                {
                    AcceptCard(card);
                }
                container.ClearCards();
            }
        }

        // the top card in this pile of cards
        public Card TopCard
        {
            get
            {
                Card card = null;
                if (m_Cards.Count > 0)
                {
                    card = m_Cards[m_Cards.Count-1];
                }
                return card;
            }
        }

        // the second card in this pile of cards
        public Card SecondCard
        {
            get
            {
                Card card = null;
                if (m_Cards.Count > 1)
                {
                    card = m_Cards[m_Cards.Count - 2];
                }
                return card;
            }
        }

        // the bottom card in this pile of cards
        public Card BottomCard
        {
            get
            {
                Card card = null;
                if (m_Cards.Count > 0)
                {
                    card = m_Cards[0];
                }
                return card;
            }
        }

        // retrieve a specific card from this container
        public Card GetCard(int index)
        {
            Card card = null;
            if (m_Cards.Count > 0 && index >= 0 && index < m_Cards.Count)
            {
                card = m_Cards[index];
            }
            return card;
        }

        // how many cards do we have?
        public int CardCount
        {
            get { return m_Cards.Count; }
        }

        // remove the top card from this container
        public Card DrawCard()
        {
            Card card = TopCard;
            if (card != null)
            {
                m_Cards.Remove(card);
            }
            return card;
        }

        // helper for shuffling the cards in this container
        protected Random m_rand = new Random();

        // randomize the order of cards in this container
        public void Shuffle(int repeat)
        {
            // shuffle the deck "repeat" times
            if (repeat > 0 && m_Cards.Count > 0)
            {
                // helpers for shuffling
                Card swap;
                int card1;
                int card2;

                for (int i = 0; i < m_Cards.Count * repeat; i++)
                {
                    // randomly pick two cards, then swap them
                    card1 = m_rand.Next(m_Cards.Count);
                    card2 = m_rand.Next(m_Cards.Count);
                    swap = m_Cards[card1];
                    m_Cards[card1] = m_Cards[card2];
                    m_Cards[card2] = swap;
                }

                // mark all the cards in this container as face down
                Orient(false);
            }
        }

        // randomize the order of cards in this container
        public void Shuffle()
        {
            Shuffle(1);
        }

        // mark all cards in this container as face up or face down
        public void Orient(bool faceUp)
        {
            foreach (Card card in m_Cards)
            {
                card.IsFaceUp = faceUp;
            }
        }

        // reverse the order of the cards in this container
        // useful for stock and waste piles
        public void Reverse()
        {
            m_Cards.Reverse();
        }

        // duration of moving cards, in seconds
        protected const float MOVE_DURATION = 0.5f;

        // number of seconds into current animation
        protected double m_MoveProgress = 0;

        // where are the cards heading?
        protected CardContainer m_MoveTowards = null;
        public CardContainer MoveTowards
        {
            get { return m_MoveTowards; }
            set
            {
                m_MoveTowards = value;
                m_MoveProgress = 0;
            }
        }

        // for the chutes, may be moving more than one card
        protected int m_MoveStartingWith = -1;
        public int MoveStartingWith
        {
            get { return m_MoveStartingWith; }
            set { m_MoveStartingWith = value; }
        }

        // the position of this container on the screen
        protected Vector2 m_Position = Vector2.Zero;
        public Vector2 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        // update, useful for card animations
        public bool Update(double elapsed)
        {
            bool animating = false;
            if (MoveTowards != null)
            {
                // we're in the middle of an animation
                m_MoveProgress += elapsed;
                if (m_MoveProgress > MOVE_DURATION)
                {
                    // the animation is done, actually move the cards now
                    if (MoveStartingWith >= 0)
                    {
                        // moving multiple cards
                        int count = m_Cards.Count - MoveStartingWith;
                        for (int i = 0; i < count; i++)
                        {
                            Card card = GetCard(MoveStartingWith);
                            MoveTowards.AcceptCard(card);
                            m_Cards.Remove(card);
                        }

                        // reset animation variables
                        MoveTowards = null;
                        MoveStartingWith = -1;
                        m_MoveProgress = 0;
                    }
                    else
                    {
                        // only moving one card
                        MoveTowards.AcceptCard(DrawCard());

                        // reset animation variables
                        MoveTowards = null;
                        MoveStartingWith = -1;
                        m_MoveProgress = 0;
                    }
                }
                else
                {
                    // not done animating yet; ignore player input until we are
                    animating = true;
                }
            }
            return animating;
        }

        // draw a non-chute container
        public void Draw(SpriteBatch batch, bool isChute)
        {
            Draw(batch, isChute, false);
        }

        // draw this container
        public void Draw(SpriteBatch batch, bool isChute,bool isSelected)
        {
            Vector2 position = this.Position;

            // unless this is a "chute", draw the empty container image
            // if there are cards in this container, they will overdraw this image
            if (!isChute)
            {
                Card.DrawNoCard(batch, position);
            }

            // if animating, how far are we?
            Vector2 progress = new Vector2((float)m_MoveProgress / MOVE_DURATION, (float)m_MoveProgress / MOVE_DURATION);

            if (m_Cards.Count > 0)
            {
                // we do have cards, draw them
                if (isChute)
                {
                    // "chute" piles arrange the cards so that the player
                    // can see each of the cards that they contain.

                    if (MoveTowards == null)
                    {
                        // we're not in an animation, just draw the cards
                        for (int i = 0; i < m_Cards.Count; i++)
                        {
                            m_Cards[i].Draw(batch, position,isSelected );
                            position += Card.CascadeOffset;
                        }
                    }
                    else
                    {
                        // we are in an animation
                        if (MoveStartingWith < 0)
                        {
                            MoveStartingWith = CardCount;
                        }
                        if (MoveStartingWith >= m_Cards.Count)
                        {
                            MoveStartingWith = m_Cards.Count - 1;
                        }

                        // draw the stationary cards ...
                        for (int i = 0; i < MoveStartingWith; i++)
                        {
                            m_Cards[i].Draw(batch, position, isSelected);
                            position += Card.CascadeOffset;
                        }

                        // ... then draw the moving cards
                        position += (MoveTowards.Position - this.Position) * progress;
                        for (int i = MoveStartingWith; i < m_Cards.Count; i++)
                        {
                            m_Cards[i].Draw(batch, position, isSelected);
                            position += Card.CascadeOffset;
                        }
                    }
                }
                else
                {
                    // this isn't a "chute" pile
                    if (MoveTowards == null)
                    {
                        // we're not in an animation, just draw the top card
                        if (TopCard != null)
                        {
                            TopCard.Draw(batch, position, isSelected);
                        }
                    }
                    else
                    {
                        // we are in an animation
                        if (SecondCard != null)
                        {
                            // if there's a second card, draw it
                            SecondCard.Draw(batch, position, isSelected);
                        }
                        if (TopCard != null)
                        {
                            // if there's a top card, draw it (moving)
                            position += (MoveTowards.Position - this.Position) * progress;
                            TopCard.Draw(batch, position, isSelected);
                        }
                    }
                }
            }
        }
    }
}
