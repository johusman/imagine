using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Imagine.Library;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Drawing.Imaging;

namespace Imagine.GUI
{
    public partial class GraphArea : UserControl
    {
        const int MACHINE_R = 25;
        const int ARROW_L = 10;
        const int ARROW_W = 5;
        const int BUBBLE_R = 7;

        public GraphArea()
        {
            InitializeComponent();
        }

        private Dictionary<GraphNode<Machine>, Point> machinePositions;
        private Dictionary<GraphPort<Machine>, Point?> outportPositions;
        private Dictionary<GraphPort<Machine>, Point?> inportPositions;
        private Graph<Machine> graph;
        private ImagineFacade facade;
        private Dictionary<Type, Type> machineGUITypes;
        private Dictionary<GraphNode<Machine>, MachineGUI> machineGUIs;

        private Pen machinepen = new Pen(Color.Gray, 1);
        private Pen machinePenPotential = new Pen(Color.Gray, 1);
        private Pen arrowpen = new Pen(Color.Black, 1);
        private Pen arrowPenPotential = new Pen(Color.Gray, 1);
        private Brush arrowbrush = Brushes.Black;
        private Brush arrowBrushPotential = Brushes.Gray;
        private Brush machinebrush = Brushes.Bisque;
        private Color tooltipColor = Color.OldLace;

        private enum ManipulationState { None, Dragging, Inserting, Connecting };
        private ManipulationState manipulationState = ManipulationState.None;
        private GraphNode<Machine> manipulatedNode = null;
        private Point manipulationOffset;
        private GraphNode<Machine> manipulationDestination = null;
        private int choosenPort = -1;

        private bool showTooltips = true;
        private ToolTip tooltip = null;
        private Object tooltipObject = null;

        public Graph<Machine> Graph
        {
            get { return graph; }
            set
            { 
                graph = value;

                if (graph != null)
                {
                    inportPositions = new Dictionary<GraphPort<Machine>, Point?>();
                    outportPositions = new Dictionary<GraphPort<Machine>, Point?>();
                    machinePositions = new Dictionary<GraphNode<Machine>, Point>();
                    Random random = new Random();
                    List<GraphNode<Machine>> nodes = graph.GetTopologicalOrdering();
                    foreach (GraphNode<Machine> node in nodes)
                    {
                        Point p = new Point(random.Next(this.Width - MACHINE_R * 4) + MACHINE_R * 2, random.Next(this.Height - MACHINE_R * 4) + MACHINE_R * 2);
                        machinePositions[node] = p;
                    }
                }
            }
        }

        public ImagineFacade Facade
        {
            get { return facade; }
            set
            {
                facade = value;
                if (facade != null)
                {
                    Graph = facade.Graph;
                    
                    List<string> uniqueNames = new List<string>(facade.MachineTypes.Keys);
                    uniqueNames.Remove("Imagine.Source");
                    uniqueNames.Remove("Imagine.Destination");

                    LoadMachineGUITypes(facade.WorkingDirectory);

                    ConstructNewMachineMenu(uniqueNames);
                }
            }
        }

        private void ConstructNewMachineMenu(List<string> uniqueNames)
        {
            uniqueNames.Sort();

            this.contextMenu.Items.Clear();
            ToolStripMenuItem header = new ToolStripMenuItem("New machine:");
            header.Font = new Font(header.Font, FontStyle.Bold);
            header.Enabled = false;
            this.contextMenu.Items.Add(header);

            ToolStripItemCollection currentParentCollection = this.contextMenu.Items;
            Stack<ToolStripItemCollection> collectionStack = new Stack<ToolStripItemCollection>();
            int lastLevel = 0;
            string[] lastParts = new string[0];
            
            foreach (string uniqueName in uniqueNames)
            {
                string[] nameParts = uniqueName.Split('.');
                int level = nameParts.Length - 1;
                string text = nameParts[level];

                int matchLevel = 0;
                for (int i = 0; i < level && i < lastLevel; i++)
                    if (nameParts[i] == lastParts[i])
                        matchLevel++;
                    else
                        break;

                int tempLevel = lastLevel;
                while (tempLevel > matchLevel)
                {
                    tempLevel--;
                    currentParentCollection = collectionStack.Pop();
                }

                while (tempLevel < level)
                {
                    tempLevel++;
                    ToolStripMenuItem newGroupItem = new ToolStripMenuItem();
                    newGroupItem.Text = nameParts[tempLevel-1];
                    currentParentCollection.Add(newGroupItem);
                    collectionStack.Push(currentParentCollection);
                    currentParentCollection = newGroupItem.DropDownItems;
                }

                Image image = null;
                if(machineGUITypes.ContainsKey(facade.MachineTypes[uniqueName]))
                    image = ((MachineGUI)Activator.CreateInstance(machineGUITypes[facade.MachineTypes[uniqueName]])).HalfDimmedBitmap;
                
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Tag = uniqueName;
                item.Text = text;
                item.Image = image;
                item.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                item.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
                currentParentCollection.Add(item);

                lastLevel = level;
                lastParts = nameParts;
            }
        }

