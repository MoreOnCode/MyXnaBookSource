// My3DObject.cs
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Chapter06
{
    public abstract class My3DObject
    {
        // location of this object in the world
        protected Vector3 m_Location = Vector3.Zero;
        public Vector3 Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        // move object, relative to its current location
        public void Move(float x, float y, float z)
        {
            m_Location.X += x;
            m_Location.Y += y;
            m_Location.Z += z;
        }

        // place object in an absolute location within the world
        public void MoveTo(float x, float y, float z)
        {
            m_Location = new Vector3(x, y, z);
        }

        // rotations (for each axis)
        protected Vector3 m_Rotation = Vector3.Zero;
        public Vector3 Rotation
        {
            get { return m_Rotation; }
            set
            {
                m_Rotation = value;
                RotationUpdated();
            }
        }

        // rotate object, relative to current rotation
        public void Rotate(float pitch, float yaw, float roll)
        {
            m_Rotation.X += pitch;
            m_Rotation.Y += yaw;
            m_Rotation.Z += roll;

            // perform post-processing
            RotationUpdated();
        }

        // set absolute rotation
        public void RotateTo(float pitch, float yaw, float roll)
        {
            m_Rotation = new Vector3(pitch, yaw, roll);

            // perform post-processing
            RotationUpdated();
        }

        // pseudo event, child classes can override to receive notification
        public virtual void RotationUpdated()
        {
            // override this method to handle rotation events
        }
    }
}
