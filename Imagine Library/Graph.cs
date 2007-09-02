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
            if(machine == null)
                throw new ArgumentNullException();
            GraphNode<T> node = new GraphNode<T>(machine);
            nodeMap.Add(machine, node);
            return node;
        }

        public void Connect(GraphNode<T> fromNode, GraphNode<T> toNode)
        {
            if(fromNode == null || toNode == null)
                throw new ArgumentNullException();

            if(!nodeMap.ContainsValue(fromNode))
                throw new ArgumentException(String.Format("Attempting to connect nodes, but the 'from' node was not a node from this graph! (From=({0}), To=({1}))", fromNode, toNode));
            if(!nodeMap.ContainsValue(toNode))
                throw new ArgumentException(String.Format("Attempting to connect nodes, but the 'to' node was not a node from this graph! (From=({0}), To=({1}))", fromNode, toNode));

            fromNode.Outputs.Add(toNode);
            toNode.Inputs.Add(fromNode);
            connectionCount++;
        }

        public GraphNode<T> GetNodeFor(T machine)
        {
            GraphNode<T> node = null;
            nodeMap.TryGetValue(machine, out node);
            return node;
        }

        public void Disconnect(GraphNode<T> fromNode, GraphNode<T> toNode)
        {
            if(!nodeMap.ContainsValue(fromNode))
                throw new ArgumentException(String.Format("Attempting to disconnect nodes, but the 'from' node was not a node from this graph! (From=({0}), To=({1}))", fromNode, toNode));
            if(!nodeMap.ContainsValue(toNode))
                throw new ArgumentException(String.Format("Attempting to disconnect nodes, but the 'to' node was not a node from this graph! (From=({0}), To=({1}))", fromNode, toNode));

            if(fromNode.Outputs.Remove(toNode))
                if(toNode.Inputs.Remove(fromNode))
                    connectionCount--;
        }
    }
}