        private void LoadMachineGUITypes(string workingDirectory)
        {
            machineGUITypes = new Dictionary<Type, Type>();
            machineGUIs = new Dictionary<GraphNode<Machine>,MachineGUI>();

            foreach (String fileName in Directory.GetFiles(workingDirectory, "*.dll", SearchOption.AllDirectories))
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

        // If we can figure out exactly when to add/remove these mappings, this function wouldn't have to exist
        private MachineGUI GUIForNode(GraphNode<Machine> node)
        {
            MachineGUI gui = null;
            if (!machineGUIs.TryGetValue(node, out gui))
            {
                if(machineGUITypes.ContainsKey(node.Machine.GetType()))
                    gui = (MachineGUI)Activator.CreateInstance(machineGUITypes[node.Machine.GetType()]);
                else
                {
                    machineGUITypes[node.Machine.GetType()] = typeof(MachineGUI);
                    gui = new MachineGUI();
                }
                gui.Node = node;
                machineGUIs[node] = gui;
            }

            return gui;
        }

        public bool ShowTooltips
        {
            get { return showTooltips; }
            set { showTooltips = value; }
        }

        public void DrawGraph(Graphics graphics)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            CalculatePortPositions();

            foreach(GraphNode<Machine> node in graph.GetTopologicalOrdering())
            {
                foreach(GraphPort<Machine> outputPort in node.Outports.Values)
                    DrawOutgoingConnection(graphics, outputPort, outputPort.RemotePort);

                DrawMachine(graphics, node);
            }
            if(manipulationState == ManipulationState.Connecting)
                DrawConnector(graphics);
        }

        private void CalculatePortPositions()
        {
            int radiusToBubbleCenter = MACHINE_R + BUBBLE_R;
            float bubbleAngle = (float)(2.0 * Math.Atan2(BUBBLE_R, radiusToBubbleCenter));

            foreach (GraphNode<Machine> node in graph.GetTopologicalOrdering())
            {
                CirkularGeometricDistributor<GraphPort<Machine>> distributor = new CirkularGeometricDistributor<GraphPort<Machine>>(2.0 * Math.PI, bubbleAngle);
                foreach (GraphPort<Machine> fromPort in node.Outports.Values)
                {
                    GraphPort<Machine> toPort = fromPort.RemotePort;
                    Point from = machinePositions[fromPort.Node];
                    Point to = machinePositions[toPort.Node];
                    if (from == to)
                        outportPositions[fromPort] = null;
                    else
                    {
                        PointF unitVector = CalculateUnitVector(from, to);
                        double angle = Math.Atan2(unitVector.Y, unitVector.X);
                        distributor.AddUnit(fromPort, angle);
                    }
                }
                foreach (GraphPort<Machine> toPort in node.Inports.Values)
                {
                    GraphPort<Machine> fromPort = toPort.RemotePort;
                    Point from = machinePositions[fromPort.Node];
                    Point to = machinePositions[toPort.Node];
                    if (from == to)
                        inportPositions[toPort] = null;
                    else
                    {
                        PointF unitVector = CalculateUnitVector(to, from);
                        double angle = Math.Atan2(unitVector.Y, unitVector.X);
                        distributor.AddUnit(toPort, angle);
                    }
                }

                Dictionary<GraphPort<Machine>, double> portPositions = distributor.getPositions();
                foreach (KeyValuePair<GraphPort<Machine>, double> pair in portPositions)
                {
                    GraphPort<Machine> port = pair.Key;
                    Point nodePos = machinePositions[node];
                    float angle = (float)pair.Value;
                    Point point = new Point(nodePos.X + (int)(radiusToBubbleCenter * Math.Cos(angle)),
                                            nodePos.Y + (int)(radiusToBubbleCenter * Math.Sin(angle)));

                    if (node.Outports.ContainsValue(port))
                        outportPositions[port] = point;
                    else
                        inportPositions[port] = point;
                }
            }
        }

