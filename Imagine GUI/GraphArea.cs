using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Imagine.Library;
using System.Collections;

namespace Imagine.GUI
{
    public partial class GraphArea : UserControl, IManipulationTarget
    {
        const int ARROW_L = 10;
        const int ARROW_W = 5;

        public GraphArea()
        {
            InitializeComponent();
        }

        private IGUIGraph guiGraph;
        private IState stateSwitch;

        private Size viewOffset = new Size(0, 0);

        private Pen machinepen = new Pen(Color.Gray, 1);
        private Pen machinePenPotential = new Pen(Color.Gray, 1);
        private Pen arrowpen = new Pen(Color.Black, 1);
        private Pen arrowPenPotential = new Pen(Color.Gray, 1);
        private Brush arrowbrush = Brushes.Black;
        private Brush arrowBrushPotential = Brushes.Gray;
        private Brush machinebrush = Brushes.Bisque;
        private Color tooltipColor = Color.OldLace;

        private bool showTooltips = true;
        private ToolTip tooltip = null;
        private Object tooltipObject = null;

        public bool ShowTooltips
        {
            get { return showTooltips; }
            set { showTooltips = value; }
        }

#region Initialization
        public void Initialize(ImagineFacade facade)
        {
            viewOffset = new Size(0, 0);
            stateSwitch = new StateSwitch(this);
            guiGraph = new GUIGraph(facade, this.Size);

            List<string> uniqueNames = new List<string>(facade.MachineTypes.Keys);
            uniqueNames.Remove("Imagine.Source");
            uniqueNames.Remove("Imagine.Destination");

            ConstructNewMachineMenu(uniqueNames);
            AddSourceAndDestinationToMachineMenu();
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

                Image image = guiGraph.CreateMachineGUIFor(uniqueName).HalfDimmedBitmap;
                
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

        private void AddSourceAndDestinationToMachineMenu()
        {
            ToolStripMenuItem sourceItem = new ToolStripMenuItem();
            sourceItem.Tag = "Imagine.Source";
            sourceItem.Text = "Source";
            sourceItem.Image = (new SourceMachineGUI()).HalfDimmedBitmap;
            sourceItem.ImageScaling = ToolStripItemImageScaling.SizeToFit;
            sourceItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            this.contextMenu.Items.Add(sourceItem);

            ToolStripMenuItem destinationItem = new ToolStripMenuItem();
            destinationItem.Tag = "Imagine.Destination";
            destinationItem.Text = "Destination";
            destinationItem.Image = (new SinkMachineGUI()).HalfDimmedBitmap;
            destinationItem.ImageScaling = ToolStripItemImageScaling.SizeToFit;
            destinationItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            this.contextMenu.Items.Add(destinationItem);
        }
#endregion

#region Serialization
        public string SerializeLayout()
        {
            return guiGraph.SerializeLayout();
        }

        public void DeserializeLayout(string input)
        {
            guiGraph.DeserializeLayout(input);
            Refresh();
        }
#endregion

#region Graph Rendering
        public void DrawGraph(Graphics graphics)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (GUINode node in guiGraph.Nodes)
            {
                node.UpdatePortPositions();
            }

            foreach (GUINode node in guiGraph.Nodes)
            {
                foreach (GUIPort port in node.UsedPorts.Values)
                    if (port.Direction == GUIPort.Directions.OUT)
                        DrawOutgoingConnection(graphics, port);

                DrawMachine(graphics, node);
            }

            stateSwitch.Draw(graphics);
        }

