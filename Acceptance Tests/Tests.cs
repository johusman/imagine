using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Imagine.Library;
using System.Drawing;

namespace Imagine.AcceptanceTests
{
    [TestFixture]
    public class Tests
    {
        private ImagineFacade facade;

        string SRC_FILE = System.IO.Directory.GetCurrentDirectory() + "\\nausicaa.png";
        string DEST_FILE = System.IO.Directory.GetCurrentDirectory() + "\\test.png";
        string COMP_FILE = System.IO.Directory.GetCurrentDirectory() + "\\nausicaa.png";
        string INV_FILE = System.IO.Directory.GetCurrentDirectory() + "\\nausicaa_inverted.png";
        string BLUE_FILE = System.IO.Directory.GetCurrentDirectory() + "\\nausicaa_blue.png";
        string RED_FILE = System.IO.Directory.GetCurrentDirectory() + "\\nausicaa_red.png";

        [SetUp]
        public void Init()
        {
            facade = new ImagineFacade();
            facade.OpenSource(SRC_FILE);
            facade.OpenDestination(DEST_FILE);
        }

        [Test]
        public void that_you_can_open_a_source_and_a_destination_and_open_and_save_a_picture_between_them()
        {
            try
            {
                facade.Generate();
                AssertBitmapFilesAreEqual(COMP_FILE, DEST_FILE);
            }
            finally
            {
                System.IO.File.Delete(DEST_FILE);
            }
        }

        [Test]
        public void that_you_can_retrieve_current_source_and_destination()
        {
            Assert.AreEqual(SRC_FILE, facade.GetSourceFilename());
            Assert.AreEqual(DEST_FILE, facade.GetDestinationFilename());
        }

        [Test]
        public void that_you_can_be_notified_of_changes_in_source_and_destination()
        {
            facade = new ImagineFacade();
            
            string SRC = "hej";
            string DEST = "hopp";
            int called = 0;

            facade.GraphChanged += new System.EventHandler(
                delegate(object sender, EventArgs args)
                { called++; });

            facade.OpenSource(SRC);
            facade.OpenDestination(DEST);

            Assert.AreEqual(2, called);
            Assert.AreEqual(SRC, facade.SourceMachine.Filename);
            Assert.AreEqual(DEST, facade.DestinationMachine.Filename);
        }

        [Test]
        public void that_you_can_be_notified_of_changes_in_machine_parameters()
        {
            object eventSender = null;

            facade = new ImagineFacade();
            facade.GraphChanged += new System.EventHandler(
                delegate(object sender, EventArgs args)
                { eventSender = sender; });
            DummyMachine dummy = new DummyMachine();
            facade.AddMachine(dummy);
            eventSender = null;
            
            dummy.DummyValue = 4;
            Assert.AreSame(dummy, eventSender);
        }

        [Test]
        public void that_source_and_destination_are_represented_as_machines()
        {
            Machine source = facade.SourceMachine;
            Machine destination = facade.DestinationMachine;

            Assert.IsNotNull(source);
            Assert.IsNotNull(destination);
            Assert.AreEqual(SRC_FILE, ((SourceMachine)source).Filename);
            Assert.AreEqual(DEST_FILE, ((SinkMachine)destination).Filename);
        }

        [Test]
        public void that_source_and_destination_are_connected()
        {
            GraphNode<Machine> source = facade.Graph.GetNodeFor(facade.SourceMachine);
            GraphNode<Machine> destination = facade.Graph.GetNodeFor(facade.DestinationMachine);

            Assert.AreSame(destination, source.Outports[0].RemotePort.Node, "Source knows destination");
            Assert.AreSame(source, destination.Inports[0].RemotePort.Node, "Destination knows source");
        }

        [Test]
        public void that_nodes_are_disconnectable()
        {
            GraphNode<Machine> source = facade.Graph.GetNodeFor(facade.SourceMachine);
            GraphNode<Machine> destination = facade.Graph.GetNodeFor(facade.DestinationMachine);

            facade.Disconnect(facade.SourceMachine, 0, facade.DestinationMachine, 0);

            Assert.AreEqual(0, source.Outports.Count);
            Assert.AreEqual(0, destination.Inports.Count);
        }