        private void DrawMachine(Graphics graphics, GraphNode<Machine> node)
        {
            Point p = machinePositions[node];
            MachineGUI gui = GUIForNode(node);
            
            graphics.FillEllipse(gui.Background, p.X - MACHINE_R, p.Y - MACHINE_R, MACHINE_R * 2, MACHINE_R * 2);
            graphics.DrawEllipse(machinepen, p.X - MACHINE_R, p.Y - MACHINE_R, MACHINE_R * 2, MACHINE_R * 2);
            
            
            if(gui.DimmedBitmap != null)
                graphics.DrawImage(GUIForNode(node).DimmedBitmap, p.X - 16, p.Y - 16);
            
            SizeF textSize = graphics.MeasureString(node.Machine.Caption, Font);
            graphics.DrawString(node.Machine.Caption, Font, arrowbrush, p.X - textSize.Width/2 + 1, p.Y - textSize.Height/2);
        }

        private void DrawOutgoingConnection(Graphics graphics, GraphPort<Machine> port, GraphPort<Machine> remotePort)
        {
            if (outportPositions[port] == null)
                return;

            Point portPos = outportPositions[port].Value;
            GraphNode<Machine> node = port.Node;
            GraphNode<Machine> remoteNode = remotePort.Node;
            Point remotePortPos = inportPositions[remotePort].Value;

            DrawCenteredCircle(graphics, machinepen, Brushes.Black, Brushes.White,
                    new PointF(portPos.X, portPos.Y),
                    BUBBLE_R, node.Machine.OutputCodes[port.PortNumber].ToString(), new Font(FontFamily.GenericMonospace, 7.0f));

            DrawCenteredCircle(graphics, machinepen, Brushes.White, Brushes.Black,
                    new PointF(remotePortPos.X, remotePortPos.Y),
                    BUBBLE_R, remoteNode.Machine.InputCodes[remotePort.PortNumber].ToString(), new Font(FontFamily.GenericMonospace, 7.0f));

            PointF unitVector = CalculateUnitVector(portPos, remotePortPos);
            if (PointDistance(portPos, remotePortPos) > BUBBLE_R * 2.0)
            {
                Point lineFromPoint = new Point((int)(portPos.X + unitVector.X * BUBBLE_R), (int)(portPos.Y + unitVector.Y * BUBBLE_R));
                Point lineToPoint = new Point((int)(remotePortPos.X - unitVector.X * BUBBLE_R), (int)(remotePortPos.Y - unitVector.Y * BUBBLE_R));
                graphics.DrawLine(arrowpen, lineFromPoint, lineToPoint);

                Point[] arrowpoints = {
                    lineToPoint,
                    new Point(remotePortPos.X - (int) (unitVector.X * (ARROW_L + BUBBLE_R) + unitVector.Y * ARROW_W), remotePortPos.Y - (int) (unitVector.Y * (ARROW_L + BUBBLE_R) - unitVector.X * ARROW_W)),
                    new Point(remotePortPos.X - (int) (unitVector.X * (ARROW_L + BUBBLE_R) - unitVector.Y * ARROW_W), remotePortPos.Y - (int) (unitVector.Y * (ARROW_L + BUBBLE_R) + unitVector.X * ARROW_W))
                };

                graphics.FillPolygon(arrowbrush, arrowpoints);
            }
        }

