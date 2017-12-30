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

namespace CH06___Graphics_3D
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        //SpriteBatch spriteBatch;

        // map friendly names to array indices for our effects
        public enum EffectType
        {
            Default,
            //Points, // No longer supported as of XNA 4.0
            WireFrame,
            //Flat, // not supported on Xbox 360
            Gouraund,
            Rubber,
            Ceramic,
            Gold,
            Chrome,
            Copper,
            WorldsWorstToonShader,
            // ------
            Count
        };

        // initial effect for the teapot
        EffectType m_CurrentTeapotEffect = EffectType.Gold;

        // allocate storage for our collection of effects
        Effect[] m_effects = new Effect[(int)EffectType.Count];

        // our collection of meshes, loaded from disk
        Model m_model = null;

        // our meshes (a teapot and some balls)
        MyMesh m_meshTeapot = null;
        MyMesh[] m_meshBall = new MyMesh[(int)EffectType.Count];

        // the environment map
        TextureCube m_texEnvironmentMap = null;

        // location of the camera
        MyCamera m_camera = new MyCamera();

        // aspect ratio for our game
        float m_aspect = (float)SCREEN_WIDTH / (float)SCREEN_HEIGHT;

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
            //// Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            m_model = Content.Load<Model>(@"media\models\example");
            m_texEnvironmentMap = Content.Load<TextureCube>(@"media\textures\c_olmForest_16M");

            InitEffects();
            InitMyMeshes();

            m_camera.Location = new Vector3(-725, 600, 590);
            m_camera.LookAt(m_meshTeapot);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        // set effect parameters for our the materials
        protected void InitEffects()
        {
            // Default effect -- created by content pipeline when mesh 
            // is loaded without an attached effect
            BasicEffect basic =
                (BasicEffect)m_model.Meshes[0].MeshParts[0].Effect;
            basic.EnableDefaultLighting();
            basic.CurrentTechnique = basic.Techniques[0];
            m_effects[(int)EffectType.Default] =
                basic.Clone();
                //basic.Clone(graphics.GraphicsDevice);

            //// Points
            //Effect points = Content.Load<Effect>(@"media\effects\point");
            //points.Parameters["MaterialDiffuseColor"].
            //    SetValue(Color.White.ToVector4());
            //m_effects[(int)EffectType.Points] =
            //    points.Clone();
            //    //points.Clone(graphics.GraphicsDevice);

            // WireFrame
            Effect wireframe =
                Content.Load<Effect>(@"media\effects\wireframe");
            wireframe.Parameters["MaterialDiffuseColor"].
                SetValue(Color.White.ToVector4());
            m_effects[(int)EffectType.WireFrame] =
                wireframe.Clone();
                //wireframe.Clone(graphics.GraphicsDevice);

            //// Flat
            //Effect flat = 
            //    content.Load<Effect>(@"media\effects\flat");
            //flat.Parameters["MaterialDiffuseColor"].
            //    SetValue(new Vector4(1.0f, 0.9f, 0.1f, 1.0f));
            //flat.Parameters["MaterialSpecularColor"].
            //    SetValue(new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
            //m_effects[(int)EffectType.Flat] = 
            //    flat.Clone(graphics.GraphicsDevice);

            // WorldsWorstToonShader
            Effect worldsWorstToonShader =
                Content.Load<Effect>(@"media\effects\badtoon");
            worldsWorstToonShader.Parameters["MaterialDiffuseColor"].
                SetValue(new Vector4(1.0f, 0.8f, 0.1f, 1.0f));
            worldsWorstToonShader.Parameters["MaterialSpecularColor"].
                SetValue(new Vector4(1.0f, 0.9f, 0.4f, 1.0f));
            worldsWorstToonShader.Parameters["MaterialSpecularPower"].
                SetValue(2);
            worldsWorstToonShader.Parameters["MaterialSpecularFalloff"].
                SetValue(0.33f);
            m_effects[(int)EffectType.WorldsWorstToonShader] =
                worldsWorstToonShader.Clone();
                //worldsWorstToonShader.Clone(graphics.GraphicsDevice);

            // load environment map effect and set common properties once
            Effect effect = Content.Load<Effect>(@"media\effects\envmap");
            effect.Parameters["MaterialAmbientColor"].
                SetValue(Color.Black.ToVector4());
            effect.Parameters["MaterialEmissiveColor"].
                SetValue(Color.Black.ToVector4());
            effect.Parameters["MaterialSpecularPower"].
                SetValue(8);
            effect.Parameters["EnvironmentMap"].
                SetValue(m_texEnvironmentMap);
            effect.CurrentTechnique = effect.Techniques[0];

            // Gouraund
            effect.Parameters["MaterialDiffuseColor"].
                SetValue(new Vector4(1.0f, 0.9f, 0.1f, 1.0f));
            effect.Parameters["MaterialSpecularColor"].
                SetValue(new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
            effect.Parameters["EnvironmentWeight"].
                SetValue(0.00f);
            m_effects[(int)EffectType.Gouraund] =
                effect.Clone();
                //effect.Clone(graphics.GraphicsDevice);

            // RedRubber
            effect.Parameters["MaterialDiffuseColor"].
                SetValue(new Vector4(1.0f, 0.1f, 0.1f, 1.0f));
            effect.Parameters["MaterialSpecularColor"].
                SetValue(new Vector4(0.9f, 0.7f, 0.7f, 1.0f));
            effect.Parameters["EnvironmentWeight"].
                SetValue(0.00f);
            m_effects[(int)EffectType.Rubber] =
                effect.Clone();
                //effect.Clone(graphics.GraphicsDevice);

            // RedCeramic
            effect.Parameters["MaterialDiffuseColor"].
                SetValue(new Vector4(1.0f, 0.1f, 0.1f, 1.0f));
            effect.Parameters["MaterialSpecularColor"].
                SetValue(new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
            effect.Parameters["EnvironmentWeight"].
                SetValue(0.125f);
            m_effects[(int)EffectType.Ceramic] =
                effect.Clone();
                //effect.Clone(graphics.GraphicsDevice);

            // Gold
            effect.Parameters["MaterialDiffuseColor"].
                SetValue(new Vector4(1.0f, 0.9f, 0.1f, 1.0f));
            effect.Parameters["MaterialSpecularColor"].
                SetValue(new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
            effect.Parameters["EnvironmentWeight"].
                SetValue(0.50f);
            m_effects[(int)EffectType.Gold] =
                effect.Clone();
                //effect.Clone(graphics.GraphicsDevice);

            // Chrome
            effect.Parameters["MaterialDiffuseColor"].
                SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
            effect.Parameters["MaterialSpecularColor"].
                SetValue(new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
            effect.Parameters["EnvironmentWeight"].
                SetValue(0.80f);
            m_effects[(int)EffectType.Chrome] =
                effect.Clone();
                //effect.Clone(graphics.GraphicsDevice);

            // Copper
            effect.Parameters["MaterialDiffuseColor"].
                SetValue(new Vector4(1.0f, 0.1f, 0.1f, 1.0f));
            effect.Parameters["MaterialSpecularColor"].
                SetValue(new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
            effect.Parameters["EnvironmentWeight"].
                SetValue(0.70f);
            m_effects[(int)EffectType.Copper] =
                effect.Clone();
                //effect.Clone(graphics.GraphicsDevice);
        }

        // init meshes before first use
        protected void InitMyMeshes()
        {
            // init teapot
            if (m_meshTeapot == null)
            {
                m_meshTeapot = new MyMesh();
                m_meshTeapot.Rotate(0, 180, 0);
            }
            m_meshTeapot.Model = m_model;
            m_meshTeapot.MeshName = "Teapot01";
            SelectEffect(m_CurrentTeapotEffect);

            // init balls (one mesh, multiple instances)
            float angle = 360.0f / (float)(m_meshBall.Length);
            for (int i = 0; i < m_meshBall.Length; i++)
            {
                if (m_meshBall[i] == null)
                {
                    m_meshBall[i] = new MyMesh();
                    m_meshBall[i].RotateTo(0, i * angle, 0);
                }
                m_meshBall[i].Model = m_model;
                m_meshBall[i].MeshName = "Sphere01";
                m_meshBall[i].Effect = m_effects[i];
            }
        }

        // map our enums to actual effects, apply selected effect to teapot
        protected void SelectEffect(EffectType type)
        {
            m_CurrentTeapotEffect = type;
            m_meshTeapot.Effect = m_effects[(int)type];
        }

        // make sure player doesn't change textures too fast
        private const double TEXTURE_DELAY = 0.25;
        private double m_timeSinceLastSelectNextEffect = TEXTURE_DELAY;

        // cycle to next effect
        protected void SelectNextEffect()
        {
            // if delay has elapsed
            if (m_timeSinceLastSelectNextEffect >= TEXTURE_DELAY)
            {
                // cycle through all available effects
                if (m_CurrentTeapotEffect == EffectType.Count - 1)
                {
                    SelectEffect(EffectType.Default);
                }
                else
                {
                    SelectEffect(m_CurrentTeapotEffect + 1);
                }

                // reset dealy timer
                m_timeSinceLastSelectNextEffect = 0;
            }
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

            // time in seconds since last call to update
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // rotate the teapot mesh
            m_meshTeapot.Rotate(17.0f * elapsed, 9.0f * elapsed, 7.5f * elapsed);

            // rotate the balls
            foreach (MyMesh ball in m_meshBall)
            {
                ball.Rotate(0, 23.0f * elapsed, 0);
            }

            // process player input, add delay between texture changes
            m_timeSinceLastSelectNextEffect += elapsed;
            ProcessInput(GamePad.GetState(PlayerIndex.One), Keyboard.GetState(), elapsed);

            base.Update(gameTime);
        }

        // process input from keyboard and gamepad
        protected void ProcessInput(GamePadState pad1, KeyboardState key1, float elapsed)
        {
            // change teapot texture?
            if (pad1.Buttons.A == ButtonState.Pressed || key1.IsKeyDown(Keys.Space))
            {
                SelectNextEffect();
            }

            m_camera.ProcessInput(pad1, key1, elapsed);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawMeshes();

            base.Draw(gameTime);
        }

        // draw all meshes
        public void DrawMeshes()
        {
            // draw teapot
            m_meshTeapot.Draw(graphics.GraphicsDevice, m_camera, m_aspect);

            // draw balls
            foreach (MyMesh ball in m_meshBall)
            {
                ball.Draw(graphics.GraphicsDevice, m_camera, m_aspect);
            }
        }
    }
}
