using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Imagine.GUI
{
    public interface IState
    {
        void NodeMouseDownLeft(GUINode node, MouseEventArgs e);
        void NodeMouseDownRight(GUINode node, MouseEventArgs e);
        void NodeMouseUp(GUINode node, MouseEventArgs e);

        void PortMouseDownLeft(GUIPort node, MouseEventArgs e);
        void PortMouseDownRight(GUIPort port, MouseEventArgs e);
        void PortMouseUp(GUIPort node, MouseEventArgs e);

        void FreeMouseDownLeft(MouseEventArgs e);
        void FreeMouseDownRight(MouseEventArgs e);
        void FreeMouseUp(MouseEventArgs e);
        
        void NewMachineTypeChosen(String type);
        void BranchPortChosen();
        void DisconnectPortChosen();
        void OutputPortChosen(GUIPort fromPort);
        void InputPortChosen(GUIPort toPort);

        void MouseMove(MouseEventArgs e);

        void Draw(Graphics graphics);
    }

    public interface IManipulationTarget {
        void ConnectPorts(GUIPort fromPort, GUIPort toPort);
        void BranchPort(GUIPort port);
        void DisconnectPort(GUIPort port);
        void ShowPortContextMenu(Point menuPosition);

        void ShowInputChooser(GUINode toNode,Point menuPosition);
        void ShowOutputChooser(GUINode fromNode, Point menuPosition);

        void ShowNewMachineContextMenu(Point point); 
        void InsertNewMachineType(string type, Point newMachinePosition);
        void LaunchSettingsForNode(GUINode node);
        void RemoveNode(GUINode node);

        void Redraw();
        void DrawConnector(Graphics graphics, Point cursorLocation, GUINode fromNode);
        void ShowState(Type type);

        void ManageToolTip(Point point);
        void KillToolTip();
    }

    public interface IStateContext {
        void Reset();
        void Transition(IState state);
    }

    public class StateSwitch : IState, IStateContext
    {
        private IState state;
        private IManipulationTarget target;

        public StateSwitch(IManipulationTarget target)
        {
            this.target = target;
            state = new GroundState(target, this);
        }

        public void Reset() { state = new GroundState(target, this); }
        public void Transition(IState state) { this.state = state; }

        public void NodeMouseDownLeft(GUINode node, MouseEventArgs e) { state.NodeMouseDownLeft(node, e); }
        public void NodeMouseDownRight(GUINode node, MouseEventArgs e) { state.NodeMouseDownRight(node, e); }
        public void NodeMouseUp(GUINode node, MouseEventArgs e) { state.NodeMouseUp(node, e); }

        public void PortMouseDownLeft(GUIPort port, MouseEventArgs e) { state.PortMouseDownLeft(port, e); }
        public void PortMouseDownRight(GUIPort port, MouseEventArgs e) { state.PortMouseDownRight(port, e); }
        public void PortMouseUp(GUIPort port, MouseEventArgs e) { state.PortMouseUp(port, e); }

        public void FreeMouseDownLeft(MouseEventArgs e) { state.FreeMouseDownLeft(e); }
        public void FreeMouseDownRight(MouseEventArgs e) { state.FreeMouseDownRight(e); }
        public void FreeMouseUp(MouseEventArgs e) { state.FreeMouseUp(e); }

        public void MouseMove(MouseEventArgs e) { state.MouseMove(e); }

        public void NewMachineTypeChosen(String type) { state.NewMachineTypeChosen(type); }
        public void BranchPortChosen() { state.BranchPortChosen(); }
        public void DisconnectPortChosen() { state.DisconnectPortChosen(); }
        public void OutputPortChosen(GUIPort fromPort) { state.OutputPortChosen(fromPort); }
        public void InputPortChosen(GUIPort toPort) { state.InputPortChosen(toPort); }

        public void Draw(Graphics graphics) { state.Draw(graphics); }
    }

    public abstract class State : IState
    {
        protected IManipulationTarget target;
        protected IStateContext context;

        public State(IManipulationTarget target, IStateContext context) {
            this.target = target;
            this.context = context;
        }

        protected void Transition(IState state) { context.Transition(state); }
        protected void Reset() { context.Reset(); }

        public virtual void NodeMouseDownLeft(GUINode node, MouseEventArgs e) { Gen_NodeMouseDown(node, e); }
        public virtual void NodeMouseDownRight(GUINode node, MouseEventArgs e) { Gen_NodeMouseDown(node, e); }
        public virtual void NodeMouseUp(GUINode node, MouseEventArgs e) { Gen_MouseUp(e); }

        public virtual void PortMouseDownLeft(GUIPort port, MouseEventArgs e) { Gen_PortMouseDown(port, e); }
        public virtual void PortMouseDownRight(GUIPort port, MouseEventArgs e) { Gen_PortMouseDown(port, e); }
        public virtual void PortMouseUp(GUIPort port, MouseEventArgs e) { Gen_MouseUp(e); }

        public virtual void FreeMouseDownLeft(MouseEventArgs e) { Gen_FreeMouseDown(e); }
        public virtual void FreeMouseDownRight(MouseEventArgs e) { Gen_FreeMouseDown(e); }
        public virtual void FreeMouseUp(MouseEventArgs e) { Gen_MouseUp(e); }

        public virtual void MouseMove(MouseEventArgs e) { }

        public virtual void NewMachineTypeChosen(String type) { }
        public virtual void BranchPortChosen() { }
        public virtual void DisconnectPortChosen() { }
        public virtual void OutputPortChosen(GUIPort fromPort) { }
        public virtual void InputPortChosen(GUIPort toPort) { }

        public virtual void Draw(Graphics graphics) { }

        // More generic forms
        public virtual void Gen_NodeMouseDown(GUINode node, MouseEventArgs e) { Gen_MouseDown(e); }
        public virtual void Gen_PortMouseDown(GUIPort port, MouseEventArgs e) { Gen_MouseDown(e); }
        public virtual void Gen_FreeMouseDown(MouseEventArgs e) { Gen_MouseDown(e); }
        public virtual void Gen_MouseDown(MouseEventArgs e) { }
        public virtual void Gen_MouseUp(MouseEventArgs e) { }
    }

    class GroundState : State
    {
        public GroundState(IManipulationTarget target, IStateContext context) : base(target, context) { target.ShowState(this.GetType()); }

        public override void NodeMouseDownLeft(GUINode node, MouseEventArgs e) {
            if (e.Clicks == 1) {
                Transition(new DraggingState(target, context, node, e.Location));
            }
            else if(e.Clicks == 2) {
                target.LaunchSettingsForNode(node);
            }
        }

        public override void NodeMouseDownRight(GUINode node, MouseEventArgs e) {
            if (e.Clicks == 1) {
                Transition(new ConnectingState(target, context, node));
            }
            else if (e.Clicks == 2) {
                target.RemoveNode(node);
            }
        }

        public override void PortMouseDownRight(GUIPort port, MouseEventArgs e) {
            if (e.Clicks == 1) {
                Transition(new PortContextState(target, context, port));
                target.ShowPortContextMenu(e.Location);
            }
            else if (e.Clicks == 2) {
                target.DisconnectPort(port);
            }
        }

        public override void FreeMouseDownRight(MouseEventArgs e) {
            Transition(new InsertingState(target, context, e.Location));
            target.ShowNewMachineContextMenu(e.Location);
        }

        public override void MouseMove(MouseEventArgs e) {
            target.ManageToolTip(e.Location);
        }
    }

    class DraggingState : State
    {
        private GUINode node;
        private Size offset;

        public DraggingState(IManipulationTarget target, IStateContext context, GUINode node, Point cursorPosition) : base(target, context) {
            this.node = node;
            this.offset = new Size(Point.Subtract(cursorPosition, new Size(node.Position)));
            target.ShowState(this.GetType());
        }

        public override void MouseMove(MouseEventArgs e) {
            target.KillToolTip();
            node.Position = Point.Subtract(e.Location, offset);
            target.Redraw();
        }

        public override void Gen_MouseUp(MouseEventArgs e) {
            Reset();
        }
    }

    class InsertingState : GroundState
    {
        private Point newMachinePosition;

        public InsertingState(IManipulationTarget target, IStateContext context, Point cursorPosition) : base(target, context) {
            this.newMachinePosition = cursorPosition;
        }

        public override void NewMachineTypeChosen(String type) {
            target.InsertNewMachineType(type, newMachinePosition);
            Reset();
        }
    }

    class ConnectingState : State
    {
        private GUINode fromNode;
        private Point cursorLocation;

        public ConnectingState(IManipulationTarget target, IStateContext context, GUINode fromNode) : base(target, context) {
            this.fromNode = fromNode;
            this.cursorLocation = fromNode.Position;
            target.ShowState(this.GetType());
        }

        public override void Draw(Graphics graphics) {
            target.DrawConnector(graphics, cursorLocation, fromNode);
        }

        public override void NodeMouseUp(GUINode node, MouseEventArgs e) {
            if (node != fromNode) {
                Transition(new ConnectingPortState(target, context, fromNode, node, e.Location));
                target.ShowOutputChooser(fromNode, e.Location);
            } else {
                Reset();
            }
        }

        // Called if ! NodeMouseUp
        public override void Gen_MouseUp(MouseEventArgs e) {
            Reset();
        }

        public override void MouseMove(MouseEventArgs e) {
            target.ManageToolTip(e.Location);
            cursorLocation = e.Location;
            target.Redraw();
        }
    }

    class ConnectingPortState : GroundState
    {
        private GUINode fromNode; // Not actually needed
        private GUINode toNode;
        private GUIPort fromPort;
        private Point menuPosition;

        public ConnectingPortState(IManipulationTarget target, IStateContext context, GUINode fromNode, GUINode toNode, Point cursorPosition) : base(target, context) {
            this.fromNode = fromNode;
            this.toNode = toNode;
            this.menuPosition = cursorPosition;
        }

        public override void OutputPortChosen(GUIPort fromPort) {
            this.fromPort = fromPort;
            target.ShowInputChooser(toNode, menuPosition);
        }

        public override void InputPortChosen(GUIPort toPort) {
            target.ConnectPorts(fromPort, toPort);
            Reset();
        }
    }

    class PortContextState : GroundState
    {
        private GUIPort port;

        public PortContextState(IManipulationTarget target, IStateContext context, GUIPort port) : base(target, context) {
            this.port = port;
        }

        public override void DisconnectPortChosen() {
            target.DisconnectPort(port);
            Reset();
        }

        public override void BranchPortChosen() {
            target.BranchPort(port);
            Reset();
        }
    }
}