        private void DrawPotentialConnection(Graphics graphics, GraphPort<Machine> fromPort, GraphPort<Machine> toPort)
        {
            Point from = machinePositions[fromPort.Node];
            Point to = machinePositions[toPort.Node];

            PointF unitVector = CalculateUnitVector(from, to);
            int arrow_offset = MACHINE_R + BUBBLE_R * 2;

            graphics.DrawLine(arrowPenPotential,
                from.X + unitVector.X * arrow_offset, from.Y + unitVector.Y * arrow_offset,
                to.X - unitVector.X * arrow_offset, to.Y - unitVector.Y * arrow_offset);

            Point[] arrowpoints = {
                new Point(to.X - (int) (unitVector.X * arrow_offset), to.Y - (int) (unitVector.Y * arrow_offset)),
                new Point(to.X - (int) (unitVector.X * (ARROW_L + arrow_offset) + unitVector.Y * ARROW_W), to.Y - (int) (unitVector.Y * (ARROW_L + arrow_offset) - unitVector.X * ARROW_W)),
                new Point(to.X - (int) (unitVector.X * (ARROW_L + arrow_offset) - unitVector.Y * ARROW_W), to.Y - (int) (unitVector.Y * (ARROW_L + arrow_offset) + unitVector.X * ARROW_W))
            };

            graphics.FillPolygon(arrowBrushPotential, arrowpoints);

            DrawCenteredCircle(graphics, machinePenPotential, Brushes.Transparent, Brushes.Black, new PointF(
                to.X - (unitVector.X * (MACHINE_R + BUBBLE_R)),
                to.Y - (unitVector.Y * (MACHINE_R + BUBBLE_R))),
                BUBBLE_R, "");

            DrawCenteredCircle(graphics, machinePenPotential, Brushes.Transparent, Brushes.White, new PointF(
                from.X + (unitVector.X * (MACHINE_R + BUBBLE_R)),
                from.Y + (unitVector.Y * (MACHINE_R + BUBBLE_R))),
                BUBBLE_R, "");
        }

        private static PointF CalculateUnitVector(Point from, Point to)
        {
            PointF unitVector = new PointF(to.X - from.X, to.Y - from.Y);
            float vectorLength = (float)Math.Sqrt(unitVector.X * unitVector.X + unitVector.Y * unitVector.Y);
            unitVector.X = unitVector.X / vectorLength;
            unitVector.Y = unitVector.Y / vectorLength;
            return unitVector;
        }

        private void DrawCenteredCircle(Graphics g, Pen pen, Brush brush, Brush textBrush, PointF p, float radius, string text)
        {
            DrawCenteredCircle(g, pen, brush, textBrush, p, radius, text, Font);
        }

        private void DrawCenteredCircle(Graphics g, Pen pen, Brush brush, Brush textBrush, PointF p, float radius, string text, Font font)
        {
            g.FillEllipse(brush, p.X - radius, p.Y - radius, radius * 2, radius * 2);
            g.DrawEllipse(pen, p.X - radius, p.Y - radius, radius * 2, radius * 2);

            SizeF textSize = g.MeasureString(text, font);
            g.DrawString(text, font, textBrush, p.X - textSize.Width / 2.0f + 1, p.Y - textSize.Height / 2.0f);
        }

        private void DrawConnector(Graphics g)
        {
            GraphNode<Machine> destinationNode = GetMachineAtCoordinate(manipulationOffset);
            if(destinationNode == null)
            {
                Point origin = machinePositions[manipulatedNode];
                PointF unitVector = CalculateUnitVector(origin, manipulationOffset);
                origin.Offset((int)(unitVector.X * MACHINE_R), (int)(unitVector.Y * MACHINE_R));

                g.DrawLine(arrowPenPotential, origin, manipulationOffset);
                g.DrawEllipse(arrowPenPotential, manipulationOffset.X - BUBBLE_R, manipulationOffset.Y - BUBBLE_R, BUBBLE_R * 2, BUBBLE_R * 2);
            }
            else
                DrawPotentialConnection(g, new GraphPort<Machine>(manipulatedNode, -1), new GraphPort<Machine>(destinationNode, -1));
        }

        private void GraphArea_Paint(object sender, PaintEventArgs e)
        {
            if(graph != null)
                DrawGraph(e.Graphics);
        }

