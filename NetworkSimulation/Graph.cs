using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class Graph
    {
        public Dictionary<int, Vertex> vertexes;

        public Graph()
        {
            vertexes = new Dictionary<int, Vertex>();
        }

        public void AddVertex(int value)
        {
            Vertex newVertex = new Vertex(value);
            vertexes.Add(value, newVertex);
        }

        public void AddNeighboor(int first, int second, int weight)
        {
            if ((vertexes.Count >= first) && (vertexes.Count >= second))
            {
                vertexes[first].neighboors.Add(vertexes[second], weight);
                vertexes[second].neighboors.Add(vertexes[first], weight);
            }
            else
                Console.WriteLine(string.Format("Can't add {0} and {1}", first, second));
        }

        public string PrintGraph()
        {
            List<Vertex> visited = new List<Vertex>();
            Queue<Vertex> queue = new Queue<Vertex>();
            StringBuilder graph = new StringBuilder();
            visited.Add(vertexes[0]);
            queue.Enqueue(vertexes[0]);
            graph.Append(0).Append(" ");
            while(queue.Count != 0)
            {
                foreach (var neighboor in queue.Dequeue().neighboors)
                {
                    if (!visited.Contains(neighboor.Key))
                    {
                        queue.Enqueue(neighboor.Key);
                        visited.Add(neighboor.Key);
                        graph.Append(neighboor.Key.value).Append(" ");
                    }
                }
            }
            return graph.ToString();
        }

        public string FindShortestPath(int from, int to)
        {
            Queue<Vertex> queue = new Queue<Vertex>();
            List<int> visited = new List<int>();
            Dictionary<int, List<int>> path = new Dictionary<int, List<int>>();
            foreach (var vertex in vertexes.Keys)
            {
                path.Add(vertex, new List<int>());
            }
            queue.Enqueue(vertexes[from]);
            visited.Add(from);
            path[from].Add(from);
            while(queue.Count != 0)
            {
                Vertex current = queue.Dequeue();
                foreach (var neighboor in current.neighboors)
                {
                    if (!visited.Contains(neighboor.Key.value))
                    {
                        path[neighboor.Key.value].Clear();
                        queue.Enqueue(neighboor.Key);
                        visited.Add(neighboor.Key.value);
                        foreach (var way in path[current.value])
                        {
                            path[neighboor.Key.value].Add(way);
                        }
                        path[neighboor.Key.value].Add(neighboor.Key.value);
                    }
                }
            }
            StringBuilder str = new StringBuilder();
            str.Append(" way: ");
            foreach (var way in path[to])
            {
                str.Append(way).Append(" -> ");
            }
            return str.ToString();
        }

        public string FindShortestPathByWeight(int from, int to)
        {
            Queue<Vertex> queue = new Queue<Vertex>();
            Dictionary<int,Path> path = new Dictionary<int,Path>();
            foreach (var vertex in vertexes.Keys)
            {
                path.Add(vertex,new Path());
                path[vertex].distance = int.MaxValue;
                path[vertex].path = new List<int>();
            }
            queue.Enqueue(vertexes[from]);
            path[from].distance = 0;
            path[from].path.Add(from);
            while(queue.Count != 0)
            {
                Vertex current = queue.Dequeue();
                foreach (var neighboor in current.neighboors)
                {
                    if (path[neighboor.Key.value].distance > (neighboor.Value + path[current.value].distance))
                    {
                        path[neighboor.Key.value].path.Clear();
                        queue.Enqueue(neighboor.Key);
                        path[neighboor.Key.value].distance = neighboor.Value + path[current.value].distance;
                        foreach (var way in path[current.value].path)
                        {
                            path[neighboor.Key.value].path.Add(way);
                        }
                        path[neighboor.Key.value].path.Add(neighboor.Key.value);
                    }
                }
            }
            StringBuilder str = new StringBuilder();
            str.Append("distance: ").Append(path[to].distance.ToString()).Append(" way: ");
            foreach (var way in path[to].path)
            {
                str.Append(way).Append(" -> ");
            }
            return str.ToString();
        }
    }

    public class Path
    {
        public int distance;
        public List<int> path;
    }

    class Vertex
    {
        public int value;
        public Dictionary<Vertex, int> neighboors;

        public Vertex(int v_value)
        {
            value = v_value;
            neighboors = new Dictionary<Vertex, int>();
        }
    }
}
