using System;
using System.Collections.Generic;
using System.Text;

namespace Imagine.Library
{
    public class GraphPort<T>
    {
        private int portNumber;
        private GraphNode<T> node;
        private GraphPort<T> remotePort;

        public int PortNumber
        {
            get { return portNumber; }
            set { portNumber = value; }
        }

        public GraphNode<T> Node
        {
            get { return node; }
            set { node = value; }
        }

        public GraphPort<T> RemotePort
        {
            get { return remotePort; }
            set { remotePort = value; }
        }

        public GraphPort(GraphNode<T> node, int portNumber)
        {
            this.node = node;
            this.portNumber = portNumber;
        }
    }
}
