using System;
using System.Collections.Generic;
using System.Text;

namespace Imagine.Library
{
    public class ImagineFacade
    {
        private SourceMachine sourceMachine;
        private SinkMachine destinationMachine;

        private Graph<Machine> graph;

        public Graph<Machine> Graph
        {
            get { return graph; }
            set { graph = value; }
        }

        public event System.EventHandler SourceChanged;
        public event System.EventHandler DestinationChanged;

        public SourceMachine SourceMachine
        {
            get { return sourceMachine; }
        }

        public SinkMachine DestinationMachine
        {
            get { return destinationMachine; }
        }


        public ImagineFacade()
        {
            graph = new Graph<Machine>();
            sourceMachine = new SourceMachine();
            destinationMachine = new SinkMachine();

            graph.Connect(graph.AddNode(sourceMachine), graph.AddNode(destinationMachine));
        }

        public void OpenSource(string filename)
        {
            sourceMachine.Filename = filename;
            if(SourceChanged != null)
                SourceChanged.Invoke(this, new StringEventArg(filename));
        }

        public void OpenDestination(string filename)
        {
            destinationMachine.Filename = filename;
            if(DestinationChanged != null)
                DestinationChanged.Invoke(this, new StringEventArg(filename));
        }

        public void Generate()
        {
            System.IO.File.Copy(sourceMachine.Filename, destinationMachine.Filename, true);
        }

        public string GetSourceFilename()
        {
            return sourceMachine.Filename;
        }

        public string GetDestinationFilename()
        {
            return destinationMachine.Filename;
        }

        public void Disconnect(Machine machine1, Machine machine2)
        {
            graph.Disconnect(graph.GetNodeFor(machine1), graph.GetNodeFor(machine2));
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
}