        [Test]
        public void that_we_can_put_an_image_inverter_between_source_and_destination()
        {
            facade.Disconnect(facade.SourceMachine, 0, facade.DestinationMachine, 0);

            Machine inverter = facade.NewMachine("Imagine.Inverter");
            facade.Connect(facade.SourceMachine, 0, inverter, 0);
            facade.Connect(inverter, 0, facade.DestinationMachine, 0);

            try
            {
                facade.Generate();
                AssertBitmapFilesAreEqual(INV_FILE, DEST_FILE);
            }
            finally
            {
                System.IO.File.Delete(DEST_FILE);
            }
        }

        [Test]
        public void that_we_can_connect_to_different_ports()
        {
            facade.Disconnect(facade.SourceMachine, 0, facade.DestinationMachine, 0);

            Machine splitter = facade.NewMachine("Imagine.RGBSplitter");
            Machine composer = facade.NewMachine("Imagine.RGBJoiner");
            facade.Connect(facade.SourceMachine, 0, splitter, 0);
            facade.Connect(splitter, 2, composer, 2);
            facade.Connect(composer, 0, facade.DestinationMachine, 0);
            

            try
            {
                facade.Generate();
                AssertBitmapFilesAreEqual(BLUE_FILE, DEST_FILE);
            }
            finally
            {
                System.IO.File.Delete(DEST_FILE);
            }

            facade.Disconnect(splitter, 2, composer, 2);
            facade.Connect(splitter, 0, composer, 0);
            
            try
            {
                facade.Generate();
                AssertBitmapFilesAreEqual(RED_FILE, DEST_FILE);
            }
            finally
            {
                System.IO.File.Delete(DEST_FILE);
            }

        }

        [Test]
        public void that_generation_reports_on_progress()
        {
            facade.Disconnect(facade.SourceMachine, 0, facade.DestinationMachine, 0);

            Machine inverter = facade.NewMachine("Imagine.Inverter");
            facade.Connect(facade.SourceMachine, 0, inverter, 0);
            facade.Connect(inverter, 0, facade.DestinationMachine, 0);

            try
            {
                facade.Generate(ReportProgress);
                AssertBitmapFilesAreEqual(INV_FILE, DEST_FILE);
                Assert.Contains(new ProgressReport(0, 3, facade.SourceMachine, 0), reports);
                Assert.Contains(new ProgressReport(0, 3, facade.SourceMachine, 100), reports);
                Assert.Contains(new ProgressReport(1, 3, inverter, 0), reports);
                Assert.Contains(new ProgressReport(1, 3, inverter, 100), reports);
                Assert.Contains(new ProgressReport(2, 3, facade.DestinationMachine, 0), reports);
                Assert.Contains(new ProgressReport(2, 3, facade.DestinationMachine, 100), reports);
            }
            finally
            {
                System.IO.File.Delete(DEST_FILE);
            }
        }

        List<ProgressReport> reports = new List<ProgressReport>();
        public void ReportProgress(int machineIndex, int totalMachines, Machine currentMachine, int currentPercent)
        {
            reports.Add(new ProgressReport(machineIndex, totalMachines, currentMachine, currentPercent));
        }

        [Test]
        public void that_we_can_save_a_simple_graph()
        {
            string serialize = facade.SerializeGraph();
            Assert.AreEqual("Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +
                "\tImagine.Destination 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t}\n" +
                "}",
                serialize);
        }

