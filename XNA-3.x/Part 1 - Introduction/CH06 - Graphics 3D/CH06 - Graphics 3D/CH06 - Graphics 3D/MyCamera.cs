using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace CH06___Graphics_3D
{
    public class MyCamera : My3DObject
    {
        protected Quaternion m_QuaternionRotation =
            Quaternion.CreateFromAxisAngle(Vector3.Forward, 0);

        public Quaternion QuaternionRotation
        {
            get { return m_QuaternionRotation; }
        }

        public Vector3 Up
        {
            get
            {
                return Vector3.Transform(
                    Vector3.Up,
                    Matrix.CreateFromQuaternion(m_QuaternionRotation));
            }
        }

        public Vector3 Right
        {
            get
            {
                return Vector3.Transform(
                    Vector3.Right,
                    Matrix.CreateFromQuaternion(m_QuaternionRotation));
            }
        }

        public Vector3 Forward
        {
            get
            {
                return Vector3.Transform(
                    Vector3.Forward,
                    Matrix.CreateFromQuaternion(m_QuaternionRotation));
            }
        }

        // units per second to move or strafe
        protected float m_MoveSpeed = 500.0f;
        public float MoveSpeed
        {
            get { return m_MoveSpeed; }
            set { m_MoveSpeed = value; }
        }

        // degrees per second to rotate, tilt, or roll
        protected float m_RotateSpeed = 36.0f;
        public float RotateSpeed
        {
            get { return m_RotateSpeed; }
            set { m_RotateSpeed = value; }
        }

        protected float m_AspectRatio = 640f / 480f;
        public float AspectRatio
        {
            get { return m_AspectRatio; }
            set { m_AspectRatio = value; }
        }

        protected float m_NearPlane = 1.0f;
        public float NearPlane
        {
            get { return m_NearPlane; }
            set { m_NearPlane = value; }
        }

        protected float m_FarPlane = 100000.0f;
        public float FarPlane
        {
            get { return m_FarPlane; }
            set { m_FarPlane = value; }
        }

        protected float m_FieldOfVision = 45.0f;
        public float FieldOfVision
        {
            get { return m_FieldOfVision; }
            set { m_FieldOfVision = value; }
        }


        // point camera down -Z
        public void ResetLookAt()
        {
            m_QuaternionRotation =
                Quaternion.CreateFromAxisAngle(Vector3.Forward, 0);
            QuaternionToEulerAngles(ref m_QuaternionRotation, ref m_Rotation);
        }

        // point camera at the specified object
        public void LookAt(My3DObject obj)
        {
            LookAt(obj.Location, Vector3.Up);
        }

        // point camera at the specified point
        public void LookAt(Vector3 point, Vector3 up)
        {
            // rather than calculating the quaternion by hand, I'll
            // cheat by using the CreateLookAt function to generate
            // a rotation matrix, which I can then use to generate
            // my quaternion. The matrix that CreateLookAt generates
            // contains more than just rotation data, so we'll use
            // the Transpose method to clean it up first.
            Matrix rotation = Matrix.CreateLookAt(Location, point, Vector3.Up);
            rotation = Matrix.Transpose(rotation);
            m_QuaternionRotation = Quaternion.CreateFromRotationMatrix(rotation);

            // our helper to keep the Euler angles in sync
            QuaternionToEulerAngles(ref m_QuaternionRotation, ref m_Rotation);
        }

        // convert angle vector to a quat
        protected void EulerAnglesToQuaternion(ref Vector3 angles, out Quaternion rotation)
        {
            EulerAnglesToQuaternion(angles.X, angles.Y, angles.Z, out rotation);
        }

        // helper method to convert Euler angles to a quaternion
        protected void EulerAnglesToQuaternion(float pitch, float yaw, float roll, out Quaternion rotation)
        {
            // deal with half angles, convert to radians
            pitch = MathHelper.ToRadians(pitch) / 2;
            yaw = MathHelper.ToRadians(yaw) / 2;
            roll = MathHelper.ToRadians(roll) / 2;

            // go ahead and calc our trig functions [cosine]
            float cosPitch = (float)Math.Cos(pitch);
            float cosYaw = (float)Math.Cos(yaw);
            float cosRoll = (float)Math.Cos(roll);

            // go ahead and calc our trig functions [sine]
            float sinPitch = (float)Math.Sin(pitch);
            float sinYaw = (float)Math.Sin(yaw);
            float sinRoll = (float)Math.Sin(roll);

            // quaternion voodoo, construct a quat from our angles
            Quaternion quat = new Quaternion(
                cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw,
                sinRoll * cosPitch * cosYaw - cosRoll * sinPitch * sinYaw,
                cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw,
                cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw);
            rotation = quat;
        }

        // determine Euler angles, given a quat
        protected void QuaternionToEulerAngles(
            ref Quaternion rotation, ref Vector3 angles)
        {
            QuaternionToEulerAngles(
                ref rotation, ref angles.X, ref angles.Y, ref angles.Z);
        }

        // helper method to convert quaternions to Euler angles
        protected void QuaternionToEulerAngles(
            ref Quaternion rotation, ref float pitch,
            ref float yaw, ref float roll)
        {
            // following code assumes normalized quat
            rotation.Normalize();

            // temp variables to save some typing
            float q0 = rotation.X;
            float q1 = rotation.Y;
            float q2 = rotation.Z;
            float q3 = rotation.W;

            // test for naughty cases (anomalies at north and south poles)
            float check = q0 * q1 + q2 * q3;

            if (Math.Abs(check) >= 0.499)
            {
                // special case to avoid unreliable data near poles
                check /= Math.Abs(check); // determine sign (1 or -1)

                // calculate angles for special case
                pitch = check * (float)Math.PI / 2.0f;
                yaw = check * 2.0f * (float)Math.Atan(q0 * q3);
                roll = check * 0.0f;
            }
            else
            {
                // looks good; calculate angles for typical case
                pitch = (float)Math.Asin(-2 * (q1 * q3 - q0 * q2));
                yaw = (float)Math.Atan2(2 * (q1 * q2 + q0 * q3),
                    q0 * q0 + q1 * q1 - q2 * q2 - q3 * q3);
                roll = (float)Math.Atan2(2 * (q2 * q3 + q0 * q1),
                    q0 * q0 - q1 * q1 - q2 * q2 + q3 * q3);
            }

            // we're done; convert back to degrees
            pitch = MathHelper.ToDegrees(pitch);
            yaw = MathHelper.ToDegrees(yaw);
            roll = MathHelper.ToDegrees(roll);
        }

        public void ProcessInput(
            GamePadState pad1, KeyboardState key1, float elapsed)
        {
            // dolly camera (move forward or back)
            if (pad1.ThumbSticks.Left.Y != 0)
            {
                Location -= m_MoveSpeed * Forward *
                    -pad1.ThumbSticks.Left.Y * elapsed;
            }
            else if (key1.IsKeyDown(Keys.W))
            {
                Location += m_MoveSpeed * Forward * elapsed;
            }
            else if (key1.IsKeyDown(Keys.S))
            {
                Location -= m_MoveSpeed * Forward * elapsed;
            }

            // strafe left or right (keep looking ahead)
            if (pad1.ThumbSticks.Left.X != 0)
            {
                Location += m_MoveSpeed * Right *
                    pad1.ThumbSticks.Left.X * elapsed;
            }
            else if (key1.IsKeyDown(Keys.A))
            {
                Location -= m_MoveSpeed * Right * elapsed;
            }
            else if (key1.IsKeyDown(Keys.D))
            {
                Location += m_MoveSpeed * Right * elapsed;
            }

            // look left or right
            float dYaw = 0.0f;
            if (pad1.ThumbSticks.Right.X != 0)
            {
                dYaw -= m_RotateSpeed * pad1.ThumbSticks.Right.X;
            }
            else if (key1.IsKeyDown(Keys.Left))
            {
                dYaw = m_RotateSpeed;
            }
            else if (key1.IsKeyDown(Keys.Right))
            {
                dYaw = -m_RotateSpeed;
            }

            // look up or down
            float dPitch = 0.0f;
            if (pad1.ThumbSticks.Right.Y != 0)
            {
                dPitch = m_RotateSpeed * pad1.ThumbSticks.Right.Y;
            }
            else if (key1.IsKeyDown(Keys.Up))
            {
                dPitch = m_RotateSpeed;
            }
            else if (key1.IsKeyDown(Keys.Down))
            {
                dPitch = -m_RotateSpeed;
            }

            // roll camera
            float dRoll = 0.0f;
            if (pad1.Triggers.Left > 0)
            {
                dRoll = -m_RotateSpeed * pad1.Triggers.Left;
            }
            else if (pad1.Triggers.Right > 0)
            {
                dRoll = m_RotateSpeed * pad1.Triggers.Right;
            }
            else if (key1.IsKeyDown(Keys.D2))
            {
                dRoll = m_RotateSpeed;
            }
            else if (key1.IsKeyDown(Keys.D1))
            {
                dRoll = -m_RotateSpeed;
            }

            // did player update camera position or direction?
            if (dPitch != 0.0f || dYaw != 0.0f || dRoll != 0.0f)
            {
                // perform rotations relative to camera's local axis
                m_QuaternionRotation *= Quaternion.CreateFromAxisAngle(
                    Vector3.Right, MathHelper.ToRadians(dPitch * elapsed));
                m_QuaternionRotation *= Quaternion.CreateFromAxisAngle(
                    Vector3.Up, MathHelper.ToRadians(dYaw * elapsed));
                m_QuaternionRotation *= Quaternion.CreateFromAxisAngle(
                    Vector3.Forward, MathHelper.ToRadians(dRoll * elapsed));

                // update our Euler running totals
                QuaternionToEulerAngles(
                    ref m_QuaternionRotation, ref m_Rotation);
            }
        }

        // helper to keep Euler angles in sync
        public sealed override void RotationUpdated()
        {
            EulerAnglesToQuaternion(ref m_Rotation, out m_QuaternionRotation);
        }

        protected Matrix m_ProjectionMatrix = Matrix.Identity;
        public Matrix ProjectionMatrix
        {
            get
            {
                Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(FieldOfVision),
                    AspectRatio,
                    NearPlane,
                    FarPlane,
                    out m_ProjectionMatrix);
                return m_ProjectionMatrix;
            }
        }

        protected Vector3 m_TempLookAt = Vector3.Forward;
        public Matrix ViewMatrix
        {
            get
            {
                return Matrix.CreateLookAt(Location, Location + Forward, Up);
            }
        }
    }
}
