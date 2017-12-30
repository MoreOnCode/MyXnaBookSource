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

namespace CH07___Input_GamePad
{
    class ButtonSprite
    {
        // texture for the released digital button state
        private Texture2D m_TextureNormal;
        public Texture2D TextureNormal
        {
            get { return m_TextureNormal; }
            set { m_TextureNormal = value; }
        }

        // bounds of the released state graphic
        private Rectangle m_RectNormal = Rectangle.Empty;
        public Rectangle RectNormal
        {
            get { return m_RectNormal; }
            set { m_RectNormal = value; }
        }

        // texture for the pressed digital button state
        private Texture2D m_TexturePressed;
        public Texture2D TexturePressed
        {
            get { return m_TexturePressed; }
            set { m_TexturePressed = value; }
        }

        // bounds of the pressed state graphic
        private Rectangle m_RectPressed = Rectangle.Empty;
        public Rectangle RectPressed
        {
            get { return m_RectPressed; }
            set { m_RectPressed = value; }
        }

        // location on the screen where this button should be drawn
        public Vector2 Location = Vector2.Zero;
    }
}
