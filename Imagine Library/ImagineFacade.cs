using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace Imagine.Library
{
    public class ImagineFacade
    {
        private SourceMachine sourceMachine;
        private SinkMachine destinationMachine;
        private Dictionary<string, Type> machineTypes;

        private Graph<Machine> graph;

        public Graph<Machine> Graph
        {
            get { return graph; }
            set { graph = value; }
        }

        public event System.EventHandler SourceChanged;
        public event System.EventHandler DestinationChanged;

        public SourceMachine SourceMachine
        {
            get { return sourceMachine; }
        }

        public SinkMachine DestinationMachine
        {
            get { return destinationMachine; }
        }


        public ImagineFacade()
        {
            machineTypes = new Dictionary<string, Type>();
            machineTypes["Imagine.Inverter"] = typeof(InverterMachine);

            graph = new Graph<Machine>();
            sourceMachine = new SourceMachine();
            destinationMachine = new SinkMachine();

            graph.Connect(graph.AddNode(sourceMachine), graph.AddNode(destinationMachine));
        }

        public void OpenSource(string filename)
        {
            sourceMachine.Filename = filename;
            if(SourceChanged != null)
                SourceChanged.Invoke(this, new StringEventArg(filename));
        }

        public void OpenDestination(string filename)
        {
            destinationMachine.Filename = filename;
            if(DestinationChanged != null)
                DestinationChanged.Invoke(this, new StringEventArg(filename));
        }

        public string GetSourceFilename()
        {
            return sourceMachine.Filename;
        }

        public string GetDestinationFilename()
        {
            return destinationMachine.Filename;
        }

        public Machine NewMachine(string type)
        {
            Machine machine = (Machine)Activator.CreateInstance(machineTypes[type]);
            graph.AddNode(machine);
            return machine;
        }

        public void Connect(Machine machine1, Machine machine2)
        {
            graph.Connect(graph.GetNodeFor(machine1), graph.GetNodeFor(machine2));
        }

        public void Disconnect(Machine machine1, Machine machine2)
        {
            graph.Disconnect(graph.GetNodeFor(machine1), graph.GetNodeFor(machine2));
        }

        public void Generate()
        {
            Dictionary<GraphNode<Machine>, Bitmap> resultMap = new Dictionary<GraphNode<Machine>, Bitmap>();

            List<GraphNode<Machine>> ordering = graph.GetTopologicalOrdering();
            foreach(GraphNode<Machine> node in ordering)
            {
                Bitmap[] inputs = new Bitmap[node.InputCount];
                for(int i = 0; i < node.InputCount; i++)
                    inputs[i] = resultMap[node.Inputs[i]];

                resultMap[node] = node.Machine.Process(inputs);
            }

            //destinationMachine.Process(new Bitmap[] { sourceMachine.Process(null) });
        }
    }

    public class StringEventArg : EventArgs
    {
        private string _string;

        public StringEventArg(string _string)
        {
            this._string = _string;
        }

        public override string ToString()
        {
            return _string;
        }
    }
}
