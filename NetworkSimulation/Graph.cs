using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace NetworkSimulation
{
    public class Graph
    {
        public Dictionary<int, Vertex> vertexes;
        //public Dictionary<int, Dictionary<int, RoutingTable>> routingTable;

        public Graph()
        {
            vertexes = new Dictionary<int, Vertex>();
            //routingTable = new Dictionary<int, Dictionary<int, RoutingTable>>();
        }

        public void AddVertex(int value)
        {
            Vertex newVertex = new Vertex(value);
            vertexes.Add(value, newVertex);
        }

        public void AddNeighboor(int first, int second)
        {
            if (vertexes.Keys.Contains(first) && vertexes.Keys.Contains(second))
            {
                if (!vertexes[first].neighboors.Keys.Contains(vertexes[second]))
                {
                    vertexes[first].neighboors.Add(vertexes[second], 1);
                    vertexes[second].neighboors.Add(vertexes[first], 1);
                }
            }
            else
                Console.WriteLine(string.Format("Can't add {0} and {1}", first, second));
        }

        public void ChangeNeighboorWeight(int first, int second, int weight)
        {
            if (vertexes.Keys.Contains(first) && vertexes.Keys.Contains(second))
            {
                vertexes[first].neighboors[vertexes[second]] = weight;
                vertexes[second].neighboors[vertexes[first]] = weight;
            }
            else
                Console.WriteLine(string.Format("Can't add {0} and {1}", first, second));
        }

        public void RemoveNeighboor(int vertex, int neighboor)
        {
            vertexes[vertex].neighboors.Remove(vertexes[neighboor]);
            vertexes[neighboor].neighboors.Remove(vertexes[vertex]);
        }

        public void RemoveVertex(int vertex)
        {
            foreach (var ver in vertexes[vertex].neighboors)
            {
                ver.Key.neighboors.Remove(vertexes[vertex]);
            }
            vertexes.Remove(vertex);
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

        public List<int> FindShortestPath(int from, int to)
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
            /*StringBuilder str = new StringBuilder();
            str.Append(" way: ");
            foreach (var way in path[to])
            {
                str.Append(way).Append(" -> ");
            }*/
            return path[to];
        }
        public int GetWeightByPath(int from, int to)
        {
            int weight = 0;
            foreach (var neighboor in vertexes[from].neighboors)
            {
                if(to == neighboor.Key.value)
                {
                    weight = neighboor.Value;
                    break;
                }
            }
            return weight;
        }
        public Dictionary<int,List<int>> FindShortestPathByWeight(int from, int to)
        {
            Queue<Vertex> queue = new Queue<Vertex>();
            Dictionary<int,Path> path = new Dictionary<int,Path>();
            foreach (var vertex in vertexes.Keys)
            {
                path.Add(vertex,new Path());
                path[vertex].cost = int.MaxValue;
                path[vertex].path = new List<int>();
            }
            queue.Enqueue(vertexes[from]);
            path[from].cost = 0;
            path[from].path.Add(from);
            while(queue.Count != 0)
            {
                Vertex current = queue.Dequeue();
                foreach (var neighboor in current.neighboors)
                {
                    if (path[neighboor.Key.value].cost > (neighboor.Value + path[current.value].cost))
                    {
                        path[neighboor.Key.value].path.Clear();
                        queue.Enqueue(neighboor.Key);
                        path[neighboor.Key.value].cost = neighboor.Value + path[current.value].cost;
                        foreach (var way in path[current.value].path)
                        {
                            path[neighboor.Key.value].path.Add(way);
                        }
                        path[neighboor.Key.value].path.Add(neighboor.Key.value);
                    }
                }
            }
            /*StringBuilder str = new StringBuilder();
            str.Append("cost: ").Append(path[to].cost.ToString()).Append(" way: ");
            foreach (var way in path[to].path)
            {
                str.Append(way).Append(" -> ");
            }*/
            Dictionary<int, List<int>> p = new Dictionary<int, List<int>>();
            p.Add(path[to].cost, path[to].path);
            return p;
        }

        public void StartRoutingTable()
        {
            foreach (var vertex in vertexes)
                vertex.Value.CreateRoutingTable();

            bool noupdates = false;
            while (!noupdates)
            {
                noupdates = true;
                foreach (var vertex in vertexes)
                {
                    if (vertex.Value.update)
                    {
                        vertex.Value.SendRoutingTableToNeigbhoors();
                        noupdates = false;
                    }
                }
            }
            foreach (var vertex in vertexes)
                vertex.Value.DeleteRows();
        }

        public string PrintTable()
        {
            StringBuilder routingTables = new StringBuilder();
            foreach (var vertex in vertexes)
            {
                routingTables.Append("\"").Append(vertex.Value.value).Append("\"").
                                Append(Environment.NewLine);
                routingTables.Append("   Dest  |   Cost   |   NextHop   ").Append(Environment.NewLine);
                routingTables.Append("--------+--------+-------------").Append(Environment.NewLine);
                foreach (var table in vertex.Value.table)
                {
                    routingTables.Append(string.Format("   {0,3}     |", table.destination)).Append(string.Format("   {0,3}       |", table.cost)).
                                    Append(string.Format("   {0,3}    ", table.nextHop)).Append(Environment.NewLine);
                }
                routingTables.Append(Environment.NewLine);
            }

            return routingTables.ToString();
        }

        public Dictionary<int, List<int>> FindPath(int from, int to)
        {
            Dictionary<int, List<int>> path = new Dictionary<int, List<int>>();
            List<int> way = new List<int>();
            int cost = 0;
            Vertex first = vertexes[from];
            way.Add(from);
            for (int i = 0; i < first.table.Count; i++)
            {
                if(first.table[i].destination == to)
                {
                    if(first.value == to)
                        break;

                    way.Add(first.table[i].nextHop);
                    if(cost == 0)
                        cost = first.table[i].cost;
                    if (vertexes.Keys.Contains(first.table[i].nextHop))
                    {
                        first = vertexes[first.table[i].nextHop];
                        i = 0;
                    }
                    else
                        break;
                }
            }
            path.Add(cost,way);
            return path;
        }

    }

    public class RoutingTable
    {
        public int destination;
        public int nextHop;
        public int cost;

        public RoutingTable(int dest, int next, int cost)
        {
            destination = dest;
            nextHop = next;
            this.cost = cost;
        }
    }

    public class Path
    {
        public int cost;
        public List<int> path;
    }

    public class Vertex
    {
        public int value;
        public Dictionary<Vertex, int> neighboors;
        public List<RoutingTable> table;
        public bool update;
        public int maxHop;

        public Vertex(int v_value)
        {
            value = v_value;
            neighboors = new Dictionary<Vertex, int>();
            table = new List<RoutingTable>();
            table.Add(new RoutingTable(value, -1, 0));
            CreateRoutingTable();
            update = false;
        }

        public void CreateRoutingTable()
        {
            foreach (var neighboor in neighboors)
            {
                bool was = true;
                for (int i = 0; i < table.Count; i++)
                {
                    if (table[i].destination == neighboor.Key.value)
                    {
                        if(table[i].cost != neighboors[neighboor.Key])
                        {
                            table[i].cost = neighboors[neighboor.Key];
                            table[i].nextHop = neighboor.Key.value;
                        }
                        was = false;
                    }
                }
                if (was)
                    table.Add(new RoutingTable(neighboor.Key.value, neighboor.Key.value, neighboor.Value));

                update = true;
            }
            for (int i = 0; i < table.Count; i++)
            {
                bool exist = false;
                if (table[i].nextHop != -1)
                {
                    foreach (var neighboor in neighboors)
                    {
                        if (table[i].nextHop == neighboor.Key.value)
                            exist = true;
                    }
                    if (!exist)
                        table[i].cost = Int32.MaxValue;
                }
            }
            maxHop = 15;
        }

        public void SendRoutingTableToNeigbhoors()
        {
            if (!(maxHop < 0))
            {
                foreach (var neighboor in neighboors)
                    neighboor.Key.UpdateRoutingTable(this);

                maxHop--;
                update = false;
            }
            else
                update = false;
        }

        public void UpdateRoutingTable(Vertex current)
        {
            for (int i = 0; i < current.table.Count; i++)
            {
                bool was = false;
                for (int j = 0; j < table.Count; j++)
                {
                    if (current.table[i].destination == table[j].destination)
                    {
                        if(table[j].nextHop == current.value)
                        {
                            if (current.table[i].cost + neighboors[current] != table[j].cost)
                            {
                                if (maxHop == 0)
                                {
                                    table[j].cost = Int32.MaxValue;
                                }
                                else if (current.table[i].cost != Int32.MaxValue)
                                {
                                    table[j].cost = current.table[i].cost + neighboors[current];

                                }
                                else
                                {
                                    table[j].cost = Int32.MaxValue;
                                }
                                update = true;

                            }
                        }
                        if (table[j].cost > current.table[i].cost + neighboors[current])
                        {
                            if (maxHop == 0)
                                table[j].cost = Int32.MaxValue;
                            else if (current.table[i].cost != Int32.MaxValue)
                            {
                                table[j].cost = current.table[i].cost + neighboors[current];
                                table[j].nextHop = current.value;
                            }
                            update = true;
                        }
                        was = true;
                    }
                }
                if (!was)
                {
                    if (current.table[i].cost != Int32.MaxValue && maxHop != 0)
                    {
                        table.Add(new RoutingTable(current.table[i].destination, current.value, current.table[i].cost + neighboors[current]));
                        update = true;
                    }
                }
            }
        }

        public void DeleteRows()
        {
            for (int i = 0; i < table.Count; i++)
            {
                if(table[i].cost == Int32.MaxValue)
                {
                    table.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
