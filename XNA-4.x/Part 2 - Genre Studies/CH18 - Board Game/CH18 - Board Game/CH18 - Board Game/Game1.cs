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

namespace CH18___Board_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // the one and only game board
        GameBoard m_GameBoard = new GameBoard();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

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

            base.Initialize();
        }

        // screen constants
        const int SCREEN_WIDTH = 640;
        const int SCREEN_HEIGHT = 480;

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

            GameBoard.Texture = Content.Load<Texture2D>(@"media\game");
            NumberSprite.Texture = Content.Load<Texture2D>(@"media\numbers");
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

            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            m_PressDelay -= elapsed;
            ProcessInput();
            m_GameBoard.Update(elapsed);

            base.Update(gameTime);
        }

        // process (human) player input
        public void ProcessInput()
        {
            // should we even process input now? no if current player is CPU
            bool process = m_GameBoard.CurrentPlayerType() == PlayerType.Human;
            process |= m_GameBoard.State == GameState.GameOver;

            if (process)
            {
                // grab device states
                GamePadState pad1 = GamePad.GetState(PlayerIndex.One);
                GamePadState pad2 = GamePad.GetState(PlayerIndex.Two);
                KeyboardState key1 = Keyboard.GetState();

                // if there's only one gamepad, share it
                if (!pad2.IsConnected) pad2 = pad1;

                // process input based on current player
                switch (m_GameBoard.CurrentPlayer)
                {
                    case Player.One:
                        ProcessInput(pad1, key1);
                        break;
                    case Player.Two:
                        ProcessInput(pad2, key1);
                        break;
                }
            }
        }

        // very simple way to keep from registering multiple button presses
        private double m_PressDelay = 0;
        private const double PRESS_DELAY = 0.25;

        // actually process the input
        private void ProcessInput(GamePadState pad, KeyboardState kbd)
        {
            bool pressed;

            // move to previous valid move
            pressed = pad.Triggers.Left > 0;
            pressed |= pad.DPad.Left == ButtonState.Pressed;
            pressed |= pad.ThumbSticks.Left.X < 0;
            pressed |= kbd.IsKeyDown(Keys.Left);
            if (pressed && m_PressDelay <= 0)
            {
                m_PressDelay = PRESS_DELAY;
                m_GameBoard.PreviousValidMove();
            }

            // move to next valid move
            pressed = pad.Triggers.Right > 0;
            pressed |= pad.DPad.Right == ButtonState.Pressed;
            pressed |= pad.ThumbSticks.Left.X > 0;
            pressed |= kbd.IsKeyDown(Keys.Right);
            if (pressed && m_PressDelay <= 0)
            {
                m_PressDelay = PRESS_DELAY;
                m_GameBoard.NextValidMove();
            }

            // commit selection as your move
            pressed = pad.Buttons.A == ButtonState.Pressed;
            pressed |= kbd.IsKeyDown(Keys.Space);
            if (pressed && m_PressDelay <= 0)
            {
                m_PressDelay = PRESS_DELAY;
                m_GameBoard.MakeMove();
            }

            // start a new game
            // only works when it's a human's turn so that AI threads
            // (if any) have time to complete
            pressed = pad.Buttons.Start == ButtonState.Pressed;
            pressed |= kbd.IsKeyDown(Keys.Enter);
            if (pressed && m_PressDelay <= 0)
            {
                m_PressDelay = PRESS_DELAY;
                m_GameBoard = new GameBoard();
            }

            // pick previous player two type
            pressed = pad.Buttons.LeftShoulder == ButtonState.Pressed;
            pressed |= kbd.IsKeyDown(Keys.PageUp);
            if (pressed && m_PressDelay <= 0)
            {
                m_PressDelay = PRESS_DELAY;
                m_GameBoard.PreviousPlayerType();
            }

            // pick next player two type
            pressed = pad.Buttons.RightShoulder == ButtonState.Pressed;
            pressed |= kbd.IsKeyDown(Keys.PageDown);
            if (pressed && m_PressDelay <= 0)
            {
                m_PressDelay = PRESS_DELAY;
                m_GameBoard.NextPlayerType();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            m_GameBoard.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
