using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace Imagine.Library
{
    public class ImagineFacade
    {
        private SourceMachine sourceMachine;
        private SinkMachine destinationMachine;
        private Dictionary<string, Type> machineTypes;

        public Dictionary<string, Type> MachineTypes
        {
            get { return machineTypes; }
        }

        private Graph<Machine> graph;

        public Graph<Machine> Graph
        {
            get { return graph; }
            set { graph = value; }
        }

        public event System.EventHandler GraphChanged;
        public delegate void ProgressCallback(int machineIndex, int totalMachines, Machine currentMachine, int currentPercent);

        private bool disableEvents = false;

        public SourceMachine SourceMachine
        {
            get { return sourceMachine; }
        }

        public SinkMachine DestinationMachine
        {
            get { return destinationMachine; }
        }

        private string workingDirectory = ".";
        public string WorkingDirectory
        {
            get { return workingDirectory; }
        }


        public ImagineFacade(string workingDirectory)
        {
            this.workingDirectory = System.IO.Path.GetFullPath(workingDirectory);

            LoadMachines();

            graph = new Graph<Machine>();
            sourceMachine = new SourceMachine();
            sourceMachine.MachineChanged += MachineChangedHandler;
            destinationMachine = new SinkMachine();
            destinationMachine.MachineChanged += MachineChangedHandler;

            graph.Connect(graph.AddNode(sourceMachine), 0, graph.AddNode(destinationMachine), 0);
        }

        private void LoadMachines()
        {
            machineTypes = new Dictionary<string, Type>();
            
            foreach (String fileName in Directory.GetFiles(workingDirectory, "*.dll", SearchOption.AllDirectories))
                if (System.IO.Path.GetFileName(fileName).ToLower() != System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location.ToLower()))
                    foreach (Type type in Assembly.LoadFile(fileName).GetTypes())
                        if (type.IsSubclassOf(typeof(Machine)))
                            if (type.GetCustomAttributes(typeof(UniqueName), false).Length == 1)
                            {
                                string name = ((UniqueName)type.GetCustomAttributes(typeof(UniqueName), false)[0]).Value;
                                machineTypes[name] = type;
                            }

            machineTypes["Imagine.Source"] = typeof(SourceMachine);
            machineTypes["Imagine.Destination"] = typeof(SinkMachine);
        }

        public void OpenSource(string filename)
        {
            sourceMachine.Filename = filename;
        }

        public void OpenDestination(string filename)
        {
            destinationMachine.Filename = filename;
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
            AddMachine(machine);
            return machine;
        }

        public void AddMachine(Machine machine)
        {
            machine.MachineChanged += MachineChangedHandler;
            graph.AddNode(machine);

            if(!disableEvents && GraphChanged != null)
                GraphChanged.Invoke(this, null);
        }

        private void MachineChangedHandler(object sender, EventArgs e)
        {
            if (!disableEvents && GraphChanged != null)
                GraphChanged.Invoke(sender, null);
        }

        public void RemoveMachine(Machine machine)
        {
            graph.RemoveNode(graph.GetNodeFor(machine));

            if (!disableEvents && GraphChanged != null)
                GraphChanged.Invoke(this, null);
        }

        public void Connect(Machine machine1, int port1, Machine machine2, int port2)
        {
            if(port1 >= machine1.OutputCount)
                throw new MachineOutputIndexOutOfRangeException();
            if(port2 >= machine2.InputCount)
                throw new MachineInputIndexOutOfRangeException();

            graph.Connect(graph.GetNodeFor(machine1), port1, graph.GetNodeFor(machine2), port2);

            if (!disableEvents && GraphChanged != null)
                GraphChanged.Invoke(this, null);
        }

        public void Disconnect(Machine machine1, int port1, Machine machine2, int port2)
        {
            graph.Disconnect(graph.GetNodeFor(machine1), port1, graph.GetNodeFor(machine2), port2);

            if (!disableEvents && GraphChanged != null)
                GraphChanged.Invoke(this, null);
        }

        public void Generate()
        {
            Generate(null);
        }

        public void Generate(ProgressCallback progressCallback)
        {
            Dictionary<GraphPort<Machine>, ImagineImage> resultMap = new Dictionary<GraphPort<Machine>, ImagineImage>();

            List<GraphNode<Machine>> ordering = graph.GetTopologicalOrdering();
            
            int machineIndex = 0;
            int totalMachines = ordering.Count;
            Machine currentMachine = null;

            Machine.ProgressCallback machineCallback = new Machine.ProgressCallback(delegate(int percent) {
                if(progressCallback != null)
                    progressCallback.Invoke(machineIndex, totalMachines, currentMachine, percent);
            });
            
            foreach(GraphNode<Machine> node in ordering)
            {
                currentMachine = node.Machine;

                ImagineImage[] inputs = new ImagineImage[currentMachine.InputCount];
                for (int i = 0; i < currentMachine.InputCount; i++)
                {
                    GraphPort<Machine> inPort = null;
                    if (node.Inports.TryGetValue(i, out inPort))
                    {
                        inputs[i] = resultMap[inPort.RemotePort];
                        // This is an attempt to free up memory; it might turn out
                        // to be a bad idea for future features. Should work now though.
                        resultMap.Remove(inPort.RemotePort);
                    }
                }

                ImagineImage[] results = currentMachine.Process(inputs, machineCallback);
                for(int i = 0; i < node.Machine.OutputCount; i++)
                    if(node.Outports.ContainsKey(i))
                        resultMap[node.Outports[i]] = results[i];

                machineIndex++;
            }

            System.GC.Collect();
        }

        public string SerializeGraph()
        {
            List<GraphNode<Machine>> ordering = graph.GetTopologicalOrdering();

            string text = "Graph {\n";

            foreach (GraphNode<Machine> node in ordering)
            {
                string machineString;
                if (node.InputCount == 0)
                {
                    machineString = String.Format("\t{0} 'machine{1}' {{}}\n",
                        node.Machine.ToString(),
                        ordering.IndexOf(node));
                }
                else
                {
                    string connections = "";
                    foreach(KeyValuePair<int, GraphPort<Machine>> pair in node.Inports)
                    {
                        char thisCode = node.Machine.InputCodes[pair.Key];
                        GraphNode<Machine> otherNode = pair.Value.RemotePort.Node;
                        string otherName = ordering.IndexOf(otherNode).ToString();
                        char otherCode = otherNode.Machine.OutputCodes[pair.Value.RemotePort.PortNumber];
                        connections += String.Format("\t\t'machine{0}'{1} -> {2}\n",
                            otherName,
                            otherCode == ' ' ? "" : ":" + otherCode.ToString(),
                            thisCode == ' ' ? "" : thisCode.ToString());
                    }

                    
                    string settingsText = "";
                    string settings = node.Machine.SaveSettings();
                    if (settings != null)
                        settingsText = String.Format("\t\t[ {0} ]\n", settings);

                    machineString = String.Format("\t{0} 'machine{1}' {{\n{2}{3}\t}}\n",
                        node.Machine.ToString(),
                        ordering.IndexOf(node),
                        connections,
                        settingsText);
                }

                text += machineString;
            }

            text += "}";

            return text;
        }

        public List<string> DeserializeGraph(string input)
        {
            List<string> unrecognizedTypes = new List<string>();

            disableEvents = true;

            graph = new Graph<Machine>();
            sourceMachine = null;
            destinationMachine = null;

            Dictionary<string, Machine> machines = new Dictionary<string,Machine>();

            string data = input.Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ');
            string graphData = ImagineFileFormat.ExtractSections(data)["Graph"];
            Group machineGroup = Regex.Match(graphData, "^(\\s*(?<machine>\\S+\\s+'[^']+'\\s*{[^}]*})\\s*)*$").Groups["machine"];
            foreach (Capture capture in machineGroup.Captures)
            {
                string machineData = capture.Value;
                Match machineMatch = Regex.Match(machineData, "^(?<type>\\S+)\\s+'(?<name>[^']+)'\\s*{(?<connections>[^}]*)}$");
                string machineType = machineMatch.Groups["type"].Value;
                string machineName = machineMatch.Groups["name"].Value;
                string connectionsData = machineMatch.Groups["connections"].Value;

                if (machineTypes.ContainsKey(machineType))
                {
                    Machine machine = NewMachine(machineType);
                    machines[machineName] = machine;
                    if (machineType.Trim() == "Imagine.Source")
                        sourceMachine = (SourceMachine)machine;
                    if (machineType.Trim() == "Imagine.Destination")
                        destinationMachine = (SinkMachine)machine;

                    if (!Regex.IsMatch(connectionsData, "^\\s*{\\s*(\\[.*\\])?\\s*}\\s*$"))
                    {
                        Group connectionsGroup = Regex.Match(connectionsData, "^(\\s*(?<connection>'[^']+'(\\s*:\\S)?\\s*->(\\s*\\S)?)\\s*)+(\\[.*\\])?\\s*$").Groups["connection"];
                        foreach (Capture connectionCapture in connectionsGroup.Captures)
                        {
                            string connectionData = connectionCapture.Value;
                            Match connectionMatch = Regex.Match(connectionData, "^\\s*'(?<fromname>[^']+)'(\\s*:(?<fromport>\\S))?\\s*->\\s*(?<toport>(\\S)?)\\s*$");
                            string fromName = connectionMatch.Groups["fromname"].Value;
                            string fromPort = connectionMatch.Groups["fromport"].Value;
                            string toPort = connectionMatch.Groups["toport"].Value;

                            if (machines.ContainsKey(fromName))
                            {
                                Machine fromMachine = machines[fromName];
                                int fromPortIndex = 0;
                                if (fromPort.Length > 0)
                                    fromPortIndex = Array.IndexOf(fromMachine.OutputCodes, fromPort[0]);
                                int toPortIndex = 0;
                                if (toPort.Length > 0)
                                    toPortIndex = Array.IndexOf(machine.InputCodes, toPort[0]);

                                Connect(fromMachine, fromPortIndex, machine, toPortIndex);
                            }
                        }
                    }

                    if (Regex.IsMatch(connectionsData, "^[^\\[]*\\[(\\s*\\w+\\s*=\\s*'[^']*'\\s*)+\\][^\\]]*$"))
                    {
                        Match settingsMatch = Regex.Match(connectionsData, "^[^\\[]*\\[(?<settings>(\\s*\\w+\\s*=\\s*'[^']*'\\s*)+)\\][^\\]]*$");
                        machine.LoadSettings(settingsMatch.Groups["settings"].Value);
                    }
                }
                else
                    if (!unrecognizedTypes.Contains(machineType))
                        unrecognizedTypes.Add(machineType);
            }

            disableEvents = false;

            return unrecognizedTypes;
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
