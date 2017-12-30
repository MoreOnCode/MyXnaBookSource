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
using Microsoft.Xna.Framework.Storage;

namespace CH11___Storage
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // the one and only game texture
        Texture2D m_Texture;

        // coordinates of the images within the texture
        Rectangle m_rectSave = new Rectangle(0, 0, 64, 26);
        Rectangle m_rectLoad = new Rectangle(0, 28, 64, 26);
        Rectangle m_rectArrow = new Rectangle(0, 54, 25, 32);
        Rectangle m_rectStamp = new Rectangle(6, 89, 33, 31);

        // instance of our game data object
        GameData m_data = new GameData();

        // position of the arror
        Vector2 m_cursor = Vector2.One * 200.0f;

        // location of the buttons
        Vector2 m_posButtonSave = new Vector2(64, 64);
        Vector2 m_posButtonLoad = new Vector2(64, 94);

        // about to save or load data?
        bool m_pressedSave = false;
        bool m_pressedLoad = false;

        // storage API references
        StorageDevice m_storage = null;
        IAsyncResult m_resultStorage = null;

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

            // load our game graphics
            m_Texture = Content.Load<Texture2D>(@"media\game");
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

            // get a reference to the input states, and process them
            GamePadState pad1 = GamePad.GetState(PlayerIndex.One);
            KeyboardState key1 = Keyboard.GetState();
            ProcessInput(pad1, key1);

            base.Update(gameTime);
        }

        protected void ProcessInput(GamePadState pad1, KeyboardState key1)
        {
            // is the player moving left or right?
            if (pad1.ThumbSticks.Left.X < 0)
            {
                m_cursor.X += pad1.ThumbSticks.Left.X * 3.0f;
            }
            else if (key1.IsKeyDown(Keys.Left))
            {
                m_cursor.X -= 3.0f;
            }
            else if (pad1.ThumbSticks.Left.X > 0)
            {
                m_cursor.X += pad1.ThumbSticks.Left.X * 3.0f;
            }
            else if (key1.IsKeyDown(Keys.Right))
            {
                m_cursor.X += 3.0f;
            }

            // is the player moving up or down?
            if (pad1.ThumbSticks.Left.Y < 0)
            {
                m_cursor.Y -= pad1.ThumbSticks.Left.Y * 3.0f;
            }
            else if (key1.IsKeyDown(Keys.Down))
            {
                m_cursor.Y += 3.0f;
            }
            else if (pad1.ThumbSticks.Left.Y > 0)
            {
                m_cursor.Y -= pad1.ThumbSticks.Left.Y * 3.0f;
            }
            else if (key1.IsKeyDown(Keys.Up))
            {
                m_cursor.Y -= 3.0f;
            }

            // is the player pressing the clear button?
            if (pad1.Buttons.Start == ButtonState.Pressed ||
                key1.IsKeyDown(Keys.Enter))
            {
                // clear our list of stamps
                m_data.Stamps.Clear();
            }

            // is the player pressing the action button?
            if (pad1.Buttons.A == ButtonState.Pressed ||
                key1.IsKeyDown(Keys.Space))
            {
                // only register a save or load if there's not
                // a save or load currently in progress
                if (m_resultStorage == null)
                {
                    // clear the button press states
                    m_pressedSave = false;
                    m_pressedLoad = false;

                    // is player pressing the save button?
                    if (InRect(m_cursor, m_rectSave, m_posButtonSave))
                    {
                        m_pressedSave = true;
                    }
                    // is player pressing the load button?
                    else if (InRect(m_cursor, m_rectLoad, m_posButtonLoad))
                    {
                        m_pressedLoad = true;
                    }
                    // add a new stamp to our list
                    else if (!m_data.Stamps.Contains(m_cursor))
                    {
                        m_data.Stamps.Add(m_cursor);
                    }
                }
            }
            // the player isn't pressing the action button
            else
            {
                // there is no load or save in progress
                if (m_resultStorage == null)
                {
                    // the player just released the action button
                    if (m_pressedLoad || m_pressedSave)
                    {
                        // show the storage guide on Xbox, has no 
                        // effect on Windows
                        m_resultStorage =
                            //Guide.BeginShowStorageDeviceSelector(null, null);
                            StorageDevice.BeginShowSelector(null, null);
                    }
                }
                // there is a load or save in progress
                else
                {
                    // has the player selected a device?
                    if (m_resultStorage.IsCompleted)
                    {
                        // get a reference to the selected device
                        m_storage =
                            //Guide.EndShowStorageDeviceSelector(m_resultStorage);
                            StorageDevice.EndShowSelector(m_resultStorage);

                        // save was requested, save our data
                        if (m_pressedSave)
                        {
                            GameStorage.Save(m_storage, m_data, "test01");
                        }
                        // load was requested, load our data
                        else if (m_pressedLoad)
                        {
                            m_data = GameStorage.Load(m_storage, "test01");
                        }

                        // reset up our load / save state data
                        m_storage = null;
                        m_resultStorage = null;
                        m_pressedSave = false;
                        m_pressedLoad = false;
                    }
                }
            }
        }

        // determine whether or not the cursor is over a button
        protected bool InRect(Vector2 cursor, Rectangle rect, Vector2 loc)
        {
            // move the rect to the cursor position
            rect.X = (int)Math.Round(loc.X);
            rect.Y = (int)Math.Round(loc.Y);

            // check the cursor position against the button rect
            return
                cursor.X >= rect.Left && cursor.X <= rect.Right &&
                cursor.Y >= rect.Top && cursor.Y <= rect.Bottom;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // draw all of the stamps that the player has made
            DrawStamps();

            // if the player is pressing the save button, highlight it
            if (m_pressedSave)
            {
                spriteBatch.Draw(
                    m_Texture, m_posButtonSave, m_rectSave, Color.Goldenrod);
            }
            // draw the normal save button
            else
            {
                spriteBatch.Draw(
                    m_Texture, m_posButtonSave, m_rectSave, Color.White);
            }

            // if the player is pressing the load button, highlight it
            if (m_pressedLoad)
            {
                spriteBatch.Draw(
                    m_Texture, m_posButtonLoad, m_rectLoad, Color.Goldenrod);
            }
            // draw the normal load button
            else
            {
                spriteBatch.Draw(
                    m_Texture, m_posButtonLoad, m_rectLoad, Color.White);
            }

            // draw the cursor
            spriteBatch.Draw(m_Texture, m_cursor, m_rectArrow, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        // used to calc the center of the stamp
        private Vector2 m_StampOffset = Vector2.Zero;
        protected void DrawStamps()
        {
            // center the stamp on the cursor location
            m_StampOffset.X = 0 - m_rectStamp.Width / 2;
            m_StampOffset.Y = 0 - m_rectStamp.Height / 2;

            // draw each stamp in our list
            foreach (Vector2 pos in m_data.Stamps)
            {
                spriteBatch.Draw(
                    m_Texture, pos + m_StampOffset, m_rectStamp, Color.White);
            }
        }
    }
}