        private void DrawMachine(Graphics graphics, GUINode node)
        {
            Point p = Point.Add(node.Position, viewOffset);
            MachineGUI gui = node.MachineGUI;

            graphics.FillEllipse(gui.Background, p.X - GUINode.RADIUS, p.Y - GUINode.RADIUS, GUINode.RADIUS * 2, GUINode.RADIUS * 2);
            graphics.DrawEllipse(machinepen, p.X - GUINode.RADIUS, p.Y - GUINode.RADIUS, GUINode.RADIUS * 2, GUINode.RADIUS * 2);


            if (gui.DimmedBitmap != null)
                graphics.DrawImage(gui.DimmedBitmap, p.X - 16, p.Y - 16);

            String caption = node.Machine.Caption; // Would be nice if this was part of the gui
            SizeF textSize = graphics.MeasureString(caption, Font);
            graphics.DrawString(caption, Font, arrowbrush, p.X - textSize.Width / 2 + 1, p.Y - textSize.Height / 2);
        }

        private void DrawOutgoingConnection(Graphics graphics, GUIPort port)
        {
            if (port.Position == null)
                return;

            GUIPort remotePort = port.RemotePort;

            Point portPos = Point.Add(port.Position.Value, viewOffset);
            GUINode node = port.Node;
            GUINode remoteNode = remotePort.Node;
            Point remotePortPos = Point.Add(remotePort.Position.Value, viewOffset);

            DrawCenteredCircle(graphics, machinepen, Brushes.Black, Brushes.White,
                    new PointF(portPos.X, portPos.Y),
                    GUIPort.RADIUS, port.Code, new Font(FontFamily.GenericMonospace, 7.0f));

            DrawCenteredCircle(graphics, machinepen, Brushes.White, Brushes.Black,
                    new PointF(remotePortPos.X, remotePortPos.Y),
                    GUIPort.RADIUS, remotePort.Code, new Font(FontFamily.GenericMonospace, 7.0f));

            PointF unitVector = CalculateUnitVector(portPos, remotePortPos);
            if (PointDistance(portPos, remotePortPos) > GUIPort.RADIUS * 2.0)
            {
                Point lineFromPoint = new Point((int)(portPos.X + unitVector.X * GUIPort.RADIUS), (int)(portPos.Y + unitVector.Y * GUIPort.RADIUS));
                Point lineToPoint = new Point((int)(remotePortPos.X - unitVector.X * GUIPort.RADIUS), (int)(remotePortPos.Y - unitVector.Y * GUIPort.RADIUS));
                graphics.DrawLine(arrowpen, lineFromPoint, lineToPoint);

                Point[] arrowpoints = {
                    lineToPoint,
                    new Point(remotePortPos.X - (int) (unitVector.X * (ARROW_L + GUIPort.RADIUS) + unitVector.Y * ARROW_W), remotePortPos.Y - (int) (unitVector.Y * (ARROW_L + GUIPort.RADIUS) - unitVector.X * ARROW_W)),
                    new Point(remotePortPos.X - (int) (unitVector.X * (ARROW_L + GUIPort.RADIUS) - unitVector.Y * ARROW_W), remotePortPos.Y - (int) (unitVector.Y * (ARROW_L + GUIPort.RADIUS) + unitVector.X * ARROW_W))
                };

                graphics.FillPolygon(arrowbrush, arrowpoints);
            }
        }

        private void DrawPotentialConnection(Graphics graphics, GUINode fromNode, GUINode toNode)
        {
            Point from = Point.Add(fromNode.Position, viewOffset);
            Point to = Point.Add(toNode.Position, viewOffset);

            PointF unitVector = CalculateUnitVector(from, to);
            int arrow_offset = GUINode.RADIUS + GUIPort.RADIUS * 2;

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
                to.X - (unitVector.X * (GUINode.RADIUS + GUIPort.RADIUS)),
                to.Y - (unitVector.Y * (GUINode.RADIUS + GUIPort.RADIUS))),
                GUIPort.RADIUS, "");

            DrawCenteredCircle(graphics, machinePenPotential, Brushes.Transparent, Brushes.White, new PointF(
                from.X + (unitVector.X * (GUINode.RADIUS + GUIPort.RADIUS)),
                from.Y + (unitVector.Y * (GUINode.RADIUS + GUIPort.RADIUS))),
                GUIPort.RADIUS, "");
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

