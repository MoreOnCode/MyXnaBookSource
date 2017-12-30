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

using PixelPerfect2D;
using GameFonts;

namespace CH14___BrickBreaker
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // main game texture
        protected static Texture2D m_GameTexture = null;
        public static Texture2D GameTexture
        {
            get { return m_GameTexture; }
        }

        // bitmap game font and texture
        protected static GameFont m_GameFont = null;
        protected static Texture2D m_GameFontTexture = null;

        // boundry rectangles
        public static readonly Rectangle BackgroundRect =
            new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
        public static readonly Rectangle PlayableRect =
            new Rectangle(40, 40, 400, 400);

        // texture rectangles
        public static readonly Rectangle BallRect =
            new Rectangle(65, 481, 14, 14);
        public static readonly Rectangle PaddleRect =
            new Rectangle(4, 481, 56, 14);
        public static readonly Rectangle BrickRect =
            new Rectangle(80, 481, 40, 20);

        // brick sprites for current level
        protected List<BrickSprite> m_Bricks = new List<BrickSprite>();

        // sprites for paddle and ball
        protected PaddleSprite m_spritePaddle = new PaddleSprite();
        protected BallSprite m_spriteBall = new BallSprite();

        // shared opaque data, used by pixel-perfect 
        // 2d collision detection helper
        public static bool[,] BrickOpaqueData = null;
        public static bool[,] PaddleOpaqueData = null;
        public static bool[,] BallOpaqueData = null;

        // game level (display is one greater than actual level number)
        protected int m_Level = 0;
        protected string LevelString = "1";
        public int Level
        {
            get { return m_Level; }
            set
            {
                if (m_Level != value)
                {
                    m_Level = value;
                    LevelString = (m_Level + 1).ToString("0");
                }
            }
        }

        // score for current game
        protected int m_Score = 0;
        protected string ScoreString = "0";
        public int Score
        {
            get { return m_Score; }
            set
            {
                m_Score = value;
                ScoreString = m_Score.ToString("#,##0");
                if (Score > HiScore) { HiScore = Score; }
            }
        }

        // highest score seen for this session
        protected int m_HiScore = 0;
        protected string HiScoreString = "0";
        public int HiScore
        {
            get { return m_HiScore; }
            set
            {
                if (m_HiScore != value)
                {
                    m_HiScore = value;
                    HiScoreString = m_HiScore.ToString("#,##0");
                }
            }
        }

        // number of lives remaining
        protected int m_Lives = 3;
        protected string LivesString = "3";
        public int Lives
        {
            get { return m_Lives; }
            set
            {
                if (m_Lives != value)
                {
                    m_Lives = value;
                    LivesString = m_Lives.ToString("0");
                }
            }
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

            // load our game levels into memory
            LevelManager.LoadProject(@"levels\example.bbp");

            // reset the game
            InitializeGame();

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

            // load the main game texture
            m_GameTexture =
                Content.Load<Texture2D>(@"media\brickbreaker");

            // extract the opaque data from the sprite images
            PaddleOpaqueData =
                PixelPerfectHelper.GetOpaqueData(m_spritePaddle);
            BallOpaqueData =
                PixelPerfectHelper.GetOpaqueData(m_spriteBall);
            BrickOpaqueData =
                PixelPerfectHelper.GetOpaqueData(new BrickSprite());

            // initialize our game font
            m_GameFontTexture =
                Content.Load<Texture2D>(@"media\Verdana8Bold");
            m_GameFont = GameFont.FromTexture2D(m_GameFontTexture);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        // reset state variables, restart level 0
        protected void InitializeGame()
        {
            Level = 0;
            Lives = 3;
            Score = 0;
            InitializeLevel();
        }

        // reset level state variables
        protected void InitializeLevel()
        {
            // get level data
            LevelData level = LevelManager.GetLevel(Level);

            // clear existing bricks, repopulate our collection 
            // with those stored in the current level data
            m_Bricks.Clear();
            foreach (Brick brick in level.Bricks)
            {
                // copy the data so that it's available when the 
                // level is replayed
                BrickSprite sprite = new BrickSprite();
                sprite.Brick = brick;
                m_Bricks.Add(sprite);
            }

            // reset paddle and ball
            InitializePaddle();
        }

        // random number helper
        protected Random m_rand = new Random();

        // center paddle and ball, reset ball speed
        protected void InitializePaddle()
        {
            // center paddle and ball
            m_spritePaddle.Location = new Vector2(217, 415);
            m_spriteBall.Location = new Vector2(233, 401);

            // set initial speed and direction of ball
            m_spriteBall.Movement =
                new Vector2(m_rand.Next(120) - 60, -90);
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

            // total elapsed seconds since last frame
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;

            // process player input from keyboard and game pad
            ProcessInput(elapsed);

            // have the ball update itself; returns true as long as 
            // the ball is still in play
            if (m_spriteBall.Update(elapsed))
            {
                CheckForBallBrickCollision(elapsed);
                CheckForBallPaddleCollision(elapsed);
            }
            else
            {
                // out of bounds! subtract a life
                Lives--;

                // check for game over condition
                if (Lives <= 0)
                {
                    // out of lives, restart game
                    InitializeGame();
                }
                else
                {
                    // still have a life left, reset paddle and ball
                    InitializePaddle();
                }
            }

            // no active bricks? start new level
            if (!ScreenHasBricks())
            {
                Level++;
                InitializeLevel();
            }

            base.Update(gameTime);
        }

        // see if there are any bricks on the screen
        protected bool ScreenHasBricks()
        {
            bool foundBrick = false;

            // scan for active bricks
            foreach (BrickSprite sprite in m_Bricks)
            {
                foundBrick |= sprite.Active;

                // found at least one, stop searching
                if (foundBrick) { break; }
            }

            return foundBrick;
        }

        // did the ball collide with any bricks?
        protected void CheckForBallBrickCollision(double elapsed)
        {
            // calculate the center of the ball
            Vector2 ballCenter = Vector2.Zero;
            ballCenter.X =
                m_spriteBall.Location.X +
                m_spriteBall.TextureRect.Width / 2;
            ballCenter.Y =
                m_spriteBall.Location.Y +
                m_spriteBall.TextureRect.Height / 2;

            // check for collision with each brick
            foreach (BrickSprite brickSprite in m_Bricks)
            {
                // only check active bricks (drawn bricks)
                bool collision =
                    brickSprite.Active && PixelPerfectHelper
                    .DetectCollision(m_spriteBall, brickSprite);

                // did ball collide with an ACTIVE brick?
                if (collision)
                {
                    // local variables to save some typing
                    float dx = m_spriteBall.Movement.X;
                    float dy = m_spriteBall.Movement.Y;

                    // local variables to save some typing
                    float ballWidth = m_spriteBall.TextureRect.Width;
                    float brickLeft = brickSprite.Location.X;
                    float brickWidth = brickSprite.TextureRect.Width;
                    float brickRight = brickLeft + brickWidth;

                    // what from which direction did ball hit?
                    if (ballCenter.X < brickLeft)
                    {
                        // struck brick from left
                        dx = -Math.Max(Math.Abs(dx), ballWidth / 2);
                    }
                    else if (ballCenter.X > brickRight)
                    {
                        // struck brick from right
                        dx = Math.Max(Math.Abs(dx), ballWidth / 2);
                    }
                    else
                    {
                        // hit broad side of brick
                        dy = -dy;
                    }

                    // update the ball's location and movement
                    m_spriteBall.Movement = new Vector2(dx, dy);

                    // back track to avoid multiple hits when the ball is
                    // traveling at max (or near max) velocity
                    m_spriteBall.Update(elapsed);

                    // add some points for each brick cleared
                    Score += 5;

                    // notify the brick that it has been hit
                    brickSprite.Brick.RegisterHit();
                }
            }
        }

        // did the ball collide with the paddle?
        protected void CheckForBallPaddleCollision(double elapsed)
        {
            // calculate the horizontal center of the ball
            float ballCenterX =
                m_spriteBall.Location.X +
                m_spriteBall.TextureRect.Width / 2;

            // only register paddle collision when ball is moving down
            bool collision =
                m_spriteBall.Movement.Y > 0 && PixelPerfectHelper
                .DetectCollision(m_spriteBall, m_spritePaddle);

            // did ball collide with paddle?
            if (collision)
            {
                // simple deflection where movement.x is as function 
                // of the ball's distance from the center of the paddle
                float paddleCenterX =
                    m_spritePaddle.Location.X +
                    m_spritePaddle.TextureRect.Width / 2;
                float dx = (ballCenterX - paddleCenterX) * 2.5f;

                // reverse direction, and increase speed by 10%;
                // not too fast, though
                float dy = Math.Max(
                    -1.1f * m_spriteBall.Movement.Y,
                    -380.0f);

                // update the ball's movement vector with our calculations
                m_spriteBall.SetMovement(dx, dy);

                // increment score slightly for each volley
                Score += 1;
            }
        }

        // process the player's input from game pad and keyboard
        protected void ProcessInput(double elapsed)
        {
            // grab keyboard and game pad states
            GamePadState pad1 = GamePad.GetState(PlayerIndex.One);
            KeyboardState key1 = Keyboard.GetState();

            // temp variables to calc new location for paddle
            Vector2 loc = m_spritePaddle.Location;
            float dx = 0.0f;

            // calc dx for paddle, based on input
            if (pad1.ThumbSticks.Left.X != 0.0f)
            {
                dx = pad1.ThumbSticks.Left.X;
            }
            else if (key1.IsKeyDown(Keys.Left))
            {
                dx = -1.0f;
            }
            else if (key1.IsKeyDown(Keys.Right))
            {
                dx = 1.0f;
            }

            // apply dx to location
            loc.X += (float)(255.0f * dx * elapsed);

            // stay within playable bounds
            if (loc.X < PlayableRect.Left)
            {
                loc.X = PlayableRect.Left;
            }
            else if
                (loc.X > PlayableRect.Right - m_spritePaddle.TextureRect.Width)
            {
                loc.X = PlayableRect.Right - m_spritePaddle.TextureRect.Width;
            }

            // actually update the paddle's location
            m_spritePaddle.Location = loc;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // draw game objects
            DrawBackground(spriteBatch);
            DrawBricks(spriteBatch);
            DrawPaddle(spriteBatch);
            DrawBall(spriteBatch);

            // draw HUD, with simple "dropshadow"
            DrawHUD(spriteBatch, 473, 43, Color.Black);
            DrawHUD(spriteBatch, 472, 44, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        // draw the background
        protected void DrawBackground(SpriteBatch batch)
        {
            batch.Draw(
                GameTexture,
                BackgroundRect,
                BackgroundRect,
                Color.White);
        }

        // generic helper to draw any game sprite
        protected void DrawSprite(
            SpriteBatch batch, IPixelPerfectSprite sprite)
        {
            if (sprite.Active)
            {
                batch.Draw(
                    sprite.TextureData,
                    sprite.Location,
                    sprite.TextureRect,
                    sprite.Tint);
            }
        }

        // draw all active bricks
        protected void DrawBricks(SpriteBatch batch)
        {
            foreach (BrickSprite sprite in m_Bricks)
            {
                DrawSprite(batch, sprite);
            }
        }

        // draw the paddle in it's current location
        protected void DrawPaddle(SpriteBatch batch)
        {
            DrawSprite(batch, m_spritePaddle);
        }

        // draw the ball in it's current location
        protected void DrawBall(SpriteBatch batch)
        {
            DrawSprite(batch, m_spriteBall);
        }

        // draw important heads-up-display data
        protected void DrawHUD(SpriteBatch batch, int x, int y, Color tint)
        {
            int locX = x;
            int locY = y;
            int maxWidth = 0;
            Vector2 size = Vector2.Zero;

            // draw labels in first column
            string[] data = { "Level:", "Lives:", "Score:", "HiScore:" };
            foreach (string text in data)
            {
                size = m_GameFont.DrawString(batch, text, locX, locY, tint);
                locY += m_GameFont.FontHeight;
                maxWidth = Math.Max((int)(size.X + 0.5f), maxWidth);
            }

            // set location to start of second column
            locX += maxWidth + 7;
            locY = y;

            // draw data in second column
            data = new string[] { LevelString, LivesString, ScoreString, HiScoreString };
            foreach (string text in data)
            {
                m_GameFont.DrawString(batch, text, locX, locY, tint);
                locY += m_GameFont.FontHeight;
            }
        }
    }
}
