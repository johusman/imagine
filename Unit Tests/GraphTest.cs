using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Imagine.Library
{
    [TestFixture]
    public class GraphTest
    {
        private Graph<String> graph;
        String[] machines = { "machine1", "machine2", "machine3" };

        [SetUp]
        public void init()
        {
            graph = new Graph<String>();
        }

        [Test]
        public void that_we_can_instantiate_a_graph()
        {
            Assert.AreEqual(0, graph.NodeCount, "NodeCount");
            Assert.AreEqual(0, graph.ConnectionCount, "ConnectionCount");
        }

        [Test]
        public void that_we_can_add_a_node()
        {
            GraphNode<String> node = graph.AddNode(machines[0]);

            Assert.AreEqual(1, graph.NodeCount, "NodeCount");
            Assert.AreEqual(0, graph.ConnectionCount, "ConnectionCount");
            Assert.AreSame(machines[0], node.Machine, "Machine");
            Assert.AreEqual(0, node.InputCount, "InputCount");
            Assert.AreEqual(0, node.OutputCount, "OutputCount");
        }

        [Test, ExpectedExceptionAttribute(typeof(ArgumentNullException))]
        public void that_adding_null_is_an_error()
        {
            graph.AddNode(null);
        }

        [Test]
        public void that_we_can_retrieve_the_node_corresponding_to_a_particular_machine()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);
            
            Assert.AreSame(node1, graph.GetNodeFor(machines[0]));
            Assert.AreSame(node2, graph.GetNodeFor(machines[1]));
        }

        [Test]
        public void various_GetNodeFor_error_conditions()
        {
            // Null
            try
            {
                graph.GetNodeFor(null);
                Assert.Fail("Expected ArgumentNullException");
            } catch(ArgumentNullException) {}

            // Nonexistent node
            Assert.IsNull(graph.GetNodeFor("unregistered"));
        }
    }
}