        [Test]
        public void that_we_can_save_a_complex_graph()
        {
            facade.Disconnect(facade.SourceMachine, 0, facade.DestinationMachine, 0);

            Machine inverter = facade.NewMachine("Imagine.Inverter");
            facade.Connect(facade.SourceMachine, 0, inverter, 0);

            Machine splitter = facade.NewMachine("Imagine.RGBSplitter");
            facade.Connect(inverter, 0, splitter, 0);

            Machine branch = facade.NewMachine("Imagine.Branch4");
            facade.Connect(splitter, 0, branch, 0);

            Machine joiner = facade.NewMachine("Imagine.RGBJoiner");
            facade.Connect(branch, 0, joiner, 0);
            facade.Connect(branch, 1, joiner, 1);
            facade.Connect(splitter, 2, joiner, 2);

            facade.Connect(joiner, 0, facade.DestinationMachine, 0);

            string serialize = facade.SerializeGraph();
            Assert.AreEqual("Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +

                "\tImagine.Inverter 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t}\n" +

                "\tImagine.RGBSplitter 'machine2' {\n" +
                "\t\t'machine1' -> \n" +
                "\t}\n" +

                "\tImagine.Branch4 'machine3' {\n" +
                "\t\t'machine2':r -> \n" +
                "\t}\n" +

                "\tImagine.RGBJoiner 'machine4' {\n" +
                "\t\t'machine3':1 -> r\n" +
                "\t\t'machine3':2 -> g\n" +
                "\t\t'machine2':b -> b\n" +
                "\t}\n" +

                "\tImagine.Destination 'machine5' {\n" +
                "\t\t'machine4' -> \n" +
                "\t}\n" +
                "}",
                serialize);
        }

        [Test]
        public void that_we_can_save_an_unconnected_graph()
        {
            facade.Disconnect(facade.SourceMachine, 0, facade.DestinationMachine, 0);

            string serialize = facade.SerializeGraph();
            Assert.AreEqual("Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +
                "\tImagine.Destination 'machine1' {}\n" +
                "}",
                serialize);
        }

        [Test]
        public void that_we_can_load_a_simple_graph()
        {
            Machine oldSource = facade.SourceMachine;
            Machine oldDestination = facade.DestinationMachine;

            string serialize =
                "Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +
                "\tImagine.Destination 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t}\n" +
                "}";

            facade.DeserializeGraph(serialize);
            Assert.AreNotSame(oldSource, facade.SourceMachine);
            Assert.AreNotSame(oldDestination, facade.DestinationMachine);
            Assert.AreSame(facade.DestinationMachine, facade.Graph.GetNodeFor(facade.SourceMachine).Outports[0].RemotePort.Node.Machine);
        }

        [Test]
        public void that_we_can_load_an_unconnected_graph()
        {
            Machine oldSource = facade.SourceMachine;
            Machine oldDestination = facade.DestinationMachine;
            
            string serialize =
                "Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +
                "\tImagine.Destination 'machine1' {}\n" +
                "}";
            facade.DeserializeGraph(serialize);
            Assert.AreNotSame(oldSource, facade.SourceMachine);
            Assert.AreNotSame(oldDestination, facade.DestinationMachine);
            Assert.AreEqual(0, facade.Graph.GetNodeFor(facade.SourceMachine).OutputCount);
            Assert.AreEqual(0, facade.Graph.GetNodeFor(facade.DestinationMachine).InputCount);
            Assert.AreEqual(2, facade.Graph.NodeCount);
            Assert.AreEqual(0, facade.Graph.ConnectionCount);
        }

        [Test]
        public void that_we_can_load_a_complex_graph()
        {
            Machine oldSource = facade.SourceMachine;
            Machine oldDestination = facade.DestinationMachine;

            string serialize =
                "Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +

                "\tImagine.Inverter 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t}\n" +

                "\tImagine.RGBSplitter 'machine2' {\n" +
                "\t\t'machine1' -> \n" +
                "\t}\n" +

                "\tImagine.Branch4 'machine3' {\n" +
                "\t\t'machine2':r -> \n" +
                "\t}\n" +

                "\tImagine.RGBJoiner 'machine4' {\n" +
                "\t\t'machine3':1 -> r\n" +
                "\t\t'machine3':2 -> g\n" +
                "\t\t'machine2':b -> b\n" +
                "\t}\n" +

                "\tImagine.Destination 'machine5' {\n" +
                "\t\t'machine4' -> \n" +
                "\t}\n" +
                "}";
            facade.DeserializeGraph(serialize);
            Graph<Machine> graph = facade.Graph;
            Assert.AreNotSame(oldSource, facade.SourceMachine);
            Assert.AreNotSame(oldDestination, facade.DestinationMachine);
            Assert.AreEqual(1, graph.GetNodeFor(facade.SourceMachine).OutputCount);
            Assert.AreEqual(1, graph.GetNodeFor(facade.DestinationMachine).InputCount);
            Assert.AreEqual(6, graph.NodeCount);
            Assert.AreEqual(7, graph.ConnectionCount);
        }

