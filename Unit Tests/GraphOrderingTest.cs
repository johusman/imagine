using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Imagine.Library
{
    [TestFixture]
    public class GraphOrderingTest
    {
        private Graph<String> graph;
        String[] machines = { "machine1", "machine2", "machine3" };

        [SetUp]
        public void init()
        {
            graph = new Graph<String>();
        }

        [Test]
        public void that_we_can_generate_a_simple_topological_ordering()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);
            GraphNode<String> node3 = graph.AddNode(machines[2]);

            graph.Connect(node1, 0, node2, 0);
            graph.Connect(node2, 0, node3, 0);

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

            graph.Connect(node1, 0, node2, 0);
            graph.Connect(node1, 1, node3, 0);
            graph.Connect(node1, 2, node4, 0);
            graph.Connect(node2, 0, node3, 1);
            graph.Connect(node4, 0, node5, 0);
            graph.Connect(node3, 0, node5, 1);

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

            graph.Connect(node1, 0, node2, 0);
            graph.Connect(node1, 1, node3, 0);
            graph.Connect(node1, 2, node4, 0);
            graph.Connect(node1, 3, node5, 0);
            graph.Connect(node2, 0, node3, 1);
            graph.Connect(node2, 1, node4, 1);
            graph.Connect(node3, 0, node4, 2);
            graph.Connect(node3, 1, node5, 1);
            graph.Connect(node6, 0, node3, 2);

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

            graph.Connect(node1, 0, node2, 0);
            graph.Connect(node1, 1, node3, 0);
            graph.Connect(node1, 2, node4, 0);
            graph.Connect(node1, 3, node5, 0);
            graph.Connect(node2, 0, node3, 1);
            graph.Connect(node2, 1, node4, 1);
            graph.Connect(node3, 0, node4, 2);
            graph.Connect(node3, 1, node5, 1);
            graph.Connect(node6, 0, node3, 2);
            graph.Connect(node4, 0, node6, 0); // <- This closes a cycle

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
