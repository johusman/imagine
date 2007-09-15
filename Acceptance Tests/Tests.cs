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

        string callbackSourceFilename;
        string callbackDestinationFilename;

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

            facade.SourceChanged += new System.EventHandler(
                delegate(object sender, EventArgs args)
                { callbackSourceFilename = args.ToString(); });
            facade.DestinationChanged += new System.EventHandler(
                delegate(object sender, EventArgs args)
                { callbackDestinationFilename = args.ToString(); });

            facade.OpenSource(SRC);
            facade.OpenDestination(DEST);

            Assert.AreEqual(SRC, callbackSourceFilename);
            Assert.AreEqual(DEST, callbackDestinationFilename);
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
}
