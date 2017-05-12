using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace NetworkSimulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _vertex;
        private Graph _graph;
        private Dictionary<int, Point> _possitions;
        private Dictionary<int, Ellipse> _ellipses;
        private bool mouseIsDown;
        private int _currentVertex;
        private Line line;

        public MainWindow()
        {
            InitializeComponent();
            _vertex = 0;
            _possitions = new Dictionary<int, Point>();
            _ellipses = new Dictionary<int, Ellipse>();
            _graph = new Graph();
            //_graph.AddVertex(0);
            //_graph.AddVertex(1);
            //_graph.AddVertex(2);
            //_graph.AddVertex(3);
            //_graph.AddVertex(4);
            //_graph.AddVertex(5);
            //_graph.AddVertex(6);


            /*_graph.AddNeighboor(0, 1, 5);
            _graph.AddNeighboor(0, 2, 31);
            _graph.AddNeighboor(0, 3, 20);
            _graph.AddNeighboor(1, 4, 7);
            _graph.AddNeighboor(2, 4, 10);
            _graph.AddNeighboor(2, 5, 1);
            _graph.AddNeighboor(3, 5, 5);
            _graph.AddNeighboor(4, 6, 40);
            _graph.AddNeighboor(5, 6, 14);

            Console.WriteLine(_graph.PrintGraph());

            foreach (var vertex in _graph.vertexes)
            {
                foreach (var n in vertex.Value.neighboors)
                {
                    Console.WriteLine("Key: " + vertex.Key.ToString()+" neighboor: " + n.Key.value + " weight: " + n.Value.ToString());
                }
                Console.WriteLine();
            }

            Console.WriteLine("Shortest path by weight:");
            Console.WriteLine("From: 0 to: 6 " + _graph.FindShortestPathByWeight(0, 6));
            Console.WriteLine("From: 0 to: 5 " + _graph.FindShortestPathByWeight(0, 5));
            Console.WriteLine("From: 0 to: 2 " + _graph.FindShortestPathByWeight(0, 2));
            Console.WriteLine("From: 0 to: 4 " + _graph.FindShortestPathByWeight(0, 4));
            Console.WriteLine("From: 0 to: 1 " + _graph.FindShortestPathByWeight(0, 1));
            Console.WriteLine();

            Console.WriteLine("Shortest path:");
            Console.WriteLine("From: 0 to: 6 " + _graph.FindShortestPath(0, 6));
            Console.WriteLine("From: 0 to: 5 " + _graph.FindShortestPath(0, 5));
            Console.WriteLine("From: 0 to: 2 " + _graph.FindShortestPath(0, 2));
            Console.WriteLine("From: 0 to: 4 " + _graph.FindShortestPath(0, 4));
            Console.WriteLine("From: 0 to: 1 " + _graph.FindShortestPath(0, 1));
            */

        }

        private void graphBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine("GRAPH event");
            bool isPossition = false;
            Dictionary<int, Ellipse> currentEllipse = new Dictionary<int, Ellipse>();
            foreach (var p in _possitions)
            {
                //Console.WriteLine("X: " + (e.GetPosition(this).X - p.Value.X) + " Y: " + (e.GetPosition(this).Y - p.Value.Y));
                if (((e.GetPosition(this).X - p.Value.X <  32 && e.GetPosition(this).X - p.Value.X > -1)) &&
                    ((e.GetPosition(this).Y - p.Value.Y <  31 && e.GetPosition(this).Y - p.Value.Y > -1)))
                {
                    isPossition = true;
                    currentEllipse[p.Key] = _ellipses[p.Key];
                    _currentVertex = p.Key;
                    Console.WriteLine("IS possition");
                }
            }
            if (!isPossition)
            {
                Console.WriteLine("NEW");
                Point possition = e.GetPosition(this);
                _possitions[_vertex] = possition;
                Ellipse circle = new Ellipse { Height = 30, Width = 30, StrokeThickness = 2, Stroke = Brushes.Black };
                _ellipses[_vertex] = circle;
                TextBlock txt = new TextBlock { Height = 20, Width = 20, FontSize = 15 };
                txt.Text = " " + _vertex;
                txt.HorizontalAlignment = HorizontalAlignment.Center;
                txt.VerticalAlignment = VerticalAlignment.Center;

                Grid grid = new Grid();
                grid.Children.Add(circle);
                grid.Children.Add(txt);
                Canvas.SetLeft(grid, possition.X);
                Canvas.SetTop(grid, possition.Y);

                graphBorder.Children.Add(grid);


                _graph.AddVertex(_vertex);
                _vertex++;
            }
            else
                ellipse_MouseLeftButtonUp(currentEllipse, sender, e);

        }

        private void ellipse_MouseLeftButtonUp(Dictionary<int, Ellipse> ellipse, object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("ELLIPSE event");
            //Line line = new Line();
            //graphBorder.MouseMove += new MouseEventArgs(ellipse_MouseMove);
            mouseIsDown = true;
        }

        private void graphBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                graphBorder.Children.Remove(line);
                Point p = e.GetPosition(this);
                Point point = _possitions[_currentVertex];

                line = new Line();
                line.Visibility = Visibility.Visible;
                line.StrokeThickness = 2;
                line.Stroke = Brushes.Black;
                line.X1 = point.X+15;
                line.X2 = p.X;
                line.Y1 = point.Y+15;
                line.Y2 = p.Y;
                line.MouseLeftButtonUp += ellipse_MouseLeftButtonUp;

                graphBorder.Children.Add(line);
            }
        }

        private void ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseIsDown)
            {
                mouseIsDown = false;
                line = new Line();
                foreach (var ellipse in _ellipses)
                {
                    Canvas.SetLeft(ellipse.Value, _possitions[ellipse.Key].X);
                    Canvas.SetTop(ellipse.Value, _possitions[ellipse.Key].Y);
                }
            }
        }
    }
}
