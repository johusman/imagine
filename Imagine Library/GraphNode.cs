using System;
using System.Collections.Generic;
using System.Text;

namespace Imagine.Library
{
    public class GraphNode<T>
    {
        public GraphNode(T machine)
        {
            this.machine = machine;
        }

        private T machine;
        public T Machine
        {
            get { return machine; }
            set { machine = value; }
        }

        private Dictionary<int, GraphPort<T>> inports = new Dictionary<int, GraphPort<T>>();
        public Dictionary<int, GraphPort<T>> Inports
        {
            get { return inports; }
            set { inports = value; }
        }

        private Dictionary<int, GraphPort<T>> outports = new Dictionary<int, GraphPort<T>>();
        public Dictionary<int, GraphPort<T>> Outports
        {
            get { return outports; }
            set { outports = value; }
        }
        

        public int InputCount
        {
            get { return inports.Count; }
        }

        public int OutputCount
        {
            get { return outports.Count; }
        }

        public override string ToString()
        {
            return String.Format("GraphNode ({0})", machine.ToString());
        }

        internal GraphPort<T> CreateInport(int portNumber)
        {
            GraphPort<T> port = new GraphPort<T>(this, portNumber);
            inports[portNumber] = port;
            return port;
        }

        internal GraphPort<T> CreateOutport(int portNumber)
        {
            GraphPort<T> port = new GraphPort<T>(this, portNumber);
            outports[portNumber] = port;
            return port;
        }
    }
}
