using System;
using System.Collections.Generic;
using System.Text;

namespace Imagine.Library
{
    public class Graph<T>
    {
        private Dictionary<T, GraphNode<T>> nodeMap = new Dictionary<T, GraphNode<T>>();

        public int NodeCount
        {
            get { return nodeMap.Count; }
        }

        private int connectionCount = 0;
        public int ConnectionCount
        {
            get
            {
                return connectionCount;
            }
        }

        public GraphNode<T> AddNode(T machine)
        {
            GraphNode<T> node = new GraphNode<T>(machine);
            nodeMap.Add(machine, node);
            return node;
        }

        public void Connect(GraphNode<T> node1, GraphNode<T> node2)
        {
            node1.Outputs.Add(node2);
            node2.Inputs.Add(node1);
            connectionCount++;
        }

        public GraphNode<T> GetNodeFor(T machine)
        {
            return nodeMap[machine];
        }
    }
}
