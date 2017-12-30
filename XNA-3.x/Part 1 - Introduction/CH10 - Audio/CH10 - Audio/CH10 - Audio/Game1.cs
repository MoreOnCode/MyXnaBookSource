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

namespace CH10___Audio
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // our sound objects
        AudioEngine m_audio;
        WaveBank m_wave;
        SoundBank m_sound;

        // sound categories
        AudioCategory m_catMusic;
        AudioCategory m_catDefault;

        // keep a reference to Zoe's story so we can pause and resume it
        Cue m_story;

        // texture rects for volume HUD
        Rectangle m_rectVolumeMusic = new Rectangle(0, 0, 32, 256);
        Rectangle m_rectVolumeSfx = new Rectangle(32, 0, 32, 256);

        // on-screen size and locations for volume HUD elements
        Vector2 m_v2HudMusic = new Vector2(100, 112);
        Vector2 m_v2HudSfx = new Vector2(508, 112);

        // the one and only game texture
        Texture2D m_Texture = null;

        // number of discrete volume steps
        public const float VOLUME_STEP = 0.01f;

        // background music volume
        private float m_MusicVolume = 1.0f;
        public float MusicVolume
        {
            get { return m_MusicVolume; }
            set
            {
                m_MusicVolume = MathHelper.Clamp(value, 0.0f, 1.0f);
                m_catMusic.SetVolume(m_MusicVolume);
            }
        }

        // laser and story volume
        private float m_GameVolume = 1.0f;
        public float GameVolume
        {
            get { return m_GameVolume; }
            set
            {
                m_GameVolume = MathHelper.Clamp(value, 0.0f, 1.0f);
                m_catDefault.SetVolume(m_GameVolume);
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

            // initialize our sound objects
            m_audio = new AudioEngine(@"Content\example.xgs");
            m_wave = new WaveBank(m_audio, @"Content\example.xwb");
            m_sound = new SoundBank(m_audio, @"Content\example.xsb");

            // get a reference to our two sound categories
            m_catMusic = m_audio.GetCategory("Music");
            m_catDefault = m_audio.GetCategory("Default");

            // get a reference to Zoe's story
            m_story = m_sound.GetCue("zoe");

            // start playing our background music
            m_sound.PlayCue("ensalada");

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

            // load our game's only texture
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

            // allow the Audio APIs to do their magic
            m_audio.Update();

            // process player input
            GamePadState pad1 = GamePad.GetState(PlayerIndex.One);
            KeyboardState key1 = Keyboard.GetState();
            m_PressDelay += gameTime.ElapsedGameTime.TotalSeconds;
            ProcessInput(pad1, key1);

            base.Update(gameTime);
        }

        private const double PRESS_DELAY = 0.25;
        private double m_PressDelay = PRESS_DELAY;

        public void ProcessInput(GamePadState pad1, KeyboardState key1)
        {
            // process music volume changes immediately, 
            // don't worry about the PRESS_DELAY here.
            if (pad1.ThumbSticks.Left.Y < 0 ||
                key1.IsKeyDown(Keys.Down))
            {
                // decrease the volume of the music
                MusicVolume -= VOLUME_STEP;
            }
            else if (pad1.ThumbSticks.Left.Y > 0 ||
                key1.IsKeyDown(Keys.Up))
            {
                // increase the volume of the music
                MusicVolume += VOLUME_STEP;
            }

            // process game volume changes immediately, 
            // don't worry about the PRESS_DELAY here.
            if (pad1.ThumbSticks.Right.Y < 0 ||
                key1.IsKeyDown(Keys.PageDown))
            {
                // decrease the volume of the laser and story
                GameVolume -= VOLUME_STEP;
            }
            else if (pad1.ThumbSticks.Right.Y > 0 ||
                key1.IsKeyDown(Keys.PageUp))
            {
                // increase the volume of the laser and story
                GameVolume += VOLUME_STEP;
            }

            // enforce delay before laser sound and playing
            // or pausing Zoe's story. without the delay,
            // we might register 2 or more presses before the 
            // player can release the button.
            if (m_PressDelay >= PRESS_DELAY)
            {
                // assume that we will handle the button or key press
                bool PressWasHandled = true;

                if (pad1.Buttons.A == ButtonState.Pressed ||
                    key1.IsKeyDown(Keys.Space))
                {
                    // kick off a new instance of the laser cue
                    m_sound.PlayCue("laser");
                }
                else if (pad1.Buttons.B == ButtonState.Pressed ||
                    key1.IsKeyDown(Keys.Enter))
                {
                    // play or pause Zoe's story
                    if (m_story.IsPaused)
                    {
                        // resume a prepared and paused cue
                        m_story.Resume();
                    }
                    else if (m_story.IsPlaying)
                    {
                        // pause a prepared and playing cue
                        m_story.Pause();
                    }
                    else if (m_story.IsPrepared)
                    {
                        // play a prepared cue
                        m_story.Play();
                    }
                    else
                    {
                        // prepare and play a stopped cue
                        m_story = m_sound.GetCue("zoe");
                        m_story.Play();
                    }
                }
                else
                {
                    // we did not handle this button or key press
                    PressWasHandled = false;
                }

                if (PressWasHandled)
                {
                    // reset delay counter
                    m_PressDelay = 0;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (m_PressDelay < 0.125)
            {
                // when a new butotn or key press is registered, 
                // flash the screen
                graphics.GraphicsDevice.Clear(Color.Wheat);
            }
            else
            {
                // otherwise, show our familiar friend, CornflowerBlue
                graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            }

            spriteBatch.Begin();

            // draw white music volume meter
            Rectangle rect = m_rectVolumeMusic;
            Vector2 loc = m_v2HudMusic;
            spriteBatch.Draw(m_Texture, loc, rect, Color.White);

            // draw green music volume meter
            rect.Height = (int)Math.Round(rect.Height * MusicVolume);
            loc.Y += m_rectVolumeMusic.Height - rect.Height;
            rect.Y = m_rectVolumeMusic.Height - rect.Height;
            spriteBatch.Draw(m_Texture, loc, rect, Color.Lime);

            // draw white game volume meter
            rect = m_rectVolumeSfx;
            loc = m_v2HudSfx;
            spriteBatch.Draw(m_Texture, loc, rect, Color.White);

            // draw green game volume meter
            rect.Height = (int)Math.Round(rect.Height * GameVolume);
            loc.Y += m_rectVolumeSfx.Height - rect.Height;
            rect.Y += m_rectVolumeSfx.Height - rect.Height;
            spriteBatch.Draw(m_Texture, loc, rect, Color.Lime);

            // if Zoe's story is playing, render blue meter 
            if (m_story.IsPlaying && !m_story.IsStopped && !m_story.IsPaused)
            {
                spriteBatch.Draw(m_Texture, loc, rect, Color.RoyalBlue);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
