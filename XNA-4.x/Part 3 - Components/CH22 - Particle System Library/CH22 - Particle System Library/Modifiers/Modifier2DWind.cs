using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Codetopia.Graphics.ParticleSystem
{
    // blow particle to left or right, accounting for current momentum
    public class Modifier2DWind : Modifier2D
    {
        protected float m_Wind = 200.0f;

        public Modifier2DWind() { }
        public Modifier2DWind(float wind)
        {
            m_Wind = wind;
        }

        // called for each particle, every frame, by emitter, if enabled
        public override void Update(Particle2D particle, float elapsed)
        {
            Vector2 v2 = particle.Movement;
            v2.X += m_Wind * elapsed;
            particle.Movement = v2;
        }
    }
}
