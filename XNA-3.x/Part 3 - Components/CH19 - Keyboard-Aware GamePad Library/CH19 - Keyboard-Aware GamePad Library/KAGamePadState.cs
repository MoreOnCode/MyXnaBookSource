using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Codetopia.Input
{
    // a drop-in replacement for the GamePadState class
    public class KAGamePadState
    {
        // the current state of the digital buttons
        protected KAGamePadButtons m_Buttons = new KAGamePadButtons();
        public KAGamePadButtons Buttons
        {
            get { return m_Buttons; }
        }

        // the current state of the directional buttons
        protected KAGamePadDPad m_DPad = new KAGamePadDPad();
        public KAGamePadDPad DPad
        {
            get { return m_DPad; }
        }

        // the current state of the thumbsticks
        protected KAGamePadThumbSticks m_ThumbSticks = 
            new KAGamePadThumbSticks();
        public KAGamePadThumbSticks ThumbSticks
        {
            get { return m_ThumbSticks; }
        }

        // the current state of the triggers
        protected KAGamePadTriggers m_Triggers = new KAGamePadTriggers();
        public KAGamePadTriggers Triggers
        {
            get { return m_Triggers; }
        }

        // is this virtual gamepad connected?
        protected bool m_IsConnected = false;
        public bool IsConnected
        {
            get { return m_IsConnected; }
            set { m_IsConnected = value; }
        }

        // the packet number for the current state, parallels the 
        // PacketNumber of the standard GamePadState class
        protected static int m_PacketNumberMaster = 0;
        protected int m_PacketNumber = 0;
        public int PacketNumber
        {
            get { return m_PacketNumber; }
        }

        // don't allow callers to crete their own 
        // instances of this class
        private KAGamePadState() { }

        // create an instance of our gamepad state object, using an 
        // instance of the standard gamepad state object
        public KAGamePadState(GamePadState state)
        {
            m_IsConnected = state.IsConnected;

            if (IsConnected)
            {
                m_Buttons.A = state.Buttons.A;
                m_Buttons.B = state.Buttons.B;
                m_Buttons.Back = state.Buttons.Back;
                m_Buttons.LeftShoulder = state.Buttons.LeftShoulder;
                m_Buttons.LeftStick = state.Buttons.LeftStick;
                m_Buttons.RightShoulder = state.Buttons.RightShoulder;
                m_Buttons.RightStick = state.Buttons.RightStick;
                m_Buttons.Start = state.Buttons.Start;
                m_Buttons.X = state.Buttons.X;
                m_Buttons.Y = state.Buttons.Y;

                m_DPad.Down = state.DPad.Down;
                m_DPad.Left = state.DPad.Left;
                m_DPad.Right = state.DPad.Right;
                m_DPad.Up = state.DPad.Up;

                m_ThumbSticks.Left = state.ThumbSticks.Left;
                m_ThumbSticks.Right = state.ThumbSticks.Right;

                m_Triggers.Left = state.Triggers.Left;
                m_Triggers.Right = state.Triggers.Right;

                m_PacketNumber = m_PacketNumberMaster++;
            }
        }

        // overridden "not equals" operator
        public static bool operator !=
            (KAGamePadState state1, KAGamePadState state2)
        {
            return !AreSame(state1, state2);
        }

        // overridden "equals" operator
        public static bool operator ==
            (KAGamePadState state1, KAGamePadState state2)
        {
            return AreSame(state1, state2);
        }

        // overridden Equals method; are the current and 
        // specified instances the same?
        public override bool Equals(object obj)
        {
            bool same = false;

            if (obj != null && obj is KAGamePadState)
            {
                same = AreSame(this, (KAGamePadState)obj);
            }

            return same;
        }

        // overridden GetHashCode method
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // are the two specified instances the same?
        public static bool AreSame(KAGamePadState state1, KAGamePadState state2)
        {
            return
                state1.Buttons == state2.Buttons &&
                state1.DPad == state2.DPad &&
                state1.IsConnected == state2.IsConnected &&
                state1.ThumbSticks == state2.ThumbSticks &&
                state1.Triggers == state2.Triggers;
        }

        // a string representaion of this instance
        public override string ToString()
        {
            return
                Buttons.ToString() +
                DPad.ToString() +
                IsConnected.ToString() +
                ThumbSticks.ToString() +
                Triggers.ToString();
        }

        // true when no gamepad buttons are being pressed
        public bool IsIdle
        {
            get { return this.ToString() == m_IdleState; }
        }

        // create a string representation of the default 
        // instance of the KAGamePadState class
        protected static readonly string m_IdleState =
            new KAGamePadState().ToString();
    }
}
