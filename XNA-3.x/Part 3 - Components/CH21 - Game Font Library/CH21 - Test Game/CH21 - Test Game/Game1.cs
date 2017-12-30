using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Codetopia.Graphics;

namespace CH21___Test_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // textures for game fonts
        GameFont m_fontVerdana = null;
        GameFont m_fontMistral = null;

        // animated text variables
        float m_fAngle = 0.0f;
        char[] m_chrAnimatedText =
            "This text animated for an old sk00l wave effect ..."
            .ToCharArray();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        // screen constants
        const int SCREEN_WIDTH = 640;
        const int SCREEN_HEIGHT = 480;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // use a fixed frame rate of 30 frames per second
            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 33);

            // init back buffer
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load our font textures, create corresponding game fonts
            m_fontVerdana = GameFont.FromTexture2D(
                Content.Load<Texture2D>(@"fonts\Verdana12Bold"));
            m_fontMistral = GameFont.FromTexture2D(
                Content.Load<Texture2D>(@"fonts\Mistral11"));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // update angle for animated text
            m_fAngle += 2.0f;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // left edge of drawable screen
            int xMin = 32;

            // keep track of current text row
            int y = 32;

            // draw Verdana font info
            y += (int)m_fontVerdana.DrawString(spriteBatch,
                "Font - Verdana 12 Bold", xMin, y).Y;
            y += (int)m_fontVerdana.DrawString(spriteBatch,
                "Font Height: " + m_fontVerdana.FontHeight, xMin, y).Y;
            y += (int)m_fontVerdana.DrawString(spriteBatch,
                "Font Ascent: " + m_fontVerdana.FontAscent, xMin, y).Y;

            // draw Mistral font info
            y += (int)m_fontMistral.DrawString(spriteBatch,
                "Font - Mistral 11 Normal", xMin, y).Y;
            y += (int)m_fontMistral.DrawString(spriteBatch,
                "Font Height: " + m_fontMistral.FontHeight, xMin, y).Y;
            y += (int)m_fontMistral.DrawString(spriteBatch,
                "Font Ascent: " + m_fontMistral.FontAscent, xMin, y).Y;

            // draw text using two fonts, aligned by their baselines
            int x = xMin;
            int dAscent = m_fontVerdana.FontAscent - m_fontMistral.FontAscent;
            x += (int)m_fontMistral.DrawString(spriteBatch,
                "This is a ", x, y + dAscent).X;
            x += (int)m_fontVerdana.DrawString(spriteBatch,
                "Big Fat", x, y, Color.Gold).X;
            x += (int)m_fontMistral.DrawString(spriteBatch,
                " test!", x, y + dAscent).X;

            // animated text demo
            DrawWavingText(spriteBatch,
                m_fontVerdana, m_chrAnimatedText, Color.GreenYellow);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        // render animated text, moves according to m_angle
        protected void DrawWavingText(SpriteBatch batch,
            GameFont font, char[] letters, Color color)
        {
            // top left of text, if it were drawn just below 
            // the vertical center of the screen
            int x = 32;
            int y = SCREEN_HEIGHT / 2;

            // vary the top location of each character
            int dy = 0;
            for (int i = 0; i < letters.Length; i++)
            {
                // simple sin function to produce a wave from -20.0f to +20.0f
                dy = (int)Math.Round(
                    Math.Sin(MathHelper.ToRadians(m_fAngle + (i * 10))) *
                    20.0f);
                // render each letter seperately
                x += (int)font.DrawString(batch,
                    letters[i], x, y + dy, color).X;
            }
        }
    }
}
