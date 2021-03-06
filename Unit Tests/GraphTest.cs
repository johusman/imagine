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

        [Test]
        public void that_we_can_delete_a_node()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);

            graph.Connect(node1, 0, node2, 0);

            graph.RemoveNode(node1);

            Assert.AreEqual(1, graph.NodeCount, "NodeCount");
            Assert.AreEqual(0, graph.ConnectionCount, "ConnectionCount");
            Assert.AreEqual(0, node2.InputCount, "node2.InputCount");
            Assert.AreEqual(0, node1.OutputCount, "node1.OutputCount");
        }

        [Test]
        public void that_deleting_an_external_node_has_no_effect()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);
            GraphNode<String> node3 = new GraphNode<string>(machines[2]); // External node

            graph.Connect(node1, 0, node2, 0);

            graph.RemoveNode(node3);

            Assert.AreEqual(2, graph.NodeCount, "NodeCount");
            Assert.AreEqual(1, graph.ConnectionCount, "ConnectionCount");
            Assert.AreEqual(1, node2.InputCount, "node2.InputCount");
            Assert.AreEqual(1, node1.OutputCount, "node1.OutputCount");
        }
    }
}
