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

        public int InputCount
        {
            get { return inputs.Count; }
        }

        public int OutputCount
        {
            get { return outputs.Count; }
        }

        private List<GraphNode<T>> inputs = new List<GraphNode<T>>();
        public List<GraphNode<T>> Inputs
        {
            get { return inputs; }
            set { inputs = value; }
        }

        private List<GraphNode<T>> outputs = new List<GraphNode<T>>();
        public List<GraphNode<T>> Outputs
        {
            get { return outputs; }
            set { outputs = value; }
        }
    }
}
