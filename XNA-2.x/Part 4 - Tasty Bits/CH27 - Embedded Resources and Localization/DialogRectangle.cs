// DialogRectangle.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chapter27
{
    public class DialogRectangle
    {
        // on-screen area where dialog is to be centered
        protected Rectangle m_DrawableArea = Rectangle.Empty;
        public Rectangle DrawableArea
        {
            get { return m_DrawableArea; }
            set { m_DrawableArea = value; }
        }

        // draw the dialog bubble
        public void Draw(SpriteBatch batch, DialogSprite sprite)
        {
            // dimensions of target rectangle, centered in drawable area
            int width = (int)Math.Round(sprite.Size.X);
            int height = (int)Math.Round(sprite.Size.Y);
            int top = DrawableArea.Top + DrawableArea.Height / 2 - height / 2;
            int left = DrawableArea.Left + DrawableArea.Width / 2 - width / 2;
            int border = TextureRectNW.Width;

            // draw corners (they don't scale)
            Vector2 loc = new Vector2(left, top);
            batch.Draw(sprite.Texture, loc, TextureRectNW, sprite.Tint);

            loc.X = left + width - border;
            batch.Draw(sprite.Texture, loc, TextureRectNE, sprite.Tint);

            loc.Y = top + height - border;
            batch.Draw(sprite.Texture, loc, TextureRectSE, sprite.Tint);

            loc.X = left;
            batch.Draw(sprite.Texture, loc, TextureRectSW, sprite.Tint);

            // draw sides (they only scale vertically)
            Rectangle rect = new Rectangle(
                left, 
                top + border, 
                border, 
                height - 2 * border);
            batch.Draw(sprite.Texture, rect, TextureRectWE, sprite.Tint);

            rect.X = left + width - border;
            batch.Draw(sprite.Texture, rect, TextureRectEA, sprite.Tint);

            // draw top and bottom (they only scale horizontally)
            rect = new Rectangle(
                left + border, 
                top, 
                width - 2 * border, 
                border);
            batch.Draw(sprite.Texture, rect, TextureRectNO, sprite.Tint);

            rect.Y = top + height - border;
            batch.Draw(sprite.Texture, rect, TextureRectSO, sprite.Tint);

            // draw client area (scales both ways)
            rect = new Rectangle(
                left + border, 
                top + border, 
                width - 2 * border, 
                height - 2 * border);
            batch.Draw(sprite.Texture, rect, TextureRectCenter, sprite.Tint);

            // draw the arrow for the dialog bubble
            loc.X = left + border;
            loc.Y = top + height - 4.0f; // yuck: hard-coded.
            batch.Draw(
                sprite.Texture, 
                loc, 
                sprite.TextureRectQuote, 
                sprite.Tint);
        }

        #region "Texture Rectangles"

        // break the dialog texture rectangle into it's nine components
        public void InitTextureRects(Rectangle rect, int border)
        {
            // handy local variables to save some typing
            int top = rect.Top;
            int left = rect.Left;
            int right = rect.Right;
            int bottom = rect.Bottom;
            int width = rect.Width;
            int height = rect.Height;

            // corners
            m_TextureRectNW = 
                new Rectangle(left, top, border, border);
            m_TextureRectNE = 
                new Rectangle(right - border, top, border, border);
            m_TextureRectSW = 
                new Rectangle(left, bottom - border, border, border);
            m_TextureRectSE = new Rectangle(
                right - border, 
                bottom - border, 
                border, 
                border);

            // top and bottom
            m_TextureRectNO = new Rectangle(
                left + border, 
                top, 
                width - 2 * border, 
                border);
            m_TextureRectSO = new Rectangle(
                left + border, 
                bottom - border, 
                width - 2 * border, 
                border);

            // sides
            m_TextureRectWE = new Rectangle(
                left, 
                top + border, 
                border, 
                height - 2 * border);
            m_TextureRectEA = new Rectangle(
                right - border, 
                top + border, 
                border, 
                height - 2 * border);

            // center
            m_TextureRectCenter = new Rectangle(
                TextureRectNO.Left,
                TextureRectWE.Top,
                TextureRectNO.Width,
                TextureRectWE.Height);
        }

        // center of the dialog bubble in source texture
        protected Rectangle m_TextureRectCenter = Rectangle.Empty;
        public Rectangle TextureRectCenter
        { get { return m_TextureRectCenter; } }

        // top, right of the dialog bubble in source texture
        protected Rectangle m_TextureRectNE = Rectangle.Empty;
        public Rectangle TextureRectNE
        { get { return m_TextureRectNE; } }

        // right of the dialog bubble in source texture
        protected Rectangle m_TextureRectEA = Rectangle.Empty;
        public Rectangle TextureRectEA
        { get { return m_TextureRectEA; } }

        // bottom, right of the dialog bubble in source texture
        protected Rectangle m_TextureRectSE = Rectangle.Empty;
        public Rectangle TextureRectSE
        { get { return m_TextureRectSE; } }

        // bottom of the dialog bubble in source texture
        protected Rectangle m_TextureRectSO = Rectangle.Empty;
        public Rectangle TextureRectSO
        { get { return m_TextureRectSO; } }

        // bottom, left of the dialog bubble in source texture
        protected Rectangle m_TextureRectSW = Rectangle.Empty;
        public Rectangle TextureRectSW
        { get { return m_TextureRectSW; } }

        // left of the dialog bubble in source texture
        protected Rectangle m_TextureRectWE = Rectangle.Empty;
        public Rectangle TextureRectWE
        { get { return m_TextureRectWE; } }

        // top, left of the dialog bubble in source texture
        protected Rectangle m_TextureRectNW = Rectangle.Empty;
        public Rectangle TextureRectNW
        { get { return m_TextureRectNW; } }

        // top of the dialog bubble in source texture
        protected Rectangle m_TextureRectNO = Rectangle.Empty;
        public Rectangle TextureRectNO
        { get { return m_TextureRectNO; } }

        #endregion
    }
}
