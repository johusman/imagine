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
        private ImagineFacade facade = new ImagineFacade();

        string SRC_FILE = System.IO.Directory.GetCurrentDirectory() + "\\Leighton_Idyll.jpg";
        string DEST_FILE = System.IO.Directory.GetCurrentDirectory() + "\\test.jpg";

        string callbackSourceFilename;
        string callbackDestinationFilename;

        [SetUp]
        public void Init()
        {
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
        public void that_source_and_destination_are_represented_as_nodes()
        {
            Machine sourceNode = facade.SourceNode;
            Machine destinationNode = facade.DestinationNode;

            Assert.IsNotNull(sourceNode);
            Assert.IsNotNull(destinationNode);
            Assert.AreEqual(SRC_FILE, ((SourceNode)sourceNode).Filename);
            Assert.AreEqual(DEST_FILE, ((SinkNode)destinationNode).Filename);
        }

        [Test]
        public void that_source_and_destination_are_connected()
        {
            GraphNode<Machine> source = facade.Graph.GetNodeFor(facade.SourceNode);
            GraphNode<Machine> destination = facade.Graph.GetNodeFor(facade.DestinationNode);

            Assert.That(source.Outputs.Contains(destination));
            Assert.That(destination.Inputs.Contains(source));
        }
    }
}
