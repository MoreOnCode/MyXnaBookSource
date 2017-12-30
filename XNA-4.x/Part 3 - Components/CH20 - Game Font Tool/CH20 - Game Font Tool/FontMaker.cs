// Given a TTF and a filename, generates a bitmap font for 
// use in games

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;

namespace GameFontUtility
{
    class FontMaker
    {
        // collection of char / rectangle pairs, 
        // defines glyph boundries
        private Dictionary<char, Rectangle> m_GlyphBounds =
            new Dictionary<char, Rectangle>();

        // the only supported pixel format ARGB-32
        protected PixelFormat m_pixfmt =
            PixelFormat.Format32bppArgb;

        // add a single item to the list of glyphs to capture
        public void AddCharToEncode(char c)
        {
            if (!m_GlyphBounds.ContainsKey(c))
            {
                m_GlyphBounds.Add(c, new Rectangle());
            }
        }

        // add a range of items to the list of glyphs 
        // to capture
        public void AddCharsToEncode(char c1, char c2)
        {
            for (char c = c1; c <= c2; c++)
            {
                AddCharToEncode(c);
            }
        }

        // clear the list of glyphs to capture
        public void ClearCharsToEncode()
        {
            m_GlyphBounds.Clear();
        }

        // only used by DefaultCharsToEncode()
        public enum EncodeDefault
        {
            NONE,
            ASCII,
        }

        // shortcut for defining glyphs to capture
        public void DefaultCharsToEncode(EncodeDefault def)
        {
            ClearCharsToEncode();
            switch (def)
            {
                case EncodeDefault.NONE:
                    break;
                case EncodeDefault.ASCII:
                default:
                    AddCharsToEncode(' ', '~');
                    AddCharToEncode('©');
                    AddCharToEncode('®');
                    AddCharToEncode('«');
                    AddCharToEncode('»');
                    break;
            }
        }

        // render hint font render option
        protected TextRenderingHint m_RenderHint =
            TextRenderingHint.AntiAliasGridFit;
        public TextRenderingHint RenderHint
        {
            get { return m_RenderHint; }
            set { m_RenderHint = value; }
        }

        // string format font render option
        protected StringFormat m_RenderFormat =
            StringFormat.GenericTypographic;
        public StringFormat RenderFormat
        {
            get { return m_RenderFormat; }
            set { m_RenderFormat = value; }
        }

        // used by Encode() to denote a premature "end of 
        // row" with an invalid character. 
        protected Color m_EndOfRow =
            Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF);