        private void GraphArea_MouseDown(object sender, MouseEventArgs e)
        {
            manipulatedNode = GetMachineAtCoordinate(e.Location);
            if(manipulatedNode != null)
            {
                if(e.Button == MouseButtons.Left && e.Clicks == 1)
                {
                    manipulationState = ManipulationState.Dragging;
                    manipulationOffset = Point.Subtract(e.Location, new Size(machinePositions[manipulatedNode]));
                }
                else if (e.Button == MouseButtons.Left && e.Clicks == 2)
                {
                    GUIForNode(manipulatedNode).LaunchSettings(this);
                }
                else if (e.Button == MouseButtons.Right && e.Clicks == 1)
                {
                    manipulationState = ManipulationState.Connecting;
                    manipulationOffset = e.Location;
                }
                else if (e.Button == MouseButtons.Right && e.Clicks == 2)
                {
                    if (Control.ModifierKeys != Keys.Shift)
                    {
                        DialogResult result = MessageBox.Show(this.ParentForm, "Do you wish to delete this machine?", "Delete machine?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.No)
                            return;
                    }

                    machinePositions.Remove(manipulatedNode);
                    foreach (GraphPort<Machine> outPort in manipulatedNode.Outports.Values)
                    {
                        outportPositions.Remove(outPort);
                        inportPositions.Remove(outPort.RemotePort);
                    }
                    foreach (GraphPort<Machine> inPort in manipulatedNode.Inports.Values)
                    {
                        inportPositions.Remove(inPort);
                        outportPositions.Remove(inPort.RemotePort);
                    }
                    facade.RemoveMachine(manipulatedNode.Machine);

                    this.Invalidate();
                }
            }
            else
            {
                GraphPort<Machine> port = GetPortAtCoordinate(e.Location);
                if (port != null)
                {
                    if (e.Button == MouseButtons.Right && e.Clicks == 2)
                    {
                        if(Control.ModifierKeys != Keys.Shift)
                        {
                            DialogResult result = MessageBox.Show(this.ParentForm, "Do you wish to break this connection?", "Break connection?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.No)
                                return;
                        }

                        if (outportPositions.ContainsKey(port))
                        {
                            outportPositions.Remove(port);
                            inportPositions.Remove(port.RemotePort);
                            facade.Disconnect(port.Node.Machine, port.PortNumber, port.RemotePort.Node.Machine, port.RemotePort.PortNumber);
                        }
                        else
                        {
                            outportPositions.Remove(port.RemotePort);
                            inportPositions.Remove(port);
                            facade.Disconnect(port.RemotePort.Node.Machine, port.RemotePort.PortNumber, port.Node.Machine, port.PortNumber);
                        }
                        this.Invalidate();
                    }
                }
                else
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        manipulationState = ManipulationState.Inserting;
                        manipulationOffset = e.Location;
                        contextMenu.Show(this, Point.Subtract(e.Location, new Size(10, 10)));
                    }
                }                    
            }
        }

        private void GraphArea_MouseUp(object sender, MouseEventArgs e)
        {
            if(manipulationState == ManipulationState.Connecting)
            {
                manipulationDestination = GetMachineAtCoordinate(e.Location);
                if(manipulationDestination != null)
                {
                    if(manipulationDestination != manipulatedNode)
                    {
                        manipulationOffset = e.Location;
                        ShowOutputChooser(manipulationOffset);
                    }
                }
            }
            else
                manipulatedNode = null;

            if(manipulationState != ManipulationState.Inserting)
                manipulationState = ManipulationState.None;

            this.Invalidate();
        }

        private void GraphArea_MouseMove(object sender, MouseEventArgs e)
        {
            if(manipulationState == ManipulationState.Dragging)
            {
                KillToolTip();
                machinePositions[manipulatedNode] = Point.Subtract(e.Location, new Size(manipulationOffset));
                this.Invalidate();
            }
            else if (manipulationState == ManipulationState.Connecting)
            {
                ManageToolTip(e.Location);
                manipulationOffset = e.Location;
                this.Invalidate();
            }
            else
            {
                ManageToolTip(e.Location);
            }
        }

