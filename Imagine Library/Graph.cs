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

        public void RemoveNode(GraphNode<T> node)
        {
            Dictionary<int, GraphPort<T>> ports;

            if(!nodeMap.ContainsValue(node))
                return;

            ports = new Dictionary<int, GraphPort<T>>(node.Inports);
            foreach(GraphPort<T> port in ports.Values)
                Disconnect(port.RemotePort.Node, port.RemotePort.PortNumber, node, port.PortNumber);

            ports = new Dictionary<int, GraphPort<T>>(node.Outports);
            foreach(GraphPort<T> port in ports.Values)
                Disconnect(node, port.PortNumber, port.RemotePort.Node, port.RemotePort.PortNumber);

            nodeMap.Remove(node.Machine);
        }

        public void Connect(GraphNode<T> fromNode, int fromPortNumber, GraphNode<T> toNode, int toPortNumber)
        {
            if(fromNode == null || toNode == null)
                throw new ArgumentNullException();

            if(!nodeMap.ContainsValue(fromNode))
                throw new ArgumentException(String.Format("Attempting to connect nodes, but the 'from' node was not a node from this graph! (From=({0}), To=({1}))", fromNode, toNode));
            if(!nodeMap.ContainsValue(toNode))
                throw new ArgumentException(String.Format("Attempting to connect nodes, but the 'to' node was not a node from this graph! (From=({0}), To=({1}))", fromNode, toNode));

            if(fromNode.Outports.ContainsKey(fromPortNumber))
                throw new PortAlreadyConnectedException();
            if(toNode.Inports.ContainsKey(toPortNumber))
                throw new PortAlreadyConnectedException();

            GraphPort<T> sourcePort = fromNode.CreateOutport(fromPortNumber);
            GraphPort<T> destinationPort = toNode.CreateInport(toPortNumber);

            sourcePort.RemotePort = destinationPort;
            destinationPort.RemotePort = sourcePort;

            connectionCount++;
        }

        public GraphNode<T> GetNodeFor(T machine)
        {
            GraphNode<T> node = null;
            nodeMap.TryGetValue(machine, out node);
            return node;
        }

        public void Disconnect(GraphNode<T> fromNode, int fromPortNumber, GraphNode<T> toNode, int toPortNumber)
        {
            if(!nodeMap.ContainsValue(fromNode))
                throw new ArgumentException(String.Format("Attempting to disconnect nodes, but the 'from' node was not a node from this graph! (From=({0}), To=({1}))", fromNode, toNode));
            if(!nodeMap.ContainsValue(toNode))
                throw new ArgumentException(String.Format("Attempting to disconnect nodes, but the 'to' node was not a node from this graph! (From=({0}), To=({1}))", fromNode, toNode));

            if(fromNode.Outports.ContainsKey(fromPortNumber) && toNode.Inports.ContainsKey(toPortNumber))
                if(fromNode.Outports[fromPortNumber].RemotePort == toNode.Inports[toPortNumber])
                    if(toNode.Inports[toPortNumber].RemotePort == fromNode.Outports[fromPortNumber])
                    {
                        fromNode.Outports.Remove(fromPortNumber);
                        toNode.Inports.Remove(toPortNumber);
                        connectionCount--;
                    }
        }

        public List<GraphNode<T>> GetTopologicalOrdering()
        {
            List<GraphNode<T>> ordering = new List<GraphNode<T>>();

            SortContext context = BuildContext(nodeMap);
            while(context.HasMoreNodesWithOnlyVisitedEdges())
            {
                GraphNode<T> topNode = context.ExtractNodeWithOnlyVisitedEdges();
                ordering.Add(topNode);

                foreach(GraphPort<T> port in topNode.Outports.Values)
                    context.DecreaseUnvisitedEdgeCount(port.RemotePort.Node);
            }

            if(context.UnvisitedInputsRemaining())
                throw new GraphCycleException();

            return ordering;
        }

        public List<GraphNode<T>> GetAllNodes()
        {
            return new List<GraphNode<T>>(nodeMap.Values);
        }

        private SortContext BuildContext(Dictionary<T, GraphNode<T>> nodeMap)
        {
            SortContext context = new SortContext();

            foreach(GraphNode<T> node in nodeMap.Values)
                context.AddNode(node);

            return context;
        }


        private class SortContext
        {
            // Synchronized indexes of unvisited input node count,
            // from the view of count and node, respectively.

            Dictionary<int, List<GraphNode<T>>> nodesByUnvisitedInputCount;
            Dictionary<GraphNode<T>, int> unvisitedInputCountByNode;
            
            internal SortContext()
            {
                nodesByUnvisitedInputCount = new Dictionary<int, List<GraphNode<T>>>();
                nodesByUnvisitedInputCount.Add(0, new List<GraphNode<T>>());

                unvisitedInputCountByNode = new Dictionary<GraphNode<T>, int>();
            }

            internal void AddNode(GraphNode<T> node)
            {
                RegisterNodeWithCount(node, node.InputCount);
            }

            private void RegisterNodeWithCount(GraphNode<T> node, int count)
            {
                if(!nodesByUnvisitedInputCount.ContainsKey(count))
                    nodesByUnvisitedInputCount.Add(count, new List<GraphNode<T>>());

                nodesByUnvisitedInputCount[count].Add(node);

                unvisitedInputCountByNode[node] = count;
            }

            internal bool HasMoreNodesWithOnlyVisitedEdges()
            {
                return nodesByUnvisitedInputCount[0].Count > 0;
            }

            internal GraphNode<T> ExtractNodeWithOnlyVisitedEdges()
            {
                GraphNode<T> node = nodesByUnvisitedInputCount[0][0];
                nodesByUnvisitedInputCount[0].RemoveAt(0);

                return node;
            }

            internal void DecreaseUnvisitedEdgeCount(GraphNode<T> node)
            {
                int count = unvisitedInputCountByNode[node];
                nodesByUnvisitedInputCount[count].Remove(node);
                
                RegisterNodeWithCount(node, count - 1);
            }

            internal bool UnvisitedInputsRemaining()
            {
                foreach(List<GraphNode<T>> list in nodesByUnvisitedInputCount.Values)
                    if(list.Count > 0)
                        return true;

                return false;
            }
        }
    }

    public class GraphCycleException : Exception
    {
    }

    public class PortAlreadyConnectedException : Exception
    {
    }
}
