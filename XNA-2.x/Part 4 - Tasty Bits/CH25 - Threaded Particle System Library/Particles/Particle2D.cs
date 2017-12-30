// Particle2D.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Codetopia.Graphics.ParticleSystem
{
    public class Particle2D
    {
        // only update and draw active particles
        protected bool m_IsActive = false;
        public bool IsActive
        {
            get { return m_IsActive; }
            set
            {
                m_Age = 0.0f;
                m_IsActive = value;
            }
        }

        // age of this particle (in seconds)
        protected double m_Age = 0.0f;
        public double Age
        {
            get { return m_Age; }
            set { m_Age = value; }
        }

        // max age of this particle
        protected double m_LifeTime = 0.0f;
        public double LifeTime
        {
            get { return m_LifeTime; }
            set { m_LifeTime = value; }
        }

        // current location of the particle
        protected Vector2 m_Position;
        public Vector2 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        // momentum of this particle
        protected Vector2 m_Movement;
        public Vector2 Movement
        {
            get { return m_Movement; }
            set { m_Movement = value; }
        }

        // color (tint) of this particle
        protected Vector4 m_Color;
        public Vector4 Color
        {
            get { return m_Color; }
            set { m_Color = value; }
        }

        // rotation of this particle
        protected float m_Rotation = 0.0f;
        public float Rotation
        {
            get { return m_Rotation; }
            set { m_Rotation = value; }
        }

        // transparency of this particle
        protected float m_Opacity = 1.0f;
        public float Opacity
        {
            get { return m_Opacity; }
            set { m_Opacity = value; }
        }

        // size of this particle
        protected float m_Scale = 1.0f;
        public float Scale
        {
            get { return m_Scale; }
            set { m_Scale = value; }
        }

        // z-order of this particle
        protected float m_Depth = 0.0f;
        public float Depth
        {
            get { return m_Depth; }
            set { m_Depth = value; }
        }

        // update position (based on m_Movement) and age
        public void Update(float elapsed)
        {
            // only update active particles
            if (IsActive)
            {
                // move the particle
                m_Position += m_Movement * elapsed;

                // check for expired particles
                if (LifeTime != 0.0f) // 0.0f == never dies
                {
                    Age += elapsed;
                    if (Age > LifeTime)
                    {
                        IsActive = false;
                    }
                }
            }
        }

        // render the particle
        public void Draw(SpriteBatch batch, Texture2D texture, Rectangle clipRect)
        {
            // only draw active particles
            if (IsActive)
            {
                batch.Draw(
                    texture,
                    Position,
                    clipRect,
                    new Color(Color),
                    Rotation,
                    Vector2.Zero,
                    Scale,
                    SpriteEffects.None,
                    Depth);
            }
        }
    }
}
