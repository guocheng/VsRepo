using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DrawCurve
{
    // The quadratic curve is of the form P(t) = a + bt + ct^2
    class QuadraticHermiteCurve
    {
        private Point p0, p1;
        private Point tangent;
        private Point a,b,c;   //coefficient

        public QuadraticHermiteCurve(Point p0, Point p1, Point tangent)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.tangent = tangent;
        }

        public Point P0
        {
            get
            {
                return p0;
            }

            set
            {
                p0 = value;
            }
        }

        public Point P1
        {
            get
            {
                return p1;
            }

            set
            {
                p1 = value;
            }
        }

        public Point Tangent
        {
            get
            {
                return tangent;
            }

            set
            {
                tangent = value;
            }
        }


        // P(0) = a
        // P'(t) = b + 2ct
        // P'(0) = b
        // P(1) = a + b * 1 + c * 1^2
        // P(1) = a + b + c
        // P(1) = P(0) + P'(0) + c
        // c = P(1) - P(0) - P'(0)
        public  void CalcCoef()
        {
            a.X = p0.X;
            a.Y = p0.Y;

            //b.X = tangent - p0.X;
            //b.Y = tangent - p0.Y;

            b.X = tangent.X - p0.X;
            b.Y = tangent.Y - p0.Y;

            c.X = p1.X - p0.X - b.X;
            c.Y = p1.Y - p0.Y - b.Y;
        }

        public Point GetPoint(double t)
        {
            double x = a.X + b.X * t + c.X * Math.Pow(t, 2);
            double y = a.Y + b.Y  * t + c.Y * Math.Pow(t, 2);
            return new Point(x, y);
        }
    }
}
