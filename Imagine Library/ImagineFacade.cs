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
            machineTypes["Imagine.Source"] = typeof(SourceMachine);
            machineTypes["Imagine.Destination"] = typeof(SinkMachine);
            machineTypes["Imagine.Branch4"] = typeof(Branch4Machine);
            machineTypes["Imagine.Adder4"] = typeof(Adder4Machine);
            machineTypes["Imagine.AlphaMultiplier4"] = typeof(AlphaMultiply4Machine);
            machineTypes["Imagine.Inverter"] = typeof(InverterMachine);
            machineTypes["Imagine.RGBSplitter"] = typeof(RGBSplitterMachine);
            machineTypes["Imagine.RGBJoiner"] = typeof(RGBJoinerMachine);
            machineTypes["Imagine.Halver"] = typeof(HalverMachine);
            machineTypes["Imagine.HSLSplitter"] = typeof(HSLSplitterMachine);
            machineTypes["Imagine.HSLJoiner"] = typeof(HSLJoinerMachine);

            graph = new Graph<Machine>();
            sourceMachine = new SourceMachine();
            destinationMachine = new SinkMachine();

            graph.Connect(graph.AddNode(sourceMachine), 0, graph.AddNode(destinationMachine), 0);
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

        public void OverrideDestination(SinkMachine machine)
        {
            destinationMachine = machine;
            graph.AddNode(machine);
        }

        public void OverrideSource(SourceMachine machine)
        {
            sourceMachine = machine;
            graph.AddNode(machine);
        }

        public Machine NewMachine(string type)
        {
            Machine machine = (Machine)Activator.CreateInstance(machineTypes[type]);
            graph.AddNode(machine);
            return machine;
        }

        public void AddMachine(Machine machine)
        {
            graph.AddNode(machine);
        }

        public void RemoveMachine(Machine machine)
        {
            graph.RemoveNode(graph.GetNodeFor(machine));
        }

        public void Connect(Machine machine1, int port1, Machine machine2, int port2)
        {
            if(port1 >= machine1.OutputCount)
                throw new MachineOutputIndexOutOfRangeException();
            if(port2 >= machine2.InputCount)
                throw new MachineInputIndexOutOfRangeException();

            graph.Connect(graph.GetNodeFor(machine1), port1, graph.GetNodeFor(machine2), port2);
        }

        public void Disconnect(Machine machine1, int port1, Machine machine2, int port2)
        {
            graph.Disconnect(graph.GetNodeFor(machine1), port1, graph.GetNodeFor(machine2), port2);
        }

        public void Generate()
        {
            Dictionary<GraphPort<Machine>, ImagineImage> resultMap = new Dictionary<GraphPort<Machine>, ImagineImage>();

            List<GraphNode<Machine>> ordering = graph.GetTopologicalOrdering();
            foreach(GraphNode<Machine> node in ordering)
            {
                ImagineImage[] inputs = new ImagineImage[node.Machine.InputCount];
                for (int i = 0; i < node.Machine.InputCount; i++)
                {
                    GraphPort<Machine> inPort = null;
                    if(node.Inports.TryGetValue(i, out inPort))
                        inputs[i] = resultMap[inPort.RemotePort];
                }

                ImagineImage[] results = node.Machine.Process(inputs);
                for(int i = 0; i < node.Machine.OutputCount; i++)
                    if(node.Outports.ContainsKey(i))
                        resultMap[node.Outports[i]] = results[i];
            }
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

    public class MachineInputIndexOutOfRangeException : Exception { }
    public class MachineOutputIndexOutOfRangeException : Exception { }
}