        // given a TTF Windows font, create a game font
        public void Encode(Font font, string file)
        {
            // temp rect variable used throughout method
            Rectangle r = Rectangle.Empty;

            // get sorted list of chars to capture
            char[] keys = new char[m_GlyphBounds.Count];
            m_GlyphBounds.Keys.CopyTo(keys, 0);
            Array.Sort(keys);

            // dummy graphics objs to get font data ...
            Bitmap bmp = new Bitmap(10, 10, m_pixfmt);
            Graphics g = Graphics.FromImage(bmp);
            g.TextRenderingHint = RenderHint;

            // ... in graphics units
            float ffLineSpacing = font.FontFamily
                .GetLineSpacing(font.Style);
            float ffAscent = font.FontFamily
                .GetCellAscent(font.Style);
            float ffDescent = font.FontFamily
                .GetCellDescent(font.Style);
            float ffHeight = font.FontFamily
                .GetEmHeight(font.Style);

            // ... in pixels
            int height = font.Height;
            float ascent =
                height * ffAscent / ffLineSpacing;

            // get width of each char in font
            SizeF glyphSize;
            foreach (char c in keys)
            {
                if (c == ' ')
                {
                    // MeasureString returns 0 for ' ' when 
                    // using StringFormat.GenericTypographic
                    SizeF asa = g.MeasureString("a a", font,
                        0xFFFFFF, RenderFormat);
                    SizeF aa = g.MeasureString("aa", font,
                        0xFFFFFF, RenderFormat);

                    // the difference between the widths of 
                    // "a a" and "aa" should be the typical
                    // width of a space in the current font
                    glyphSize = asa - aa;
                }
                else
                {
                    // measure non-space characters
                    glyphSize = g.MeasureString("" + c, font,
                        0xFFFFFF, RenderFormat);
                }
                // get the current glyph bounds
                r = m_GlyphBounds[c];

                // update the glyph's width and height
                r.Width = (int)Math.Ceiling(glyphSize.Width);
                r.Height = height;

                // record the current glyph bounds
                m_GlyphBounds[c] = r;
            }

            // Try to fit the glyphs onto the smallest possible
            // image. Start with a 128 x 128 image, and expand
            // those bounds until we can make the glyphs fit.
            int wout = 128;
            int hout = 128;

            // when the glyphs fit the image, match is true
            bool match = false;
            while (!match)
            {
                // start at top, left. y equals 1 since the first 
                // row of pixels (where y == 0) for every row of 
                // glyphs is reserved to mark the start, end, and 
                // character value of the glyph. x = 1 since the 
                // height and ascent of the font are encoded in 
                // the first column of the first row of glyphs.
                int x = 1;
                int y = 1;

                // assume success
                match = true;

                // step through each glyph
                foreach (char c in keys)
                {
                    // get width and height of next glyph
                    r = m_GlyphBounds[c];

                    // is glyph too wide for this row?
                    if (x + r.Width < wout)
                    {
                        // glyph fits, record new x and y
                        r.Y = y;
                        r.X = x;
                        m_GlyphBounds[c] = r;

                        // get ready for the next glyph
                        x += r.Width;
                    }
                    else
                    {
                        // glyph is too wide, try the next row
                        x = 0;
                        y += height + 1;

                        // did we run out of rows?
                        if (y + height > hout)
                        {
                            // note failure and exit for loop
                            match = false;
                            break;
                        }

                        // is glyph too wide for this new row?
                        if (x + r.Width < hout)
                        {
                            // glyph fits, record new x and y
                            r.Y = y;
                            r.X = x;
                            m_GlyphBounds[c] = r;

                            // get ready for the next glyph
                            x += r.Width;
                        }
                        else
                        {
                            // note failure and exit for loop
                            match = false;
                            break;
                        }
                    }
                }

                // did we run out of space? expand the image
                if (!match)
                {
                    // try to keep the image square, and sized 
                    // as a power of two.
                    if (wout == hout)
                    {
                        // double the height if the image is 
                        // already square.
                        hout = hout << 1;
                    }
                    else
                    {
                        // otherwise, double the width to make 
                        // the image square.
                        wout = wout << 1;
                    }
                }
            }

            // free old resources
            g.Dispose();
            bmp.Dispose();

            // create new resources
            bmp = new Bitmap(wout, hout, m_pixfmt);

            g = Graphics.FromImage(bmp);
            g.TextRenderingHint = RenderHint;

            // define our transparent pixel, clear image
            Color transaprent = Color.FromArgb(0x00, Color.White);
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    bmp.SetPixel(x, y, transaprent);
                }
            }

            // now that we've made sure everything fits, it's
            // time to stamp them onto our new image
            foreach (char c in keys)
            {
                // get location and size of next glyph
                r = m_GlyphBounds[c];

                // this is inefficient, but need to make sure 
                // that glyphs don't invade each other's space
                // creating a tiny Bitmap for each glyph rather
                // than just drawing the image on the master 
                // Bitmap ensures there's no overlap between 
                // characters
                Bitmap bmpGlyph =
                    new Bitmap(r.Width, height, m_pixfmt);
                Graphics gGlyph = Graphics.FromImage(bmpGlyph);
                gGlyph.TextRenderingHint = RenderHint;

                // clear the tiny Bitmap
                for (int yc = 0; yc < height; yc++)
                {
                    for (int xc = 0; xc < r.Width; xc++)
                    {
                        bmpGlyph.SetPixel(xc, yc, transaprent);
                    }
                }

                // draw the glyph onto the tiny Bitmap
                gGlyph.DrawString("" + c, font,
                    Brushes.White, 0, 0, RenderFormat);
                gGlyph.Flush(System.Drawing.Drawing2D.
                    FlushIntention.Sync);

                // draw the new glyph onto the master Bitmap
                g.DrawImage(bmpGlyph, r.X, r.Y);
                g.Flush(System.Drawing.Drawing2D.FlushIntention.Sync);

                // go ahead and mark the next glyph as the end 
                // of row. if it's not the last glyph on the row,
                // the next glyph will overwrite this value. by
                // assuming failure and marking it now, we don't 
                // need to have special logic to handle an end-of-
                // row condition.
                if (r.X + r.Width < bmp.Width)
                {
                    bmp.SetPixel(r.X + r.Width, r.Y - 1, m_EndOfRow);
                }

                // mark the start of the glyph and encode its 
                // char value 
                bmp.SetPixel(r.X, r.Y - 1,
                    Color.FromArgb(0xFF,
                        0x00,
                        (c & 0xFF00) >> 8,
                        c & 0xFF));
            }

            // preserve the existing green and blue values 
            // for the top, left pixel since that's where 
            // we encode the character value for the first 
            // glyph
            g.Flush(System.Drawing.Drawing2D.FlushIntention.Sync);
            Color firstPixel = bmp.GetPixel(0, 0);

            // set the first magic number (0xC0DE) for the 
            // new image, and note the height of the font. 
            // the magic number helps us distinguish this 
            // image file (which contains font data) from 
            // generic images
            bmp.SetPixel(0, 0,
                Color.FromArgb(0xC0, 0xDE,
                    (height >> 8) & 0xFF,
                    (height >> 0) & 0xFF
                ));

            // set the second magic number (0xC0DE) for the 
            // new image, and note the ascent of the font. 
            // the ascent is the distance from the top of a 
            // glyph to its baseline. knowing this value
            // helps us to render fonts side-by-side, 
            // aligned by their baseline. 
            int fontAscent = (int)ascent;
            bmp.SetPixel(0, 1,
                Color.FromArgb(0xC0, 0xDE,
                    (fontAscent >> 8) & 0xFF,
                    (fontAscent >> 0) & 0xFF));

            // NOTE: this routine assumes that the glyph is 
            // at least two pixels tall. you may want to add 
            // extra code to enforce this restriction if 
            // you're using very small fonts, but a 
            // 2-pixel tall font won't be ledgible 
            // anyway, so it's not really worth effort.

            // save new game font
            bmp.Save(file, ImageFormat.Png);
        }
    }
}