        [Test]
        public void that_we_can_load_from_correct_section()
        {
            Machine oldSource = facade.SourceMachine;
            Machine oldDestination = facade.DestinationMachine;

            string serialize =
                "Blah { hej { } hopp {\n hejsan {\n}} }\n" +

                "Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +

                "\tImagine.Inverter 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t}\n" +

                "\tImagine.RGBSplitter 'machine2' {\n" +
                "\t\t'machine1' -> \n" +
                "\t}\n" +

                "\tImagine.Branch4 'machine3' {\n" +
                "\t\t'machine2':r -> \n" +
                "\t}\n" +

                "\tImagine.RGBJoiner 'machine4' {\n" +
                "\t\t'machine3':1 -> r\n" +
                "\t\t'machine3':2 -> g\n" +
                "\t\t'machine2':b -> b\n" +
                "\t}\n" +

                "\tImagine.Destination 'machine5' {\n" +
                "\t\t'machine4' -> \n" +
                "\t}\n" +
                "}\n" +
                
                "Hoho { mjau { } hopp {\n hejsan {\n}} }\n";
            facade.DeserializeGraph(serialize);
            Graph<Machine> graph = facade.Graph;
            Assert.AreNotSame(oldSource, facade.SourceMachine);
            Assert.AreNotSame(oldDestination, facade.DestinationMachine);
            Assert.AreEqual(1, graph.GetNodeFor(facade.SourceMachine).OutputCount);
            Assert.AreEqual(1, graph.GetNodeFor(facade.DestinationMachine).InputCount);
            Assert.AreEqual(6, graph.NodeCount);
            Assert.AreEqual(7, graph.ConnectionCount);
        }

        private void AssertBitmapsAreEqual(Bitmap bitmap1, Bitmap bitmap2)
        {
            Assert.AreEqual(bitmap1.Width, bitmap2.Width, "Width");
            Assert.AreEqual(bitmap1.Height, bitmap2.Height, "Height");

            for(int x = 0; x < bitmap1.Width; x++)
                for(int y = 0; y < bitmap1.Height; y++)
                    Assert.AreEqual(bitmap1.GetPixel(x, y), bitmap2.GetPixel(x, y), "Pixel at (" + x + ", " + y + ")");
        }

        private void AssertBitmapFilesAreEqual(string filename1, string filename2)
        {
            using(Bitmap bitmap1 = (Bitmap)Image.FromFile(filename1))
            using(Bitmap bitmap2 = (Bitmap)Image.FromFile(filename2))
                AssertBitmapsAreEqual(bitmap1, bitmap2);
        }
    }

    struct ProgressReport
    {
        public ProgressReport(int machineIndex, int totalMachines, Machine currentMachine, int currentPercent)
        {
            this.machineIndex = machineIndex;
            this.totalMachines = totalMachines;
            this.currentMachine = currentMachine;
            this.currentPercent = currentPercent;
        }

        public int machineIndex;
        public int totalMachines;
        public Machine currentMachine;
        public int currentPercent;
    }

    public class DummyMachine : Machine
    {
        public DummyMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output1" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Does nothing.";
        }

        private int dummyValue;
        public int DummyValue
        {
            get { return dummyValue; }
            set { dummyValue = value; OnMachineChanged(); }
        }

        public override string Caption
        {
            get { return "Dummy"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            return null;
        }
    }
}
