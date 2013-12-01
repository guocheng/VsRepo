using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DrawCurve
{
    class InteractiveDot : Shape
    {
        public static readonly double SIZE = 20;
        public static readonly double ON_FOCUS_SIZE = 25;
        EllipseGeometry ellipse;
        private int id;


        public InteractiveDot(int id, SolidColorBrush color)
        {
            this.id = id;
            ellipse = new EllipseGeometry();
            ellipse.RadiusX = ellipse.RadiusY = SIZE;
            this.Fill = color;
        }

        public double X
        {
            get
            {
                return ellipse.Center.X;
            }

            set
            {
                ellipse.Center = new Point(value, ellipse.Center.Y);
            }
        }

        public double Y
        {
            get
            {
                return ellipse.Center.Y;
            }

            set
            {
                ellipse.Center = new Point(ellipse.Center.X, value);
            }
        }

        public int ID
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public void UpdatePosition(Point p)
        {
            ellipse.Center = p;
        }

        public void OnFocus()
        {
            ellipse.RadiusX = ellipse.RadiusY = ON_FOCUS_SIZE;
        }

        public void OnLeave()
        {
            ellipse.RadiusX = ellipse.RadiusY = SIZE;
        }

        protected override Geometry DefiningGeometry
        {
            get 
            {
                return ellipse;
            }
        }
    }
}
