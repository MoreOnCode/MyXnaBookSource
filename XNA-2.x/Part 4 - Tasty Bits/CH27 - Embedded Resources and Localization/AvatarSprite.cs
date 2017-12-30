// AvatarSprite.cs
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace Chapter27
{
    // idle states for the avatar
    public enum AvatarStates
    {
        Blink,
        LookLeft,
        LookRight,
        LookAhead,
        Smile,
        // ------
        Count,
    };

    public class AvatarSprite : SpriteBase 
    {
        // reference to the dialog sprite
        // certain actions are prevented while the dialog 
        // is transitioning from one size to another
        protected DialogSprite m_Dialog = null;
        public DialogSprite Dialog
        {
            get { return m_Dialog; }
            set { m_Dialog = value; }
        }

        // initialize member variables, create seperate 
        // sprites for each idle state of the avatar
        public AvatarSprite()
        {
            // init member variables
            TextureRect = new Rectangle(0, 0, 256, 256);
            Location = new Vector2(70.0f, 175.0f);

            // create a seperate sprite for each idle state
            for (int i = 0; i < m_Avatars.Length; i++)
            {
                m_Avatars[i] = new SpriteBase();
                m_Avatars[i].Location = Location;
                m_Avatars[i].TextureRect = TextureRect;
                m_Avatars[i].TimeDelay = 3.0;
                m_Avatars[i].TimeIdle = 0.0;
            }

            // make blink state much shorter than the others
            this[AvatarStates.Blink].TimeDelay = 0.13;
        }

        // the idle state of the avatar, used to make the character
        // more interesting when the player is idle
        protected AvatarStates m_State = AvatarStates.LookAhead;
        public AvatarStates State
        {
            get { return m_State; }
            set
            {
                if (m_State != value)
                {
                    BeforeSetState();
                    m_State = value;
                    AfterSetState();
                }
            }
        }

        // pre-state change logic
        protected void BeforeSetState()
        {
            CurrentSprite.TimeIdle = 0;
        }

        // post-state change logic
        protected void AfterSetState()
        {
            CurrentSprite.TimeIdle = 0;
        }

        // an arrays of base sprites, one for each avatar state
        protected SpriteBase[] m_Avatars = 
            new SpriteBase[(int)AvatarStates.Count];
        
        // in indexed property to get a reference to a sprite
        // based on the avatar's current state
        public SpriteBase this[AvatarStates state]
        {
            get
            {
                if (state == AvatarStates.Count)
                {
                    state = AvatarStates.LookAhead;
                }
                return m_Avatars[(int)state];
            }
        }

        // get a reference to the sprite that's associated 
        // with the avatar's current state
        public SpriteBase CurrentSprite
        {
            get { return this[State]; }
        }

        // update the state of the current sprite, 
        // and process player input
        public sealed override void Update(float elapsed)
        {
            base.Update(elapsed);
            ManageState(elapsed);
            CurrentSprite.Update(elapsed);
        }

        // draw the current sprite, possibly offset during toot shake
        public sealed override void Draw(SpriteBatch batch)
        {
            CurrentSprite.Draw(batch, m_LocationDelta);
        }

        // true when toot action is requested by the player, and 
        // allowed by their culture-specific settings
        public bool IsToot
        {
            get
            {
                return LocalizedContent.ResourceHelper.AllowCrude &&
                    GamePadState.Buttons.X == ButtonState.Pressed;
            }
        }

        // true whenever the dialog sprite is visible
        public bool IsSpeaking
        {
            get { return Dialog != null && Dialog.VisibleLineCount > 0; }
        }

        // helper member to generate random values for idle states
        protected Random m_rand = new Random();

        // time spent in toot mode, used to 
        // increase intensity of the effect
        protected double m_TimeToot = 0.0f;

        // shake offset, used in toot animation
        protected Vector2 m_LocationDelta = Vector2.Zero;
        public Vector2 LocationDelta { get { return m_LocationDelta; } }

        // process player input
        protected void ManageState(float elapsed)
        {
            // we've just come out of toot mode
            if (!IsToot && m_TimeToot > 0)
            {
                // reset counters, kill shaking
                m_TimeToot = 0;
                m_LocationDelta = Vector2.Zero;
                GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
            }

            if (IsSpeaking)
            {
                // dialog sprite is visible, ignore everything else
                State = AvatarStates.Smile;
            }
            else if (IsToot)
            {
                // we're in toot mode, set avatar state and increment timer
                State = AvatarStates.Blink;
                m_TimeToot += elapsed;

                // calculate the shake for the controller
                float motorLeft = (float)Math.Min(1.0f, m_TimeToot / 4.0f);
                float motorRight = (float)Math.Min(1.0f, m_TimeToot / 10.0f);
                GamePad.SetVibration(PlayerIndex.One, motorLeft, motorRight);

                // calculate the shake for the sprite
                float deltaMax = (float)Math.Min(4.0f, m_TimeToot / 3.0f);
                m_LocationDelta.X = m_rand.Next((int)(deltaMax + 1));
                m_LocationDelta.Y = m_rand.Next((int)(deltaMax + 1));
            }
            else
            {
                // we're not speaking or tooting, 
                // randomly select an idle state
                if ((State == AvatarStates.Smile) || 
                    (CurrentSprite.TimeIdle > CurrentSprite.TimeDelay))
                {
                    State = (AvatarStates)m_rand.Next(
                        (int)AvatarStates.Smile);
                }
            }
        }
    }
}
