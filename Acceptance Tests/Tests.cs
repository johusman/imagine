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
        string DEST_FILE2 = System.IO.Directory.GetCurrentDirectory() + "\\test2.png";
        string COMP_FILE = System.IO.Directory.GetCurrentDirectory() + "\\nausicaa.png";
        string INV_FILE = System.IO.Directory.GetCurrentDirectory() + "\\nausicaa_inverted.png";
        string BLUE_FILE = System.IO.Directory.GetCurrentDirectory() + "\\nausicaa_blue.png";
        string RED_FILE = System.IO.Directory.GetCurrentDirectory() + "\\nausicaa_red.png";

        [SetUp]
        public void Init()
        {
            facade = new ImagineFacade(".");
            facade.Sources[0].Filename = SRC_FILE;
            facade.Destinations[0].Filename = DEST_FILE;
            facade.Connect(facade.Sources[0], 0, facade.Destinations[0], 0);
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
            Assert.AreEqual(SRC_FILE, facade.Sources[0].Filename);
            Assert.AreEqual(DEST_FILE, facade.Destinations[0].Filename);
        }

        [Test]
        public void that_you_can_be_notified_of_changes_in_source_and_destination()
        {
            facade = new ImagineFacade(".");
            
            string SRC = "hej";
            string DEST = "hopp";
            int called = 0;

            facade.GraphChanged += new System.EventHandler(
                delegate(object sender, EventArgs args)
                { called++; });

            facade.Sources[0].Filename = SRC;
            facade.Destinations[0].Filename = DEST;

            Assert.AreEqual(2, called);
            Assert.AreEqual(SRC, facade.Sources[0].Filename);
            Assert.AreEqual(DEST, facade.Destinations[0].Filename);
        }

        [Test]
        public void that_you_can_be_notified_of_changes_in_machine_parameters()
        {
            object eventSender = null;

            facade = new ImagineFacade(".");
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
            Machine source = facade.Sources[0];
            Machine destination = facade.Destinations[0];

            Assert.IsNotNull(source);
            Assert.IsNotNull(destination);
            Assert.AreEqual(SRC_FILE, ((SourceMachine)source).Filename);
            Assert.AreEqual(DEST_FILE, ((SinkMachine)destination).Filename);
        }

        [Test]
        public void that_source_and_destination_are_connectable()
        {
            // Already connected in Setup

            GraphNode<Machine> source = facade.Graph.GetNodeFor(facade.Sources[0]);
            GraphNode<Machine> destination = facade.Graph.GetNodeFor(facade.Destinations[0]);

            Assert.AreSame(destination, source.Outports[0].RemotePort.Node, "Source knows destination");
            Assert.AreSame(source, destination.Inports[0].RemotePort.Node, "Destination knows source");
        }

        [Test]
        public void that_nodes_are_disconnectable()
        {
            // Already connected in Setup

            GraphNode<Machine> source = facade.Graph.GetNodeFor(facade.Sources[0]);
            GraphNode<Machine> destination = facade.Graph.GetNodeFor(facade.Destinations[0]);

            facade.Disconnect(facade.Sources[0], 0, facade.Destinations[0], 0);

            Assert.AreEqual(0, source.Outports.Count);
            Assert.AreEqual(0, destination.Inports.Count);
        }

        [Test]
        public void that_we_can_put_an_image_inverter_between_source_and_destination()
        {
            facade.Disconnect(facade.Sources[0], 0, facade.Destinations[0], 0);

            Machine inverter = facade.NewMachine("Imagine.Img.Inverter");
            facade.Connect(facade.Sources[0], 0, inverter, 0);
            facade.Connect(inverter, 0, facade.Destinations[0], 0);

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
            facade.Disconnect(facade.Sources[0], 0, facade.Destinations[0], 0);

            Machine splitter = facade.NewMachine("Imagine.Img.RGBSplitter");
            Machine composer = facade.NewMachine("Imagine.Ctrl.RGBJoiner");
            facade.Connect(facade.Sources[0], 0, splitter, 0);
            facade.Connect(splitter, 2, composer, 2);
            facade.Connect(composer, 0, facade.Destinations[0], 0);
            

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
        public void that_we_support_multiple_sources_and_destinations()
        {
            facade.AddMachine(new SourceMachine());
            facade.AddMachine(new SinkMachine());

            facade.Disconnect(facade.Sources[0], 0, facade.Destinations[0], 0);

            Assert.AreEqual(2, facade.Sources.Count);
            Assert.AreEqual(2, facade.Destinations.Count);

            facade.Sources[0].Filename = BLUE_FILE;
            facade.Sources[1].Filename = RED_FILE;
            facade.Connect(facade.Sources[0], 0, facade.Destinations[1], 0);
            facade.Connect(facade.Sources[1], 0, facade.Destinations[0], 0);
            facade.Destinations[0].Filename = DEST_FILE;
            facade.Destinations[1].Filename = DEST_FILE2;

            try
            {
                facade.Generate();
                AssertBitmapFilesAreEqual(RED_FILE, DEST_FILE);
                AssertBitmapFilesAreEqual(BLUE_FILE, DEST_FILE2);
            }
            finally
            {
                System.IO.File.Delete(DEST_FILE);
                System.IO.File.Delete(DEST_FILE2);
            }

            facade.RemoveMachine(facade.Sources[0]);
            facade.RemoveMachine(facade.Destinations[0]);

            Assert.AreEqual(1, facade.Sources.Count);
            Assert.AreEqual(1, facade.Destinations.Count);
        }

        [Test]
        public void that_we_support_zero_sources_and_destinations()
        {
            facade.RemoveMachine(facade.Sources[0]);
            facade.RemoveMachine(facade.Destinations[0]);

            Assert.AreEqual(0, facade.Sources.Count);
            Assert.AreEqual(0, facade.Destinations.Count);

            facade.Generate();
        }
            
        [Test]
        public void that_generation_reports_on_progress()
        {
            facade.Disconnect(facade.Sources[0], 0, facade.Destinations[0], 0);

            Machine inverter = facade.NewMachine("Imagine.Img.Inverter");
            facade.Connect(facade.Sources[0], 0, inverter, 0);
            facade.Connect(inverter, 0, facade.Destinations[0], 0);

            try
            {
                facade.Generate(ReportProgress);
                AssertBitmapFilesAreEqual(INV_FILE, DEST_FILE);
                Assert.Contains(new ProgressReport(0, 3, facade.Sources[0], 0), reports);
                Assert.Contains(new ProgressReport(0, 3, facade.Sources[0], 100), reports);
                Assert.Contains(new ProgressReport(1, 3, inverter, 0), reports);
                Assert.Contains(new ProgressReport(1, 3, inverter, 100), reports);
                Assert.Contains(new ProgressReport(2, 3, facade.Destinations[0], 0), reports);
                Assert.Contains(new ProgressReport(2, 3, facade.Destinations[0], 100), reports);
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

    [UniqueName("Test.Dummy")]
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
