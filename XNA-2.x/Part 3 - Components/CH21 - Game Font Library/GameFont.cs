// GameFont.cs
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Codetopia.Graphics
{
    public class GameFont
    {
        // useful constants
        protected static readonly uint ROW_COLUMN_MASK = 0xFF000000;
        protected static readonly uint CHAR_MASK = 0x0000FFFF;
        protected static readonly uint MAGIC_MASK = 0xFFFF0000;
        protected static readonly uint MAGIC_NUMBER = 0xC0DE0000;
        protected static readonly char INVALID_CHAR = '\xFFFF';

        // don't allow creating an instance with the new operator
        private GameFont() { }

        // the height of the text, in pixels
        protected int _fontHeight = 0;
        public int FontHeight
        {
            get { return _fontHeight; }
        }

        // the ascent of the font, in pixels.
        // difference in y location between top of glyph and the 
        // baseline of the glyph
        protected int _fontAscent = 0;
        public int FontAscent
        {
            get { return _fontAscent; }
        }

        // useful in debugger to see why the font didn't initialize
        private List<string> _messages = new List<string>();
        public string[] ErrorMessages
        {
            get { return _messages.ToArray(); }
        }
        protected void AddMessage(string msg)
        {
            _messages.Add(msg);
        }

        // the texture that holds the glyphs
        private Texture2D _texture = null;
        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        // a collection of glyph descriptors, indexed by unicode char
        private Dictionary<char, GlyphDescriptor> _descriptors = 
            new Dictionary<char, GlyphDescriptor>();

        // given a texture with encoded glyphs, return a BitmapFont object
        public static GameFont FromTexture2D(Texture2D texture)
        {
            // new instance placeholder
            GameFont font = null;

            // first, make sure it's a valid texture
            if (texture == null)
            {
                throw new GameFontException("Texture2D cannot be null.");
            }
            else
            {
                // try to extract the glyphs from the texture
                font = new GameFont();
                font.Texture = texture;
                font.ExtractGlyphDescriptors();
            }

            // return the fruits of our labor
            return font;
        }

        // interpret the encoded data to determine the individual 
        // glyph boundries
        protected void ExtractGlyphDescriptors()
        {
            // save some typing
            int w = Texture.Width;
            int h = Texture.Height;

            // grab the pixels of the texture so we can inspect them
            uint[] data = new uint[w * h];
            Texture.GetData<uint>(data);

            // check for magic numbers
            bool valid = w > 0 && h > 1;
            valid = valid && ((data[0] & MAGIC_MASK) == MAGIC_NUMBER);
            valid = valid && ((data[w] & MAGIC_MASK) == MAGIC_NUMBER);

            // is this a valid font texture
            if (valid)
            {
                // record the height and ascent of this font
                _fontHeight = (int)(data[0] & CHAR_MASK);
                _fontAscent = (int)(data[w] & CHAR_MASK);

                // scan the image for our glyph markers
                for (int y = 0; y < h; y += _fontHeight + 1)
                {
                    // we encode the height and ascent in the first column of 
                    // the first row, so it cannot be a valid glyph. skip it.
                    int nFirstColumn = (y == 0 ? 1 : 0);

                    // if there's no glyph marker here (in the first column), 
                    // there's no point in looking at the rest of the row
                    if ((data[y * w + nFirstColumn] & ROW_COLUMN_MASK) == 
                        ROW_COLUMN_MASK)
                    {
                        // found a marker, scan the row for glyphs
                        for (int x = nFirstColumn; x < w; x++)
                        {
                            // is this a glyph?
                            if ((data[y * w + x] & ROW_COLUMN_MASK) == 
                                ROW_COLUMN_MASK)
                            {
                                // yes. record the details and ...
                                char key = (char)(data[y * w + x] & CHAR_MASK);
                                int top = y + 1;
                                int left = x;
                                int width = 1;

                                // ... keep scanning to determine the width
                                while ((x + width < w - 1) && 
                                    ((data[y * w + x + width] & ROW_COLUMN_MASK)
                                    != ROW_COLUMN_MASK))
                                {
                                    width++;
                                }

                                // record this glyph in our master list
                                AddGlyph(key, left, top, width, _fontHeight);
                                
                                // make sure the scan catches the next glyph
                                x += width - 1;
                            }
                        }
                    }
                }
            }
            else
            {
                // this may be a texture, but it's no gamefont!
                AddMessage("ERROR: Invalid texture. Bad MAGIC_NUMBER.");
            }
        }

        // add glyph to our list of recognized characters
        // top, left, right, and bottom define the texture coordinates
        protected void AddGlyph(char key, 
            int left, int top, int width, int height)
        {
            // make sure we haven't already seen this character
            if (!_descriptors.ContainsKey(key))
            {
                // perform some simple validation
                if (left < 0 || top < 0 || width < 1 || height < 1)
                {
                    // texture bounds specified can't be drawn
                    AddMessage(string.Format(
                        "WARNING: Invalid glyph bounds. [{0},{1},{2},{3}]", 
                        left, top, width, height));
                }
                else
                {
                    // looks good. add it to our list.
                    _descriptors.Add(
                        key, 
                        new GlyphDescriptor(left, top, width, height));
                }
            }
        }

        // draw each character of the string and return the width 
        // and height drawn
        public Vector2 DrawString(SpriteBatch batch, string text, 
            int x, int y, Color color, bool draw)
        {
            // keep track of what's been drawn
            Vector2 v2 = Vector2.Zero;

            // make sure the glyph texture is still there
            if (_texture != null)
            {
                // init return value, assume at least one char was drawn
                v2.Y = _fontHeight;
                v2.X = 0.0f;

                // the location to draw the next character
                Vector2 dest = Vector2.Zero;
                dest.X = x;
                dest.Y = y;

                // break string into characters and process each
                foreach (char c in text.ToCharArray())
                {
                    // make sure this is a recognized glyph
                    if (_descriptors.ContainsKey(c))
                    {
                        // don't actually draw glyph if we're just measuring
                        if (draw)
                        {
                            batch.Draw(
                                _texture, 
                                dest, 
                                _descriptors[c].GetRectangle(), 
                                color);
                        }
                        // increment next location and total width
                        dest.X += _descriptors[c].Width;
                        v2.X += _descriptors[c].Width;
                    }
                }
            }

            // return the bounds of the rendered string
            return v2;
        }

        // overload to draw text in specified color
        public Vector2 DrawString(SpriteBatch batch, string text, 
            int x, int y, Color color)
        {
            return DrawString(batch, text, x, y, color, true);
        }

        // overload to draw white text (default color)
        public Vector2 DrawString(SpriteBatch batch, string text, int x, int y)
        {
            return DrawString(batch, text, x, y, Color.White, true);
        }

        // usefull overload for character-based animated effects
        public Vector2 DrawString(SpriteBatch batch, char c, 
            int x, int y, Color color, bool draw)
        {
            // report size of glyph, if valid
            Vector2 v2 = Vector2.Zero;

            // make sure we have a valid texture
            if (_texture != null)
            {
                // make sure this is a valid glyph
                if (_descriptors.ContainsKey(c))
                {
                    // don't draw if we're just measuring
                    if (draw)
                    {
                        batch.Draw(
                            _texture, 
                            new Vector2(x,y), 
                            _descriptors[c].GetRectangle(), 
                            color);
                    }
                    // glyph was valid, return its measurements
                    v2.Y = _fontHeight;
                    v2.X = _descriptors[c].Width;
                }
            }
            return v2;
        }

        // usefull overload for character-based animated effects
        public Vector2 DrawString(SpriteBatch batch, 
            char c, int x, int y, Color color)
        {
            return DrawString(batch, c, x, y, color, true);
        }

        // usefull overload for character-based animated effects
        public Vector2 DrawString(SpriteBatch batch, char c, int x, int y)
        {
            return DrawString(batch, c, x, y, Color.White, true);
        }


        // go through the motion of drawing the string, without actually 
        // rendering it to the batch. other than blitting the pixels to 
        // the screen, these two methods do pretty much the same tasks, 
        // so why not combine them?
        public Vector2 MeasureString(string text)
        {
            return DrawString(null, text, 0, 0, Color.White, false);
        }

        // usefull overload for character-based animated effects
        public Vector2 MeasureString(char c)
        {
            return DrawString(null, c, 0, 0, Color.White, false);
        }

    }

    // simple class to store individual glyph bounds
    public class GlyphDescriptor
    {
        // left bounds of glyph
        public int Left { get { return Rect.Left; } }

        // top bounds of glyph
        public int Top { get { return Rect.Top; } }

        // width of glyph bounds
        public int Width { get { return Rect.Width; } }

        // height of glyph bounds
        public int Height { get { return Rect.Height; } }

        // most APIs will want a Rect to define bounds
        private Rectangle _rect = new Rectangle();
        protected Rectangle Rect { get { return _rect; } }
        public Rectangle GetRectangle() { return Rect; }

        // only way to set properties is via the constructor
        public GlyphDescriptor(int left, int top, int width, int height)
        {
            _rect = new Rectangle(left, top, width, height);
        }
    }
}