        public void DrawConnector(Graphics g, Point connectorPosition, GUINode origNode)
        {
            GUINode destinationNode = guiGraph.GetNodeAt(Point.Subtract(connectorPosition, viewOffset));
            if (destinationNode == null)
            {
                Point origin = Point.Add(origNode.Position, viewOffset);
                PointF unitVector = CalculateUnitVector(origin, connectorPosition);
                origin.Offset((int)(unitVector.X * GUINode.RADIUS), (int)(unitVector.Y * GUINode.RADIUS));

                g.DrawLine(arrowPenPotential, origin, connectorPosition);
                g.DrawEllipse(arrowPenPotential, connectorPosition.X - GUIPort.RADIUS, connectorPosition.Y - GUIPort.RADIUS, GUIPort.RADIUS * 2, GUIPort.RADIUS * 2);
            }
            else
                DrawPotentialConnection(g, origNode, destinationNode);
        }

        private void GraphArea_Paint(object sender, PaintEventArgs e)
        {
            DrawGraph(e.Graphics);
        }

        private float PointDistance(Point p1, Point p2)
        {
            float x = p1.X - p2.X;
            float y = p1.Y - p2.Y;

            return (float)Math.Sqrt(x * x + y * y);
        }
#endregion

#region Mouse Event Handling
        private void GraphArea_MouseDown(object sender, MouseEventArgs e)
        {
            GUINode node = guiGraph.GetNodeAt(Point.Subtract(e.Location, viewOffset));
            if (node != null)
            {
                if (e.Button == MouseButtons.Left)
                    stateSwitch.NodeMouseDownLeft(node, e);
                else if (e.Button == MouseButtons.Right)
                    stateSwitch.NodeMouseDownRight(node, e);
            }
            else
            {
                GUIPort port = guiGraph.GetPortAt(Point.Subtract(e.Location, viewOffset));
                if (port != null)
                {
                    if (e.Button == MouseButtons.Left)
                        stateSwitch.PortMouseDownLeft(port, e);
                    else if (e.Button == MouseButtons.Right)
                        stateSwitch.PortMouseDownRight(port, e);
                }
                else
                {
                    if (e.Button == MouseButtons.Left)
                        stateSwitch.FreeMouseDownLeft(e);
                    else if (e.Button == MouseButtons.Right)
                        stateSwitch.FreeMouseDownRight(e);
                }
            }
        }

        private void GraphArea_MouseUp(object sender, MouseEventArgs e)
        {
            GUINode node = guiGraph.GetNodeAt(Point.Subtract(e.Location, viewOffset));
            if (node != null)
                stateSwitch.NodeMouseUp(node, e);
            else
            {
                GUIPort port = guiGraph.GetPortAt(Point.Subtract(e.Location, viewOffset));
                if (port != null)
                    stateSwitch.PortMouseUp(port, e);
                else
                    stateSwitch.FreeMouseUp(e);
            }

            this.Invalidate();
        }

