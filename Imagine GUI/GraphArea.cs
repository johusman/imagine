using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Imagine.Library;

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

        private Pen machinepen = new Pen(Color.Gray, 1);
        private Pen machinePenPotential = new Pen(Color.Gray, 1);
        private Pen arrowpen = new Pen(Color.Black, 1);
        private Pen arrowPenPotential = new Pen(Color.Gray, 1);
        private Brush arrowbrush = Brushes.Black;
        private Brush arrowBrushPotential = Brushes.Gray;
        private Brush machinebrush = Brushes.Bisque;

        private enum ManipulationState { None, Dragging, Inserting, Connecting };
        private ManipulationState manipulationState = ManipulationState.None;
        private GraphNode<Machine> manipulatedNode = null;
        private Point manipulationOffset;
        private GraphNode<Machine> manipulationDestination = null;
        private int choosenPort = -1;

        public Graph<Machine> Graph
        {
            get { return graph; }
            set
            { 
                graph = value;

                inportPositions = new Dictionary<GraphPort<Machine>, Point?>();
                outportPositions = new Dictionary<GraphPort<Machine>, Point?>();
                machinePositions = new Dictionary<GraphNode<Machine>, Point>();
                Random random = new Random();
                List<GraphNode<Machine>> nodes = graph.GetTopologicalOrdering();
                foreach(GraphNode<Machine> node in nodes)
                {
                    Point p = new Point(random.Next(this.Width - MACHINE_R * 4) + MACHINE_R * 2, random.Next(this.Height - MACHINE_R * 4) + MACHINE_R * 2);
                    machinePositions[node] = p;
                }
            }
        }

        public ImagineFacade Facade
        {
            get { return facade; }
            set
            {
                facade = value;
                Graph = facade.Graph;
            }
        }

        public void DrawGraph(Graphics graphics)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            CalculatePortPositions();

            foreach(GraphNode<Machine> node in graph.GetTopologicalOrdering())
            {
                //DrawOutgoingConnections(graphics, node);
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
            graphics.FillEllipse(machinebrush, p.X - MACHINE_R, p.Y - MACHINE_R, MACHINE_R * 2, MACHINE_R * 2);
            graphics.DrawEllipse(machinepen, p.X - MACHINE_R, p.Y - MACHINE_R, MACHINE_R * 2, MACHINE_R * 2);
            SizeF textSize = graphics.MeasureString(node.Machine.ToString(), Font);
            graphics.DrawString(node.Machine.ToString(), Font, arrowbrush, p.X - textSize.Width/2 + 1, p.Y - textSize.Height/2);
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
                    BUBBLE_R, node.Machine.OutputCodes[port.PortNumber].ToString());

            DrawCenteredCircle(graphics, machinepen, Brushes.White, Brushes.Black,
                    new PointF(remotePortPos.X, remotePortPos.Y),
                    BUBBLE_R, remoteNode.Machine.InputCodes[remotePort.PortNumber].ToString());

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
            g.FillEllipse(brush, p.X - radius, p.Y - radius, radius * 2, radius * 2);
            g.DrawEllipse(pen, p.X - radius, p.Y - radius, radius * 2, radius * 2);

            SizeF textSize = g.MeasureString(text, Font);
            g.DrawString(text, Font, textBrush, p.X - textSize.Width / 2.0f + 1, p.Y - textSize.Height / 2.0f);
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
                if(e.Button == MouseButtons.Left)
                {
                    manipulationState = ManipulationState.Dragging;
                    manipulationOffset = Point.Subtract(e.Location, new Size(machinePositions[manipulatedNode]));
                }
                else if(e.Button == MouseButtons.Right)
                {
                    manipulationState = ManipulationState.Connecting;
                    manipulationOffset = e.Location;
                }
            }
            else
            {
                if(e.Button == MouseButtons.Right)
                {
                    manipulationState = ManipulationState.Inserting;
                    manipulationOffset = e.Location;
                    contextMenu.Show(this, Point.Subtract(e.Location, new Size(10, 10)));
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
                machinePositions[manipulatedNode] = Point.Subtract(e.Location, new Size(manipulationOffset));
                this.Invalidate();
            }
            else if(manipulationState == ManipulationState.Connecting)
            {
                manipulationOffset = e.Location;
                this.Invalidate();
            }
        }

        private GraphNode<Machine> GetMachineAtCoordinate(Point point)
        {
            foreach(GraphNode<Machine> node in graph.GetTopologicalOrdering())
            {
                Point machinePoint = machinePositions[node];
                if(PointDistance(point, machinePoint) < MACHINE_R)
                    return node;
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
            string[] outputNames = manipulatedNode.Machine.OutputNames;
            for(int i = 0; i < outputNames.Length; i++)
                if(outputNames[i] == ((ToolStripMenuItem)sender).Text)
                {
                    choosenPort = i;
                    ShowInputChooser(manipulationOffset);
                    break;
                }
        }

        private void chooseInputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] inputNames = manipulationDestination.Machine.InputNames;
            for(int i = 0; i < inputNames.Length; i++)
                if(inputNames[i] == ((ToolStripMenuItem)sender).Text)
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
            if(manipulatedNode.Machine.OutputCount > 1)
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.ShowImageMargin = false;

                ToolStripMenuItem header = new ToolStripMenuItem("Bind from output:");
                header.Font = new Font(header.Font, FontStyle.Bold);
                header.Enabled = false;
                contextMenu.Items.Add(header);

                foreach(string outputName in manipulatedNode.Machine.OutputNames)
                    contextMenu.Items.Add(new ToolStripMenuItem(outputName, null, new System.EventHandler(this.chooseOutputToolStripMenuItem_Click)));
                contextMenu.Show(this, Point.Subtract(location, new Size(10, 10)));
            }
            else
            {
                choosenPort = 0;
                ShowInputChooser(manipulationOffset);
            }
        }

        private void ShowInputChooser(Point location)
        {
            if(manipulationDestination.Machine.InputCount > 1)
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.ShowImageMargin = false;

                ToolStripMenuItem header = new ToolStripMenuItem("Bind to input:");
                header.Font = new Font(header.Font, FontStyle.Bold);
                header.Enabled = false;
                contextMenu.Items.Add(header);

                foreach(string inputName in manipulationDestination.Machine.InputNames)
                    contextMenu.Items.Add(new ToolStripMenuItem(inputName, null, new System.EventHandler(this.chooseInputToolStripMenuItem_Click)));
                contextMenu.Show(this, Point.Subtract(location, new Size(10, 10)));
            }
            else
            {
                facade.Connect(manipulatedNode.Machine, choosenPort, manipulationDestination.Machine, 0);

                this.Invalidate();
                manipulatedNode = null;
                manipulationDestination = null;
                manipulationOffset = Point.Empty;
                choosenPort = -1;
            }
        }

    }
}
