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

namespace Chapter05
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // our game textures
        protected Texture2D m_texSmiley = null;
        protected Texture2D m_texTile = null;

        // simple texture "constants" for use in the example
        protected static readonly Rectangle m_rectSmiley =
            new Rectangle(0, 0, 64, 64);
        protected static readonly Vector2 m_originSmiley =
            new Vector2(32, 32);

        // constants to ignore draw options that we're not interested in
        protected const float NO_LAYER = 0.0001f;

        // used for game state calculations
        protected double m_TotalSeconds = 0.0;
        protected double m_ElapsedSeconds = 0.0;

        // used for paused state calculations
        protected double m_TotalSecondsPaused = 0.0;
        protected double m_ElapsedSecondsPaused = 0.0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //// use a fixed frame rate of 30 frames per second
            //IsFixedTimeStep = true;
            //TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 33);

            // run at full speed
            IsFixedTimeStep = false;

            // set screen size
            InitScreen();
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
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // utility class to generate random numbers
            Random rand = new Random();

            // toggle value used to alternate each smiley's rotation direction
            int toggle = 1;

            // for each element of the array, set the smiley's rotation rate
            for (int i = 0; i < m_rotateSmiley.Length; i++)
            {
                // random rotation rate between -50 and 50, excluding zero
                m_rotateSmiley[i] = rand.Next(51) + 1; // [1 to 50]
                m_rotateSmiley[i] *= toggle; // [-50 to -1, 1 to 50]

                // alternate rotation direction with each smiley
                toggle = -toggle;
            }

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

            //// load our textures
            m_texSmiley = Content.Load<Texture2D>(@"media\smiley");
            m_texTile = Content.Load<Texture2D>(@"media\smileyTile");

            // force CaptureGameScreen to recreate the texture
            m_texWorkingBuffer = null;
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
            // member variables to store current user input data
            m_PadState = GamePad.GetState(PlayerIndex.One);
            m_KeyState = Keyboard.GetState();

            if (ExitGame)
            {
                // player wants to quit the game
                this.Exit();
            }

