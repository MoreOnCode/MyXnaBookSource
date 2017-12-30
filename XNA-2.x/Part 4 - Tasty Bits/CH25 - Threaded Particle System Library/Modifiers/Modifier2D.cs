// Modifier2D.cs
using System;
using System.Collections.Generic;
using System.Text;

namespace Codetopia.Graphics.ParticleSystem
{
    // update a particle based on custom code
    // used by emitter
    public abstract class Modifier2D
    {
        // only update active modifiers
        protected bool m_Enabled = true;
        public bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        // custom code goes here, in derived class
        // called for each particle, every frame, by emitter, if enabled
        public abstract void Update(Particle2D particle, float elapsed);
    }
}