        private void GraphArea_MouseMove(object sender, MouseEventArgs e)
        {
            stateSwitch.MouseMove(e);
        }
#endregion

#region ToolTip
        public void ManageToolTip(Point location)
        {
            if (!showTooltips)
                return;

            GUINode node = guiGraph.GetNodeAt(Point.Subtract(location, viewOffset));
            if (node != null)
            {
                Point nodePosition = Point.Add(node.Position, viewOffset);
                if (tooltip == null || tooltipObject != node)
                {
                    Machine machine = node.Machine;
                    KillToolTip();
                    tooltipObject = node;
                    tooltip = new ToolTip();
                    tooltip.BackColor = tooltipColor;
                    tooltip.ToolTipTitle = machine.ToString();

                    string description = machine.Description;
                    string inputs = "";
                    string outputs = "";
                    for (int i = 0; i < machine.InputCount; i++)
                        inputs = inputs + String.Format(" [{1}] {0},", machine.InputNames[i], machine.InputCodes[i]);
                    if (inputs == "")
                        inputs = " (None)";
                    else
                        inputs = inputs.Remove(inputs.Length - 1);

                    for (int i = 0; i < machine.OutputCount; i++)
                        outputs = outputs + String.Format(" [{1}] {0},", machine.OutputNames[i], machine.OutputCodes[i]);
                    if (outputs == "")
                        outputs = " (None)";
                    else
                        outputs = outputs.Remove(outputs.Length - 1);

                    string text = String.Format("{0}\n\nInputs:\n {1}\nOutputs:\n {2}", description, inputs, outputs);
                    tooltip.Show(text, this.ParentForm, new Point(nodePosition.X + 50, nodePosition.Y));
                }
            }
            else
            {
                GUIPort port = guiGraph.GetPortAt(Point.Subtract(location, viewOffset));
                if (port != null)
                {
                    Point portPosition = Point.Add(port.Position.Value, viewOffset);
                    if (tooltip == null || tooltipObject != port)
                    {
                        KillToolTip();
                        tooltipObject = port;
                        tooltip = new ToolTip();
                        tooltip.BackColor = tooltipColor;

                        string text = null;
                        tooltip.ToolTipTitle = port.Name;
                        if (port.Direction == GUIPort.Directions.OUT)
                            text = String.Format(" (to: {0})", port.RemotePort.Name);
                        else
                            text = String.Format(" (from: {0})", port.RemotePort.Name);
                        tooltip.Show(text, this.ParentForm, new Point(portPosition.X + 25, portPosition.Y + 50));
                        
                    }
                }
                else
                    KillToolTip();
            }
        }

        public void KillToolTip()
        {
            if (tooltip != null)
            {
                tooltip.Hide(this.ParentForm);
                tooltip = null;
                tooltipObject = null;
            }
        }
#endregion

#region Context menu handlers
        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stateSwitch.NewMachineTypeChosen((string)((ToolStripItem)sender).Tag);
        }

        private void chooseOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stateSwitch.OutputPortChosen((GUIPort)((ToolStripMenuItem)sender).Tag);
        }

        private void chooseInputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stateSwitch.InputPortChosen((GUIPort)((ToolStripMenuItem)sender).Tag);
        }

        private void breakConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stateSwitch.DisconnectPortChosen();
        }

        private void branchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stateSwitch.BranchPortChosen();
        }
#endregion

