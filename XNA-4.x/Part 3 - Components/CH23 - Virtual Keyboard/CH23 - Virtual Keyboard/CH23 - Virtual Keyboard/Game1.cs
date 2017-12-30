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
using Codetopia.Input;

namespace CH23___Virtual_Keyboard
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // bitmap font for our bouncing text
        protected GameFont m_font = null;

        // virtual keyboard to edit bouncing text
        protected VirtualKeyboard m_VirtualKeyboard = new VirtualKeyboard();

        // location and speed of the bouncing text
        protected Vector2 m_TextLocation = new Vector2(20.0f, 20.0f);
        protected Vector2 m_TextVelocity = new Vector2(83.0f, 67.0f);

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
            //// use a fixed frame rate of 30 frames per second
            //IsFixedTimeStep = true;
            //TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 33);

            // run at full speed
            IsFixedTimeStep = false;

            // set screen size
            InitScreen();

            // iniialize the virtual keyboard
            m_VirtualKeyboard.Text = "Press space bar, or X on GamPad.";
            m_VirtualKeyboard.Caption = "Bouncing Text";
            m_VirtualKeyboard.CenterIn(
                new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT));

            base.Initialize();
        }

        // screen-related init tasks
        public void InitScreen()
        {
            // back buffer
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load the texture for the virtual keyboard dialog
            m_VirtualKeyboard.Texture =
                Content.Load<Texture2D>(@"media\virtualkeyboard");

            // create our bitmap font, share it with the virtual keyboard
            m_font = GameFont.FromTexture2D(
                Content.Load<Texture2D>(@"media\font"));
            m_VirtualKeyboard.Font = m_font;
        }

        // avoid exiting the game when the player cancels an edit
        protected bool m_IgnoreExit = false;

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
            // get state of pad in slot one
            GamePadState pad1 = GamePad.GetState(PlayerIndex.One);
            KeyboardState key1 = Keyboard.GetState();

            // is the player requesting that we exit?
            bool isExit =
                pad1.Buttons.Back == ButtonState.Pressed ||
                key1.IsKeyDown(Keys.Escape);

            // make sure we're not capturing an "exit game" just after 
            // a "cancel edit" since they use the same button presses
            if (!isExit)
            {
                // exit button isn't being pressed now, ignore next exit 
                // request if the virtual keyboard is currently visible
                m_IgnoreExit = m_VirtualKeyboard.Visible;
            }
            else if (!m_IgnoreExit)
            {
                // exit button is being pressed now, and it has been
                // released since the virtual keyboard was last visible
                this.Exit();
            }

            // player requested virtual keyboard?
            if (pad1.Buttons.X == ButtonState.Pressed ||
                key1.IsKeyDown(Keys.Space))
            {
                m_VirtualKeyboard.Show();
            }

            // calculate the number of seconds since the last update
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;

            // animate our bouncing text
            UpdateText(elapsed);

            // allow the virtual keyboard to process player input
            m_VirtualKeyboard.Update(elapsed);

            base.Update(gameTime);
        }

        // animate the bouncing text
        protected void UpdateText(double elapsed)
        {
            // move bouncing text along current trajectory
            m_TextLocation += (float)elapsed * m_TextVelocity;

            // check bounds (left)
            if (m_TextLocation.X < 0)
            {
                m_TextLocation.X = 0;
                m_TextVelocity.X *= -1;
            }

            // check bounds (top)
            if (m_TextLocation.Y < 0)
            {
                m_TextLocation.Y = 0;
                m_TextVelocity.Y *= -1;
            }

            // check bounds (right)
            if (m_TextLocation.X > SCREEN_WIDTH - m_VirtualKeyboard.TextSize.X)
            {
                m_TextLocation.X = SCREEN_WIDTH - m_VirtualKeyboard.TextSize.X;
                m_TextVelocity.X *= -1;
            }

            // check bounds (bottom)
            if (m_TextLocation.Y > SCREEN_HEIGHT - m_VirtualKeyboard.TextSize.Y)
            {
                m_TextLocation.Y = SCREEN_HEIGHT - m_VirtualKeyboard.TextSize.Y;
                m_TextVelocity.Y *= -1;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw virtual keyboard over the bouncing text
            spriteBatch.Begin();
            DrawText(spriteBatch);
            m_VirtualKeyboard.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // draw the bouncing text at its current location
        protected void DrawText(SpriteBatch batch)
        {
            m_font.DrawString(
                batch,
                m_VirtualKeyboard.Text,
                (int)(m_TextLocation.X + 0.5f),
                (int)(m_TextLocation.Y + 0.5f));
        }
    }
}
