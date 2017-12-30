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

namespace Chapter07
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related Content.  Calling base.Initialize will enumerate through any components
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

            base.Initialize();
        }

        // screen constants
        const int SCREEN_WIDTH = 640;
        const int SCREEN_HEIGHT = 480;

        // source for background image
        Texture2D m_texBackground;
        Rectangle m_rectScreen = new Rectangle(0, 0, 640, 480);
        Rectangle m_rectGraph = new Rectangle(640, 64, 50, 50);
        Rectangle m_rectCursor = new Rectangle(641, 1, 62, 62);
        Vector2 m_CursorCenter = new Vector2(31, 31);
        Vector2 m_CursorLocation = new Vector2(450, 205);

        // cursor angle
        float m_angle = 0.0f;

        // top-left of graph paper
        Vector2 m_GraphOrigin = Vector2.Zero;

        // handy constant for sin and cos
        const double ToRadians = Math.PI / 180.0;

        // pad states
        GamePadState m_pad1;
        GamePadState m_pad2;
        GamePadState m_pad3;
        GamePadState m_pad4;

        // all the textures
        ButtonSprite m_btnA = new ButtonSprite();
        ButtonSprite m_btnB = new ButtonSprite();
        ButtonSprite m_btnX = new ButtonSprite();
        ButtonSprite m_btnY = new ButtonSprite();
        ButtonSprite m_btnBack = new ButtonSprite();
        ButtonSprite m_btnStart = new ButtonSprite();
        ButtonSprite m_btnDPadDown = new ButtonSprite();
        ButtonSprite m_btnDPadUp = new ButtonSprite();
        ButtonSprite m_btnDPadLeft = new ButtonSprite();
        ButtonSprite m_btnDPadRight = new ButtonSprite();
        ButtonSprite m_btnShoulderLeft = new ButtonSprite();
        ButtonSprite m_btnShoulderRight = new ButtonSprite();
        ButtonSprite m_btnVBarLTrigger = new ButtonSprite();
        ButtonSprite m_btnVBarRTrigger = new ButtonSprite();
        ButtonSprite m_btnVBarLThumb = new ButtonSprite();
        ButtonSprite m_btnVBarRThumb = new ButtonSprite();
        ButtonSprite m_btnHBarLThumb = new ButtonSprite();
        ButtonSprite m_btnHBarRThumb = new ButtonSprite();
        ButtonSprite m_btnHBarArrow = new ButtonSprite();
        ButtonSprite m_btnVBarArrow = new ButtonSprite();
        ButtonSprite m_btnLThumb = new ButtonSprite();
        ButtonSprite m_btnRThumb = new ButtonSprite();
        ButtonSprite m_btnPort1 = new ButtonSprite();
        ButtonSprite m_btnPort2 = new ButtonSprite();
        ButtonSprite m_btnPort3 = new ButtonSprite();
        ButtonSprite m_btnPort4 = new ButtonSprite();
        Rectangle[] m_rectPortNum = {
            Rectangle.Empty,
            Rectangle.Empty,
            Rectangle.Empty,
            Rectangle.Empty,
        };

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your Content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // local temp variables
            Texture2D texTemp;
            Rectangle recTemp;

            // background, cursor, and graph textures
            m_texBackground =
                Content.Load<Texture2D>(@"media\background");

            // button textures
            texTemp = Content.Load<Texture2D>(@"media\buttons");

            // init A
            m_btnA.TextureNormal = texTemp;
            m_btnA.RectNormal =
                new Rectangle(0, 64, 64, 64);
            m_btnA.TexturePressed = texTemp;
            m_btnA.RectPressed =
                new Rectangle(128, 64, 64, 64);

            // init B
            m_btnB.TextureNormal = texTemp;
            m_btnB.RectNormal =
                new Rectangle(64, 64, 64, 64);
            m_btnB.TexturePressed = texTemp;
            m_btnB.RectPressed =
                new Rectangle(192, 64, 64, 64);

            // init X
            m_btnX.TextureNormal = texTemp;
            m_btnX.RectNormal =
                new Rectangle(0, 128, 64, 64);
            m_btnX.TexturePressed = texTemp;
            m_btnX.RectPressed =
                new Rectangle(128, 128, 64, 64);

            // init Y
            m_btnY.TextureNormal = texTemp;
            m_btnY.RectNormal =
                new Rectangle(64, 128, 64, 64);
            m_btnY.TexturePressed = texTemp;
            m_btnY.RectPressed =
                new Rectangle(192, 128, 64, 64);

            // init Back
            m_btnBack.TextureNormal = texTemp;
            m_btnBack.RectNormal =
                new Rectangle(0, 0, 64, 64);
            m_btnBack.TexturePressed = texTemp;
            m_btnBack.RectPressed =
                new Rectangle(128, 0, 64, 64);

            // init Start
            m_btnStart.TextureNormal = texTemp;
            m_btnStart.RectNormal =
                new Rectangle(64, 0, 64, 64);
            m_btnStart.TexturePressed = texTemp;
            m_btnStart.RectPressed =
                new Rectangle(192, 0, 64, 64);

            // init Left Thumb
            m_btnLThumb.TextureNormal = texTemp;
            m_btnLThumb.RectNormal =
                new Rectangle(0, 192, 64, 64);
            m_btnLThumb.TexturePressed = texTemp;
            m_btnLThumb.RectPressed =
                new Rectangle(128, 192, 64, 64);

            // init Right Thumb
            m_btnRThumb.TextureNormal = texTemp;
            m_btnRThumb.RectNormal =
                new Rectangle(64, 192, 64, 64);
            m_btnRThumb.TexturePressed = texTemp;
            m_btnRThumb.RectPressed =
                new Rectangle(192, 192, 64, 64);

            // init DPad Up
            m_btnDPadUp.TextureNormal = texTemp;
            m_btnDPadUp.RectNormal =
                new Rectangle(0, 256, 64, 64);
            m_btnDPadUp.TexturePressed = texTemp;
            m_btnDPadUp.RectPressed =
                new Rectangle(0, 320, 64, 64);

            // init DPad Right
            m_btnDPadRight.TextureNormal = texTemp;
            m_btnDPadRight.RectNormal =
                new Rectangle(64, 256, 64, 64);
            m_btnDPadRight.TexturePressed = texTemp;
            m_btnDPadRight.RectPressed =
                new Rectangle(64, 320, 64, 64);

            // init DPad Down
            m_btnDPadDown.TextureNormal = texTemp;
            m_btnDPadDown.RectNormal =
                new Rectangle(128, 256, 64, 64);
            m_btnDPadDown.TexturePressed = texTemp;
            m_btnDPadDown.RectPressed =
                new Rectangle(128, 320, 64, 64);

            // init DPad Left
            m_btnDPadLeft.TextureNormal = texTemp;
            m_btnDPadLeft.RectNormal =
                new Rectangle(192, 256, 64, 64);
            m_btnDPadLeft.TexturePressed = texTemp;
            m_btnDPadLeft.RectPressed =
                new Rectangle(192, 320, 64, 64);

            // init Left Shoulder
            m_btnShoulderLeft.TextureNormal = texTemp;
            m_btnShoulderLeft.RectNormal =
                new Rectangle(0, 384, 64, 32);
            m_btnShoulderLeft.TexturePressed = texTemp;
            m_btnShoulderLeft.RectPressed =
                new Rectangle(128, 384, 64, 32);

            // init Right Shoulder
            m_btnShoulderRight.TextureNormal = texTemp;
            m_btnShoulderRight.RectNormal =
                new Rectangle(64, 384, 64, 32);
            m_btnShoulderRight.TexturePressed = texTemp;
            m_btnShoulderRight.RectPressed =
                new Rectangle(192, 384, 64, 32);

            // analog bars and indicators, port state and numbers
            texTemp = Content.Load<Texture2D>(@"media\analog");

            // vertical analog bars
            recTemp = new Rectangle(0, 64, 64, 192);
            m_btnVBarLTrigger.TextureNormal = texTemp;
            m_btnVBarLTrigger.RectNormal = recTemp;
            m_btnVBarRTrigger.TextureNormal = texTemp;
            m_btnVBarRTrigger.RectNormal = recTemp;
            m_btnVBarLThumb.TextureNormal = texTemp;
            m_btnVBarLThumb.RectNormal = recTemp;
            m_btnVBarRThumb.TextureNormal = texTemp;
            m_btnVBarRThumb.RectNormal = recTemp;

            // since there's no "pressed" mode for analog
            // buttons, use RectPressed to note where the 
            // bounds of the actual bar extend
            recTemp = new Rectangle(
                7 - m_btnVBarLTrigger.RectNormal.X,
                77 - m_btnVBarLTrigger.RectNormal.Y,
                44, 160);
            m_btnVBarLTrigger.RectPressed = recTemp;
            m_btnVBarRTrigger.RectPressed = recTemp;
            m_btnVBarLThumb.RectPressed = recTemp;
            m_btnVBarRThumb.RectPressed = recTemp;

            // horizontal analog bars
            recTemp = new Rectangle(0, 0, 256, 64);
            m_btnHBarLThumb.TextureNormal = texTemp;
            m_btnHBarLThumb.RectNormal = recTemp;
            m_btnHBarRThumb.TextureNormal = texTemp;
            m_btnHBarRThumb.RectNormal = recTemp;

            // since there's no "pressed" mode for analog
            // buttons, use RectPressed to note where the 
            // bounds of the actual bar extend
            recTemp = new Rectangle(
                20 - m_btnHBarLThumb.RectNormal.X,
                8 - m_btnHBarLThumb.RectNormal.Y,
                209, 43);
            m_btnHBarLThumb.RectPressed = recTemp;
            m_btnHBarRThumb.RectPressed = recTemp;

            // analog value indicators
            m_btnVBarArrow.TextureNormal = texTemp;
            m_btnVBarArrow.RectNormal =
                new Rectangle(64, 64, 64, 64);
            m_btnHBarArrow.TextureNormal = texTemp;
            m_btnHBarArrow.RectNormal =
                new Rectangle(128, 64, 64, 64);

            // port activity indicators (inactive)
            // again, since ports aren't really buttons, I'm
            // using the existing properties for special-
            // case code. See the DrawPort() method.
            m_btnPort1.TextureNormal = texTemp;
            m_btnPort2.TextureNormal = texTemp;
            m_btnPort3.TextureNormal = texTemp;
            m_btnPort4.TextureNormal = texTemp;

            recTemp = new Rectangle(128, 128, 64, 64);
            m_btnPort1.RectNormal = recTemp;
            m_btnPort2.RectNormal = recTemp;
            m_btnPort3.RectNormal = recTemp;
            m_btnPort4.RectNormal = recTemp;

            // port activity indicators (active)
            m_btnPort1.TexturePressed = texTemp;
            m_btnPort2.TexturePressed = texTemp;
            m_btnPort3.TexturePressed = texTemp;
            m_btnPort4.TexturePressed = texTemp;

            recTemp = new Rectangle(64, 128, 64, 64);
            m_btnPort1.RectPressed = recTemp;
            m_btnPort2.RectPressed = recTemp;
            m_btnPort3.RectPressed = recTemp;
            m_btnPort4.RectPressed = recTemp;

            // port numbers
            m_rectPortNum[0] = new Rectangle(64, 192, 64, 64);
            m_rectPortNum[1] = new Rectangle(128, 192, 64, 64);
            m_rectPortNum[2] = new Rectangle(192, 192, 64, 64);
            m_rectPortNum[3] = new Rectangle(192, 128, 64, 64);

            // layout the UI
            PositionButtons();
        }

        // "buttons" can be positioned anywhere on the screen
        // it would have been better to group buttons and 
        // position individual buttons relative to the group's
        // top-left. decided to "hard-code" positions to keep 
        // the code smaller. the following values are arbitrary.
        public void PositionButtons()
        {
            // horizontal analog bars
            m_btnHBarLThumb.Location.X = 15;
            m_btnHBarLThumb.Location.Y = 15;

            m_btnHBarRThumb.Location.X = 15;
            m_btnHBarRThumb.Location.Y = 271;

            // vertical analog bars
            m_btnVBarLTrigger.Location.X = 15;
            m_btnVBarLTrigger.Location.Y = 79;

            m_btnVBarLThumb.Location.X = 79;
            m_btnVBarLThumb.Location.Y = 79;

            m_btnVBarRThumb.Location.X = 143;
            m_btnVBarRThumb.Location.Y = 79;

            m_btnVBarRTrigger.Location.X = 207;
            m_btnVBarRTrigger.Location.Y = 79;

            // Left Thumbstick button
            m_btnLThumb.Location.X = 47;
            m_btnLThumb.Location.Y = 351;

            // Left Shoulder button
            m_btnShoulderLeft.Location.X = 47;
            m_btnShoulderLeft.Location.Y = 431;

            // Right Shoulder button
            m_btnShoulderRight.Location.X = 143;
            m_btnShoulderRight.Location.Y = 351;

            // Right Thumbstick button
            m_btnRThumb.Location.X = 143;
            m_btnRThumb.Location.Y = 399;

            // back button
            m_btnBack.Location.X = 239;
            m_btnBack.Location.Y = 351;

            // start button
            m_btnStart.Location.X = 239;
            m_btnStart.Location.Y = 415;

            // A button
            m_btnA.Location.X = 303;
            m_btnA.Location.Y = 415;

            // B button
            m_btnB.Location.X = 367;
            m_btnB.Location.Y = 415;

            // X button
            m_btnX.Location.X = 303;
            m_btnX.Location.Y = 351;

            // Y button
            m_btnY.Location.X = 367;
            m_btnY.Location.Y = 351;

            // Directional Pad
            m_btnDPadLeft.Location.X = 431;
            m_btnDPadLeft.Location.Y = 383;

            m_btnDPadUp.Location.X = 495;
            m_btnDPadUp.Location.Y = 351;

            m_btnDPadDown.Location.X = 495;
            m_btnDPadDown.Location.Y = 415;

            m_btnDPadRight.Location.X = 559;
            m_btnDPadRight.Location.Y = 383;

            // active port indicators
            m_btnPort1.Location.X = 337;
            m_btnPort1.Location.Y = 15;

            m_btnPort2.Location.X = 401;
            m_btnPort2.Location.Y = 15;

            m_btnPort3.Location.X = 465;
            m_btnPort3.Location.Y = 15;

            m_btnPort4.Location.X = 529;
            m_btnPort4.Location.Y = 15;

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all Content.
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
            // capture pad state once per frame
            m_pad1 = GamePad.GetState(PlayerIndex.One);
            m_pad2 = GamePad.GetState(PlayerIndex.Two);
            m_pad3 = GamePad.GetState(PlayerIndex.Three);
            m_pad4 = GamePad.GetState(PlayerIndex.Four);

            // only process input from player one, and only if 
            // the controller is connected
            if (m_pad1.IsConnected)
            {
                // combine states to rotate left, true if any is pressed
                bool bLeft = m_pad1.DPad.Left == ButtonState.Pressed;
                bLeft |= m_pad1.ThumbSticks.Left.X < 0;
                bLeft |= m_pad1.ThumbSticks.Right.X < 0;
                if (bLeft) { m_angle -= 5.0f; }

                // combine states to rotate right, true if any is pressed
                bool bRight = m_pad1.DPad.Right == ButtonState.Pressed;
                bRight |= m_pad1.ThumbSticks.Left.X > 0;
                bRight |= m_pad1.ThumbSticks.Right.X > 0;
                if (bRight) { m_angle += 5.0f; }

                // distance to travel per frame, split into X and Y
                float dx = (float)Math.Cos(m_angle * ToRadians);
                float dy = (float)Math.Sin(m_angle * ToRadians);

                // check button states to determine thrust
                float fMove = 0.0f; // assume no movement

                // is the player moving the ship?
                if (m_pad1.ThumbSticks.Left.Y != 0.0f)
                {
                    fMove = m_pad1.ThumbSticks.Left.Y;
                }
                else if (m_pad1.ThumbSticks.Right.Y != 0.0f)
                {
                    fMove = m_pad1.ThumbSticks.Right.Y;
                }
                else if (m_pad1.Triggers.Right != 0.0f)
                {
                    fMove = m_pad1.Triggers.Right;
                }
                else if (m_pad1.Triggers.Left != 0.0f)
                {
                    fMove = -m_pad1.Triggers.Left;
                }
                else if (m_pad1.DPad.Up == ButtonState.Pressed)
                {
                    // treat as max thumbstick Y
                    fMove = 1.0f;
                }
                else if (m_pad1.DPad.Down == ButtonState.Pressed)
                {
                    // treat as min thumbstick Y
                    fMove = -1.0f;
                }

                // ship's thrust is relative to analog button states
                m_GraphOrigin.X -= dx * fMove;
                m_GraphOrigin.Y -= dy * fMove;

                // make sure that 0 <= graph origin x <= 50
                while (m_GraphOrigin.X < 0.0f)
                {
                    m_GraphOrigin.X += 50.0f;
                }
                while (m_GraphOrigin.X > 50.0f)
                {
                    m_GraphOrigin.X -= 50.0f;
                }

                // make sure that 0 <= graph origin y <= 50
                while (m_GraphOrigin.Y < 0.0f)
                {
                    m_GraphOrigin.Y += 50.0f;
                }
                while (m_GraphOrigin.Y > 50.0f)
                {
                    m_GraphOrigin.Y -= 50.0f;
                }

                // shake the controller while the A button is pressed
                if (m_pad1.Buttons.A == ButtonState.Pressed)
                {
                    GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
                }
                else
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                }
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

            // start adding sprites to the screen
            spriteBatch.Begin();

            // draw the graph paper
            DrawGraph(spriteBatch);

            // overlay the background image, graph will show
            // though the transparent parts of the background
            spriteBatch.Draw(m_texBackground, Vector2.Zero,
                m_rectScreen, Color.White);

            // draw digital and analog buttons and ports
            DrawButtons(spriteBatch);

            // finally, draw the cursor
            DrawCursor(spriteBatch);

            // let batch know that we're done
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawButtons(SpriteBatch batch)
        {
            // buttons
            DrawButton(batch, m_btnBack,
                m_pad1.Buttons.Back == ButtonState.Pressed);
            DrawButton(batch, m_btnStart,
                m_pad1.Buttons.Start == ButtonState.Pressed);
            DrawButton(batch, m_btnA,
                m_pad1.Buttons.A == ButtonState.Pressed);
            DrawButton(batch, m_btnB,
                m_pad1.Buttons.B == ButtonState.Pressed);
            DrawButton(batch, m_btnX,
                m_pad1.Buttons.X == ButtonState.Pressed);
            DrawButton(batch, m_btnY,
                m_pad1.Buttons.Y == ButtonState.Pressed);
            DrawButton(batch, m_btnLThumb,
                m_pad1.Buttons.LeftStick == ButtonState.Pressed);
            DrawButton(batch, m_btnRThumb,
                m_pad1.Buttons.RightStick == ButtonState.Pressed);

            // dpad
            DrawButton(batch, m_btnDPadUp,
                m_pad1.DPad.Up == ButtonState.Pressed);
            DrawButton(batch, m_btnDPadDown,
                m_pad1.DPad.Down == ButtonState.Pressed);
            DrawButton(batch, m_btnDPadLeft,
                m_pad1.DPad.Left == ButtonState.Pressed);
            DrawButton(batch, m_btnDPadRight,
                m_pad1.DPad.Right == ButtonState.Pressed);

            // shoulder
            DrawButton(batch, m_btnShoulderLeft,
                m_pad1.Buttons.LeftShoulder == ButtonState.Pressed);
            DrawButton(batch, m_btnShoulderRight,
                m_pad1.Buttons.RightShoulder == ButtonState.Pressed);

            // analog
            DrawHBar(batch, m_btnHBarLThumb,
                m_pad1.ThumbSticks.Left.X);
            DrawHBar(batch, m_btnHBarRThumb,
                m_pad1.ThumbSticks.Right.X);
            DrawVBar(batch, m_btnVBarLTrigger,
                m_pad1.Triggers.Left, 0.0f);
            DrawVBar(batch, m_btnVBarLThumb,
                m_pad1.ThumbSticks.Left.Y);
            DrawVBar(batch, m_btnVBarRThumb,
                m_pad1.ThumbSticks.Right.Y);
            DrawVBar(batch, m_btnVBarRTrigger,
                m_pad1.Triggers.Right, 0.0f);

            // port indicators
            DrawPort(batch, m_btnPort1, 0, m_pad1.IsConnected);
            DrawPort(batch, m_btnPort2, 1, m_pad2.IsConnected);
            DrawPort(batch, m_btnPort3, 2, m_pad3.IsConnected);
            DrawPort(batch, m_btnPort4, 3, m_pad4.IsConnected);
        }

        // draw the button at its current location in its current state
        private void DrawButton(SpriteBatch batch, ButtonSprite btn,
            bool pressed)
        {
            if (pressed)
            {
                batch.Draw(btn.TexturePressed, btn.Location,
                    btn.RectPressed, Color.White);
            }
            else
            {
                batch.Draw(btn.TextureNormal, btn.Location,
                    btn.RectNormal, Color.White);
            }
        }

        // overload for DrawVBar, with default min
        private void DrawVBar(SpriteBatch batch, ButtonSprite btn,
            float value)
        {
            DrawVBar(batch, btn, value, -1.0f);
        }

        // draw the bar and the arrow
        private void DrawVBar(SpriteBatch batch, ButtonSprite btn,
            float value, float min)
        {
            // determine the X of the arrow
            // NOTE: btn.RectNormal describes the bounds of the image
            // btn.RectPressed describes the bounds of the bar itself
            m_btnVBarArrow.Location.X =
                btn.Location.X +
                btn.RectPressed.X +
                btn.RectPressed.Width / 2 -
                m_btnVBarArrow.RectNormal.Width / 2;

            if (min < 0.0f)
            {
                // value is between -1.0f and 1.0f. offset value
                // so that value is between 0.0f and 2.0f
                value += 1.0f;
                // then scale so that so that value is 
                // between 0.0f and 1.0f
                value /= 2.0f;
            }

            // since value is now between 0 and 1, we can treat it 
            // like a percentage. so, Y becomes value percent of 
            // Height. NOTE: need to invert value since Y values
            // increase as you move down the screen. (see line with 
            // "// bottommost" comment)
            m_btnVBarArrow.Location.Y =
               btn.Location.Y + btn.RectPressed.Y +  // topmost pixel
               btn.RectPressed.Height -              // bottommost
               btn.RectPressed.Height * value -      // scaled value
               m_btnVBarArrow.RectNormal.Height / 2; // arrow midpoint

            // draw bar
            batch.Draw(btn.TextureNormal, btn.Location,
                btn.RectNormal, Color.White);
            // draw arrow
            DrawButton(batch, m_btnVBarArrow, false);
        }

        // overload for DrawHBar, with default min
        private void DrawHBar(SpriteBatch batch, ButtonSprite btn, float value)
        {
            DrawHBar(batch, btn, value, -1.0f);
        }

        // draw the bar and the arrow
        private void DrawHBar(SpriteBatch batch, ButtonSprite btn, float value, float min)
        {
            // determine the Y of the arrow
            // NOTE: btn.RectNormal describes the bounds of the image
            // btn.RectPressed describes the bounds of the bar itself
            m_btnHBarArrow.Location.Y =
                btn.Location.Y + btn.RectPressed.Y +
                btn.RectPressed.Height / 2 -
                m_btnHBarArrow.RectNormal.Height / 2;

            if (min < 0.0f)
            {
                // value is between -1.0f and 1.0f. offset value
                // so that value is between 0.0f and 2.0f
                value += 1.0f;
                // then scale so that so that value is 
                // between 0.0f and 1.0f
                value /= 2.0f;
            }

            // since value is now between 0 and 1, we can treat it 
            // like a percentage. so, X becomes value percent of 
            // Height.
            m_btnHBarArrow.Location.X =
               btn.Location.X + btn.RectPressed.X + // leftmost pixel
               btn.RectPressed.Width * value -      // scaled value
               m_btnHBarArrow.RectNormal.Width / 2; // arrow midpoint

            // draw bar
            batch.Draw(btn.TextureNormal, btn.Location,
                btn.RectNormal, Color.White);
            // draw arrow
            DrawButton(batch, m_btnHBarArrow, false);
        }

        // draw the active port indicators
        private void DrawPort(SpriteBatch batch, ButtonSprite btn, int index, bool active)
        {
            // gray (inactive) or green (active) circle
            DrawButton(batch, btn, active);
            // port number
            batch.Draw(btn.TextureNormal, btn.Location, m_rectPortNum[index], Color.White);
        }

        // render the graph paper
        private void DrawGraph(SpriteBatch batch)
        {
            // a single graph tile is only 50-by-50, so repeat
            // it as many times as needed to cover the entire 
            // game screen. since the paper is overlaid with the
            // background image, we don't need to worry too much 
            // about the edges.

            // temp variable for tiling
            Vector2 vSquare = new Vector2();

            // round to nearest pixel
            float oy = (float)Math.Round(m_GraphOrigin.Y);
            float ox = (float)Math.Round(m_GraphOrigin.X);

            for (float y = oy; y < SCREEN_HEIGHT; y += 50.0f)
            {
                // row by row
                vSquare.Y = y;
                for (float x = ox; x < SCREEN_WIDTH; x += 50.0f)
                {
                    // column by column
                    vSquare.X = x;
                    batch.Draw(m_texBackground, vSquare, m_rectGraph, Color.White);
                }
            }
        }

        // render the cursor
        private void DrawCursor(SpriteBatch batch)
        {
            batch.Draw(
                m_texBackground,              // cursor texture
                m_CursorLocation,             // cursor x, y
                m_rectCursor,                 // cursor source rect
                Color.White,                  // no tint
                (float)(m_angle * ToRadians), // cursor rotation
                m_CursorCenter,               // center of cursor
                1.0f,                         // don't scale
                SpriteEffects.None,           // no effect
                0.0f);                        // topmost layer
        }
    }
}
