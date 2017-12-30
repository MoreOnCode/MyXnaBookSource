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

namespace CH17___Solitare
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // the main logic and game rules
        protected GameLogic m_logic;

        // which card image are we using?
        protected int m_currentImage = 0;
        protected string[] m_images = {
            @"media\white",
            @"media\simple",
            @"media\bordered",
            @"media\ornamental",
        };

        // use the next card image
        protected void NextImage()
        {
            m_currentImage += 1;
            if (m_currentImage >= m_images.Length)
            {
                m_currentImage = 0;
            }
            ReloadImage();
        }

        // use the previous card image
        protected void PreviousImage()
        {
            m_currentImage -= 1;
            if (m_currentImage < 0)
            {
                m_currentImage = m_images.Length - 1;
            }
            ReloadImage();
        }

        // load the currently selected card image
        protected void ReloadImage()
        {
            GameLogic.Texture = Content.Load<Texture2D>(m_images[m_currentImage]);
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

            // load the (only) game texture
            ReloadImage();

            // if we haven't done so yet, create the game logic object
            if (m_logic == null)
            {
                // game logic has to be instanciated AFTER the game texture
                // is loaded, it inspects the texture to determine the 
                // layout of the card images
                m_logic = new GameLogic();
                m_logic.Init();
                m_logic.NewGame();
            }
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

            // record the elapsed time (in seconds)
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;

            // used for simple button delay logic
            m_LastButtonPress += elapsed;

            // process player input
            ProcessButtons();

            // update card containers
            if (m_logic.Update(elapsed))
            {
                // at least one deck is animating, block player input
                m_LastButtonPress = 0;
            }

            base.Update(gameTime);
        }

        // button repeat delay (in seconds)
        public const double BUTTON_DELAY = 0.25;

        // don't register repeat button presses on
        // every frame, limit to once every 
        // BUTTON_DELAY seconds as long as a button
        // is pressed.
        protected double m_LastButtonPress = BUTTON_DELAY;

        // actually process player input
        protected void ProcessButtons()
        {
            // support for keyboard and first game pad
            KeyboardState key1 = Keyboard.GetState();
            GamePadState pad1 = GamePad.GetState(PlayerIndex.One);

            // if no buttons are being pressed, reset the delay timer
            if (NoButtonsPressed(key1, pad1))
            {
                m_LastButtonPress = BUTTON_DELAY;
            }
            else if (m_LastButtonPress > BUTTON_DELAY)
            {
                // keep delay active as long as any registered button is pressed
                bool pressed = true;
                if (key1.IsKeyDown(Keys.Enter) ||
                    pad1.Buttons.Start == ButtonState.Pressed)
                {
                    // start a new game
                    m_logic.NewGame();
                }
                else if (key1.IsKeyDown(Keys.Tab) ||
                    pad1.Buttons.Y == ButtonState.Pressed)
                {
                    // move card from stock to waste pile
                    m_logic.DrawCardFromStock();
                }
                else if (key1.IsKeyDown(Keys.Left) ||
                    pad1.DPad.Left == ButtonState.Pressed ||
                    pad1.ThumbSticks.Left.X < 0)
                {
                    // move cursor left
                    m_logic.Cursor -= 1;
                }
                else if (key1.IsKeyDown(Keys.Right) ||
                    pad1.DPad.Right == ButtonState.Pressed ||
                    pad1.ThumbSticks.Left.X > 0)
                {
                    // move cursor right
                    m_logic.Cursor += 1;
                }
                else if (key1.IsKeyDown(Keys.LeftControl) ||
                    pad1.Buttons.B == ButtonState.Pressed)
                {
                    // try to move a card into home pile
                    m_logic.GoHome();
                }
                else if (key1.IsKeyDown(Keys.Space) ||
                    pad1.Buttons.A == ButtonState.Pressed)
                {
                    if (m_logic.Selected == m_logic.Cursor)
                    {
                        // deselect selected cards
                        m_logic.Selected = -1;
                    }
                    else if (m_logic.Selected >= 0)
                    {
                        // try to move cards between chutes
                        m_logic.MoveCards();
                    }
                    else
                    {
                        // select the current chute, or move a
                        // card from the waste pile to this chute
                        m_logic.Select();
                    }
                }
                else if (key1.IsKeyDown(Keys.PageUp) ||
                    pad1.Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    // select previous card back image
                    CardBack.PreviousCardBack();
                }
                else if (key1.IsKeyDown(Keys.PageDown) ||
                    pad1.Buttons.RightShoulder == ButtonState.Pressed)
                {
                    // select next card back image
                    CardBack.NextCardBack();
                }
                else if (key1.IsKeyDown(Keys.Home) ||
                    pad1.Triggers.Left > 0)
                {
                    // select previous game texture
                    PreviousImage();
                }
                else if (key1.IsKeyDown(Keys.End) ||
                    pad1.Triggers.Right > 0)
                {
                    // select next game texture
                    NextImage();
                }
                else
                {
                    // no buttons that we care about were pressed,
                    // reset the button delay variables
                    pressed = false;
                }

                if (pressed)
                {
                    // start countdown for next button repeat
                    m_LastButtonPress = 0;
                }
            }
        }

        // see if any buttons are being pressed 
        protected bool NoButtonsPressed(KeyboardState key1, GamePadState pad1)
        {
            return
                key1.GetPressedKeys().Length == 0 &&
                pad1.DPad.Down == ButtonState.Released &&
                pad1.DPad.Up == ButtonState.Released &&
                pad1.DPad.Left == ButtonState.Released &&
                pad1.DPad.Right == ButtonState.Released &&
                pad1.ThumbSticks.Left.X == 0 &&
                pad1.ThumbSticks.Left.Y == 0 &&
                pad1.Triggers.Left == 0 &&
                pad1.Triggers.Right == 0 &&
                pad1.Buttons.LeftShoulder == ButtonState.Released &&
                pad1.Buttons.RightShoulder == ButtonState.Released &&
                pad1.Buttons.A == ButtonState.Released &&
                pad1.Buttons.B == ButtonState.Released &&
                pad1.Buttons.X == ButtonState.Released &&
                pad1.Buttons.Y == ButtonState.Released &&
                pad1.Buttons.Start == ButtonState.Released;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw all the game objects
            spriteBatch.Begin();
            m_logic.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