#region Callback From State Machine
        public void ShowOutputChooser(GUINode fromNode, Point menuPosition)
        {
            int remainingOutputs = fromNode.UnusedOutports.Count;
            if(remainingOutputs > 1)
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.ShowImageMargin = false;

                ToolStripMenuItem header = new ToolStripMenuItem("Bind from output:");
                header.Font = new Font(header.Font, FontStyle.Bold);
                header.Enabled = false;
                contextMenu.Items.Add(header);

                foreach (GUIPort guiPort in fromNode.UnusedOutports.Values)
                {
                    String text = String.Format("({1}) {0}", guiPort.Name, guiPort.Code);
                    ToolStripMenuItem menuItem = new ToolStripMenuItem(text, null, new System.EventHandler(this.chooseOutputToolStripMenuItem_Click));
                    menuItem.Tag = guiPort;
                    contextMenu.Items.Add(menuItem);
                }
                contextMenu.Show(this, Point.Subtract(menuPosition, new Size(10, 10)));
            }
            else if (remainingOutputs == 1)
            {
                GUIPort remainingPort = null;
                foreach (GUIPort port in fromNode.UnusedOutports.Values)
                    remainingPort = port;

                stateSwitch.OutputPortChosen(remainingPort);
            }
            else
            {   
                MessageBox.Show(this.ParentForm, "There are no free outputs on the first machine", "No free outputs", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void ShowInputChooser(GUINode toNode, Point menuPosition)
        {
            int remainingInputs = toNode.UnusedInports.Count;
            if(remainingInputs > 1)
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.ShowImageMargin = false;

                ToolStripMenuItem header = new ToolStripMenuItem("Bind to input:");
                header.Font = new Font(header.Font, FontStyle.Bold);
                header.Enabled = false;
                contextMenu.Items.Add(header);

                foreach (GUIPort guiPort in toNode.UnusedInports.Values)
                {
                    String text = String.Format("({1}) {0}", guiPort.Name, guiPort.Code);
                    ToolStripMenuItem menuItem = new ToolStripMenuItem(text, null, new System.EventHandler(this.chooseInputToolStripMenuItem_Click));
                    menuItem.Tag = guiPort;
                    contextMenu.Items.Add(menuItem);
                }

                contextMenu.Show(this, Point.Subtract(menuPosition, new Size(10, 10)));
            }
            else if (remainingInputs == 1)
            {

                GUIPort remainingPort = null;
                foreach (GUIPort port in toNode.UnusedInports.Values)
                    remainingPort = port;

                stateSwitch.InputPortChosen(remainingPort);
            }
            else
            {   
                MessageBox.Show(this.ParentForm, "There are no free inputs on the second machine", "No free inputs", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void BranchPort(GUIPort port)
        {
            GUIPort fromPort = null;
            GUIPort toPort = null;
            if (port.Direction == GUIPort.Directions.OUT)
            {
                fromPort = port;
                toPort = port.RemotePort;
            }
            else
            {
                fromPort = port.RemotePort;
                toPort = port;
            }

            guiGraph.Disconnect(port);
            
            Point fromPoint = fromPort.Node.Position;
            Point toPoint = toPort.Node.Position;
            Point branchPoint = new Point((fromPoint.X + toPoint.X) / 2, (fromPoint.Y + toPoint.Y) / 2);
            
            GUINode branchNode = guiGraph.CreateNode("Imagine.Branch4", branchPoint);
            
            guiGraph.Connect(fromPort, branchNode.UnusedInports[0]);
            guiGraph.Connect(branchNode.UnusedOutports[0], toPort);

            this.Invalidate();
        }

        public void LaunchSettingsForNode(GUINode node)
        {
            node.MachineGUI.LaunchSettings(this);
        }

        public void RemoveNode(GUINode node)
        {
            if (Control.ModifierKeys != Keys.Shift)
            {
                DialogResult result = MessageBox.Show(this.ParentForm, "Do you wish to delete this machine?", "Delete machine?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return;
            }

            guiGraph.Remove(node);
            this.Invalidate();
        }

        public void ShowPortContextMenu(Point position)
        {
            portContextMenu.Show(this, Point.Subtract(position, new Size(10, -10)));
        }

        public void DisconnectPort(GUIPort port)
        {
            portContextMenu.Hide();
            if (Control.ModifierKeys != Keys.Shift)
            {
                DialogResult result = MessageBox.Show(this.ParentForm, "Do you wish to break this connection?", "Break connection?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return;
            }

            guiGraph.Disconnect(port);
            this.Invalidate();
        }

        public void ShowNewMachineContextMenu(Point menuPosition)
        {
            contextMenu.Show(this, Point.Subtract(menuPosition, new Size(10, 10)));
        }

        public void ConnectPorts(GUIPort fromPort, GUIPort toPort)
        {
            guiGraph.Connect(fromPort, toPort);
            this.Invalidate();
        }

        public void InsertNewMachineType(string type, Point newMachinePosition)
        {
            guiGraph.CreateNode(type, Point.Subtract(newMachinePosition, viewOffset));
            this.Invalidate();
        }

        public void Redraw()
        {
            this.Invalidate();
        }

        public void ShowState(Type type)
        {
            Console.WriteLine("State: " + type.ToString());
        }
#endregion
    }
}
