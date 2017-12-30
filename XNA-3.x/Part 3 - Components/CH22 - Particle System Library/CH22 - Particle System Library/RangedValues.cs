using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Codetopia.Graphics.ParticleSystem
{
    // simple class to hold min / max values and generate random values 
    // within those bounds. b ase class uses generics (templates)
    public abstract class RangedValue<T>
    {
        // default constructor, .NET type-specific initial values
        public RangedValue()
        {
        }

        // constructor to specify min and max
        public RangedValue(T min, T max)
        {
            Min = min;
            Max = max;
        }

        // the min value
        protected T m_Min;
        public T Min
        {
            get { return m_Min; }
            set { m_Min = value; }
        }

        // the max value
        protected T m_Max;
        public T Max
        {
            get { return m_Max; }
            set { m_Max = value; }
        }

        // random number generator
        protected Random m_rand = new Random();

        // generate a random value between min and max, inclusive
        public abstract T RandomValue();

        // get the last random value
        protected T m_Value;
        public T Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
    }

    // type-specific subclass
    public class RangedByte : RangedValue<byte>
    {
        public RangedByte() : base() { }
        public RangedByte(byte min, byte max) : base(min, max) { }

        // generate a random value between min and max, inclusive
        public override byte RandomValue()
        {
            Value = (byte)MathHelper.Lerp(
                (float)Min,
                (float)Max,
                (float)m_rand.NextDouble());
            return Value;

        }
    }

    // type-specific subclass
    public class RangedInt : RangedValue<int>
    {
        public RangedInt() : base() { }
        public RangedInt(int min, int max) : base(min, max) { }

        // generate a random value between min and max, inclusive
        public override int RandomValue()
        {
            // linear interpolation between min and max based on random number
            Value = (int)MathHelper.Lerp(
                (float)Min,
                (float)Max,
                (float)m_rand.NextDouble());
            return Value;

        }
    }

    // type-specific subclass
    public class RangedLong : RangedValue<long>
    {
        public RangedLong() : base() { }
        public RangedLong(long min, long max) : base(min, max) { }

        // generate a random value between min and max, inclusive
        public override long RandomValue()
        {
            // linear interpolation between min and max based on random number
            Value = (long)MathHelper.Lerp(
                (float)Min,
                (float)Max,
                (float)m_rand.NextDouble());
            return Value;

        }
    }

    // type-specific subclass
    public class RangedFloat : RangedValue<float>
    {
        public RangedFloat() : base() { }
        public RangedFloat(float min, float max) : base(min, max) { }

        // generate a random value between min and max, inclusive
        public override float RandomValue()
        {
            // linear interpolation between min and max based on random number
            Value = (float)MathHelper.Lerp(
                (float)Min,
                (float)Max,
                (float)m_rand.NextDouble());
            return Value;

        }
    }

    // type-specific subclass
    public class RangedDouble : RangedValue<double>
    {
        public RangedDouble() : base() { }
        public RangedDouble(double min, double max) : base(min, max) { }

        // generate a random value between min and max, inclusive
        public override double RandomValue()
        {
            // linear interpolation between min and max based on random number
            Value = (double)MathHelper.Lerp(
                (float)Min,
                (float)Max,
                (float)m_rand.NextDouble());
            return Value;

        }
    }

    // type-specific subclass
    public class RangedVector2 : RangedValue<Vector2>
    {
        public RangedVector2() : base() { }
        public RangedVector2(Vector2 min, Vector2 max) : base(min, max) { }

        // generate a random value between min and max, inclusive
        public override Vector2 RandomValue()
        {
            // linear interpolation between min and max based on random number
            m_Value.X = (float)MathHelper.Lerp(
                (float)Min.X,
                (float)Max.X,
                (float)m_rand.NextDouble());
            m_Value.Y = (float)MathHelper.Lerp(
                (float)Min.Y,
                (float)Max.Y,
                (float)m_rand.NextDouble());
            return Value;

        }

        // determine min and max values from a Rectangle
        public static RangedVector2 FromRectangle(Rectangle rect)
        {
            Vector2 v2Min = Vector2.Zero;
            v2Min.X = rect.Left;
            v2Min.Y = rect.Top;

            Vector2 v2Max = Vector2.Zero;
            v2Max.X = rect.Left + rect.Width;
            v2Max.Y = rect.Top + rect.Height;

            RangedVector2 rv2 = new RangedVector2();
            rv2.Min = v2Min;
            rv2.Max = v2Max;

            return rv2;
        }
    }

    // type-specific subclass
    public class RangedVector3 : RangedValue<Vector3>
    {
        public RangedVector3() : base() { }
        public RangedVector3(Vector3 min, Vector3 max) : base(min, max) { }

        // generate a random value between min and max, inclusive
        public override Vector3 RandomValue()
        {
            // linear interpolation between min and max based on random number
            m_Value.X = (float)MathHelper.Lerp(
                (float)Min.X,
                (float)Max.X,
                (float)m_rand.NextDouble());
            m_Value.Y = (float)MathHelper.Lerp(
                (float)Min.Y,
                (float)Max.Y,
                (float)m_rand.NextDouble());
            m_Value.Z = (float)MathHelper.Lerp(
                (float)Min.Z,
                (float)Max.Z,
                (float)m_rand.NextDouble());
            return Value;

        }
    }

    // type-specific subclass
    public class RangedVector4 : RangedValue<Vector4>
    {
        public RangedVector4() : base() { }
        public RangedVector4(Vector4 min, Vector4 max) : base(min, max) { }

        // generate a random value between min and max, inclusive
        public override Vector4 RandomValue()
        {
            // linear interpolation between min and max based on random number
            m_Value.X = (float)MathHelper.Lerp(
                (float)Min.X,
                (float)Max.X,
                (float)m_rand.NextDouble());
            m_Value.Y = (float)MathHelper.Lerp(
                (float)Min.Y,
                (float)Max.Y,
                (float)m_rand.NextDouble());
            m_Value.Z = (float)MathHelper.Lerp(
                (float)Min.Z,
                (float)Max.Z,
                (float)m_rand.NextDouble());
            m_Value.W = (float)MathHelper.Lerp(
                (float)Min.W,
                (float)Max.W,
                (float)m_rand.NextDouble());
            return Value;
        }

        // determine min and max values from colors
        public static RangedVector4 FromColors(Color min, Color max)
        {
            Vector4 v4Min = Vector4.Zero;
            v4Min.X = (float)min.R / (float)byte.MaxValue;
            v4Min.Y = (float)min.G / (float)byte.MaxValue;
            v4Min.Z = (float)min.B / (float)byte.MaxValue;
            v4Min.W = (float)min.A / (float)byte.MaxValue;

            Vector4 v4Max = Vector4.Zero;
            v4Max.X = (float)max.R / (float)byte.MaxValue;
            v4Max.Y = (float)max.G / (float)byte.MaxValue;
            v4Max.Z = (float)max.B / (float)byte.MaxValue;
            v4Max.W = (float)max.A / (float)byte.MaxValue;

            RangedVector4 rv4 = new RangedVector4();
            rv4.Min = v4Min;
            rv4.Max = v4Max;

            return rv4;
        }
    }
}
