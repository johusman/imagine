using System;
using System.Collections.Generic;
using System.Text;
using Imagine.Library;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Imagine.GUI
{
    public interface IGUIGraph
    {
        Dictionary<GraphNode<Machine>, GUINode>.ValueCollection Nodes { get; }
        Dictionary<GraphPort<Machine>, GUIPort>.ValueCollection Ports { get; }

        GUINode CreateNode(string type, Point position);
        void Remove(GUINode node);

        bool HasMachineGUIFor(Type type);
        MachineGUI CreateMachineGUIFor(Type type);
        MachineGUI CreateMachineGUIFor(string type);
        GUINode GetNodeAt(Point point);
        GUIPort GetPortAt(Point point);

        void Connect(GUIPort fromPort, GUIPort toPort);
        void Disconnect(GUIPort port);

        string SerializeLayout();
        void DeserializeLayout(string input);
    }

    interface IGUIGraphInternal : IGUIGraph
    {
        GUINode GuiNodeFor(GraphNode<Machine> node);
        GUIPort GuiPortFor(GraphPort<Machine> port);
        GUINode RegisterNode(GraphNode<Machine> node, Point position);
        void RegisterPort(GUIPort port);
        void DeregisterPort(GraphPort<Machine> port);
        void AddAll(IEnumerable<GraphNode<Machine>> list, Size size);
    }

    class GUIGraph : IGUIGraphInternal
    {
        private ImagineFacade facade;
        private Dictionary<GraphNode<Machine>, GUINode> nodes;
        private Dictionary<GraphPort<Machine>, GUIPort> ports;
        private Dictionary<Type, Type> machineGUITypes;

        public Dictionary<GraphNode<Machine>, GUINode>.ValueCollection Nodes
        {
            get { return nodes.Values; }
        }

        public Dictionary<GraphPort<Machine>, GUIPort>.ValueCollection Ports
        {
            get { return ports.Values; }
        }

        public GUIGraph(ImagineFacade facade, Size size)
        {
            this.nodes = new Dictionary<GraphNode<Machine>, GUINode>();
            this.ports = new Dictionary<GraphPort<Machine>, GUIPort>();
            this.facade = facade;

            LoadMachineGUITypes(facade.WorkingDirectory);
            AddAll(facade.Graph.GetAllNodes(), size);
        }

        public GUINode GuiNodeFor(GraphNode<Machine> node)
        {
            return nodes[node];
        }

        public GUIPort GuiPortFor(GraphPort<Machine> port)
        {
            return ports[port];
        }

        public GUINode CreateNode(string type, Point position)
        {
            Machine machine = facade.NewMachine(type);
            return RegisterNode(facade.Graph.GetNodeFor(machine), position);
        }

        public GUINode RegisterNode(GraphNode<Machine> graphNode, Point position)
        {
            GUINode guiNode = new GUINode(this, graphNode, position);
            nodes[graphNode] = guiNode;
            guiNode.MachineGUI = CreateMachineGUIFor(graphNode.Machine.GetType());
            guiNode.MachineGUI.Node = graphNode;
            return guiNode;
        }

        public void Remove(GUINode node)
        {
            nodes.Remove(node.GraphNode);

            List<GUIPort> ports = new List<GUIPort>(node.UsedPorts.Values);
            foreach (GUIPort port in ports)
                Disconnect(port);

            facade.RemoveMachine(node.GraphNode.Machine);
        }

        public void RegisterPort(GUIPort port)
        {
            ports[port.GraphPort] = port;
        }

        public void DeregisterPort(GraphPort<Machine> port)
        {
            ports.Remove(port);
        }

        public void AddAll(IEnumerable<GraphNode<Machine>> list, Size size)
        {
            Random random = new Random();
            foreach (GraphNode<Machine> graphNode in list)
            {
                Point pos = new Point(random.Next(size.Width - GUINode.RADIUS * 4) + GUINode.RADIUS * 2, random.Next(size.Height - GUINode.RADIUS * 4) + GUINode.RADIUS * 2);
                RegisterNode(graphNode, pos);
            }
        }

        public void LoadMachineGUITypes(string dllDirectory)
        {
            machineGUITypes = new Dictionary<Type, Type>();

            foreach (String fileName in Directory.GetFiles(dllDirectory, "*.dll", SearchOption.AllDirectories))
                foreach (Type guiType in Assembly.LoadFile(fileName).GetTypes())
                    if (guiType.IsSubclassOf(typeof(MachineGUI)))
                    {
                        if (guiType.GetCustomAttributes(typeof(GUIForMachine), false).Length == 1)
                        {
                            string name = ((GUIForMachine)guiType.GetCustomAttributes(typeof(GUIForMachine), false)[0]).Value;
                            Type machineType = facade.MachineTypes[name];
                            machineGUITypes[machineType] = guiType;
                        }
                    }
        }

        public bool HasMachineGUIFor(Type type)
        {
            return machineGUITypes.ContainsKey(type);
        }

        public MachineGUI CreateMachineGUIFor(Type type)
        {
            MachineGUI gui = null;
            if (machineGUITypes.ContainsKey(type))
                gui = (MachineGUI)Activator.CreateInstance(machineGUITypes[type]);
            else
            {
                machineGUITypes[type] = typeof(MachineGUI);
                gui = new MachineGUI();
            }
            return gui;
        }

        public MachineGUI CreateMachineGUIFor(string type)
        {
            return CreateMachineGUIFor(facade.MachineTypes[type]);
        }

        public GUINode GetNodeAt(Point point)
        {
            foreach (GUINode node in nodes.Values)
            {
                if (PointDistance(point, node.Position) < GUINode.RADIUS)
                    return node;
            }

            return null;
        }

        public GUIPort GetPortAt(Point point)
        {
            foreach (GUIPort port in ports.Values)
            {
                if (port.Position != null)
                    if (PointDistance(point, port.Position.Value) < GUIPort.RADIUS)
                        return port;
            }

            return null;
        }

        public void Connect(GUIPort fromPort, GUIPort toPort)
        {
            GUINode fromNode = fromPort.Node;
            GUINode toNode = toPort.Node;

            facade.Connect(fromNode.GraphNode.Machine, fromPort.PortNumber, toNode.GraphNode.Machine, toPort.PortNumber);

            GraphPort<Machine> fromGraphPort = fromNode.GraphNode.Outports[fromPort.PortNumber];
            GraphPort<Machine> toGraphPort = toNode.GraphNode.Inports[toPort.PortNumber];

            fromNode.RegisterPortAsInUse(fromPort, fromGraphPort);
            toNode.RegisterPortAsInUse(toPort, toGraphPort);
        }

        public void Disconnect(GUIPort port)
        {
            GUIPort remotePort = port.RemotePort;

            if (port.Direction == GUIPort.Directions.OUT)
                facade.Disconnect(port.Node.GraphNode.Machine, port.PortNumber, remotePort.Node.GraphNode.Machine, remotePort.PortNumber);
            else
                facade.Disconnect(remotePort.Node.GraphNode.Machine, remotePort.PortNumber, port.Node.GraphNode.Machine, port.PortNumber);

            port.Node.DisconnectPort(port);
            remotePort.Node.DisconnectPort(remotePort);
        }

        private float PointDistance(Point p1, Point p2)
        {
            float x = p1.X - p2.X;
            float y = p1.Y - p2.Y;

            return (float)Math.Sqrt(x * x + y * y);
        }

        public string SerializeLayout()
        {
            List<GraphNode<Machine>> nodes = facade.Graph.GetTopologicalOrdering();

            string text = "Layout {\n";

            foreach (GraphNode<Machine> gNode in nodes)
            {
                GUINode node = GuiNodeFor(gNode);
                string machineString = String.Format("\t'machine{0}' {1}, {2}\n",
                    nodes.IndexOf(gNode),
                    node.Position.X,
                    node.Position.Y);

                text += machineString;
            }

            text += "}";

            return text;
        }

        public void DeserializeLayout(string input)
        {
            List<GraphNode<Machine>> nodes = facade.Graph.GetTopologicalOrdering();

            string data = input.Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ');
            Dictionary<string, string> sections = ImagineFileFormat.ExtractSections(data);
            if (sections.ContainsKey("Layout"))
            {
                string layoutData = sections["Layout"];
                Group machineGroup = Regex.Match(layoutData, "^\\s*((?<machine>'[^']+'\\s*\\d+\\s*,\\s*\\d+)\\s*)*$").Groups["machine"];
                foreach (Capture capture in machineGroup.Captures)
                {
                    string machineData = capture.Value;
                    Match machineMatch = Regex.Match(machineData, "^'machine(?<index>[^']+)'\\s*(?<x>\\d+)\\s*,\\s*(?<y>\\d+)$");
                    int machineIndex = int.Parse(machineMatch.Groups["index"].Value);
                    int xPos = int.Parse(machineMatch.Groups["x"].Value);
                    int yPos = int.Parse(machineMatch.Groups["y"].Value);
                    if (machineIndex < nodes.Count)
                        GuiNodeFor(nodes[machineIndex]).Position = new Point(xPos, yPos);
                }
            }
        }
    }

    public class GUINode
    {
        public const int RADIUS = 25;

        private GraphNode<Machine> graphNode;
        internal GraphNode<Machine> GraphNode
        {
            get { return graphNode; }
            set { graphNode = value; }
        }

        private Dictionary<GraphPort<Machine>, GUIPort> usedPorts = new Dictionary<GraphPort<Machine>,GUIPort>();
        public Dictionary<GraphPort<Machine>, GUIPort> UsedPorts
        {
            get { return usedPorts; }
        }

        private Dictionary<int, GUIPort> unusedInports = new Dictionary<int, GUIPort>();
        public Dictionary<int, GUIPort> UnusedInports
        {
            get { return unusedInports; }
        }

        private Dictionary<int, GUIPort> unusedOutports = new Dictionary<int, GUIPort>();
        public Dictionary<int, GUIPort> UnusedOutports
        {
            get { return unusedOutports; }
        }

        private IGUIGraphInternal graph;
        public IGUIGraph Graph
        {
            get { return graph; }
        }
        internal IGUIGraphInternal InternalGraph
        {
            get { return graph; }
        }


        private Point position;
        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        public Machine Machine
        {
            get { return graphNode.Machine; }
        }

        private MachineGUI machineGUI;
        public MachineGUI MachineGUI
        {
            get { return machineGUI; }
            set { machineGUI = value; }
        }

        internal GUINode(IGUIGraphInternal guiGraph, GraphNode<Machine> graphNode, Point position)
        {
            this.graph = guiGraph;
            this.graphNode = graphNode;
            this.position = position;

            for (int i = 0; i < graphNode.Machine.InputCount; i++)
                unusedInports[i] = new GUIPort(this, null, GUIPort.Directions.IN, i);

            for (int i = 0; i < graphNode.Machine.OutputCount; i++)
                unusedOutports[i] = new GUIPort(this, null, GUIPort.Directions.OUT, i);

            foreach (GraphPort<Machine> graphPort in graphNode.Inports.Values)
                RegisterPortAsInUse(unusedInports[graphPort.PortNumber], graphPort);

            foreach (GraphPort<Machine> graphPort in graphNode.Outports.Values)
                RegisterPortAsInUse(unusedOutports[graphPort.PortNumber], graphPort);
        }

        public void UpdatePortPositions()
        {
            int radiusToBubbleCenter = GUINode.RADIUS + GUIPort.RADIUS;
            float bubbleAngle = (float)(2.0 * Math.Atan2(GUIPort.RADIUS, radiusToBubbleCenter));

            CirkularGeometricDistributor<GUIPort> distributor = new CirkularGeometricDistributor<GUIPort>(2.0 * Math.PI, bubbleAngle);
            foreach (GUIPort thisPort in usedPorts.Values)
            {
                Point thisPoint = this.position;
                Point remotePoint = thisPort.RemotePort.Node.Position;
                if (thisPoint == remotePoint)
                    thisPort.Position = null;
                else
                {
                    PointF unitVector = CalculateUnitVector(thisPoint, remotePoint);
                    double angle = Math.Atan2(unitVector.Y, unitVector.X);
                    distributor.AddUnit(thisPort, angle);
                }
            }

            Dictionary<GUIPort, double> portPositions = distributor.getPositions();
            foreach (KeyValuePair<GUIPort, double> pair in portPositions)
            {
                GUIPort port = pair.Key;
                float angle = (float)pair.Value;
                port.Position = new Point(position.X + (int)(radiusToBubbleCenter * Math.Cos(angle)),
                                          position.Y + (int)(radiusToBubbleCenter * Math.Sin(angle)));
            }
        }

        private static PointF CalculateUnitVector(Point from, Point to)
        {
            PointF unitVector = new PointF(to.X - from.X, to.Y - from.Y);
            float vectorLength = (float)Math.Sqrt(unitVector.X * unitVector.X + unitVector.Y * unitVector.Y);
            unitVector.X = unitVector.X / vectorLength;
            unitVector.Y = unitVector.Y / vectorLength;
            return unitVector;
        }

        internal void RegisterPortAsInUse(GUIPort port, GraphPort<Machine> graphPort)
        {
            Dictionary<int, GUIPort> portDictionary = (port.Direction == GUIPort.Directions.OUT) ? unusedOutports : unusedInports;

            port.GraphPort = graphPort;
            usedPorts[graphPort] = port;
            portDictionary.Remove(port.PortNumber);

            graph.RegisterPort(port);
        }

        internal void DisconnectPort(GUIPort port)
        {
            graph.DeregisterPort(port.GraphPort);
            
            Dictionary<int, GUIPort> portDictionary = (port.Direction == GUIPort.Directions.OUT) ? unusedOutports : unusedInports;
            
            usedPorts.Remove(port.GraphPort);
            port.GraphPort = null;
            port.Position = null;
            portDictionary[port.PortNumber] = port;
        }
    }

    public class GUIPort
    {
        public const int RADIUS = 7;

        public enum Directions { IN, OUT };

        private GUINode node;
        public GUINode Node
        {
            get { return node; }
        }

        private GraphPort<Machine> graphPort;
        internal GraphPort<Machine> GraphPort
        {
            get { return graphPort; }
            set { graphPort = value; }
        }

        private Point? position;
        public Point? Position
        {
            get { return position; }
            set { position = value; }
        }

        private Directions direction;
        public Directions Direction
        {
            get { return direction; }
        }

        private int portNumber;
        public int PortNumber
        {
            get { return portNumber; }
        }

        public GUIPort RemotePort
        {
            get { return node.InternalGraph.GuiPortFor(graphPort.RemotePort); }
        }

        public string Code
        {
            get {
                return (direction == Directions.OUT) ? node.Machine.OutputCodes[portNumber].ToString() : node.Machine.InputCodes[portNumber].ToString();
            }
        }

        public string Name
        {
            get
            {
                return (direction == Directions.OUT) ? node.Machine.OutputNames[portNumber].ToString() : node.Machine.InputNames[portNumber].ToString();
            }
        }

        public GUIPort(GUINode node, GraphPort<Machine> graphPort, Directions direction, int portNumber)
        {
            this.node = node;
            this.graphPort = graphPort;
            this.direction = direction;
            this.portNumber = portNumber;
        }
    }
}
