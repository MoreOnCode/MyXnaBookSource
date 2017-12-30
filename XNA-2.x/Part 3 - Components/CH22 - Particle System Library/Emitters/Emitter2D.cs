// Emitter2D.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Codetopia.Graphics.ParticleSystem
{
    public class Emitter2D
    {
        // default constructor, 1000 particles
        public Emitter2D() : this(1000) { }

        // init emitter with Max particles
        public Emitter2D(long Max)
        {
            MaxParticles = Max;
        }

        // helper to init particle locations within emitter bounds
        protected RangedVector2 m_EmitterRV2 = new RangedVector2();

        // location and size of the emitter
        protected Rectangle m_EmitterRect;
        public Rectangle EmitterRect
        {
            get { return m_EmitterRect; }
            set
            {
                m_EmitterRect = value;
                m_EmitterRV2 = RangedVector2.FromRectangle(m_EmitterRect);
            }
        }

        // helper to update the location of the emitter
        public Vector2 Position
        {
            get { return new Vector2(m_EmitterRect.Left, m_EmitterRect.Top); }
            set
            {
                EmitterRect = new Rectangle(
                    (int)Math.Round(value.X),
                    (int)Math.Round(value.Y),
                    m_EmitterRect.Width,
                    m_EmitterRect.Height);
            }
        }

        // outside of these bounds, the particle dies
        protected Rectangle m_EmitterBoundsRect;
        public Rectangle EmitterBoundsRect
        {
            get { return m_EmitterBoundsRect; }
            set { m_EmitterBoundsRect = value; }
        }

        // min and max of movement vector
        protected RangedVector2 m_RangeMovement = new RangedVector2(Vector2.One * -150.0f, Vector2.One * 150.0f);
        public RangedVector2 RangeMovement
        {
            get { return m_RangeMovement; }
            set { m_RangeMovement = value; }
        }
        
        // min and max color of new particles
        protected RangedVector4 m_RangeColor = new RangedVector4(Vector4.Zero,Vector4.One);
        public RangedVector4 RangeColor
        {
            get { return m_RangeColor; }
            set { m_RangeColor = value; }
        }

        // the particle image
        protected Texture2D m_Texture;
        public Texture2D Texture
        {
            get { return m_Texture; }
            set 
            { 
                m_Texture = value;
                TextureRect = new Rectangle(
                    0,0,
                    m_Texture.Width,
                    m_Texture.Height
                    );
            }
        }

        // coordinates of the particle graphic within the texture
        protected Rectangle m_TextureRect;
        public Rectangle TextureRect
        {
            get { return m_TextureRect; }
            set { m_TextureRect = value; }
        }

        // false? don't draw, don't update
        protected bool m_Enabled = true;
        public bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        // false? draw and update, but no new particles
        protected bool m_Active = true;
        public bool Active
        {
            get { return m_Active; }
            set { m_Active = value; }
        }

        // number of particles to spew each frame
        protected long m_ParticlesPerUpdate = 1;
        public long ParticlesPerUpdate
        {
            get { return m_ParticlesPerUpdate; }
            set { m_ParticlesPerUpdate = value; }
        }

        // max number of particles that this emitter can track
        protected long m_MaxParticles = 1000;
        public long MaxParticles
        {
            get { return m_MaxParticles; }
            set 
            {
                m_MaxParticles = (value > 0 ? value : 1);
                m_ActiveParticles.Clear();
                m_InactiveParticles.Clear();
                for (long i = 0; i < m_MaxParticles; i++)
                {
                    m_InactiveParticles.Add(new Particle2D());
                }
            }
        }

        // lifespan of particle, expressed in seconds
        protected float m_ParticleLifetime = 10.0f;
        public float ParticleLifetime
        {
            get { return m_ParticleLifetime; }
            set { m_ParticleLifetime = value; }
        }

        // keep track of active and inactive particles
        protected List<Particle2D> m_ActiveParticles = new List<Particle2D>();
        protected List<Particle2D> m_InactiveParticles = new List<Particle2D>();

        // keep track of attached modifiers
        protected List<Modifier2D> m_Modifiers = new List<Modifier2D>();
        public void AddModifier(Modifier2D modifier)
        {
            m_Modifiers.Add(modifier);
        }

        // remove a specific modifier
        public void RemoveModifier(Modifier2D modifier)
        {
            if (m_Modifiers.Contains(modifier))
            {
                m_Modifiers.Remove(modifier);
            }
        }

        // forget all attached modifiers
        public void ClearModifiers(Modifier2D modifier)
        {
            m_Modifiers.Clear();
        }

        // manage active particles, spawn new particles if it's time to do so
        public virtual void Update(float elapsed)
        {
            // only update if enabled
            if (Enabled)
            {
                // temp variables to save typing
                bool outOfBounds;
                int parX, parY;
                for (int i = 0; i < m_ActiveParticles.Count;i++ )
                {
                    Particle2D particle = m_ActiveParticles[i];

                    // when particle leaves emitter bounds, mark inactive
                    parX = (int)Math.Round(particle.Position.X);
                    parY = (int)Math.Round(particle.Position.Y);
                    outOfBounds = parX < EmitterBoundsRect.Right;
                    outOfBounds &= parX + TextureRect.Width > EmitterBoundsRect.Left;
                    outOfBounds &= parY < EmitterBoundsRect.Bottom;
                    outOfBounds &= parY + TextureRect.Height > EmitterBoundsRect.Top;
                    if (outOfBounds) particle.IsActive = false;

                    // process active particles, cleanup inactive particles
                    if (particle.IsActive)
                    {
                        // allow active modifiers to update particle
                        foreach (Modifier2D modifier in m_Modifiers)
                        {
                            if (modifier.Enabled)
                            {
                                // tell the modifier to update this particle
                                modifier.Update(particle, elapsed);
                            }
                        }
                        // tell particle to update itself
                        particle.Update(elapsed);
                    }
                    else
                    {
                        // move particle to inactive list for later reuse
                        m_InactiveParticles.Add(particle);
                        m_ActiveParticles.RemoveAt(i);
                        i--;
                    }
                }

                // try to generate ParticlesPerUpdate new particles
                for (long i = 0; Active && i < ParticlesPerUpdate; i++)
                {
                    if (m_InactiveParticles.Count > 0 )
                    {
                        // reset particle and add it to our pool of active particles
                        Particle2D particle = m_InactiveParticles[0];
                        particle.Position = m_EmitterRV2.RandomValue();
                        particle.Movement = m_RangeMovement.RandomValue();
                        particle.Color = m_RangeColor.RandomValue();
                        particle.IsActive = true;
                        particle.LifeTime = m_ParticleLifetime;
                        m_InactiveParticles.Remove(particle);
                        m_ActiveParticles.Add(particle);
                    }
                    else
                    {
                        // no more particles in our inactive pool
                        break;
                    }
                }
            }
        }
    
        // render the active particles
        public virtual void Draw(SpriteBatch batch)
        {
            // only draw particles when emitter is enabled
            if (Enabled)
            {
                foreach (Particle2D particle in m_ActiveParticles)
                {
                    // ask the particle to draw itself
                    particle.Draw(batch, Texture, TextureRect);
                }
            }
        }

    }
}
