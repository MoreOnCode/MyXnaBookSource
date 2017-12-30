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

namespace CH08___Input_Keyboard
{
    public struct Bullet
    {
        // active = on-screen, not active = available to reuse
        public bool Active;
        // current location on the screen
        public Vector2 Location;
        // delta X and delta Y for this bullet
        public Vector2 Motion;
        // current image rotation angle (spinning bullets)
        public float Angle;
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // screen constants
        const int SCREEN_WIDTH = 640;
        const int SCREEN_HEIGHT = 480;

        // movement and rotation constants
        const float DELTA_ROTATE = 12.0f;
        const float DELTA_MOVE = 2.0f;

        // handy constant for sin and cos
        const double ToRadians = Math.PI / 180.0;

        // the only game texture
        Texture2D m_texGame;

        // individual game images
        Rectangle m_rectShip = new Rectangle(1, 1, 62, 62);
        Rectangle m_rectBullet = new Rectangle(65, 1, 62, 62);
        Rectangle m_rectGraph = new Rectangle(0, 64, 50, 50);

        // max num active bullets
        const int BULLET_MAX = 75;

        // frames to wait between shots
        const int BULLET_DELAY = 9;

        // number of frames until next shot can be fired
        int m_nBulletCountDown = 0;

        // array to track bullets
        Bullet[] m_Bullets = new Bullet[BULLET_MAX];

        // top, left of first graph tile
        Vector2 m_GraphOrigin = Vector2.Zero;

        // center of the ship image
        Vector2 m_CenterShip = new Vector2(31, 31);

        // rotation of the ship
        float m_angle = 0.0f;

        // center of the bullet image
        Vector2 m_CenterBullet = new Vector2(31, 31);

