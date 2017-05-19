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
        private Dictionary<int, Grid> _grid;
        private bool mouseIsDown;
        private int _fromVertex;
        private int _toVertex;
        private Line _line;
        private List<VertexPath> _text;

        public MainWindow()
        {
            InitializeComponent();
            _vertex = 0;
            _possitions = new Dictionary<int, Point>();
            _ellipses = new Dictionary<int, Ellipse>();
            _grid = new Dictionary<int, Grid>();
            _text = new List<VertexPath>();
            _graph = new Graph();
            _line = new Line();

            AddHosts(30, 160);
            AddHosts(450, 160);
            from.Text = "0";
            to.Text = "1";

            /*
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

        public void AddHosts(int x, int y)
        {
                Point p = new Point() { X = x, Y = y };
                _possitions[_vertex] = p;

                Ellipse circle = new Ellipse { Height = 30, Width = 30, StrokeThickness = 2, Stroke = Brushes.Black, };
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Colors.LightGreen;
                circle.Fill = mySolidColorBrush;
                _ellipses[_vertex] = circle;

                TextBlock txt = new TextBlock { Height = 20, Width = 20, FontSize = 15 };
                txt.Text = " " + _vertex;
                txt.HorizontalAlignment = HorizontalAlignment.Center;
                txt.VerticalAlignment = VerticalAlignment.Center;

                Grid grid = new Grid();
                grid.Children.Add(circle);
                grid.Children.Add(txt);
                _grid[_vertex] = grid;
                Canvas.SetLeft(grid, x);
                Canvas.SetTop(grid, y);

                graphBorder.Children.Add(grid);

                _graph.AddVertex(_vertex);
                _vertex++;
        }

        private void graphBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool isPossition = false;
            foreach (var p in _possitions)
            {
                if (((e.GetPosition(this).X - p.Value.X <  40 && e.GetPosition(this).X - p.Value.X > -10)) &&
                    ((e.GetPosition(this).Y - p.Value.Y <  40 && e.GetPosition(this).Y - p.Value.Y > -10)))
                {
                    isPossition = true;
                    _fromVertex = p.Key;
                }
            }
            if (!isPossition)
            {
                Point possition = e.GetPosition(this);
                Point p = new Point() { X = possition.X - 15, Y = possition.Y - 15 };
                _possitions[_vertex] = p;

                Ellipse circle = new Ellipse { Height = 30, Width = 30, StrokeThickness = 2, Stroke = Brushes.Black, };
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 255);
                circle.Fill = mySolidColorBrush;
                _ellipses[_vertex] = circle;

                TextBlock txt = new TextBlock { Height = 20, Width = 20, FontSize = 15 };
                txt.Text = " " + _vertex;
                txt.HorizontalAlignment = HorizontalAlignment.Center;
                txt.VerticalAlignment = VerticalAlignment.Center;

                Grid grid = new Grid();
                grid.Children.Add(circle);
                grid.Children.Add(txt);
                _grid[_vertex] = grid;
                Canvas.SetLeft(grid, possition.X - 15);
                Canvas.SetTop(grid, possition.Y - 15);

                graphBorder.Children.Add(grid);

                _graph.AddVertex(_vertex);
                _vertex++;
            }
            else
                ellipse_MouseLeftButtonUp(sender, e);

        }

        private void ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseIsDown = true;
        }

        private void graphBorder_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            int vertex = -1;
            foreach (var p in _possitions)
            {

                if (((e.GetPosition(this).X - p.Value.X < 32 && e.GetPosition(this).X - p.Value.X > -1)) &&
                    ((e.GetPosition(this).Y - p.Value.Y < 31 && e.GetPosition(this).Y - p.Value.Y > -1))                    )
                {
                    if(!(p.Key == 0 || p.Key == 1))
                        vertex = p.Key;
                }
            }
            if (vertex >= 0)
            {
                graphBorder.Children.Remove(_grid[vertex]);
                _possitions.Remove(vertex);

                List<VertexPath> textBox = new List<VertexPath>();
                foreach (var t in _text)
                {
                    if (t.from == vertex || t.to == vertex)
                    {
                        graphBorder.Children.Remove(t.text);
                        graphBorder.Children.Remove(t.line);
                        textBox.Remove(t);
                    }
                    else
                        textBox.Add(t);
                }
                _text.Clear();
                _text = textBox;
                _graph.RemoveVertex(vertex);
            }

        }

        private void graphBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                graphBorder.Children.Remove(_line);
                Point p = e.GetPosition(this);
                Point point = _possitions[_fromVertex];

                _line.Visibility = Visibility.Visible;
                _line.StrokeThickness = 2;
                _line.Stroke = Brushes.Black;
                _line.X1 = point.X+15;
                _line.X2 = p.X;
                _line.Y1 = point.Y+15;
                _line.Y2 = p.Y;
                _line.MouseLeftButtonUp += line_MouseLeftButtonUp;

                graphBorder.Children.Add(_line);
            }
        }

        private void line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseIsDown)
            {
                mouseIsDown = false;
                graphBorder.Children.Remove(_line);
                _toVertex = -1;
                foreach (var p in _possitions)
                {
                    if (((e.GetPosition(this).X - p.Value.X < 32 && e.GetPosition(this).X - p.Value.X > -1)) &&
                    ((e.GetPosition(this).Y - p.Value.Y < 31 && e.GetPosition(this).Y - p.Value.Y > -1)))
                    {
                        if(!(_fromVertex == p.Key))
                            _toVertex = p.Key;
                    }
                }
                foreach (var text in _text)
                {
                    if((_fromVertex == text.from && _toVertex == text.to) || (_fromVertex == text.to && _toVertex == text.from))
                    {
                        _toVertex = -2;
                    }
                }
                if (_toVertex >= 0)
                {
                    graphBorder.Children.Remove(_grid[_fromVertex]);
                    graphBorder.Children.Remove(_grid[_toVertex]);

                    graphBorder.Children.Add(_line);
                    graphBorder.Children.Add(_grid[_fromVertex]);
                    graphBorder.Children.Add(_grid[_toVertex]);

                    TextBox text = new TextBox { Width = 24, Height = 20, Text = "1"};
                    text.KeyUp += textBox_KeyUp;
                    text.MouseDoubleClick += textBox_MouseDoubleClick;
                    double x = ((_line.X2 + _line.X1) / 2) -5;
                    double y = ((_line.Y2 + _line.Y1) / 2) - 5;
                    Canvas.SetLeft(text, x);
                    Canvas.SetTop(text, y);
                    graphBorder.Children.Add(text);
                    _text.Add(new VertexPath(_fromVertex, _toVertex, text, _line));
                    _graph.AddNeighboor(_fromVertex, _toVertex);

                    _line = null;
                    _line = new Line();
                }
                
            }

        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            int txt = -1;
            foreach (var text in _text)
                if ((sender as TextBox).Equals(text.text))
                    if (Int32.TryParse(text.text.Text, out txt))
                    {
                        _graph.ChangeNeighboorWeight(text.from, text.to, txt);
                        break;
                    }
                    else
                    {
                        text.text.Text = _graph.GetWeightByPath(text.from, text.to).ToString();
                    }
        }

        private void textBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VertexPath txt = null;
            foreach (var text in _text)
            {
                if ((sender as TextBox).Equals(text.text))
                {
                    graphBorder.Children.Remove(text.line);
                    graphBorder.Children.Remove(text.text);
                    txt = text;
                    _graph.RemoveNeighboor(text.from, text.to);
                    break;
                }
            }
            _text.Remove(txt);

        }

        private void Print()
        {
            _graph.PrintGraph();

            foreach (var vertex in _graph.vertexes)
            {
                foreach (var n in vertex.Value.neighboors)
                {
                    Console.WriteLine("Key: " + vertex.Key.ToString() + " neighboor: " + n.Key.value + " weight: " + n.Value.ToString());
                }
                Console.WriteLine();
            }
        }


        /*private void print_Click(object sender, RoutedEventArgs e)
        {
            Print();

        }*/

        private void ClearColor()
        {
            foreach (var el in _ellipses)
            {
                if (!(el.Key == 0 || el.Key == 1))
                {
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                    mySolidColorBrush.Color = Colors.White;
                    el.Value.Fill = mySolidColorBrush;
                }
            }
            foreach (var t in _text)
            {
                t.text.Background = Brushes.White;
                t.line.Stroke = Brushes.LightGray;
            }
        }

        private void ColorPath(List<int> path)
        {
            foreach (var item in path)
            {
                Console.WriteLine(item.ToString() + "  ");
            }
            int prev = -1;
            foreach (var p in path)
            {
                foreach (var el in _ellipses)
                {
                    if (el.Key == p && !(el.Key == 0 || el.Key == 1) )
                    {
                        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        mySolidColorBrush.Color = Colors.LightSteelBlue;
                        el.Value.Fill = mySolidColorBrush;
                    }

                }
                foreach (var t in _text)
                {
                    if ((t.from == prev) && (p == t.to) || (t.to == prev) && (p == t.from))
                    {
                        t.line.Stroke = Brushes.Black;
                        t.text.Background = Brushes.LightSteelBlue;
                    }

                }
                prev = p;
            }

        }

        private void find_Click(object sender, RoutedEventArgs e)
        {
            /*ClearColor();
            distance.Content = "0";
            int first, second;
            if (Int32.TryParse(from.Text, out first) && Int32.TryParse(to.Text, out second))
            {
                if (byWeight.IsChecked.Value)
                {
                    Dictionary<int, List<int>> path = _graph.FindShortestPathByWeight(first, second);
                    distance.Content = path.Keys.First();
                    ColorPath(path.Values.First());
                }
                else
                {
                    List<int> path = _graph.FindShortestPath(first, second);
                    ColorPath(path);
                }
            }*/
            //_graph.StartRouting();
            //_graph.PrintGraph();
            _graph.PrintTable();

        }

        public class VertexPath
        {
            public int from;
            public int to;
            public TextBox text;
            public Line line;

            public VertexPath(int first, int second, TextBox txt, Line l)
            {
                from = first;
                to = second;
                text = txt;
                line = l;
            }
        }
    }
}