#if !XBOX360
            // toggle between full-screen and windowed mode when
            // the game is running under windows
            if (ToggleFullScreen)
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }
#endif

            if (!IsPaused)
            {
                // track elapsed time since last frame, 
                // and total time that game has been played
                m_ElapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;
                m_TotalSeconds += m_ElapsedSeconds;

                // update horizontal offset for background
                m_BkgrTileOffset.X += -50.0f * (float)m_ElapsedSeconds;
            }
            else
            {
                // track elapsed time since last frame, 
                // and total time that game has been paused
                m_ElapsedSecondsPaused = gameTime.ElapsedGameTime.TotalSeconds;
                m_TotalSecondsPaused += m_ElapsedSecondsPaused;
            }

            base.Update(gameTime);
        }

        // player input data, Xbox 360 controller
        protected GamePadState m_PadState;
        public GamePadState PadState { get { return m_PadState; } }

        // player input data, keyboard
        protected KeyboardState m_KeyState;
        public KeyboardState KeyState { get { return m_KeyState; } }

        // exit the game?
        public bool ExitGame
        {
            get
            {
                return
                    PadState.Buttons.Back == ButtonState.Pressed ||
                    KeyState.IsKeyDown(Keys.Escape);
            }
        }

        // player requested full screen mode?
        public bool ToggleFullScreen
        {
            get
            {
                // is the toggle button pressed?
                bool pressed =
                    PadState.Triggers.Left > 0 ||
                    KeyState.IsKeyDown(Keys.Z);

                // should we toggle the screen mode?
                bool toggle =
                    (pressed && !graphics.IsFullScreen) ||
                    (!pressed && graphics.IsFullScreen);

                // account for fringe case where change in screen mode 
                // isn't immediately reflected in the IsFullScreen property
                toggle = toggle && (m_TriggerPressedPrev != pressed);
                m_TriggerPressedPrev = pressed;

                // report our findings to the caller
                return toggle;
            }
        }
        protected bool m_TriggerPressedPrev = false;

        // should we show the grid?
        public bool ShowGrid
        {
            get
            {
                return
                    PadState.Buttons.B == ButtonState.Pressed ||
                    KeyState.IsKeyDown(Keys.B);
            }
        }

        // use our pixel-by-pixel code
        public bool CustomWarp
        {
            get
            {
                return
                    PadState.Buttons.A == ButtonState.Pressed ||
                    KeyState.IsKeyDown(Keys.A) ||
                    KeyState.IsKeyDown(Keys.Space);
            }
        }

        // use standard XNA code
        public bool StandardWarp
        {
            get
            {
                return
                    PadState.Buttons.X == ButtonState.Pressed ||
                    KeyState.IsKeyDown(Keys.X);
            }
        }

        // was the game paused during the last frame?
        protected bool m_WasPaused = false;

        // were we drawing a custom warp on the last frame?
        protected bool m_WasCustomWarp = false;

        // is the game paused now?
        public bool IsPaused
        {
            get { return CustomWarp || StandardWarp; }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // the game wasn't paused last time we rendered the screen
            if (!m_WasPaused)
            {
                // draw typical sprites
                spriteBatch.Begin();
                DrawBackgroundTiles(spriteBatch);
                spriteBatch.End();

                // draw sprites that require special batch settings
                spriteBatch.Begin(
                    SpriteBlendMode.AlphaBlend,
                    SpriteSortMode.FrontToBack,
                    SaveStateMode.None);
                DrawOrbitingSmileys(spriteBatch);
                spriteBatch.End();
            }

            // is the game paused now?
            if (IsPaused)
            {
                // capture the back buffer
                CaptureGameScreen();

                // calculate the locations of our cells in the warp grid
                CalculateGridWarp();

                // get ready to draw warped image
                spriteBatch.Begin();

                if (CustomWarp)
                {
                    // draw screen pixel-by-pixel
                    DrawPausedScreenCustom(spriteBatch);
                }
                else
                {
                    // draw screen using standard XNA methods
                    DrawPausedScreenStandard(spriteBatch);
                }

                // we're done drawing, present it to the screen
                spriteBatch.End();
            }

            // let the next frame know this frame's paused state
            m_WasPaused = IsPaused;

            // let the next frame know this frame's custom warp state
            m_WasCustomWarp = CustomWarp;

            base.Draw(gameTime);
        }

        // number of rows and columns in our warp grid
        protected const int GRID_ROWS = 5;
        protected const int GRID_COLS = 7;

        // size of an unscaled cell in our warp grid
        protected const int ROW_SIZE = SCREEN_HEIGHT / GRID_ROWS;
        protected const int COL_SIZE = SCREEN_WIDTH / GRID_COLS;

        // use standard XNA methods to draw the warped image
        protected void DrawPausedScreenStandard(SpriteBatch batch)
        {
            if (m_WasCustomWarp)
            {
                // the player jumpped directly from custom to standard warp
                // the working buffer isn't filled with its original data
                // restamp the working buffer with the data from the screen
                GraphicsDevice.Textures[0] = null;
                m_texWorkingBuffer.SetData<Color>(m_colorDataBuffer);
            }

            // source and destination rectangles for our texture data
            Rectangle rectSrc = Rectangle.Empty;
            Rectangle rectDst = Rectangle.Empty;

            // for each row in the grid
            for (int row = 0; row < GRID_ROWS; row++)
            {
                // top and bottom location of the cell
                int y1 = m_GridWarpY[row + 0];
                int y2 = m_GridWarpY[row + 1];

                // for each column in the grid
                for (int col = 0; col < GRID_COLS; col++)
                {
                    // left and right location of the cell
                    int x1 = m_GridWarpX[col + 0];
                    int x2 = m_GridWarpX[col + 1];

                    // destination rectangle, warped screen coordinates
                    rectDst.X = x1;
                    rectDst.Y = y1;
                    rectDst.Width = x2 - x1;
                    rectDst.Height = y2 - y1;

                    // source rectangle, unwarped texture coordinates
                    rectSrc.X = col * COL_SIZE;
                    rectSrc.Y = row * ROW_SIZE;
                    rectSrc.Width = COL_SIZE;
                    rectSrc.Height = ROW_SIZE;

                    // use standard batch draw method
                    batch.Draw(
                        m_texWorkingBuffer,
                        rectDst,
                        rectSrc,
                        Color.White);
                }
            }

            // should we show the grid?
            if (ShowGrid)
            {
                // extract single pixel from texture to draw lines
                rectSrc.X = 1;
                rectSrc.Y = 1;
                rectSrc.Width = 1;
                rectSrc.Height = 1;

                // prepare for drawing horizontal lines
                rectDst.X = 0;
                rectDst.Width = SCREEN_WIDTH;
                rectDst.Height = 1;

                // draw a line between each row of cells
                for (int row = 1; row < GRID_ROWS; row++)
                {
                    rectDst.Y = m_GridWarpY[row];
                    batch.Draw(
                        m_texWorkingBuffer,
                        rectDst,
                        rectSrc,
                        Color.Green);
                }

                // prepare for drawing vertical lines
                rectDst.Y = 0;
                rectDst.Width = 1;
                rectDst.Height = SCREEN_HEIGHT;

                // draw a line between each column of cells
                for (int col = 1; col < GRID_COLS; col++)
                {
                    rectDst.X = m_GridWarpX[col];
                    batch.Draw(m_texWorkingBuffer, rectDst, rectSrc, Color.Green);
                }
            }
        }

        // draw the warped image, pixel-by-pixel
        protected void DrawPausedScreenCustom(SpriteBatch batch)
        {
            // for each row in the grid
            for (int row = 0; row < GRID_ROWS; row++)
            {
                // top and bottom location of the cell
                int y1 = m_GridWarpY[row + 0];
                int y2 = m_GridWarpY[row + 1];

                // sample distance between pixels in source image
                float dy = (float)ROW_SIZE / (float)(y2 - y1);

                // for each column in the grid
                for (int col = 0; col < GRID_COLS; col++)
                {
                    // left and right location of the cell
                    int x1 = m_GridWarpX[col + 0];
                    int x2 = m_GridWarpX[col + 1];

                    // sample distance between pixels in source image
                    float dx = (float)COL_SIZE / (float)(x2 - x1);

                    // set srcY to the top of the source cell
                    float srcY = row * ROW_SIZE;

                    // for each row of pixels in the new cell
                    for (int y = y1; y < y2; y++)
                    {
                        // set srcX to the left of the source cell
                        float srcX = col * COL_SIZE;

                        // for each column of pixels in the new cell
                        for (int x = x1; x < x2; x++)
                        {
                            // set pixel in our working buffer
                            m_colorDataWorking[x + y * SCREEN_WIDTH] =
                                m_colorDataBuffer[
                                    (int)Math.Round(srcX) +
                                    (int)Math.Round(srcY) * SCREEN_WIDTH];

                            // increment srcX, checking image bounds
                            srcX = Math.Min(srcX + dx, SCREEN_WIDTH - 1);
                        }
                        // increment srcY, checking image bounds
                        srcY = Math.Min(srcY + dy, SCREEN_HEIGHT - 1);
                    }
                }
            }

            // should we show the grid?
            if (ShowGrid)
            {
                // draw a line between each row of cells
                for (int row = 1; row < GRID_ROWS; row++)
                {
                    int index = m_GridWarpY[row] * SCREEN_WIDTH;
                    for (int x = 0; x < SCREEN_WIDTH; x++)
                    {
                        m_colorDataWorking[index + x] = Color.Red;
                    }
                }

                // temp variable to save some typing
                int lenData = m_colorDataWorking.Length;

                // draw a line between each column of cells
                for (int col = 1; col < GRID_COLS; col++)
                {
                    int index = m_GridWarpX[col];
                    for (int y = 0; y < lenData; y += SCREEN_WIDTH)
                    {
                        m_colorDataWorking[index + y] = Color.Red;
                    }
                }
            }

            // store our generated pixel data in our working texture
            GraphicsDevice.Textures[0] = null;
            m_texWorkingBuffer.SetData<Color>(m_colorDataWorking);

            // bounds for the entire image
            Rectangle rectSrc =
                new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);

            // use standard batch draw method to draw our generated texure
            batch.Draw(m_texWorkingBuffer, Vector2.Zero, rectSrc, Color.White);
        }

        // pixel data for the game's last rendered frame
        protected Color[] m_colorDataBuffer =
            new Color[SCREEN_WIDTH * SCREEN_HEIGHT];

        // array to store our pixel data for our generated images
        protected Color[] m_colorDataWorking =
            new Color[SCREEN_WIDTH * SCREEN_HEIGHT];

        // texure to stuff our pixel data in before drawing to screen
        protected Texture2D m_texWorkingBuffer = null;

        // grab the latest pixels from the game screen's back buffer
        // and convert the data to black-and-white
        protected void CaptureGameScreen()
        {
            if (m_texWorkingBuffer == null)
            {
                // we need to manage this texture ourselves, whenever
                // resources are reloaded, we set the member variable
                // to null, then recreate it here, just before use
                m_texWorkingBuffer = new Texture2D(
                    GraphicsDevice,
                    SCREEN_WIDTH, SCREEN_HEIGHT,
                    1, 
                    TextureUsage.AutoGenerateMipMap,
                    SurfaceFormat.Color );
            }

            // we know the game is paused now, but was it 
            // paused the last time we checked?
            if (!m_WasPaused)
            {
                // create a texture to capture the back buffer
                ResolveTexture2D tex = new ResolveTexture2D(
                    GraphicsDevice, 
                    SCREEN_WIDTH, 
                    SCREEN_HEIGHT, 
                    1, 
                    SurfaceFormat.Color);

                // grab the image, extract the pixels, then free the memory
                GraphicsDevice.ResolveBackBuffer(tex);

                tex.GetData<Color>(m_colorDataBuffer);
                tex.Dispose();

                // convert our pixel data to gray scale
                for (int i = 0; i < m_colorDataBuffer.Length; i++)
                {
                    Color color = m_colorDataBuffer[i]; // current pixel

                    // calculate the luminance (3rd component of HSL color)
                    int min = Math.Min(Math.Min(color.R, color.G), color.B);
                    int max = Math.Max(Math.Max(color.R, color.G), color.B);
                    int lum = (min + max) / 2;

                    // convert to grayscale in RGB color space
                    m_colorDataBuffer[i] =
                        new Color((byte)lum, (byte)lum, (byte)lum);
                }

                // store our grayscale pixel data in our working texture
                GraphicsDevice.Textures[0] = null;
                m_texWorkingBuffer.SetData<Color>(m_colorDataBuffer);
            }
        }

        // X and Y locations of our warped grid columns and rows
        protected int[] m_GridWarpX = new int[GRID_COLS + 1];
        protected int[] m_GridWarpY = new int[GRID_ROWS + 1];

        // calculate the location and size of cells in our warp grid
        protected void CalculateGridWarp()
        {
            // first row and column are fixed at image border
            m_GridWarpX[0] = 0;
            m_GridWarpY[0] = 0;

            // last row and column are fixed at image border
            m_GridWarpX[GRID_COLS] = SCREEN_WIDTH;
            m_GridWarpY[GRID_ROWS] = SCREEN_HEIGHT;

            // calculate the initial rotation angle (66 degrees per second)
            float degrees = 66 * (float)m_TotalSecondsPaused;

            // temp variables used to store intermediate calculations
            double radians;
            float val;

            // vary cell sizes by offsetting the rotation angle
            float dAngleRow = 360.0f / GRID_ROWS;
            float dAngleCol = 360.0f / GRID_COLS;

            // calculate row locations
            for (int row = 1; row < GRID_ROWS; row++)
            {
                radians = MathHelper.ToRadians(degrees + dAngleRow * row);
                val = (float)Math.Sin(radians);
                m_GridWarpY[row] =
                    (int)Math.Round(row * ROW_SIZE + val * ROW_SIZE / 2);
            }

            // calculate column locations
            for (int col = 1; col < GRID_COLS; col++)
            {
                radians = MathHelper.ToRadians(degrees + dAngleCol * col);
                val = (float)Math.Sin(radians);
                m_GridWarpX[col] =
                    (int)Math.Round(col * COL_SIZE + val * COL_SIZE / 2);
            }
        }

        // offset from top, left of screen to start drawing our tiled sprites
        protected Vector2 m_BkgrTileOffset = Vector2.Zero;

        // number of degrees to increment with each passing second
        protected const float BkgrDegreesPerSec = 33.0f;

        // draw the background for our example
        protected void DrawBackgroundTiles(SpriteBatch batch)
        {
            // calculate angle and convert to radians
            double degrees = BkgrDegreesPerSec * m_TotalSeconds;
            double radians = MathHelper.ToRadians((float)degrees);

            // calculate the tile Y offset
            m_BkgrTileOffset.Y = (float)(200 * Math.Sin(radians));

            // make sure that the first tile isn't too far left
            while (m_BkgrTileOffset.X < -m_texTile.Width)
            {
                m_BkgrTileOffset.X += m_texTile.Width;
            }

            // make sure that the first tile isn't too far right
            while (m_BkgrTileOffset.X > 0)
            {
                m_BkgrTileOffset.X -= m_texTile.Width;
            }

            // make sure that the first tile isn't too far up
            while (m_BkgrTileOffset.Y < -m_texTile.Height)
            {
                m_BkgrTileOffset.Y += m_texTile.Height;
            }

            // make sure that the first tile isn't too far down
            while (m_BkgrTileOffset.Y > 0)
            {
                m_BkgrTileOffset.Y -= m_texTile.Height;
            }

            // init current tile location with location of the top, left tile
            Vector2 loc = m_BkgrTileOffset;

            // draw our background tile in a grid that fills the screen
            for (int y = 0; y <= SCREEN_HEIGHT / m_texTile.Height + 1; y++)
            {
                loc.Y = m_BkgrTileOffset.Y + y * m_texTile.Height;
                for (int x = 0; x <= SCREEN_WIDTH / m_texTile.Width + 1; x++)
                {
                    loc.X = m_BkgrTileOffset.X + x * m_texTile.Width;
                    batch.Draw(m_texTile, loc, Color.White);
                }
            }
        }

        // how many smileys are orbiting?
        protected const int ORBIT_COUNT = 11;

        // what is the rotation rate of each smiley?
        protected int[] m_rotateSmiley = new int[ORBIT_COUNT];

        // draw several smileys orbiting around an invisible point
        protected void DrawOrbitingSmileys(SpriteBatch batch)
        {
            // calculate the angle for the first sprite
            double degrees = m_TotalSeconds * 33;
            double radians = MathHelper.ToRadians((float)degrees);

            // calculate the angle between each sprite (the delta)
            double delta = 2.0 * Math.PI / ORBIT_COUNT;

            // variable to hold the location of each sprite 
            Vector2 loc = Vector2.Zero;

            // calculate a virtual tilt for the orbit plane
            // to make the smileys look like they're on a 
            // tilt-a-whirl at the state fair
            double degTilt = m_TotalSeconds * 33;
            double radTilt = MathHelper.ToRadians((float)degTilt);
            float sinTilt = (float)Math.Sin(radTilt) * 2.0f;

            // for each smiley that's in orbit
            for (int i = 0; i < ORBIT_COUNT; i++)
            {
                // determine trig values for sprite to calculate 
                // each sprite's position on the screen
                float cos = (float)Math.Cos(radians + delta * i);
                float sin = (float)Math.Sin(radians + delta * i);

                // position at the center of the screen
                loc.X = SCREEN_WIDTH / 2;
                loc.Y = SCREEN_HEIGHT / 2;

                // offset, based on orbit progress
                loc.X += 200.0f * cos;           // [120 to 520]
                loc.Y += 20.0f * sin * sinTilt; // [200 to 280]

                // calculate sprite depth as function of its 
                // Y location so that higher smileys are drawn 
                // behind lower smileys. sine generates a value 
                // between -1 and 1, clamp value between 0 and 1
                float layer = (sin + 1.0f) / 2.0f;

                // avoid conflict with background (at layer = 0)
                // by choosing a value just above zero
                if (layer == 0) { layer = NO_LAYER; }

                // scale the closest smiley to 150% and the 
                // furthest smiley by 100% to give the illusion
                // of depth as each smiley completes his orbit
                float scale = 1.0f + 0.50f * layer;

                // calculate the rotation angle for this smiley
                float rotate = m_rotateSmiley[i] * (float)m_TotalSeconds;
                rotate = MathHelper.ToRadians(rotate);

                // draw our sprite at the calculated location
                batch.Draw(
                    m_texSmiley,        // the sprite texture
                    loc,                // location to draw the smiley
                    m_rectSmiley,       // bounds of our sprite within texture
                    Color.White,        // no tint
                    rotate,             // rotate each smiley at a random rate
                    m_originSmiley,     // the center of the texture
                    scale,              // scale between 1x and 2x normal size
                    SpriteEffects.None, // draw sprite normally
                    layer);             // our calculated layer
            }
        }
    }
}