        // center of the screen
        Vector2 m_CenterScreen = new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);

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
            // use a fixed frame rate of 30 frames per second
            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 33);

            // init back buffer
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();

            // init bullet array
            for (int i = 0; i < m_Bullets.Length; i++)
            {
                m_Bullets[i].Active = false;
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

            m_texGame = Content.Load<Texture2D>(@"media\game");
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
            // get keyboard state once per frame
            KeyboardState keyState = Keyboard.GetState();

            // exit the game?
            if (keyState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // turn the ship?
            if (keyState.IsKeyDown(Keys.Left))
            {
                TurnShip(-DELTA_ROTATE);
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                TurnShip(DELTA_ROTATE);
            }

            // move the ship?
            float fMove = 0.0f;
            if (keyState.IsKeyDown(Keys.Down))
            {
                fMove = DELTA_MOVE;
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                fMove = -DELTA_MOVE;
            }
            MoveShip(fMove);

            // fire delay
            if (m_nBulletCountDown > 0)
            {
                m_nBulletCountDown--;
            }

            // fire bullet?
            if (keyState.IsKeyDown(Keys.Space))
            {
                Shoot();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // start drawing our images
            spriteBatch.Begin();

            // draw game objects
            DrawBackground(spriteBatch);
            DrawBullets(spriteBatch);
            DrawShip(spriteBatch);

            // let batch know that we're done
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // fill screen with graph tile, offset by ship movement
        private void DrawBackground(SpriteBatch batch)
        {
            // round to nearest whole pixel
            float ox = (float)Math.Round(m_GraphOrigin.X);
            float oy = (float)Math.Round(m_GraphOrigin.Y);

            // temp variable used by batch.Draw()
            Vector2 v = Vector2.Zero;
            for (float y = oy; y < SCREEN_HEIGHT; y += 50)
            {
                // row by row
                v.Y = y;
                for (float x = ox; x < SCREEN_WIDTH; x += 50)
                {
                    // column by column
                    v.X = x;
                    batch.Draw(m_texGame, v, m_rectGraph, Color.White);
                }
            }
        }

        // render each active bullet
        private void DrawBullets(SpriteBatch batch)
        {
            // scan entire list of bullets
            foreach (Bullet b in m_Bullets)
            {
                // only draw active bullets
                if (b.Active)
                {
                    double angle = b.Angle * ToRadians;
                    batch.Draw(
                        m_texGame,          // bullet texture
                        b.Location,         // bullet x, y
                        m_rectBullet,       // bullet source rect
                        Color.White,        // no tint
                        (float)(angle),     // bullet rotation
                        m_CenterBullet,     // center of bullet
                        1.0f,               // don't scale
                        SpriteEffects.None, // no effect
                        0.0f);              // topmost layer
                }
            }
        }

        // render the ship, rotated
        private void DrawShip(SpriteBatch batch)
        {
            batch.Draw(
                m_texGame,                    // ship texture
                m_CenterScreen,               // ship x, y
                m_rectShip,                   // ship source rect
                Color.White,                  // no tint
                (float)(m_angle * ToRadians), // ship rotation
                m_CenterShip,                 // center of ship
                1.0f,                         // don't scale
                SpriteEffects.None,           // no effect
                0.0f);                        // topmost layer
        }

        // rotate the ship
        private void TurnShip(float delta)
        {
            m_angle += delta;
        }

        // move the ship (actually, move the world while 
        // the ship stands still)
        private void MoveShip(float delta)
        {
            // distance to travel per frame, split into X and Y
            float dx = delta * (float)Math.Cos(m_angle * ToRadians);
            float dy = delta * (float)Math.Sin(m_angle * ToRadians);

            // update graph
            m_GraphOrigin.X += dx;
            m_GraphOrigin.Y += dy;

            // make sure x is between -50 and 0
            while (m_GraphOrigin.X < -50)
            {
                m_GraphOrigin.X += 50.0f;
            }
            while (m_GraphOrigin.X > 0)
            {
                m_GraphOrigin.X -= 50.0f;
            }

            // make sure y is between -50 and 0
            while (m_GraphOrigin.Y < -50)
            {
                m_GraphOrigin.Y += 50.0f;
            }
            while (m_GraphOrigin.Y > 0)
            {
                m_GraphOrigin.Y -= 50.0f;
            }

            UpdateBullets(dx, dy);
        }

        // update location of any active bullets
        private void UpdateBullets(float dxShip, float dyShip)
        {
            // for every bullet ...
            for (int i = 0; i < m_Bullets.Length; i++)
            {
                // only update active bullets
                if (m_Bullets[i].Active)
                {
                    // see if the bullet has left the screen
                    bool bDead = false;
                    bDead = m_Bullets[i].Location.X < -64;
                    bDead |= m_Bullets[i].Location.X > SCREEN_WIDTH + 64;
                    bDead |= m_Bullets[i].Location.Y < -64;
                    bDead |= m_Bullets[i].Location.Y > SCREEN_HEIGHT + 64;

                    if (bDead)
                    {
                        // bullet is off-screen, mark for reuse
                        m_Bullets[i].Active = false;
                    }
                    else
                    {
                        // move the bullet. since the ship is 
                        // stationary and the background moves, 
                        // be sure to account to recent ship 
                        // movements
                        m_Bullets[i].Location.X += dxShip +
                            m_Bullets[i].Motion.X;
                        m_Bullets[i].Location.Y += dyShip +
                            m_Bullets[i].Motion.Y;
                        // rotate the bullet
                        m_Bullets[i].Angle += 20.0f;
                    }
                }
            }
        }

        // try to create a new on-screen bullet
        private void Shoot()
        {
            // make sure we don't inject too many bullets at once
            if (m_nBulletCountDown == 0)
            {
                // scan list of bullets for an available slot
                for (int i = 0; i < m_Bullets.Length; i++)
                {
                    // can't use a bullet that's already on-screen
                    if (!m_Bullets[i].Active)
                    {
                        // start at the center of the screen
                        m_Bullets[i].Location = m_CenterScreen;

                        // calc distance to travel per frame, 
                        // split into X and Y
                        float dx = 3.0f *
                            (float)Math.Cos(m_angle * ToRadians);
                        float dy = 3.0f *
                            (float)Math.Sin(m_angle * ToRadians);

                        // set bullet into motion
                        m_Bullets[i].Motion = new Vector2(dx, dy);

                        // init rotation
                        m_Bullets[i].Angle = 0.0f;

                        // mark as active so we stop ignoring it
                        m_Bullets[i].Active = true;

                        // reset delay counter
                        m_nBulletCountDown = BULLET_DELAY;

                        // we're done, so exit the loop
                        break;
                    }
                }
            }
        }
    }
}
