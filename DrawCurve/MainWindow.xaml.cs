using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrawCurve
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SolidColorBrush smoke = new SolidColorBrush(Color.FromArgb(128, 248, 248, 248));
        private SolidColorBrush solidSmoke = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248));
        private SolidColorBrush blue = new SolidColorBrush(Color.FromArgb(250, 0, 122, 204));
        private SolidColorBrush red = new SolidColorBrush(Color.FromArgb(250, 240, 0, 0));

        private Point Origin = new Point(0.5, 0.5);
        private ScaleTransform enlarge = new ScaleTransform();
        private ScaleTransform shrink = new ScaleTransform();
        private InteractiveDot previewDot, selectedDot;

        private QuadraticHermiteCurve qhc;
        private PointCollection points = new PointCollection();
        private PointCollection lineSegments = new PointCollection();

        private Polyline curve = new Polyline();
        private List<InteractiveDot> dots;
        private int newDotId = -1;
        private bool isDragging = false;
        private InteractiveDot tangent1;
        private Line tangentLine;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.Width = 800;
            this.Height = 800;

            enlarge.ScaleX = enlarge.ScaleY = 1.5d;
            shrink.ScaleX = shrink.ScaleY = 1d;

            previewDot = new InteractiveDot(-1, smoke);

            this.PreviewMouseDown += MainWindow_PreviewMouseDown;
            this.PreviewMouseUp += MainWindow_PreviewMouseUp;
            this.PreviewMouseMove += MainWindow_PreviewMouseMove;
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;

            tangent1 = new InteractiveDot(100, red);
            tangent1.MouseEnter += e_MouseEnter;
            tangent1.MouseLeave += e_MouseLeave;
            tangent1.MouseDown += dot_MouseDown;
            tangent1.MouseUp += dot_MouseUp;
            tangent1.X = tangent1.Y = 150d;

            dots = new List<InteractiveDot>();
            qhc = new QuadraticHermiteCurve(new Point(0, 0), new Point(0, 0), this.getTangentPoint());

            tangentLine = new Line();
        }

        void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.MainCanvas.Children.Clear();
                this.Close();
            }
        }

        void MainWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(this.MainCanvas);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (selectedDot != null)
                {
                    this.UpdateDotPosition(p, selectedDot);

                    if (selectedDot.ID != 100)
                    {   
                        this.UpdateCurve(p, selectedDot.ID);
                    }
                    else
                    {
                        this.DrawTangentLine(qhc.P0, new Point(selectedDot.X, selectedDot.Y));
                        this.CalcAndDrawCurve(qhc, points);
                    }
                }

            }
        }

        void MainWindow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            
            Point p = e.GetPosition(this.MainCanvas);
            if (e.ChangedButton.ToString() == "Left")
            {
                if (selectedDot == null)
                {
                    //this.MainCanvas.Children.Remove(previewDot);
                    
                    this.AddNewDot(p);
                }
                else
                {
                    //UpdateCurve(p, selectedDot.ID);
                }
            }
        }

        void UpdateCurve(Point p, int id)
        {
            points[id] = p;
            CalcAndDrawCurve(qhc, points);
        }

        void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (points.Count > 1)
                {
                    CalcAndDrawCurve(qhc, points);
                }
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                this.ClearScreen();
            }
        }

        void DrawTangentLine(Point p1, Point p2)
        {
            this.MainCanvas.Children.Remove(tangent1);
            this.MainCanvas.Children.Remove(tangentLine);

            this.MainCanvas.Children.Add(tangent1);
            Canvas.SetLeft(tangent1, tangent1.X - tangent1.Width / 2d);
            Canvas.SetTop(tangent1, tangent1.Y - tangent1.Height / 2d);

            
            tangentLine.X1 = p1.X;
            tangentLine.Y1 = p1.Y;
            tangentLine.X2 = p2.X;
            tangentLine.Y2 = p2.Y;

            tangentLine.StrokeThickness = 10;
            tangentLine.Stroke = smoke;
            this.MainCanvas.Children.Add(tangentLine);
            Canvas.SetZIndex(tangentLine, -1);
        }

        Point getTangentPoint()
        {
            return new Point(tangent1.X, tangent1.Y);
        }

        void CalcAndDrawCurve(QuadraticHermiteCurve qhc, PointCollection pointList)
        {
            lineSegments.Clear();
            this.MainCanvas.Children.Remove(curve);

            qhc.P0 = points[0];
            qhc.P1 = points[1];
            qhc.Tangent = getTangentPoint(); 
            qhc.CalcCoef();

            double delta = 0.02d;

            lineSegments.Add(pointList[0]);

            for (double i = delta; i < 1; i += delta)
            {
                lineSegments.Add(qhc.GetPoint(i));
            }

            lineSegments.Add(pointList[1]);

            curve.Points = lineSegments;
            curve.Stroke = blue;
            curve.StrokeThickness = 15;


            this.MainCanvas.Children.Add(curve);
            Canvas.SetZIndex(curve, -1);

            DrawTangentLine(qhc.P0, this.getTangentPoint());
        }

        void ClearScreen()
        {
            this.MainCanvas.Children.Clear();
            points.Clear();
            dots.Clear();
        }

        void AddNewDot(Point p)
        {
            InteractiveDot dot = new InteractiveDot(dots.Count, smoke);

            dot.MouseEnter += e_MouseEnter;
            dot.MouseLeave += e_MouseLeave;
            dot.MouseDown += dot_MouseDown;
            dot.MouseUp += dot_MouseUp;



            dots.Add(dot);
            points.Add(p);
            newDotId = dot.ID;

            this.MainCanvas.Children.Add(dot);
            this.UpdateDotPosition(p, dot);
        }




        void UpdateDotPosition(Point p, InteractiveDot e)
        {
            if (e != null)
            {
                if (e.ID == 100)
                {
                    e.UpdatePosition(p);
                }
                else
                {
                    points[e.ID] = p;
                    e.UpdatePosition(p);
                }

                
            }
        }

        void e_MouseLeave(object sender, MouseEventArgs e)
        {
            InteractiveDot dot = sender as InteractiveDot;

            if (dot != null)
            {
                if (!isDragging)
                {
                    dot.Fill = smoke;
                }
            }
        }

        void e_MouseEnter(object sender, MouseEventArgs e)
        {

            InteractiveDot dot = sender as InteractiveDot;

            if (dot != null)
            {
                if (selectedDot != null)
                {
                    if (selectedDot.ID == dot.ID)
                    {
                        dot.Fill = blue;
                    }
                }
                else
                {
                    dot.Fill = blue;
                }
            }
            
            
        }


        void dot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!isDragging)
                {
                    InteractiveDot dot = sender as InteractiveDot;

                    if (dot != null)
                    {
                        selectedDot = dot;
                        isDragging = true;
                    }
                }
            }
        }

        void dot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton.ToString() == "Left")
            {
                selectedDot = null;
                isDragging = false;
            }
        }
    }
}
