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
        public MainWindow()
        {
            //InitializeComponent();
            Graph graph = new Graph();
            graph.AddVertex(0);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);


            graph.AddNeighboor(0, 1, 5);
            graph.AddNeighboor(0, 2, 31);
            graph.AddNeighboor(0, 3, 20);
            graph.AddNeighboor(1, 4, 7);
            graph.AddNeighboor(2, 4, 10);
            graph.AddNeighboor(2, 5, 1);
            graph.AddNeighboor(3, 5, 5);
            graph.AddNeighboor(4, 6, 40);
            graph.AddNeighboor(5, 6, 14);

            Console.WriteLine(graph.PrintGraph());

            foreach (var vertex in graph.vertexes)
            {
                foreach (var n in vertex.Value.neighboors)
                {
                    Console.WriteLine("Key: " + vertex.Key.ToString()+" neighboor: " + n.Key.value + " weight: " + n.Value.ToString());
                }
                Console.WriteLine();
            }

            Console.WriteLine("Shortest path by weight:");
            Console.WriteLine("From: 0 to: 6 " + graph.FindShortestPathByWeight(0, 6));
            Console.WriteLine("From: 0 to: 5 " + graph.FindShortestPathByWeight(0, 5));
            Console.WriteLine("From: 0 to: 2 " + graph.FindShortestPathByWeight(0, 2));
            Console.WriteLine("From: 0 to: 4 " + graph.FindShortestPathByWeight(0, 4));
            Console.WriteLine("From: 0 to: 1 " + graph.FindShortestPathByWeight(0, 1));
            Console.WriteLine();

            Console.WriteLine("Shortest path:");
            Console.WriteLine("From: 0 to: 6 " + graph.FindShortestPath(0, 6));
            Console.WriteLine("From: 0 to: 5 " + graph.FindShortestPath(0, 5));
            Console.WriteLine("From: 0 to: 2 " + graph.FindShortestPath(0, 2));
            Console.WriteLine("From: 0 to: 4 " + graph.FindShortestPath(0, 4));
            Console.WriteLine("From: 0 to: 1 " + graph.FindShortestPath(0, 1));


        }
    }
}
