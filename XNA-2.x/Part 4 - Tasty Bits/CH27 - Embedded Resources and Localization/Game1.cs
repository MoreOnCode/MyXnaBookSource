using System;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Threading;
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
using Codetopia.Input;

namespace Chapter27
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BackgroundSprite m_background = new BackgroundSprite();
        AvatarSprite m_avatar = new AvatarSprite();
        DialogSprite m_dialog = new DialogSprite();
        SpriteBase m_flag = new SpriteBase();

        protected static readonly string[] m_cultures = {
            "en",
            "es",
            "de",
            "it",
            "ja-JP",
            "ru",
        };
        protected static int m_cultureIndex = 0;

        public static void NextCulture()
        {
            m_cultureIndex = (m_cultureIndex + 1) % m_cultures.Length;
        }

        protected bool m_WasNextCulture = false;
        public bool IsNextCulture
        {
            get
            {
                bool pressed = SpriteBase.GamePadState.Buttons.Y == ButtonState.Pressed;
                bool result = pressed && !m_WasNextCulture;
                m_WasNextCulture = pressed;
                return result;
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

            m_avatar.Dialog = m_dialog;
            m_dialog.Flag = m_flag;
            m_flag.Location = m_avatar.Location + new Vector2(137, 196);

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

            EmbeddedContent.ResourceHelper.Init(Services);
            ResourceContentManager embedded =
                EmbeddedContent.ResourceHelper.ContentManager;

            m_background.Texture = embedded.Load<Texture2D>("background");
            m_dialog.Texture = embedded.Load<Texture2D>("dialog");
            m_dialog.GameFont =
                GameFont.FromTexture2D(
                    embedded.Load<Texture2D>("gamefont"));

            m_avatar[AvatarStates.Smile].Texture =
                embedded.Load<Texture2D>("avatar_smile");
            m_avatar[AvatarStates.LookAhead].Texture =
                embedded.Load<Texture2D>("avatar_look_ahead");
            m_avatar[AvatarStates.LookLeft].Texture =
                embedded.Load<Texture2D>("avatar_look_left");
            m_avatar[AvatarStates.LookRight].Texture =
                embedded.Load<Texture2D>("avatar_look_right");
            m_avatar[AvatarStates.Blink].Texture =
                embedded.Load<Texture2D>("avatar_blink");

            UpdateCulture();
        }

        public void UpdateCulture()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture(m_cultures[m_cultureIndex]);
            LocalizedContent.ResourceHelper.Init(Services, culture);
            m_flag.Texture = Content.Load<Texture2D>(LocalizedContent.ResourceHelper.FlagPath);
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
            // Allows the default game to exit on Xbox 360 and Windows
            if (KAGamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed) { this.Exit(); }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            SpriteBase.GamePadState = KAGamePad.GetState(PlayerIndex.One);

            if (IsNextCulture)
            {
                NextCulture();
                UpdateCulture();
                m_dialog.Show(m_dialog.VisibleLineCount);
            }

            m_background.Update(elapsed);
            m_avatar.Update(elapsed);
            m_dialog.Update(elapsed);
            m_flag.Update(elapsed);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            m_background.Draw(spriteBatch);
            m_avatar.Draw(spriteBatch);
            m_dialog.Draw(spriteBatch);
            m_flag.Draw(spriteBatch, m_avatar.LocationDelta);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
