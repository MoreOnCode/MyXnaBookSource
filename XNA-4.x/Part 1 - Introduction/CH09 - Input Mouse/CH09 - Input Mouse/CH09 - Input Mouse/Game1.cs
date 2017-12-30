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

namespace CH09___Input_Mouse
{
    // simple struct to track the state of each block
    public struct BlockState
    {
        public bool IsSelected;
        public double EffectAge;
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // texture and texture rectangle data for this game
        private Texture2D Texture;
        private readonly Rectangle PixelRect = new Rectangle(0, 0, 1, 1);
        private readonly Rectangle BlockRect = new Rectangle(3, 0, 29, 29);
        private readonly Rectangle ArrowRect = new Rectangle(32, 0, 24, 24);

        // width and height of screen, in pixels
        const int SCREEN_WIDTH = 640;
        const int SCREEN_HEIGHT = 480;

        // width and height of grid, in cells
        private const int GRID_WIDTH = 20;
        private const int GRID_HEIGHT = 15;

        // width and height of cell, in pixels
        private const int CELL_WIDTH = SCREEN_WIDTH / GRID_WIDTH;
        private const int CELL_HEIGHT = SCREEN_HEIGHT / GRID_HEIGHT;

        // current location of the mouse
        private Vector2 CursorPosition = Vector2.Zero;

        // time to spend fading after mouse leaves cell
        private const double EFFECT_DURATION = 1.25;

        // active and idle colors of blocks
        private Color ColorActiveBlock = Color.White;
        private Color ColorIdleBlock = Color.CornflowerBlue;

        // array continaing the state of every block in the grid
        private BlockState[,] BlockStates = new BlockState[GRID_WIDTH, GRID_HEIGHT];

        // keep track of the last known state of the action button
        ButtonState LastMouseButtonState = ButtonState.Released;

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
            // run at full speed
            IsFixedTimeStep = false;

            // set screen size
            InitScreen();

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

            Texture = Content.Load<Texture2D>(@"media\game");
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

            // we'll work with the elapsed seconds since the last update
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;

            // get the mouse state, and record its current location
            MouseState mouse1 = Mouse.GetState();
            CursorPosition.X = mouse1.X;
            CursorPosition.Y = mouse1.Y;

            // which grid cell is this mouse over now?
            int gx = (int)Math.Floor(CursorPosition.X / CELL_WIDTH);
            int gy = (int)Math.Floor(CursorPosition.Y / CELL_HEIGHT);

            for (int y = 0; y < GRID_HEIGHT; y++) // for each row
            {
                for (int x = 0; x < GRID_WIDTH; x++) // for each column
                {
                    // if the current cell is under the mouse
                    if (gx == x && gy == y)
                    {
                        // start the fade effect
                        BlockStates[x, y].EffectAge = EFFECT_DURATION;

                        // if the player clicks, toggle the IsSelected flag
                        if (mouse1.LeftButton == ButtonState.Pressed)
                        {
                            // don't toggle more than once for a single click
                            if (mouse1.LeftButton != LastMouseButtonState)
                            {
                                BlockStates[x, y].IsSelected = !BlockStates[x, y].IsSelected;
                            }
                        }
                    }
                    else
                    {
                        // this cell isn't under the mouse, just update the fade state
                        BlockStates[x, y].EffectAge -= elapsed;
                        if (BlockStates[x, y].EffectAge < 0)
                        {
                            BlockStates[x, y].EffectAge = 0;
                        }
                    }
                }
            }

            // remember the last click state so we don't process it again
            LastMouseButtonState = mouse1.LeftButton;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw the game components
            spriteBatch.Begin();
            DrawGrid(spriteBatch);
            DrawBlocks(spriteBatch);
            DrawCursor(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // fill the screen with a grid of blue lines 
        private void DrawGrid(SpriteBatch batch)
        {
            // horizontal and vertical line rectangles
            Rectangle horizontal = new Rectangle(0, 0, SCREEN_WIDTH, 1);
            Rectangle vertical = new Rectangle(0, 0, 1, SCREEN_HEIGHT);

            // draw horizontal lines
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                horizontal.Y = y * 32;
                batch.Draw(Texture, horizontal, PixelRect, Color.CornflowerBlue);
            }

            // draw vertical lines
            for (int x = 0; x < GRID_WIDTH; x++)
            {
                vertical.X = x * 32;
                batch.Draw(Texture, vertical, PixelRect, Color.CornflowerBlue);
            }
        }

        // draw the mouse cursor at its current location
        private void DrawCursor(SpriteBatch batch)
        {
            batch.Draw(Texture, CursorPosition, ArrowRect, Color.White);
        }

        // render all of the blocks
        private void DrawBlocks(SpriteBatch batch)
        {
            // create temp variables once per call rather than once per use
            Rectangle pos = BlockRect;
            Vector3 vColor = Vector3.One;
            Vector3 vColorA = Vector3.One;
            Vector3 vColorI = Vector3.One;

            for (int y = 0; y < GRID_HEIGHT; y++) // for each row
            {
                // row as pixel
                pos.Y = y * 32 + 2;
                for (int x = 0; x < GRID_WIDTH; x++) // for each column
                {
                    // column as pixel
                    pos.X = x * 32 + 2;

                    // state of the current block
                    BlockState state = BlockStates[x, y];

                    // draw the block
                    if (state.IsSelected)
                    {
                        // is selected, draw as fully-highlighted
                        batch.Draw(Texture, pos, BlockRect, ColorActiveBlock);
                    }
                    else if (state.EffectAge > 0)
                    {
                        // start with fully-idle and fully-highlighted colors
                        vColorA = ColorActiveBlock.ToVector3();
                        vColorI = ColorIdleBlock.ToVector3();

                        // perform linear interpolation (Lerp) between the two
                        vColor.X = MathHelper.Lerp(vColorI.X, vColorA.X,
                            (float)(state.EffectAge / EFFECT_DURATION));
                        vColor.Y = MathHelper.Lerp(vColorI.Y, vColorA.Y,
                            (float)(state.EffectAge / EFFECT_DURATION));
                        vColor.Z = MathHelper.Lerp(vColorI.Z, vColorA.Z,
                            (float)(state.EffectAge / EFFECT_DURATION));

                        // use the interpolated color
                        Color col = new Color(vColor);

                        // actually draw the block
                        batch.Draw(Texture, pos, BlockRect, col);
                    }
                    else
                    {
                        // block is idle, draw as fully-idle
                        batch.Draw(Texture, pos, BlockRect, ColorIdleBlock);
                    }
                }
            }
        }
    }
}
