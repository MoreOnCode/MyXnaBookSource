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
using Codetopia.Graphics.ParticleSystem;

namespace Chapter25
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // a 2d particle emitter
        Emitter2D m_emitter;

        // handy constants to move the emitter around
        private static readonly Vector2 MOVE_LEFT = new Vector2(-5, 0);
        private static readonly Vector2 MOVE_RIGHT = new Vector2(5, 0);
        private static readonly Vector2 MOVE_UP = new Vector2(0, -5);
        private static readonly Vector2 MOVE_DOWN = new Vector2(0, 5);

        // remember pressed state between frames. without this, the 
        // action for the button would be triggered on every frame.
        private bool m_ButtonA = false;
        private bool m_ButtonB = false;
        private bool m_ButtonX = false;
        private bool m_ButtonY = false;

        // two example particle modifiers
        private static readonly Modifier2D m_ModGravity =
            new Modifier2DGravity(200.0f);
        private static readonly Modifier2D m_ModWind =
            new Modifier2DWind(200.0f);

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
            IsFixedTimeStep = false;

            // init back buffer
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();

            // init our emitter
            m_emitter = new Emitter2D();
            m_emitter.ParticlesPerUpdate = 20;
            m_emitter.MaxParticles = 15000;
            m_emitter.EmitterRect = new Rectangle(200, 200, 0, 0);
            m_emitter.RangeColor =
                RangedVector4.FromColors(Color.Orange, Color.Yellow);

            // add our modifiers to the emitter
            m_emitter.AddModifier(m_ModGravity);
            m_emitter.AddModifier(m_ModWind);

            // disable the modifiers for now
            m_ModGravity.Enabled = false;
            m_ModWind.Enabled = false;

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

            // load in particle graphic and set as texture for emitter
            Texture2D tex = Content.Load<Texture2D>(@"media\particle");
            m_emitter.Texture = tex;
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

            // support for game pad and keyboard
            GamePadState pad1 = GamePad.GetState(PlayerIndex.One);
            KeyboardState key1 = Keyboard.GetState();

            // move emitter left or right
            if (pad1.ThumbSticks.Left.X < -0.10 || key1.IsKeyDown(Keys.Left))
            {
                m_emitter.Position += MOVE_LEFT;
            }
            else if (pad1.ThumbSticks.Left.X > 0.10 || key1.IsKeyDown(Keys.Right))
            {
                m_emitter.Position += MOVE_RIGHT;
            }

            // move emitter up or down
            if (pad1.ThumbSticks.Left.Y > 0.10 || key1.IsKeyDown(Keys.Up))
            {
                m_emitter.Position += MOVE_UP;
            }
            else if (pad1.ThumbSticks.Left.Y < -0.10 || key1.IsKeyDown(Keys.Down))
            {
                m_emitter.Position += MOVE_DOWN;
            }

            // enable / disable gravity
            bool buttonPressed = (pad1.Buttons.A == ButtonState.Pressed);
            buttonPressed |= key1.IsKeyDown(Keys.A);
            if (buttonPressed && !m_ButtonA)
            {
                m_ModGravity.Enabled = !m_ModGravity.Enabled;
            }
            m_ButtonA = buttonPressed;

            // enable / disable wind
            buttonPressed = (pad1.Buttons.B == ButtonState.Pressed);
            buttonPressed |= key1.IsKeyDown(Keys.B);
            if (buttonPressed && !m_ButtonB)
            {
                m_ModWind.Enabled = !m_ModWind.Enabled;
            }
            m_ButtonB = buttonPressed;

            // enable / disable emitter
            buttonPressed = (pad1.Buttons.X == ButtonState.Pressed);
            buttonPressed |= key1.IsKeyDown(Keys.X);
            if (buttonPressed && !m_ButtonX)
            {
                m_emitter.Enabled = !m_emitter.Enabled;
            }
            m_ButtonX = buttonPressed;

            // mark emitter as active / inactive
            buttonPressed = (pad1.Buttons.Y == ButtonState.Pressed);
            buttonPressed |= key1.IsKeyDown(Keys.Y);
            if (buttonPressed && !m_ButtonY)
            {
                m_emitter.IsActive = !m_emitter.IsActive;
            }
            m_ButtonY = buttonPressed;

            // tell the emitter to update it's state
            m_emitter.Update((float)gameTime.ElapsedGameTime.TotalSeconds, 2);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // tell the emitter to draw the particles
            spriteBatch.Begin();
            m_emitter.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
