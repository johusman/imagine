using System;
using System.Collections.Generic;
using System.Text;

namespace Imagine.Library
{
    public class ImagineFacade
    {
        private SourceNode sourceNode;
        private SinkNode destinationNode;

        private Graph<Machine> graph;

        public Graph<Machine> Graph
        {
            get { return graph; }
            set { graph = value; }
        }

        public event System.EventHandler SourceChanged;
        public event System.EventHandler DestinationChanged;

        public SourceNode SourceNode
        {
            get { return sourceNode; }
        }

        public SinkNode DestinationNode
        {
            get { return destinationNode; }
        }


        public ImagineFacade()
        {
            graph = new Graph<Machine>();
            sourceNode = new SourceNode();
            destinationNode = new SinkNode();

            graph.Connect(graph.AddNode(sourceNode), graph.AddNode(destinationNode));
        }

        public void OpenSource(string filename)
        {
            sourceNode.Filename = filename;
            if(SourceChanged != null)
                SourceChanged.Invoke(this, new StringEventArg(filename));
        }

        public void OpenDestination(string filename)
        {
            destinationNode.Filename = filename;
            if(DestinationChanged != null)
                DestinationChanged.Invoke(this, new StringEventArg(filename));
        }

        public void Generate()
        {
            System.IO.File.Copy(sourceNode.Filename, destinationNode.Filename, true);
        }

        public string GetSourceFilename()
        {
            return sourceNode.Filename;
        }

        public string GetDestinationFilename()
        {
            return destinationNode.Filename;
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
