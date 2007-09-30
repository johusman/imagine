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

        private Dictionary<GraphNode<Machine>, Point> positions;
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

                Random random = new Random();
                positions = new Dictionary<GraphNode<Machine>, Point>();
                List<GraphNode<Machine>> nodes = graph.GetTopologicalOrdering();
                foreach(GraphNode<Machine> node in nodes)
                {
                    Point p = new Point(random.Next(this.Width - MACHINE_R * 4) + MACHINE_R * 2, random.Next(this.Height - MACHINE_R * 4) + MACHINE_R * 2);
                    positions[node] = p;
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
            foreach(GraphNode<Machine> node in graph.GetTopologicalOrdering())
            {
                foreach(GraphPort<Machine> outputPort in node.Outports.Values)
                    DrawOutgoingConnection(graphics, outputPort, outputPort.RemotePort, false);

                DrawMachine(graphics, node);
            }
            if(manipulationState == ManipulationState.Connecting)
                DrawConnector(graphics);
        }

        private void DrawMachine(Graphics graphics, GraphNode<Machine> node)
        {
            Point p = positions[node];
            graphics.FillEllipse(machinebrush, p.X - MACHINE_R, p.Y - MACHINE_R, MACHINE_R * 2, MACHINE_R * 2);
            graphics.DrawEllipse(machinepen, p.X - MACHINE_R, p.Y - MACHINE_R, MACHINE_R * 2, MACHINE_R * 2);
            SizeF textSize = graphics.MeasureString(node.Machine.ToString(), Font);
            graphics.DrawString(node.Machine.ToString(), Font, arrowbrush, p.X - textSize.Width/2 + 1, p.Y - textSize.Height/2);
        }

        private void DrawOutgoingConnection(Graphics graphics, GraphPort<Machine> fromPort, GraphPort<Machine> toPort, bool potential)
        {
            Point from = positions[fromPort.Node];
            Point to = positions[toPort.Node];

            Pen arrowpenLocal = arrowpen;
            Pen machinepenLocal = machinepen;
            Brush arrowbrushLocal = arrowbrush;
            if(potential)
            {
                arrowpenLocal = arrowPenPotential;
                machinepenLocal = machinePenPotential;
                arrowbrushLocal = arrowBrushPotential;
            }

            PointF unitVector = CalculateUnitVector(from, to);
            int arrow_offset = MACHINE_R + BUBBLE_R * 2;

            graphics.DrawLine(arrowpenLocal,
                from.X + unitVector.X * arrow_offset, from.Y + unitVector.Y * arrow_offset,
                to.X - unitVector.X * arrow_offset, to.Y - unitVector.Y * arrow_offset);

            Point[] arrowpoints = {
                new Point(to.X - (int) (unitVector.X * arrow_offset), to.Y - (int) (unitVector.Y * arrow_offset)),
                new Point(to.X - (int) (unitVector.X * (ARROW_L + arrow_offset) + unitVector.Y * ARROW_W), to.Y - (int) (unitVector.Y * (ARROW_L + arrow_offset) - unitVector.X * ARROW_W)),
                new Point(to.X - (int) (unitVector.X * (ARROW_L + arrow_offset) - unitVector.Y * ARROW_W), to.Y - (int) (unitVector.Y * (ARROW_L + arrow_offset) + unitVector.X * ARROW_W))
            };

            graphics.FillPolygon(arrowbrushLocal, arrowpoints);

            DrawCenteredCircle(graphics, machinepenLocal, potential ? Brushes.Transparent : Brushes.White, Brushes.Black, new PointF(
                to.X - (unitVector.X * (MACHINE_R + BUBBLE_R)),
                to.Y - (unitVector.Y * (MACHINE_R + BUBBLE_R))),
                BUBBLE_R, potential ? "" : toPort.Node.Machine.InputCodes[toPort.PortNumber].ToString());

            DrawCenteredCircle(graphics, machinepenLocal, potential ? Brushes.Transparent : Brushes.Black, Brushes.White, new PointF(
                from.X + (unitVector.X * (MACHINE_R + BUBBLE_R)),
                from.Y + (unitVector.Y * (MACHINE_R + BUBBLE_R))),
                BUBBLE_R, potential ? "" : fromPort.Node.Machine.OutputCodes[fromPort.PortNumber].ToString());
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
                Point origin = positions[manipulatedNode];
                PointF unitVector = CalculateUnitVector(origin, manipulationOffset);
                origin.Offset((int)(unitVector.X * MACHINE_R), (int)(unitVector.Y * MACHINE_R));

                g.DrawLine(arrowPenPotential, origin, manipulationOffset);
                g.DrawEllipse(arrowPenPotential, manipulationOffset.X - BUBBLE_R, manipulationOffset.Y - BUBBLE_R, BUBBLE_R * 2, BUBBLE_R * 2);
            }
            else
                DrawOutgoingConnection(g, new GraphPort<Machine>(manipulatedNode, -1), new GraphPort<Machine>(destinationNode, -1), true);
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
                    manipulationOffset = Point.Subtract(e.Location, new Size(positions[manipulatedNode]));
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
                positions[manipulatedNode] = Point.Subtract(e.Location, new Size(manipulationOffset));
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
                Point machinePoint = positions[node];
                if(pointDistance(point, machinePoint) < MACHINE_R)
                    return node;
            }

            return null;
        }

        private float pointDistance(Point p1, Point p2)
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
                positions[node] = manipulationOffset;
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
