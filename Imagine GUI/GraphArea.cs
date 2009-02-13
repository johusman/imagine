using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Imagine.Library;
using System.Collections;

namespace Imagine.GUI
{
    public partial class GraphArea : UserControl
    {
        const int ARROW_L = 10;
        const int ARROW_W = 5;

        public GraphArea()
        {
            InitializeComponent();
        }

        private IGUIGraph guiGraph;

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
        private GUINode manipulatedNode = null;
        private GUINode manipulationDestination = null;
        private GUIPort manipulatedPort = null;
        private Point manipulationOffset;
        private GUIPort chosenPort = null;

        private bool showTooltips = true;
        private ToolTip tooltip = null;
        private Object tooltipObject = null;

        public void Initialize(ImagineFacade facade)
        {
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

        public bool ShowTooltips
        {
            get { return showTooltips; }
            set { showTooltips = value; }
        }

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
            if (manipulationState == ManipulationState.Connecting)
                DrawConnector(graphics);
        }

        private void DrawMachine(Graphics graphics, GUINode node)
        {
            Point p = node.Position;
            MachineGUI gui = node.MachineGUI;

            graphics.FillEllipse(gui.Background, p.X - GUINode.MACHINE_R, p.Y - GUINode.MACHINE_R, GUINode.MACHINE_R * 2, GUINode.MACHINE_R * 2);
            graphics.DrawEllipse(machinepen, p.X - GUINode.MACHINE_R, p.Y - GUINode.MACHINE_R, GUINode.MACHINE_R * 2, GUINode.MACHINE_R * 2);


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

            Point portPos = port.Position.Value;
            GUINode node = port.Node;
            GUINode remoteNode = remotePort.Node;
            Point remotePortPos = remotePort.Position.Value;

            DrawCenteredCircle(graphics, machinepen, Brushes.Black, Brushes.White,
                    new PointF(portPos.X, portPos.Y),
                    GUIPort.BUBBLE_R, port.Code, new Font(FontFamily.GenericMonospace, 7.0f));

            DrawCenteredCircle(graphics, machinepen, Brushes.White, Brushes.Black,
                    new PointF(remotePortPos.X, remotePortPos.Y),
                    GUIPort.BUBBLE_R, remotePort.Code, new Font(FontFamily.GenericMonospace, 7.0f));

            PointF unitVector = CalculateUnitVector(portPos, remotePortPos);
            if (PointDistance(portPos, remotePortPos) > GUIPort.BUBBLE_R * 2.0)
            {
                Point lineFromPoint = new Point((int)(portPos.X + unitVector.X * GUIPort.BUBBLE_R), (int)(portPos.Y + unitVector.Y * GUIPort.BUBBLE_R));
                Point lineToPoint = new Point((int)(remotePortPos.X - unitVector.X * GUIPort.BUBBLE_R), (int)(remotePortPos.Y - unitVector.Y * GUIPort.BUBBLE_R));
                graphics.DrawLine(arrowpen, lineFromPoint, lineToPoint);

                Point[] arrowpoints = {
                    lineToPoint,
                    new Point(remotePortPos.X - (int) (unitVector.X * (ARROW_L + GUIPort.BUBBLE_R) + unitVector.Y * ARROW_W), remotePortPos.Y - (int) (unitVector.Y * (ARROW_L + GUIPort.BUBBLE_R) - unitVector.X * ARROW_W)),
                    new Point(remotePortPos.X - (int) (unitVector.X * (ARROW_L + GUIPort.BUBBLE_R) - unitVector.Y * ARROW_W), remotePortPos.Y - (int) (unitVector.Y * (ARROW_L + GUIPort.BUBBLE_R) + unitVector.X * ARROW_W))
                };

                graphics.FillPolygon(arrowbrush, arrowpoints);
            }
        }

        private void DrawPotentialConnection(Graphics graphics, GUINode fromNode, GUINode toNode)
        {
            Point from = fromNode.Position;
            Point to = toNode.Position;

            PointF unitVector = CalculateUnitVector(from, to);
            int arrow_offset = GUINode.MACHINE_R + GUIPort.BUBBLE_R * 2;

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
                to.X - (unitVector.X * (GUINode.MACHINE_R + GUIPort.BUBBLE_R)),
                to.Y - (unitVector.Y * (GUINode.MACHINE_R + GUIPort.BUBBLE_R))),
                GUIPort.BUBBLE_R, "");

