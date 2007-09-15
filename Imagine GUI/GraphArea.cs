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
        const int MACHINE_W = 50;
        const int ARROW_L = 10;
        const int ARROW_W = 5;
        const int BUBBLE_W = 10;

        public GraphArea()
        {
            InitializeComponent();
        }

        private Dictionary<GraphNode<Machine>, Point> positions;
        private Graph<Machine> graph;
        private Pen machinepen = new Pen(Color.Gray, 1);
        private Pen arrowpen = new Pen(Color.Black, 1);
        private Brush arrowbrush = Brushes.Black;
        private Brush machinebrush = Brushes.Bisque;

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
                    Point p = new Point(random.Next(this.Width - MACHINE_W * 2) + MACHINE_W, random.Next(this.Height - MACHINE_W * 2) + MACHINE_W);
                    positions[node] = p;
                }
            }
        }

        public void DrawGraph(Graphics graphics)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach(GraphNode<Machine> node in graph.GetTopologicalOrdering())
            {
                foreach(GraphPort<Machine> outputPort in node.Outports.Values)
                    DrawOutgoingConnection(graphics, node, outputPort.RemotePort.Node);

                DrawMachine(graphics, node);
            }
        }

        private void DrawMachine(Graphics graphics, GraphNode<Machine> node)
        {
            Point p = positions[node];
            graphics.FillEllipse(machinebrush, p.X - MACHINE_W/2, p.Y - MACHINE_W/2, MACHINE_W, MACHINE_W);
            graphics.DrawEllipse(machinepen, p.X - MACHINE_W / 2, p.Y - MACHINE_W / 2, MACHINE_W, MACHINE_W);
            SizeF textSize = graphics.MeasureString(node.Machine.ToString(), Font);
            graphics.DrawString(node.Machine.ToString(), Font, arrowbrush, p.X - textSize.Width/2, p.Y - textSize.Height/2);
        }

        private void DrawOutgoingConnection(Graphics graphics, GraphNode<Machine> fromNode, GraphNode<Machine> toNode)
        {
            Point from = positions[fromNode];
            Point to = positions[toNode];

            graphics.DrawLine(arrowpen, from, to);

            PointF unitVector = new PointF(to.X - from.X, to.Y - from.Y);
            float vectorLength = (float) Math.Sqrt(unitVector.X * unitVector.X + unitVector.Y * unitVector.Y);
            unitVector.X = unitVector.X / vectorLength;
            unitVector.Y = unitVector.Y / vectorLength;

            int arrow_offset = MACHINE_W / 2 + BUBBLE_W;

            Point[] arrowpoints = {
                new Point(to.X - (int) (unitVector.X * arrow_offset), to.Y - (int) (unitVector.Y * arrow_offset)),
                new Point(to.X - (int) (unitVector.X * (ARROW_L + arrow_offset) + unitVector.Y * ARROW_W), to.Y - (int) (unitVector.Y * (ARROW_L + arrow_offset) - unitVector.X * ARROW_W)),
                new Point(to.X - (int) (unitVector.X * (ARROW_L + arrow_offset) - unitVector.Y * ARROW_W), to.Y - (int) (unitVector.Y * (ARROW_L + arrow_offset) + unitVector.X * ARROW_W))
            };

            graphics.FillPolygon(arrowbrush, arrowpoints);

            DrawCenteredCircle(graphics, machinepen, new PointF(
                to.X - (unitVector.X * (MACHINE_W + BUBBLE_W)/2.0f),
                to.Y - (unitVector.Y * (MACHINE_W + BUBBLE_W)/2.0f)), BUBBLE_W/2.0f);

            DrawCenteredCircle(graphics, machinepen, new PointF(
                from.X + (unitVector.X * (MACHINE_W + BUBBLE_W) / 2.0f),
                from.Y + (unitVector.Y * (MACHINE_W + BUBBLE_W) / 2.0f)), BUBBLE_W / 2.0f);
        }

        private void DrawCenteredCircle(Graphics g, Pen pen, PointF p, float radius)
        {
            g.DrawEllipse(pen, p.X - radius, p.Y - radius, radius * 2, radius * 2);
        }

        private void GraphArea_Paint(object sender, PaintEventArgs e)
        {
            if(graph != null)
                DrawGraph(e.Graphics);
        }

    }
}
