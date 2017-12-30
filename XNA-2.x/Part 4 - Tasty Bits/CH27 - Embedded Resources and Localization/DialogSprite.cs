// DialogSprite.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Codetopia.Graphics;

namespace Chapter27
{
    public class DialogSprite : SpriteBase
    {
        // reference to the flag sprite
        // the DialogSprite class is responsible for using 
        // localized content, and the flag is locale-specific
        protected SpriteBase m_Flag = null;
        public SpriteBase Flag
        {
            get { return m_Flag; }
            set { m_Flag = value; }
        }

        // color of the text within the dialog
        protected Color m_TextTint = Color.Black;
        public Color TextTint
        {
            get { return m_TextTint; }
        }

        // true when the dialog is done transitioning
        // from one size to the next
        protected bool Ready
        {
            get
            {
                return TargetSize == Size;
            }
        }

        // a helper class to handle the details of
        // scaling our on-screen dialog bubble
        protected DialogRectangle m_DialogRect = null;

        // a helper class to draw text to the screen
        public GameFont GameFont = null;

        // the location of the little arrow that sits just 
        // below the dialog bubble, pointing to the avatar.
        protected Rectangle m_TextureRectQuote;
        public Rectangle TextureRectQuote
        {
            get { return m_TextureRectQuote; }
            set { m_TextureRectQuote = value; }
        }

        // simple constructor to set initial vaules
        public DialogSprite()
        {
            TextureRect = new Rectangle(0, 0, 32, 32);
            TextureRectQuote = new Rectangle(35, 0, 23, 20);
            m_TargetSize = Vector2.One * 32;
            m_Size = m_TargetSize;
            m_DialogRect = new DialogRectangle();
            m_DialogRect.DrawableArea = new Rectangle(309, 64, 256, 352);
            m_DialogRect.InitTextureRects(TextureRect, 5);
            m_Tint = Color.CornflowerBlue;
            m_TextTint = Color.White;
        }

        // process player input and update game objects
        public sealed override void Update(float elapsed)
        {
            base.Update(elapsed);

            if (Ready)
            {
                // we're not transitioning, check the A button
                if (GamePadState.Buttons.A == ButtonState.Pressed)
                {
                    // cycle between no, 1, 2, and 3 lines of text
                    int lines = VisibleLineCount + 1;
                    if (lines > 3) { lines = 0; }
                    Show(lines);
                }
            }
            else
            {
                // we're in the middle of a transition from one
                // dialog size to another

                // update horizintal size
                m_Size.X += elapsed * m_SizeChange.X;
                if ((m_SizeChange.X < 0 && m_Size.X < m_TargetSize.X) ||
                    (m_SizeChange.X > 0 && m_Size.X > m_TargetSize.X))
                {
                    m_Size.X = m_TargetSize.X;
                    m_SizeChange.X = 0;
                }

                // update vertical size
                m_Size.Y += elapsed * m_SizeChange.Y;
                if ((m_SizeChange.Y < 0 && m_Size.Y < m_TargetSize.Y) ||
                    (m_SizeChange.Y > 0 && m_Size.Y > m_TargetSize.Y))
                {
                    m_Size.Y = m_TargetSize.Y;
                    m_SizeChange.Y = 0;
                }
            }
        }

        // the (localized) three lines of text and 
        // their on-screen widths
        protected string[] m_Text = new string[3];
        protected int[] m_TextWidth = new int[3];

        // time to transition between dialog sizes
        protected const float TRANSITION = 0.25f;

        // change in size of the dialog (in pixels per second)
        protected Vector2 m_SizeChange = Vector2.Zero;

        // the size we want our dialog to be
        protected Vector2 m_TargetSize = Vector2.Zero;
        public Vector2 TargetSize
        {
            get { return m_TargetSize; }
            set
            {
                m_TargetSize = value;
                m_SizeChange.X = (TargetSize.X - Size.X) / TRANSITION;
                m_SizeChange.Y = (TargetSize.Y - Size.Y) / TRANSITION;
                //m_TimeTransitioning = 0;
            }
        }

        // the current size of the dialog
        protected Vector2 m_Size = Vector2.Zero;
        public Vector2 Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }

        // number of lines that are being displayed
        protected int m_VisibleLineCount = 0;
        public int VisibleLineCount { get { return m_VisibleLineCount; } }

        // show or hide the dialog, cycle between no, 1, 2, or 3 lines
        public void Show(int lines)
        {
            // make sure lines is between 0 and 3
            lines = Math.Min(lines, 3);
            lines = Math.Max(lines, 0);

            // set the desired line count
            m_VisibleLineCount = lines;

            if (lines > 0)
            {
                // we're showing text now, capture the localized text
                m_Text[0] = LocalizedContent.ResourceHelper.Hello;
                m_Text[1] = LocalizedContent.ResourceHelper.HowAreYou;
                m_Text[2] = LocalizedContent.ResourceHelper.Goodbye;

                // measure the localized text
                m_TextWidth[0] = (int)Math.Round(
                    GameFont.MeasureString(m_Text[0]).X);
                m_TextWidth[1] = (int)Math.Round(
                    GameFont.MeasureString(m_Text[1]).X);
                m_TextWidth[2] = (int)Math.Round(
                    GameFont.MeasureString(m_Text[2]).X);

                // determine the height of the client area
                float y = (lines + 1) * GameFont.FontHeight;

                // determine the width of the client area
                float x = 32.0f; // minimum width is 32
                for (int i = 0; i < lines; i++)
                {
                    x = Math.Max(x, m_TextWidth[i]);
                }

                // set the desired size to initiate the transition
                TargetSize = new Vector2(x + 32, y);
            }
            else
            {
                // we're hiding the dialog, shrink it before hiding
                // by setting desired size to kick off transition
                TargetSize = new Vector2(32, 32);
            }
        }

        // actually draw our dialog
        public sealed override void Draw(SpriteBatch batch)
        {
            if (VisibleLineCount > 0 || !Ready)
            {
                // we won't draw text if the dialog is hidden or the 
                // dialog is transitioning between sizes
                m_DialogRect.Draw(batch, this);
            }

            if (VisibleLineCount > 0 && Ready)
            {
                // dialog is full-size and has visible lines

                // center the dialog within the drawable area
                Rectangle rect = m_DialogRect.DrawableArea;
                int centerX = rect.Left + rect.Width / 2;
                int centerY = rect.Top + rect.Height / 2;

                // determine the location of the first line of text
                int y = centerY - VisibleLineCount * GameFont.FontHeight / 2;

                // draw each line, centered, each below the previous
                for (int i = 0; i < VisibleLineCount; i++)
                {
                    int x = centerX - m_TextWidth[i] / 2;
                    GameFont.DrawString(
                        batch,
                        m_Text[i],
                        x, y,
                        TextTint);
                    y += GameFont.FontHeight;
                }
            }
        }
    }
}