        private void ManageToolTip(Point location)
        {
            if (!showTooltips)
                return;

            GraphNode<Machine> node = GetMachineAtCoordinate(location);
            if (node != null)
            {
                if (tooltip == null || tooltipObject != node)
                {
                    KillToolTip();
                    tooltipObject = node;
                    tooltip = new ToolTip();
                    tooltip.BackColor = tooltipColor;
                    tooltip.ToolTipTitle = node.Machine.ToString();

                    string description = node.Machine.Description;
                    string inputs = "";
                    string outputs = "";
                    for (int i = 0; i < node.Machine.InputCount; i++)
                        inputs = inputs + String.Format(" [{1}] {0},", node.Machine.InputNames[i], node.Machine.InputCodes[i]);
                    if (inputs == "")
                        inputs = " (None)";
                    else
                        inputs = inputs.Remove(inputs.Length - 1);

                    for (int i = 0; i < node.Machine.OutputCount; i++)
                        outputs = outputs + String.Format(" [{1}] {0},", node.Machine.OutputNames[i], node.Machine.OutputCodes[i]);
                    if (outputs == "")
                        outputs = " (None)";
                    else
                        outputs = outputs.Remove(outputs.Length - 1);

                    string text = String.Format("{0}\n\nInputs:\n {1}\nOutputs:\n {2}", description, inputs, outputs);
                    tooltip.Show(text, this.ParentForm, new Point(machinePositions[node].X + 50, machinePositions[node].Y));
                }
            }
            else
            {
                GraphPort<Machine> port = GetPortAtCoordinate(location);
                if (port != null)
                {
                    if (tooltip == null || tooltipObject != port)
                    {
                        KillToolTip();
                        tooltipObject = port;
                        tooltip = new ToolTip();
                        tooltip.BackColor = tooltipColor;

                        GraphNode<Machine> portNode = port.Node;
                        GraphNode<Machine> remoteNode = port.RemotePort.Node;
                        int index = port.PortNumber;
                        int remoteIndex = port.RemotePort.PortNumber;
                        string text = null;
                        if (outportPositions.ContainsKey(port))
                        {
                            tooltip.ToolTipTitle = portNode.Machine.OutputNames[index];
                            text = String.Format(" (--> {0})", remoteNode.Machine.InputNames[remoteIndex]);
                            tooltip.Show(text, this.ParentForm, new Point(outportPositions[port].Value.X + 25, outportPositions[port].Value.Y + 50));
                        }
                        else
                        {
                            tooltip.ToolTipTitle = portNode.Machine.InputNames[index];
                            text = String.Format(" (<-- {0})", remoteNode.Machine.OutputNames[remoteIndex]);
                            tooltip.Show(text, this.ParentForm, new Point(inportPositions[port].Value.X + 25, inportPositions[port].Value.Y + 50));
                        }
                        
                    }
                }
                else
                    KillToolTip();
            }
        }

        private void KillToolTip()
        {
            if (tooltip != null)
            {
                tooltip.Hide(this.ParentForm);
                tooltip = null;
                tooltipObject = null;
            }
        }

        private GraphNode<Machine> GetMachineAtCoordinate(Point point)
        {
            List<GraphNode<Machine>> list = graph.GetTopologicalOrdering();
            list.Reverse();
            foreach(GraphNode<Machine> node in list)
            {
                Point machinePoint = machinePositions[node];
                if(PointDistance(point, machinePoint) < MACHINE_R)
                    return node;
            }

            return null;
        }

        private GraphPort<Machine> GetPortAtCoordinate(Point point)
        {
            foreach(KeyValuePair<GraphPort<Machine>, Point?> pair in outportPositions)
            {
                if (PointDistance(point, pair.Value.Value) < BUBBLE_R)
                    return pair.Key;
            }
            foreach (KeyValuePair<GraphPort<Machine>, Point?> pair in inportPositions)
            {
                if (PointDistance(point, pair.Value.Value) < BUBBLE_R)
                    return pair.Key;
            }

            return null;
        }

        private float PointDistance(Point p1, Point p2)
        {
            float x = p1.X - p2.X;
            float y = p1.Y - p2.Y;

            return (float) Math.Sqrt(x * x + y * y);
        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(manipulationState == ManipulationState.Inserting)
            {
                Machine machine = facade.NewMachine((string) ((ToolStripItem)sender).Tag);
                GraphNode<Machine> node = graph.GetNodeFor(machine);
                machinePositions[node] = manipulationOffset;
                manipulationState = ManipulationState.None;
                this.Invalidate();
            }
        }

        private void chooseOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string chosenText = ((ToolStripMenuItem)sender).Text.Substring(4);
            string[] outputNames = manipulatedNode.Machine.OutputNames;
            for(int i = 0; i < outputNames.Length; i++)
                if(outputNames[i] == chosenText)
                {
                    choosenPort = i;
                    ShowInputChooser(manipulationOffset);
                    break;
                }
        }

