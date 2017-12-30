using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Codetopia.Graphics;

namespace Chapter28
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // the only texture for this game
        protected static Texture2D m_texture;
        public Texture2D Texture
        {
            get { return m_texture; }
        }

        // collidable, on-screen objects
        public const int NUM_OBSTACLES = 20;
        protected GameSprite[] m_sprites = new GameSprite[NUM_OBSTACLES];

        // the player-controlled object
        protected GameSprite m_playerSprite = new GameSprite();

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

            // (re)load game texture
            m_texture = Content.Load<Texture2D>(@"media\game");

            // reset game sprites so that they know 
            // to grab the updated texture
            Array.Clear(m_sprites, 0, m_sprites.Length - 1);
            m_playerSprite = null;

            // reset our obstacle course
            ResetSprites();
        }

        // reset our obstacle course
        protected void ResetSprites()
        {
            Random rand = new Random();
            for (int i = 0; i < NUM_OBSTACLES; i++)
            {
                if (m_sprites[i] == null)
                {
                    m_sprites[i] = new GameSprite();
                    m_sprites[i].TextureData = this.Texture;
                    m_sprites[i].OpaqueData = 
                        PixelPerfectHelper.GetOpaqueData(m_sprites[i], 128);
                }
                m_sprites[i].Location = new Vector2(
                    rand.Next(SCREEN_WIDTH), 
                    rand.Next(SCREEN_HEIGHT));
                m_sprites[i].Location = new Vector2(
                    rand.Next(SCREEN_WIDTH), 
                    rand.Next(SCREEN_HEIGHT));
            }

            if (m_playerSprite == null)
            {
                m_playerSprite = new GameSprite();
                m_playerSprite.TextureData = this.Texture;
                m_playerSprite.OpaqueData = PixelPerfectHelper.GetOpaqueData(m_playerSprite, 128);
            }
            m_playerSprite.Location = Vector2.One * 100.0f;
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

            GamePadState pad1 = GamePad.GetState(PlayerIndex.One);
            KeyboardState key1 = Keyboard.GetState();
            ProcessInput(pad1, key1);

            base.Update(gameTime);
        }

        protected void ProcessInput(GamePadState pad1, KeyboardState key1)
        {
            // track changes to the player's location
            Vector2 loc = m_playerSprite.Location;

            // move left or right?
            if (pad1.ThumbSticks.Left.X != 0)
            {
                loc.X += pad1.ThumbSticks.Left.X * 3.0f;
            }
            else if (key1.IsKeyDown(Keys.Left))
            {
                loc.X -= 3.0f;
            }
            else if (key1.IsKeyDown(Keys.Right))
            {
                loc.X += 3.0f;
            }

            // move up or down?
            if (pad1.ThumbSticks.Left.Y != 0)
            {
                loc.Y -= pad1.ThumbSticks.Left.Y * 3.0f;
            }
            else if (key1.IsKeyDown(Keys.Up))
            {
                loc.Y -= 3.0f;
            }
            else if (key1.IsKeyDown(Keys.Down))
            {
                loc.Y += 3.0f;
            }
            m_playerSprite.Location = loc;

            // reset the obstacle course?
            if (key1.IsKeyDown(Keys.Enter) ||
                pad1.Buttons.Start == ButtonState.Pressed)
            {
                ResetSprites();
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

            // draw each obstacle sprite
            foreach (GameSprite sprite in m_sprites)
            {
                if (PixelPerfectHelper.DetectCollision(sprite, m_playerSprite))
                {
                    // collision detected between this sprite and the player
                    sprite.Draw(spriteBatch, Color.Red);
                }
                else
                {
                    // no collision detected
                    sprite.Draw(spriteBatch, Color.Goldenrod);
                }
            }

            // draw the player
            m_playerSprite.Draw(spriteBatch, Color.LightGreen);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
