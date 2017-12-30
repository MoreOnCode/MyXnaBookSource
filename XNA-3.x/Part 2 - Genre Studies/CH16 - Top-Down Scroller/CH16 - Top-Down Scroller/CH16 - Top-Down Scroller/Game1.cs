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

namespace CH16___Top_Down_Scroller
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // single-instance game objects
        protected static ScrollingBackground m_background =
            new ScrollingBackground();
        public static Player m_PlayerOne = new Player();
        public static Player m_PlayerTwo = new Player();
        protected static HUD m_Hud = new HUD();

        // the one and only game texture
        protected static Texture2D m_Texture;
        public static Texture2D Texture
        {
            get { return m_Texture; }
            set { m_Texture = value; }
        }

        // a simple helper property to let supporting 
        // routines know when both players are inactive
        public static bool GameOver
        {
            get { return !(m_PlayerOne.IsActive || m_PlayerTwo.IsActive); }
        }

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

            // initialize the brains of our game
            GameObjectManager.Init();

            // initialize our scrolling background helper
            m_background.InitTiles();

            // data that's specific to player one
            m_PlayerOne.StartPosition = new Vector2(160, 360);
            m_PlayerOne.Color = Color.Tomato;
            m_PlayerOne.HudPosition = new Vector2(64, 64);

            // data that's specific to player two
            m_PlayerTwo.StartPosition = new Vector2(480, 360);
            m_PlayerTwo.Color = Color.SlateBlue;
            m_PlayerTwo.HudPosition = new Vector2(384, 64);

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

            Game1.Texture = Content.Load<Texture2D>(@"media\game");
            NumberSprite.Texture =
                Content.Load<Texture2D>(@"media\numbers");
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

            // time (in seconds) since the last time Update was called
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;

            // update game state, attempt to add a new enemy ship
            GameObjectManager.Update(elapsed);
            AddEnemy(elapsed);

            // handle player input
            ProcessInput(elapsed);

            // update our single-instance game objects
            m_background.Update(elapsed);
            m_PlayerOne.Update(elapsed);
            m_PlayerTwo.Update(elapsed);
            m_Hud.Update(elapsed);

            base.Update(gameTime);
        }

        // add a new enemy every two seconds
        protected double EnemyDelay = 2;
        protected double EnemyCountDown = 0;
        protected void AddEnemy(double elapsed)
        {
            EnemyCountDown -= elapsed;
            if (!GameOver && EnemyCountDown < 0)
            {
                EnemyCountDown = EnemyDelay;
                GameObjectManager.AddEnemy();
            }
        }

        // collect and process player input
        public void ProcessInput(double elapsed)
        {
            // collect input data
            GamePadState pad1 = GamePad.GetState(PlayerIndex.One);
            GamePadState pad2 = GamePad.GetState(PlayerIndex.Two);
            KeyboardState key = Keyboard.GetState();

            // process collected data
            ProcessInput(elapsed, m_PlayerOne, pad1);
            ProcessInput(elapsed, m_PlayerOne, key);
            ProcessInput(elapsed, m_PlayerTwo, pad2);
        }

        // handle game pad input
        public void ProcessInput(double elapsed, Player player, GamePadState pad)
        {
            if (!player.IsActive && pad.Buttons.Start == ButtonState.Pressed)
            {
                // start or join the game
                player.Init();
                player.IsActive = true;
            }
            else if (player.IsActive)
            {
                // change in location
                Vector2 delta = Vector2.Zero;

                // moving left or right?
                if (pad.ThumbSticks.Left.X < 0)
                {
                    delta.X = (float)(-player.MovePixelsPerSecond * elapsed);
                }
                else if (pad.ThumbSticks.Left.X > 0)
                {
                    delta.X = (float)(player.MovePixelsPerSecond * elapsed);
                }

                // moving up or down?
                if (pad.ThumbSticks.Left.Y > 0)
                {
                    delta.Y = (float)(-player.MovePixelsPerSecond * elapsed);
                }
                else if (pad.ThumbSticks.Left.Y < 0)
                {
                    delta.Y = (float)(player.MovePixelsPerSecond * elapsed);
                }

                // actually move the player
                player.Location += delta;

                // if the player is pressing the action 
                // button, try to fire a bullet
                if (pad.Buttons.A == ButtonState.Pressed && player.Fire())
                {
                    GameObjectManager.AddBullet(player, true);
                }
            }
        }

        // handle player input from the keyboard
        public void ProcessInput(double elapsed, Player player, KeyboardState key)
        {
            if (!player.IsActive && key.IsKeyDown(Keys.Enter))
            {
                // start or join the game
                player.Init();
                player.IsActive = true;
            }
            else if (player.IsActive)
            {
                // change in location
                Vector2 delta = Vector2.Zero;

                // moving left or right?
                if (key.IsKeyDown(Keys.Left))
                {
                    delta.X = (float)(-player.MovePixelsPerSecond * elapsed);
                }
                else if (key.IsKeyDown(Keys.Right))
                {
                    delta.X = (float)(player.MovePixelsPerSecond * elapsed);
                }

                // moving up or down?
                if (key.IsKeyDown(Keys.Up))
                {
                    delta.Y = (float)(-player.MovePixelsPerSecond * elapsed);
                }
                else if (key.IsKeyDown(Keys.Down))
                {
                    delta.Y = (float)(player.MovePixelsPerSecond * elapsed);
                }


                // actually move the player
                player.Location += delta;

                // if the player is pressing the action 
                // button, try to fire a bullet
                if (key.IsKeyDown(Keys.Space) && player.Fire())
                {
                    GameObjectManager.AddBullet(player, true);
                }
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

            m_background.Draw(spriteBatch);
            GameObjectManager.Draw(spriteBatch);
            m_PlayerOne.Draw(spriteBatch);
            m_PlayerTwo.Draw(spriteBatch);
            m_Hud.Draw(spriteBatch, m_PlayerOne);
            m_Hud.Draw(spriteBatch, m_PlayerTwo);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