        private void chooseInputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string chosenText = ((ToolStripMenuItem)sender).Text.Substring(4);
            string[] inputNames = manipulationDestination.Machine.InputNames;
            for(int i = 0; i < inputNames.Length; i++)
                if(inputNames[i] == chosenText)
                {
                    facade.Connect(manipulatedNode.Machine, choosenPort, manipulationDestination.Machine, i);
                    
                    this.Invalidate();
                    manipulatedNode = null;
                    manipulationDestination = null;
                    manipulationOffset = Point.Empty;
                    choosenPort = -1;
                    break;
                }
        }

        private void ShowOutputChooser(Point location)
        {
            int remainingOutputs = manipulatedNode.Machine.OutputCount - manipulatedNode.OutputCount;
            if(remainingOutputs > 1)
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.ShowImageMargin = false;

                ToolStripMenuItem header = new ToolStripMenuItem("Bind from output:");
                header.Font = new Font(header.Font, FontStyle.Bold);
                header.Enabled = false;
                contextMenu.Items.Add(header);

                for(int i = 0; i < manipulatedNode.Machine.OutputCount; i++)
                    if (!manipulatedNode.Outports.ContainsKey(i))
                    {
                        String text = String.Format("({1}) {0}", manipulatedNode.Machine.OutputNames[i], manipulatedNode.Machine.OutputCodes[i]);
                        contextMenu.Items.Add(new ToolStripMenuItem(text, null, new System.EventHandler(this.chooseOutputToolStripMenuItem_Click)));
                    }
                contextMenu.Show(this, Point.Subtract(location, new Size(10, 10)));
            }
            else if (remainingOutputs == 1)
            {
                for (int i = 0; i < manipulatedNode.Machine.OutputCount; i++)
                    if (!manipulatedNode.Outports.ContainsKey(i))
                    {
                        choosenPort = i;
                        ShowInputChooser(manipulationOffset);
                        return;
                    }
            }
            else
            {
                MessageBox.Show(this.ParentForm, "There are no free outputs on the first machine", "No free outputs", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowInputChooser(Point location)
        {
            int remainingInputs = manipulationDestination.Machine.InputCount - manipulationDestination.InputCount;
            if(remainingInputs > 1)
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.ShowImageMargin = false;

                ToolStripMenuItem header = new ToolStripMenuItem("Bind to input:");
                header.Font = new Font(header.Font, FontStyle.Bold);
                header.Enabled = false;
                contextMenu.Items.Add(header);

                for (int i = 0; i < manipulationDestination.Machine.InputCount; i++)
                    if (!manipulationDestination.Inports.ContainsKey(i))
                    {
                        String text = String.Format("({1}) {0}", manipulationDestination.Machine.InputNames[i], manipulationDestination.Machine.InputCodes[i]);
                        contextMenu.Items.Add(new ToolStripMenuItem(text, null, new System.EventHandler(this.chooseInputToolStripMenuItem_Click)));
                    }

                contextMenu.Show(this, Point.Subtract(location, new Size(10, 10)));
            }
            else if (remainingInputs == 1)
            {
                for (int i = 0; i < manipulationDestination.Machine.InputCount; i++)
                    if (!manipulationDestination.Inports.ContainsKey(i))
                    {
                        facade.Connect(manipulatedNode.Machine, choosenPort, manipulationDestination.Machine, i);

                        this.Invalidate();
                        manipulatedNode = null;
                        manipulationDestination = null;
                        manipulationOffset = Point.Empty;
                        choosenPort = -1;
                        return;
                    }
            }
            else
            {
                MessageBox.Show(this.ParentForm, "There are no free inputs on the second machine", "No free inputs", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        public string SerializeLayout()
        {
            List<GraphNode<Machine>> ordering = graph.GetTopologicalOrdering();

            string text = "Layout {\n";

            foreach (GraphNode<Machine> node in ordering)
            {
                string machineString = String.Format("\t'machine{0}' {1}, {2}\n",
                    ordering.IndexOf(node),
                    machinePositions[node].X,
                    machinePositions[node].Y);

                text += machineString;
            }

            text += "}";

            return text;
        }

        public void DeserializeLayout(string input)
        {
            List<GraphNode<Machine>> ordering = graph.GetTopologicalOrdering();

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
                    if(machineIndex < ordering.Count)
                        machinePositions[ordering[machineIndex]] = new Point(xPos, yPos);
                }
            }

            Refresh();
        }
    }
}