            DrawCenteredCircle(graphics, machinePenPotential, Brushes.Transparent, Brushes.White, new PointF(
                from.X + (unitVector.X * (GUINode.MACHINE_R + GUIPort.BUBBLE_R)),
                from.Y + (unitVector.Y * (GUINode.MACHINE_R + GUIPort.BUBBLE_R))),
                GUIPort.BUBBLE_R, "");
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
            GUINode destinationNode = guiGraph.GetNodeAt(manipulationOffset);
            if (destinationNode == null)
            {
                Point origin = manipulatedNode.Position;
                PointF unitVector = CalculateUnitVector(origin, manipulationOffset);
                origin.Offset((int)(unitVector.X * GUINode.MACHINE_R), (int)(unitVector.Y * GUINode.MACHINE_R));

                g.DrawLine(arrowPenPotential, origin, manipulationOffset);
                g.DrawEllipse(arrowPenPotential, manipulationOffset.X - GUIPort.BUBBLE_R, manipulationOffset.Y - GUIPort.BUBBLE_R, GUIPort.BUBBLE_R * 2, GUIPort.BUBBLE_R * 2);
            }
            else
                DrawPotentialConnection(g, manipulatedNode, destinationNode);
        }

        private void GraphArea_Paint(object sender, PaintEventArgs e)
        {
            DrawGraph(e.Graphics);
        }

        private void GraphArea_MouseDown(object sender, MouseEventArgs e)
        {
            manipulatedNode = guiGraph.GetNodeAt(e.Location);
            if (manipulatedNode != null)
            {
                if (e.Button == MouseButtons.Left && e.Clicks == 1)
                {
                    manipulationState = ManipulationState.Dragging;
                    manipulationOffset = Point.Subtract(e.Location, new Size(manipulatedNode.Position));
                }
                else if (e.Button == MouseButtons.Left && e.Clicks == 2)
                {
                    manipulatedNode.MachineGUI.LaunchSettings(this);
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

                    guiGraph.Remove(manipulatedNode);
                    this.Invalidate();
                }
            }
            else
            {
                GUIPort port = guiGraph.GetPortAt(e.Location);
                if (port != null)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        if (e.Clicks == 1)
                        {
                            manipulatedPort = port;
                            portContextMenu.Show(this, Point.Subtract(e.Location, new Size(10, -10)));
                        }
                        else if (e.Clicks == 2)
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
                manipulationDestination = guiGraph.GetNodeAt(e.Location);
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
                manipulatedNode.Position = Point.Subtract(e.Location, new Size(manipulationOffset));
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

            GUINode node = guiGraph.GetNodeAt(location);
            if (node != null)
            {
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
                    tooltip.Show(text, this.ParentForm, new Point(node.Position.X + 50, node.Position.Y));
                }
            }
            else
            {
                GUIPort port = guiGraph.GetPortAt(location);
                if (port != null)
                {
                    if (tooltip == null || tooltipObject != port)
                    {
                        KillToolTip();
                        tooltipObject = port;
                        tooltip = new ToolTip();
                        tooltip.BackColor = tooltipColor;

                        //GUINode portNode = port.Node;
                        //GUINode remoteNode = port.RemotePort.Node;
                        //int index = port.PortNumber;
                        //int remoteIndex = port.RemotePort.PortNumber;
                        string text = null;
                        tooltip.ToolTipTitle = port.Name;
                        if (port.Direction == GUIPort.Directions.OUT)
                        {
                            //tooltip.ToolTipTitle = port.Name; // portNode.Machine.OutputNames[index];
                            text = String.Format(" (to: {0})", port.RemotePort.Name /* remoteNode.Machine.InputNames[remoteIndex]*/);
                            //tooltip.Show(text, this.ParentForm, new Point(port.Position.Value.X + 25, port.Position.Value.Y + 50));
                        }
                        else
                        {
                            //tooltip.ToolTipTitle = port.Name; // portNode.Machine.InputNames[index];
                            text = String.Format(" (from: {0})", port.RemotePort.Name /* remoteNode.Machine.OutputNames[remoteIndex]*/);
                            //tooltip.Show(text, this.ParentForm, new Point(port.Position.Value.X + 25, port.Position.Value.Y + 50));
                        }
                        tooltip.Show(text, this.ParentForm, new Point(port.Position.Value.X + 25, port.Position.Value.Y + 50));
                        
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
                guiGraph.CreateNode((string)((ToolStripItem)sender).Tag, manipulationOffset);
                manipulationState = ManipulationState.None;
                this.Invalidate();
            }
        }

        // Come on, there has to be a better way.. using the tag for example
        private void chooseOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string chosenText = ((ToolStripMenuItem)sender).Text.Substring(4);
            foreach(GUIPort guiPort in manipulatedNode.UnusedOutports.Values)
                if (guiPort.Name == chosenText)
                {
                    chosenPort = guiPort;
                    ShowInputChooser(manipulationOffset);
                    break;
                }
        }

        // Come on, there has to be a better way.. using the tag for example
        private void chooseInputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string chosenText = ((ToolStripMenuItem)sender).Text.Substring(4);
            foreach (GUIPort guiPort in manipulationDestination.UnusedInports.Values)
                if (guiPort.Name == chosenText)
                {
                    guiGraph.Connect(chosenPort, guiPort);

                    this.Invalidate();
                    manipulatedNode = null;
                    manipulationDestination = null;
                    manipulationOffset = Point.Empty;
                    //choosenPort = -1;
                    chosenPort = null;
                    break;
                }
        }

        private void ShowOutputChooser(Point location)
        {
            int remainingOutputs = manipulatedNode.UnusedOutports.Count;
            if(remainingOutputs > 1)
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.ShowImageMargin = false;

                ToolStripMenuItem header = new ToolStripMenuItem("Bind from output:");
                header.Font = new Font(header.Font, FontStyle.Bold);
                header.Enabled = false;
                contextMenu.Items.Add(header);

                foreach (GUIPort guiPort in manipulatedNode.UnusedOutports.Values)
                {
                    String text = String.Format("({1}) {0}", guiPort.Name, guiPort.Code);
                    contextMenu.Items.Add(new ToolStripMenuItem(text, null, new System.EventHandler(this.chooseOutputToolStripMenuItem_Click)));
                }
                contextMenu.Show(this, Point.Subtract(location, new Size(10, 10)));
            }
            else if (remainingOutputs == 1)
            {
                foreach (GUIPort port in manipulatedNode.UnusedOutports.Values)
                    chosenPort = port;
                ShowInputChooser(manipulationOffset);
                return;
            }
            else
            {
                MessageBox.Show(this.ParentForm, "There are no free outputs on the first machine", "No free outputs", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowInputChooser(Point location)
        {
            int remainingInputs = manipulationDestination.UnusedInports.Count;
            if(remainingInputs > 1)
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.ShowImageMargin = false;

                ToolStripMenuItem header = new ToolStripMenuItem("Bind to input:");
                header.Font = new Font(header.Font, FontStyle.Bold);
                header.Enabled = false;
                contextMenu.Items.Add(header);

                foreach (GUIPort guiPort in manipulationDestination.UnusedInports.Values)
                {
                    String text = String.Format("({1}) {0}", guiPort.Name, guiPort.Code);
                    contextMenu.Items.Add(new ToolStripMenuItem(text, null, new System.EventHandler(this.chooseInputToolStripMenuItem_Click)));
                }

                contextMenu.Show(this, Point.Subtract(location, new Size(10, 10)));
            }
            else if (remainingInputs == 1)
            {
                GUIPort otherPort = null;
                foreach (GUIPort port in manipulationDestination.UnusedInports.Values)
                    otherPort = port;

                guiGraph.Connect(chosenPort, otherPort);

                this.Invalidate();
                manipulatedNode = null;
                manipulationDestination = null;
                manipulationOffset = Point.Empty;
                //choosenPort = -1;
                chosenPort = null;
                return;
            }
            else
            {
                MessageBox.Show(this.ParentForm, "There are no free inputs on the second machine", "No free inputs", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Yuck .. these should be in the GUIGraph

        public string SerializeLayout()
        {
            return guiGraph.SerializeLayout();
        }

        public void DeserializeLayout(string input)
        {
            guiGraph.DeserializeLayout(input);
            Refresh();
        }

        private void breakConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guiGraph.Disconnect(manipulatedPort);
            manipulatedPort = null;
            this.Invalidate();
        }

        private void branchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertBranchInConnectionForPort(manipulatedPort);
            manipulatedPort = null;
            this.Invalidate();
        }

        private void InsertBranchInConnectionForPort(GUIPort port)
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
        }
    }
}
