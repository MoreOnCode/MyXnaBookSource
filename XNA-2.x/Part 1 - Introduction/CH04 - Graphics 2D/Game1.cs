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

namespace Chapter04
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
        protected Texture2D m_texAnim = null;
        protected Texture2D m_texTile = null;

        // simple texture "constants" for use in the example
        protected static readonly Rectangle m_rectSmiley =
            new Rectangle(0, 0, 64, 64);
        protected static readonly Vector2 m_originSmiley =
            new Vector2(32, 32);

        // constants to ignore draw options that we're not interested in
        protected const float NO_ROTATE = 0.0f;
        protected const float NO_SCALE = 1.0f;
        protected const float NO_LAYER = 0.5f;

        // used for game state calculations
        protected double m_TotalSeconds = 0.0;
        protected double m_ElapsedSeconds = 0.0;

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
            // TODO: Add your initialization logic here

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

            // load our textures
            m_texSmiley = Content.Load<Texture2D>(@"media\smiley");
            m_texAnim = Content.Load<Texture2D>(@"media\smileyAnim");
            m_texTile = Content.Load<Texture2D>(@"media\smileyTile");
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

            // track elapsed time since last frame, and since the game started
            m_ElapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            m_TotalSeconds += m_ElapsedSeconds;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw typical sprites
            spriteBatch.Begin();
            DrawBackgroundTiles(spriteBatch);
            DrawTintedSmiley(spriteBatch);
            DrawFadedSmiley(spriteBatch);
            DrawAnimatedSmiley(spriteBatch);
            spriteBatch.End();

            // draw sprites that require special batch settings
            spriteBatch.Begin(
                SpriteBlendMode.AlphaBlend,
                SpriteSortMode.FrontToBack,
                SaveStateMode.None);
            DrawOrbitingSmileys(spriteBatch);
            DrawScalingSmiley(spriteBatch);
            DrawRotatingSmiley(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
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

            // calculate the tile offsets
            m_BkgrTileOffset.X += -50.0f * (float)m_ElapsedSeconds;
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
        protected const int ORBIT_COUNT = 8;

        // draw several smileys orbiting around an invisible point
        protected void DrawOrbitingSmileys(SpriteBatch batch)
        {
            // calculate the angle for the first sprite and
            double degrees = m_TotalSeconds * 33;
            double radians = MathHelper.ToRadians((float)degrees);

            // calculate the angle between each sprite (the delta)
            double delta = 2.0 * Math.PI / ORBIT_COUNT;

            // variable to hold the location of each sprite 
            Vector2 loc = Vector2.Zero;

            // for each smiley that's in orbit
            for (int i = 0; i < ORBIT_COUNT; i++)
            {
                // determine trig values for sprite to calculate 
                // each sprite's position on the screen
                float cos = (float)Math.Cos(radians + delta * i);
                float sin = (float)Math.Sin(radians + delta * i);

                // calculate the position of each sprite
                loc.X = 200.0f + 90.0f * cos; // [110 to 290]
                loc.Y = 140.0f + 20.0f * sin; // [120 to 160]

                // calculate sprite depth as function of its 
                // Y location so that higher smileys are drawn 
                // behind lower smileys. sine generates a value 
                // between -1 and 1, clamp value between 0 and 1
                float layer = (sin + 1.0f) / 2.0f;

                // draw our sprite at the calculated location
                batch.Draw(
                    m_texSmiley,        // the sprite texture
                    loc,                // location to draw the smiley
                    m_rectSmiley,       // bounds of our sprite within texture
                    Color.White,        // no tint
                    NO_ROTATE,          // no rotation (zero radians)
                    m_originSmiley,     // the center of the texture
                    NO_SCALE,           // no scale (1x times original)
                    SpriteEffects.None, // draw sprite normally
                    layer);             // our calculated layer
            }
        }

        // scale the smiley sprite from 50% to 150% of its original size
        protected void DrawScalingSmiley(SpriteBatch batch)
        {
            // use simple sine function to animate between extreems of scale
            double degrees = m_TotalSeconds * 66;
            double radians = MathHelper.ToRadians((float)degrees);
            float scale = (float)Math.Sin(radians);

            // sine generates a value between -1 and 1, 
            // clamp value between 0.5 and 1.5
            scale = (scale + 1.0f) / 2.0f + 0.5f;

            // draw our sprite at the specified location
            Vector2 loc = new Vector2(200, 250);
            batch.Draw(
                m_texSmiley,        // the sprite texture
                loc,                // location to draw the smiley
                m_rectSmiley,       // bounds of our sprite within texture
                Color.White,        // no tint
                NO_ROTATE,          // no rotation (zero radians)
                m_originSmiley,     // the center of the texture
                scale,              // our calculated scale factor
                SpriteEffects.None, // draw sprite normally
                NO_LAYER);          // constant layer depth
        }

        // rotate smiley at a constant rate
        protected void DrawRotatingSmiley(SpriteBatch batch)
        {
            // increment angle of rotation
            double degrees = m_TotalSeconds * 66;
            double radians = MathHelper.ToRadians((float)degrees);
            float rotate = (float)radians;

            // draw our sprite at the specified location
            Vector2 loc = new Vector2(200, 360);
            batch.Draw(
                m_texSmiley,        // the sprite texture
                loc,                // location to draw the smiley
                m_rectSmiley,       // bounds of our sprite within texture
                Color.White,        // no tint
                rotate,             // our calculated rotation angle
                m_originSmiley,     // the center of the texture
                NO_SCALE,           // no scale (1x times original)
                SpriteEffects.None, // draw sprite normally
                NO_LAYER);          // constant layer depth
        }

        // pulse smiley between red and no tint
        protected void DrawTintedSmiley(SpriteBatch batch)
        {
            // use simple sine function to pulse our tint between red and white
            double degrees = m_TotalSeconds * 120;
            double radians = MathHelper.ToRadians((float)degrees);
            float bias = (float)Math.Sin(radians);

            // clamp the output of sine (-1 .. 1) to the (0 .. 1) range
            bias = (bias + 1.0f) / 2.0f;

            // get our new color
            Vector3 v3Color = new Vector3(1.0f, bias, bias);
            Color color = new Color(v3Color);

            // draw our sprite at the specified location
            Vector2 loc = new Vector2(440, 108);
            batch.Draw(m_texSmiley, loc, color);
        }

        // fade smiley in and out by varying the opacity of the sprite
        protected void DrawFadedSmiley(SpriteBatch batch)
        {
            // use sine function to pulse between transparent and opaque
            double degrees = m_TotalSeconds * 66;
            double radians = MathHelper.ToRadians((float)degrees);
            float opacity = (float)Math.Sin(radians);

            // clamp the output of sine (-1 .. 1) to the (0 .. 1) range
            opacity = (opacity + 1.0f) / 2.0f;

            // get our new color
            Vector4 v4Color = new Vector4(1.0f, 1.0f, 1.0f, opacity);
            Color color = new Color(v4Color);

            // draw our sprite at the specified location
            Vector2 loc = new Vector2(440, 218);
            batch.Draw(m_texSmiley, loc, color);
        }

        // current frame of animation
        protected int m_AnimFrameNum = 0;

        // delay between animation frames, and time since last frame change
        protected double m_AnimFrameElapsed = 0;
        protected const double m_AnimFrameDelay = 0.1;

        // draw animated sprite where individual animation
        // frames are contained in a single texture, side 
        // by side, and drawn to the screen in rapid succession
        protected void DrawAnimatedSmiley(SpriteBatch batch)
        {
            // increment elapsed frame time
            m_AnimFrameElapsed += m_ElapsedSeconds;

            // is it time to move on to the next frame?
            if (m_AnimFrameElapsed >= m_AnimFrameDelay)
            {
                // our frames are square, calculate the number of 
                // frames in the texture
                int frameCount = m_texAnim.Width / m_texAnim.Height;

                // add one, take modulus of the frame count to keep
                // the index within the valid range
                m_AnimFrameNum = (m_AnimFrameNum + 1) % frameCount;

                // reset the elapsed counter, start counting again
                m_AnimFrameElapsed = 0;
            }

            // select the current animation frame, based on our index
            Rectangle rect = new Rectangle(
                m_AnimFrameNum * m_texAnim.Height,
                0,
                m_texAnim.Height,
                m_texAnim.Height);

            // location is constant for this example
            Vector2 loc = new Vector2(440, 328);

            // draw the selected sprite
            batch.Draw(m_texAnim, loc, rect, Color.White);
        }
    }
}
