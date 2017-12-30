using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

// WARNING: This code is unused, and untested.

namespace Chapter05
{
    public class HslColor
    {
        protected double m_H = 0;
        public double H
        {
            get { return m_H; }
            set
            {
                m_H = Math.Max(Math.Min(value, 1.0), 0.0);
            }
        }

        protected double m_S = 0;
        public double S
        {
            get { return m_S; }
            set
            {
                m_S = Math.Max(Math.Min(value, 1.0), 0.0);
            }
        }

        protected double m_L = 0;
        public double L
        {
            get { return m_L; }
            set
            {
                m_L = Math.Max(Math.Min(value, 1.0), 0.0);
            }
        }

        protected double m_A = 0;
        public double A
        {
            get { return m_A; }
            set
            {
                m_A = Math.Max(Math.Min(value, 1.0), 0.0);
            }
        }

        public HslColor(double h, double s, double l, double a)
        {
            H = h;
            S = s;
            L = l;
            A = a;
        }

        public HslColor(float h, float s, float l, float a)
        {
            H = h;
            S = s;
            L = l;
            A = a;
        }

        public HslColor(Vector3 v3)
        {
            H = v3.X;
            S = v3.Y;
            L = v3.Z;
            A = 1.0;
        }

        public HslColor(Vector4 v4)
        {
            H = v4.X;
            S = v4.Y;
            L = v4.Z;
            A = v4.W;
        }

        public HslColor(Color rgba)
        {
            Vector4 v4 = rgba.ToVector4();

            double min = Math.Min(Math.Min(v4.X, v4.Y), v4.Z);
            double max = Math.Max(Math.Max(v4.X, v4.Y), v4.Z);

            L = (min + max) / 2.0;
            H = double.NaN;

            if (min == max)
            {
                S = 0;
                H = 0;
            }
            else if (L < 0.5)
            {
                S = (max - min) / (max + min);
            }
            else
            {
                S = (max - min) / (2.0 - max - min);
            }

            if (H == double.NaN)
            {
                if (v4.X == max)
                {
                    H = ((v4.Y - v4.Z) / (max - min)) / 6.0;
                }
                else if (v4.Y == max)
                {
                    H = (2.0 + (v4.Z - v4.X) / (max - min)) / 6.0;
                }
                else
                {
                    H = (4.0 + (v4.X - v4.Y) / (max - min)) / 6.0;
                }
            }
        }

        public Color ToColor()
        {
            double r = double.NaN;
            double g = double.NaN;
            double b = double.NaN;
            double a = A;

            if (S == 0)
            {
                r = g = b = (int)(L * 255.0);
            }
            else
            {
                double t1 = 0;
                double t2 = 0;
                double t3 = 0;

                if (L < 0.5)
                {
                    t2 = L * (1.0 + S);
                }
                else
                {
                    t2 = L + S - L * S;
                }

                t1 = 2.0 * L - t2;

                t3 = H + 1.0 / 3.0;
                t3 += (t3 < 0 ? 1.0 : 0.0);
                t3 -= (t3 > 1 ? 1.0 : 0.0);
                r = CalcColorComponent(t1, t2, t3);

                t3 = H;
                t3 += (t3 < 0 ? 1.0 : 0.0);
                t3 -= (t3 > 1 ? 1.0 : 0.0);
                g = CalcColorComponent(t1, t2, t3);

                t3 = H - 1.0 / 3.0;
                t3 += (t3 < 0 ? 1.0 : 0.0);
                t3 -= (t3 > 1 ? 1.0 : 0.0);
                b = CalcColorComponent(t1, t2, t3);
            }

            return new Color(
                (byte)(r * 255.0),
                (byte)(g * 255.0),
                (byte)(b * 255.0),
                (byte)(a * 255.0));
        }

        protected double CalcColorComponent(double t1, double t2, double t3)
        {
            double c = 0;

            if (6.0 * t3 < 1)
            {
                c = t1 + (t2 - t1) * 6.0 * t3;
            }
            else if (2.0 * t3 < 1)
            {
                c = t2;
            }
            else if (3.0 * t3 < 2)
            {
                c = t1 + (t2 - t1) * ((2.0 / 3.0) - t3) * 6.0;
            }
            else
            {
                c = t1;
            }

            return c;
        }

        public static Color GrayScale(Color color)
        {
            HslColor hsl = new HslColor(color);
            hsl.S = 0;
            return hsl.ToColor();
        }
    }
}
