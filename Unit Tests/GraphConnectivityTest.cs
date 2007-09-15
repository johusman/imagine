using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Imagine.Library
{
    [TestFixture]
    public class GraphConnectivityTest
    {
        private Graph<String> graph;
        String[] machines = { "machine1", "machine2", "machine3" };

        [SetUp]
        public void init()
        {
            graph = new Graph<String>();
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

            graph.Connect(node1, 0, node2, 0);

            Assert.AreEqual(1, graph.ConnectionCount, "ConnectionCount");
            Assert.AreEqual(0, node1.InputCount, "node1.InputCount");
            Assert.AreEqual(1, node1.OutputCount, "node1.OutputCount");
            Assert.AreEqual(1, node2.InputCount, "node2.InputCount");
            Assert.AreEqual(0, node2.OutputCount, "node2.OutputCount");

            Assert.AreSame(node2, node1.Outports[0].RemotePort.Node, "node1.Outputs[0].RemotePort.Node");
            Assert.AreSame(node1, node2.Inports[0].RemotePort.Node, "node2.Inputs[0].RemotePort.Node");
        }

        [Test]
        public void that_connection_involving_null_is_an_error()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);

            try
            {
                graph.Connect(node1, 0, null, 0);
                Assert.Fail("Expected ArgumentNullException");
            }
            catch(ArgumentNullException) { }

            try
            {
                graph.Connect(null, 0, node2, 0);
                Assert.Fail("Expected ArgumentNullException");
            }
            catch(ArgumentNullException) { }

            try
            {
                graph.Connect(null, 0, null, 0);
                Assert.Fail("Expected ArgumentNullException");
            }
            catch(ArgumentNullException) { }
        }

        [Test]
        public void that_we_cannot_connect_nodes_from_outside_the_graph()
        {
            GraphNode<String> node = graph.AddNode(machines[0]);
            GraphNode<String> externalNode = new GraphNode<string>("external");

            try
            {
                graph.Connect(node, 0, externalNode, 0);
                Assert.Fail("Expected ArgumentException");
            }
            catch(ArgumentException) { }

            try
            {
                graph.Connect(externalNode, 0, node, 0);
                Assert.Fail("Expected ArgumentException");
            }
            catch(ArgumentException) { }
        }

        [Test]
        public void that_we_can_connect_nodes_on_different_ports()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);
            GraphNode<String> node3 = graph.AddNode(machines[2]);

            graph.Connect(node1, 0, node2, 1);
            graph.Connect(node1, 1, node2, 0);
            graph.Connect(node3, 1, node2, 2);

            Assert.AreEqual(3, graph.ConnectionCount, "ConnectionCount");
            Assert.AreEqual(0, node1.InputCount, "node1.InputCount");
            Assert.AreEqual(2, node1.OutputCount, "node1.OutputCount");
            Assert.AreEqual(3, node2.InputCount, "node2.InputCount");
            Assert.AreEqual(0, node2.OutputCount, "node2.OutputCount");
            Assert.AreEqual(0, node3.InputCount, "node3.InputCount");
            Assert.AreEqual(1, node3.OutputCount, "node3.OutputCount");

            Assert.AreSame(node2, node1.Outports[0].RemotePort.Node, "node1.Outputs[0].RemotePort.Node");
            Assert.AreSame(node2, node1.Outports[1].RemotePort.Node, "node1.Outputs[1].RemotePort.Node");
            Assert.AreSame(node2, node3.Outports[1].RemotePort.Node, "node3.Outputs[1].RemotePort.Node");
            Assert.AreSame(node1, node2.Inports[0].RemotePort.Node, "node2.Inputs[0].RemotePort.Node");
            Assert.AreSame(node1, node2.Inports[1].RemotePort.Node, "node2.Inputs[1].RemotePort.Node");
            Assert.AreSame(node3, node2.Inports[2].RemotePort.Node, "node2.Inputs[2].RemotePort.Node");
        }

        [Test]
        public void that_we_cannot_connect_already_connected_ports()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);
            GraphNode<String> node3 = graph.AddNode(machines[2]);

            graph.Connect(node1, 0, node3, 0);

            try
            {
                graph.Connect(node2, 0, node3, 0);
                Assert.Fail("Expected AlreadyConnectedException");
            }
            catch(PortAlreadyConnectedException) {}

            try
            {
                graph.Connect(node1, 0, node3, 0);
                Assert.Fail("Expected AlreadyConnectedException");
            }
            catch(PortAlreadyConnectedException) { }
        }

        [Test]
        public void that_we_can_disconnect_nodes()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);

            graph.Connect(node1, 0, node2, 0);
            graph.Disconnect(node1, 0, node2, 0);

            Assert.AreEqual(2, graph.NodeCount, "NodeCount");
            Assert.AreEqual(0, graph.ConnectionCount, "ConnectionCount");
            Assert.AreEqual(0, node1.InputCount, "node1.InputCount");
            Assert.AreEqual(0, node1.OutputCount, "node1.OutputCount");
            Assert.AreEqual(0, node2.InputCount, "node2.InputCount");
            Assert.AreEqual(0, node2.OutputCount, "node2.OutputCount");
        }

        [Test]
        public void that_disconnecting_unconnected_ports_does_nothing()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);

            graph.Connect(node1, 0, node2, 0);
            graph.Disconnect(node1, 0, node2, 1); // <- other ports

            Assert.AreEqual(2, graph.NodeCount, "NodeCount");
            Assert.AreEqual(1, graph.ConnectionCount, "ConnectionCount");
            Assert.AreEqual(0, node1.InputCount, "node1.InputCount");
            Assert.AreEqual(1, node1.OutputCount, "node1.OutputCount");
            Assert.AreEqual(1, node2.InputCount, "node2.InputCount");
            Assert.AreEqual(0, node2.OutputCount, "node2.OutputCount");

            graph.Disconnect(node1, 1, node2, 0); // <- still other ports

            Assert.AreEqual(2, graph.NodeCount, "NodeCount");
            Assert.AreEqual(1, graph.ConnectionCount, "ConnectionCount");
            Assert.AreEqual(0, node1.InputCount, "node1.InputCount");
            Assert.AreEqual(1, node1.OutputCount, "node1.OutputCount");
            Assert.AreEqual(1, node2.InputCount, "node2.InputCount");
            Assert.AreEqual(0, node2.OutputCount, "node2.OutputCount");
        }

        [Test]
        public void that_disconnecting_unconnected_nodes_does_nothing()
        {
            GraphNode<String> node1 = graph.AddNode(machines[0]);
            GraphNode<String> node2 = graph.AddNode(machines[1]);

            graph.Disconnect(node1, 0, node2, 0);

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
            graph2.Connect(externalNode1, 0, externalNode2, 0);

            try
            {
                graph.Disconnect(node, 0, externalNode1, 0);
                Assert.Fail("Expected ArgumentException");
            }
            catch(ArgumentException) { }

            try
            {
                graph.Disconnect(externalNode1, 0, node, 0);
                Assert.Fail("Expected ArgumentException");
            }
            catch(ArgumentException) { }

            try
            {
                graph.Disconnect(externalNode1, 0, externalNode2, 0);
                Assert.Fail("Expected ArgumentException");
            }
            catch(ArgumentException) { }
        }
    }
}
