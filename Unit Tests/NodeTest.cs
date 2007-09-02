using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Imagine.Library
{
    [TestFixture]
    public class NodeTest
    {
        [Test]
        public void that_source_nodes_can_be_instantiated_and_configured()
        {
            SourceNode node = new SourceNode();
            node.Filename = "Hejhopp";

            Assert.AreEqual("Hejhopp", node.Filename);
            Assert.IsNotNull(node as Machine);
        }

        [Test]
        public void that_destination_nodes_can_be_instantiated_and_configured()
        {
            SinkNode node = new SinkNode();
            node.Filename = "Hejhopp";

            Assert.AreEqual("Hejhopp", node.Filename);
            Assert.IsNotNull(node as Machine);
        }
    }
}
