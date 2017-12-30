// Modifier2DGravity.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Codetopia.Graphics.ParticleSystem
{
    // pull particle down (or up), accounting for current momentum
    public class Modifier2DGravity : Modifier2D
    {
        protected float m_Gravity = 200.0f;

        public Modifier2DGravity() { }
        public Modifier2DGravity(float gravity)
        {
            m_Gravity = gravity;
        }

        // called for each particle, every frame, by emitter, if enabled
        public override void Update(Particle2D particle, float elapsed)
        {
            Vector2 v2 = particle.Movement;
            v2.Y += m_Gravity * elapsed;
            particle.Movement = v2;
        }
    }
}
