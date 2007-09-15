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
        public void that_we_can_connect_two_nodes()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);

            Assert.AreEqual(2, graph.NodeCount, "NodeCount");
            Assert.AreEqual(0, graph.ConnectionCount, "ConnectionCount");
            Assert.AreSame(machines[0], node1.Machine, "node1.Machine");
            Assert.AreSame(machines[1], node2.Machine, "node2.Machine");
            Assert.AreEqual(0, node1.InputCount, "node1.InputCount");
            Assert.AreEqual(0, node1.OutputCount, "node1.OutputCount");
            Assert.AreEqual(0, node2.InputCount, "node2.InputCount");
            Assert.AreEqual(0, node2.OutputCount, "node2.OutputCount");

            graph.Connect(node1, node2);

            Assert.AreEqual(1, graph.ConnectionCount, "ConnectionCount");
            Assert.AreEqual(0, node1.InputCount, "node1.InputCount");
            Assert.AreEqual(1, node1.OutputCount, "node1.OutputCount");
            Assert.AreEqual(1, node2.InputCount, "node2.InputCount");
            Assert.AreEqual(0, node2.OutputCount, "node2.OutputCount");

            Assert.AreSame(node2, node1.Outputs[0], "node1.Outputs[0]");
            Assert.AreSame(node1, node2.Inputs[0], "node2.Inputs[0]");
        }

        [Test]
        public void that_connection_involving_null_is_an_error()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);

            try
            {
                graph.Connect(node1, null);
                Assert.Fail("Expected ArgumentNullException");
            } catch(ArgumentNullException) {}

            try
            {
                graph.Connect(null, node2);
                Assert.Fail("Expected ArgumentNullException");
            } catch(ArgumentNullException) {}

            try
            {
                graph.Connect(null, null);
                Assert.Fail("Expected ArgumentNullException");
            } catch(ArgumentNullException) { }
        }

        [Test]
        public void that_we_cannot_connect_nodes_from_outside_the_graph()
        {
            GraphNode<String> node = graph.AddNode(machines[0]);
            GraphNode<String> externalNode = new GraphNode<string>("external");

            try
            {
                graph.Connect(node, externalNode);
                Assert.Fail("Expected ArgumentException");
            }
            catch(ArgumentException) { }

            try
            {
                graph.Connect(externalNode, node);
                Assert.Fail("Expected ArgumentException");
            }
            catch(ArgumentException) { }
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
        public void that_we_can_disconnect_nodes()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);

            graph.Connect(node1, node2);
            graph.Disconnect(node1, node2);
            
            Assert.AreEqual(2, graph.NodeCount, "NodeCount");
            Assert.AreEqual(0, graph.ConnectionCount, "ConnectionCount");
            Assert.AreSame(machines[0], node1.Machine, "node1.Machine");
            Assert.AreSame(machines[1], node2.Machine, "node2.Machine");
            Assert.AreEqual(0, node1.InputCount, "node1.InputCount");
            Assert.AreEqual(0, node1.OutputCount, "node1.OutputCount");
            Assert.AreEqual(0, node2.InputCount, "node2.InputCount");
            Assert.AreEqual(0, node2.OutputCount, "node2.OutputCount");
        }

        [Test]
        public void that_disconnecting_unconnected_nodes_does_nothing()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);

            graph.Disconnect(node1, node2);

            Assert.AreEqual(2, graph.NodeCount, "NodeCount");
            Assert.AreEqual(0, graph.ConnectionCount, "ConnectionCount");
            Assert.AreSame(machines[0], node1.Machine, "node1.Machine");
            Assert.AreSame(machines[1], node2.Machine, "node2.Machine");
            Assert.AreEqual(0, node1.InputCount, "node1.InputCount");
            Assert.AreEqual(0, node1.OutputCount, "node1.OutputCount");
            Assert.AreEqual(0, node2.InputCount, "node2.InputCount");
            Assert.AreEqual(0, node2.OutputCount, "node2.OutputCount");
        }

        [Test]
        public void that_we_cannot_disconnect_nodes_from_outside_the_graph()
        {
            GraphNode<String> node = graph.AddNode(machines[0]);
            
            Graph<String> graph2 = new Graph<string>();
            GraphNode<String> externalNode1 = graph2.AddNode("external1");
            GraphNode<String> externalNode2 = graph2.AddNode("external2");
            graph2.Connect(externalNode1, externalNode2);

            try
            {
                graph.Disconnect(node, externalNode1);
                Assert.Fail("Expected ArgumentException");
            } catch(ArgumentException) { }

            try
            {
                graph.Disconnect(externalNode1, node);
                Assert.Fail("Expected ArgumentException");
            } catch(ArgumentException) { }

            try
            {
                graph.Disconnect(externalNode1, externalNode2);
                Assert.Fail("Expected ArgumentException");
            }
            catch(ArgumentException) { }
        }

        [Test]
        public void that_we_can_generate_a_simple_topological_ordering()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);
            GraphNode<String> node3 = graph.AddNode(machines[2]);

            graph.Connect(node1, node2);
            graph.Connect(node2, node3);

            List<GraphNode<String>> order = graph.GetTopologicalOrdering();

            Assert.AreEqual(node1, order[0]);
            Assert.AreEqual(node2, order[1]);
            Assert.AreEqual(node3, order[2]);
        }

        [Test]
        public void that_we_can_generate_a_more_complex_topological_ordering()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);
            GraphNode<String> node3 = graph.AddNode(machines[2]);
            GraphNode<String> node4 = graph.AddNode("machine4");
            GraphNode<String> node5 = graph.AddNode("machine5");

            graph.Connect(node1, node2);
            graph.Connect(node1, node3);
            graph.Connect(node1, node4);
            graph.Connect(node2, node3);
            graph.Connect(node4, node5);
            graph.Connect(node3, node5);

            List<GraphNode<String>> order = graph.GetTopologicalOrdering();

            Assert.AreEqual(node1, order[0]);
            Assert.That(order[1] == node2 || order[1] == node4);
            Assert.That(order[2] == node2 || order[2] == node4);
            Assert.AreEqual(node3, order[3]);
            Assert.AreEqual(node5, order[4]);
        }

        [Test]
        public void that_we_can_generate_a_very_complex_topological_ordering()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);
            GraphNode<String> node3 = graph.AddNode(machines[2]);
            GraphNode<String> node4 = graph.AddNode("machine4");
            GraphNode<String> node5 = graph.AddNode("machine5");
            GraphNode<String> node6 = graph.AddNode("machine6");

            graph.Connect(node1, node2);
            graph.Connect(node1, node3);
            graph.Connect(node1, node4);
            graph.Connect(node1, node5);
            graph.Connect(node2, node3);
            graph.Connect(node2, node4);
            graph.Connect(node3, node4);
            graph.Connect(node3, node5);
            graph.Connect(node6, node3);

            List<GraphNode<String>> order = graph.GetTopologicalOrdering();

            Assert.That(order[0] == node1 || order[0] == node6);
            Assert.That(order[1] == node1 || order[1] == node6);
            Assert.AreEqual(node2, order[2]);
            Assert.AreEqual(node3, order[3]);
            Assert.That(order[4] == node4 || order[4] == node5);
            Assert.That(order[5] == node4 || order[5] == node5);
        }

        [Test, ExpectedExceptionAttribute(typeof(GraphCycleException))]
        public void that_topological_ordering_generates_exception_if_cycles_are_found()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);
            GraphNode<String> node3 = graph.AddNode(machines[2]);
            GraphNode<String> node4 = graph.AddNode("machine4");
            GraphNode<String> node5 = graph.AddNode("machine5");
            GraphNode<String> node6 = graph.AddNode("machine6");

            graph.Connect(node1, node2);
            graph.Connect(node1, node3);
            graph.Connect(node1, node4);
            graph.Connect(node1, node5);
            graph.Connect(node2, node3);
            graph.Connect(node2, node4);
            graph.Connect(node3, node4);
            graph.Connect(node3, node5);
            graph.Connect(node6, node3);
            graph.Connect(node4, node6); // <- This closes a cycle

            List<GraphNode<String>> order = graph.GetTopologicalOrdering();

            Assert.That(order[0] == node1 || order[0] == node6);
            Assert.That(order[1] == node1 || order[1] == node6);
            Assert.AreEqual(node2, order[2]);
            Assert.AreEqual(node3, order[3]);
            Assert.That(order[4] == node4 || order[4] == node5);
            Assert.That(order[5] == node4 || order[5] == node5);

        }
    }
}
