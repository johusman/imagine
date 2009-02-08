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
    interface IGUIGraph
    {
        Dictionary<GraphNode<Machine>, GUINode>.ValueCollection Nodes { get; }
        Dictionary<GraphPort<Machine>, GUIPort>.ValueCollection Ports { get; }

        GUINode GuiNodeFor(GraphNode<Machine> node);
        GUIPort GuiPortFor(GraphPort<Machine> port);

        GUINode CreateNode(string type, Point position);

        GUINode Add(GraphNode<Machine> node, Point position);
        void Remove(GraphNode<Machine> node);
        void Remove(GUINode node);
        GUIPort Add(GraphPort<Machine> port, bool outport);
        void Remove(GraphPort<Machine> port);
        void Remove(GUIPort port);
        void AddAll(IEnumerable<GraphNode<Machine>> list, Size size);

        bool HasMachineGUIFor(Type type);
        MachineGUI CreateMachineGUIFor(Type type);
        MachineGUI CreateMachineGUIFor(string type);
        GUINode GetNodeAt(Point point);
        GUIPort GetPortAt(Point point);

        void Connect(GUINode fromNode, int fromPort, GUINode toNode, int toPort);
        void Disconnect(GraphPort<Machine> port);
        void Disconnect(GUIPort port);

        string SerializeLayout();
        void DeserializeLayout(string input);
    }

    class GUIGraph : IGUIGraph
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
            return Add(facade.Graph.GetNodeFor(machine), position);
        }

        public GUINode Add(GraphNode<Machine> node, Point position)
        {
            GUINode guiNode = new GUINode(this, node, position);
            nodes[node] = guiNode;
            guiNode.MachineGUI = CreateMachineGUIFor(node.Machine.GetType());
            guiNode.MachineGUI.Node = node;
            foreach (GraphPort<Machine> port in node.Inports.Values)
                this.Add(port, false);
            foreach (GraphPort<Machine> port in node.Outports.Values)
                this.Add(port, true);
            return guiNode;
        }

        public void Remove(GraphNode<Machine> node)
        {
            nodes.Remove(node);
            List<GraphPort<Machine>> graphPorts = new List<GraphPort<Machine>>(node.Inports.Values);
            graphPorts.AddRange(node.Outports.Values);
            foreach (GraphPort<Machine> port in graphPorts)
                Disconnect(port);

            facade.RemoveMachine(node.Machine);
        }

        public void Remove(GUINode node)
        {
            Remove(node.GraphNode);
        }

        public GUIPort Add(GraphPort<Machine> port, bool outport)
        {
            GUINode node = this.GuiNodeFor(port.Node);
            GUIPort guiPort = node.AddPort(port, outport ? GUIPort.Directions.OUT : GUIPort.Directions.IN);
            ports[port] = guiPort;
            return guiPort;
        }

        public void Remove(GraphPort<Machine> port)
        {
            ports[port].Node.Ports.Remove(port);
            ports.Remove(port);
        }

        public void Remove(GUIPort port)
        {
            Remove(port.GraphPort);
        }

        public void AddAll(IEnumerable<GraphNode<Machine>> list, Size size)
        {
            Random random = new Random();
            foreach (GraphNode<Machine> gNode in list)
            {
                Point pos = new Point(random.Next(size.Width - GUINode.MACHINE_R * 4) + GUINode.MACHINE_R * 2, random.Next(size.Height - GUINode.MACHINE_R * 4) + GUINode.MACHINE_R * 2);
                Add(gNode, pos);
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
                if (PointDistance(point, node.Position) < GUINode.MACHINE_R)
                    return node;
            }

            return null;
        }

        public GUIPort GetPortAt(Point point)
        {
            foreach (GUIPort port in ports.Values)
            {
                if (port.Position != null)
                    if (PointDistance(point, port.Position.Value) < GUIPort.BUBBLE_R)
                        return port;
            }

            return null;
        }

        public void Connect(GUINode fromNode, int fromPort, GUINode toNode, int toPort)
        {
            facade.Connect(fromNode.GraphNode.Machine, fromPort, toNode.GraphNode.Machine, toPort);

            GraphPort<Machine> fromGPort = fromNode.GraphNode.Outports[fromPort];
            GraphPort<Machine> toGPort = toNode.GraphNode.Inports[toPort];

            ports[fromGPort] = fromNode.AddPort(fromGPort, GUIPort.Directions.OUT);
            ports[toGPort] = toNode.AddPort(toGPort, GUIPort.Directions.IN);
        }

        public void Disconnect(GraphPort<Machine> gPort)
        {
            GraphPort<Machine> remoteGPort = gPort.RemotePort;

            GUIPort thisPort = ports[gPort];
            GUIPort remotePort = ports[remoteGPort];
            thisPort.Node.RemovePort(thisPort);
            remotePort.Node.RemovePort(remotePort);
            ports.Remove(gPort);
            ports.Remove(remoteGPort);

            if (thisPort.Direction == GUIPort.Directions.OUT)
                facade.Disconnect(gPort.Node.Machine, gPort.PortNumber, remoteGPort.Node.Machine, remoteGPort.PortNumber);
            else
                facade.Disconnect(remoteGPort.Node.Machine, remoteGPort.PortNumber, gPort.Node.Machine, gPort.PortNumber);
        }

        public void Disconnect(GUIPort port)
        {
            Disconnect(port.GraphPort);
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

    class GUINode
    {
        public const int MACHINE_R = 25;

        private GraphNode<Machine> graphNode;
        public GraphNode<Machine> GraphNode
        {
            get { return graphNode; }
            set { graphNode = value; }
        }

        private Dictionary<GraphPort<Machine>, GUIPort> ports = new Dictionary<GraphPort<Machine>,GUIPort>();
        public Dictionary<GraphPort<Machine>, GUIPort> Ports
        {
            get { return ports; }
            set { ports = value; }
        }

        private IGUIGraph graph;
        public IGUIGraph Graph
        {
            get { return graph; }
            set { graph = value; }
        }

        private Point position;
        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        private MachineGUI machineGUI;
        public MachineGUI MachineGUI
        {
            get { return machineGUI; }
            set { machineGUI = value; }
        }

        public GUINode(IGUIGraph guiGraph, GraphNode<Machine> graphNode, Point position)
        {
            this.graph = guiGraph;
            this.graphNode = graphNode;
            this.position = position;
        }

        public void UpdatePortPositions()
        {
            int radiusToBubbleCenter = GUINode.MACHINE_R + GUIPort.BUBBLE_R;
            float bubbleAngle = (float)(2.0 * Math.Atan2(GUIPort.BUBBLE_R, radiusToBubbleCenter));

            CirkularGeometricDistributor<GUIPort> distributor = new CirkularGeometricDistributor<GUIPort>(2.0 * Math.PI, bubbleAngle);
            foreach (GraphPort<Machine> fromPort in graphNode.Outports.Values)
            {
                GraphPort<Machine> toPort = fromPort.RemotePort;
                Point from = this.position;
                Point to = graph.GuiNodeFor(toPort.Node).Position;
                if (from == to)
                    ports[fromPort].Position = null;
                else
                {
                    PointF unitVector = CalculateUnitVector(from, to);
                    double angle = Math.Atan2(unitVector.Y, unitVector.X);
                    distributor.AddUnit(ports[fromPort], angle);
                }
            }
            foreach (GraphPort<Machine> toPort in graphNode.Inports.Values)
            {
                GraphPort<Machine> fromPort = toPort.RemotePort;
                Point from = graph.GuiNodeFor(fromPort.Node).Position;
                Point to = this.position;
                if (from == to)
                    ports[toPort].Position = null;
                else
                {
                    PointF unitVector = CalculateUnitVector(to, from);
                    double angle = Math.Atan2(unitVector.Y, unitVector.X);
                    distributor.AddUnit(ports[toPort], angle);
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

        internal GUIPort AddPort(GraphPort<Machine> port, GUIPort.Directions direction)
        {
            GUIPort guiPort = new GUIPort(this, port, direction);
            ports[port] = guiPort;
            return guiPort;
        }

        internal void RemovePort(GUIPort remotePort)
        {
            ports.Remove(remotePort.GraphPort);
        }
    }

    class GUIPort
    {
        public const int BUBBLE_R = 7;

        public enum Directions { IN, OUT };

        private GUINode node;
        public GUINode Node
        {
            get { return node; }
            set { node = value; }
        }

        private GraphPort<Machine> graphPort;
        public GraphPort<Machine> GraphPort
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
            set { direction = value; }
        }

        public GUIPort RemotePort
        {
            get { return this.node.Graph.GuiPortFor(graphPort.RemotePort); }
        }

        public string OutputCode
        {
            get { return node.GraphNode.Machine.OutputCodes[graphPort.PortNumber].ToString(); }
        }

        public string InputCode
        {
            get { return node.GraphNode.Machine.InputCodes[graphPort.PortNumber].ToString(); }
        }

        public GUIPort(GUINode node, GraphPort<Machine> graphPort, Directions direction)
        {
            this.node = node;
            this.graphPort = graphPort;
            this.direction = direction;
        }
    }
}
