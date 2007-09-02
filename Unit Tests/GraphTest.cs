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
            GraphNode<String> node = graph.AddNode("Dummy");

            Assert.AreEqual(1, graph.NodeCount, "NodeCount");
            Assert.AreEqual(0, graph.ConnectionCount, "ConnectionCount");
            Assert.AreEqual("Dummy", node.Machine, "Machine");
            Assert.AreEqual(0, node.InputCount, "InputCount");
            Assert.AreEqual(0, node.OutputCount, "OutputCount");
        }

        [Test]
        public void that_we_can_add_a_real_machine()
        {
            String machine = "machine";
            GraphNode<String> node = graph.AddNode(machine);

            Assert.AreSame(machine, node.Machine, "Machine");
        }

        [Test]
        public void that_we_can_connect_two_nodes()
        {
            String machine1 = "machine1";
            String machine2 = "machine2";

            GraphNode<String> node1 = graph.AddNode(machine1);
            GraphNode<String> node2 = graph.AddNode(machine2);

            Assert.AreEqual(2, graph.NodeCount, "NodeCount");
            Assert.AreEqual(0, graph.ConnectionCount, "ConnectionCount");
            Assert.AreSame(machine1, node1.Machine, "node1.Machine");
            Assert.AreSame(machine2, node2.Machine, "node2.Machine");
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
        public void that_we_can_retrieve_the_node_corresponding_to_a_particular_machine()
        {
            String machine1 = "machine1";
            String machine2 = "machine2";

            GraphNode<String> node1 = graph.AddNode(machine1);
            GraphNode<String> node2 = graph.AddNode(machine2);

            Assert.AreSame(node1, graph.GetNodeFor(machine1));
            Assert.AreSame(node2, graph.GetNodeFor(machine2));
        }
    }
}
