using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Imagine.Library;

namespace Imagine.AcceptanceTests
{
    [TestFixture]
    public class Tests
    {
        private ImagineFacade facade;

        string SRC_FILE = System.IO.Directory.GetCurrentDirectory() + "\\Leighton_Idyll.jpg";
        string DEST_FILE = System.IO.Directory.GetCurrentDirectory() + "\\test.jpg";

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
                FileAssert.AreEqual(SRC_FILE, DEST_FILE);
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

            Assert.That(source.Outputs.Contains(destination), "Source knows destination");
            Assert.That(destination.Inputs.Contains(source), "Destination knows source");
        }

        [Test]
        public void that_nodes_are_disconnectable()
        {
            GraphNode<Machine> source = facade.Graph.GetNodeFor(facade.SourceMachine);
            GraphNode<Machine> destination = facade.Graph.GetNodeFor(facade.DestinationMachine);

            facade.Disconnect(facade.SourceMachine, facade.DestinationMachine);

            Assert.IsTrue(!source.Outputs.Contains(destination));
            Assert.IsTrue(!destination.Inputs.Contains(source));
        }

    }
}
