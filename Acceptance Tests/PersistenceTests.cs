using System;
using System.Collections.Generic;
using System.Text;
using Imagine.Library;
using NUnit.Framework;
using Imagine.Library.Machines.Core;

namespace Imagine.AcceptanceTests
{
    [TestFixture]
    public class PersistenceTests
    {
        private ImagineFacade facade;

        [SetUp]
        public void Init()
        {
            facade = new ImagineFacade(".");
        }

        [Test]
        public void that_we_can_save_a_simple_graph()
        {
            facade.Connect(facade.Sources[0], 0, facade.Destinations[0], 0);
            
            string serialize = facade.SerializeGraph();
            Assert.AreEqual("Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +
                "\tImagine.Destination 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t}\n" +
                "}",
                serialize);
        }

        [Test]
        public void that_we_can_save_a_complex_graph()
        {
            Machine inverter = facade.NewMachine("Imagine.Img.Inverter");
            facade.Connect(facade.Sources[0], 0, inverter, 0);

            Machine splitter = facade.NewMachine("Imagine.Img.RGBSplitter");
            facade.Connect(inverter, 0, splitter, 0);

            Machine branch = facade.NewMachine("Imagine.Branch4");
            facade.Connect(splitter, 0, branch, 0);

            Machine joiner = facade.NewMachine("Imagine.Ctrl.RGBJoiner");
            facade.Connect(branch, 0, joiner, 0);
            facade.Connect(branch, 1, joiner, 1);
            facade.Connect(splitter, 2, joiner, 2);

            facade.Connect(joiner, 0, facade.Destinations[0], 0);

            string serialize = facade.SerializeGraph();
            Assert.AreEqual("Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +

                "\tImagine.Img.Inverter 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t}\n" +

                "\tImagine.Img.RGBSplitter 'machine2' {\n" +
                "\t\t'machine1' -> \n" +
                "\t}\n" +

                "\tImagine.Branch4 'machine3' {\n" +
                "\t\t'machine2':r -> \n" +
                "\t}\n" +

                "\tImagine.Ctrl.RGBJoiner 'machine4' {\n" +
                "\t\t'machine3':1 -> r\n" +
                "\t\t'machine3':2 -> g\n" +
                "\t\t'machine2':b -> b\n" +
                "\t}\n" +

                "\tImagine.Destination 'machine5' {\n" +
                "\t\t'machine4' -> \n" +
                "\t}\n" +
                "}",
                serialize);
        }

        [Test]
        public void that_we_can_save_an_unconnected_graph()
        {
            string serialize = facade.SerializeGraph();
            Assert.AreEqual("Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +
                "\tImagine.Destination 'machine1' {}\n" +
                "}",
                serialize);
        }

        [Test]
        public void that_we_can_save_multiple_sources_and_destinations()
        {
            facade.AddMachine(new SourceMachine());
            facade.AddMachine(new SinkMachine());
            facade.Connect(facade.Sources[0], 0, facade.Destinations[1], 0);
            facade.Connect(facade.Sources[1], 0, facade.Destinations[0], 0);
            
            string serialize = facade.SerializeGraph();
            Assert.AreEqual("Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +
                "\tImagine.Source 'machine1' {}\n" +
                "\tImagine.Destination 'machine2' {\n" +
                "\t\t'machine0' -> \n" +
                "\t}\n" +
                "\tImagine.Destination 'machine3' {\n" +
                "\t\t'machine1' -> \n" +
                "\t}\n" +
                "}",
                serialize);
        }

        [Test]
        public void that_we_can_save_an_empty_graph()
        {
            facade.RemoveMachine(facade.Sources[0]);
            facade.RemoveMachine(facade.Destinations[0]);

            string serialize = facade.SerializeGraph();
            Assert.AreEqual("Graph {\n" +
                "}",
                serialize);
        }

        [Test]
        public void that_we_can_load_a_simple_graph()
        {
            Machine oldSource = facade.Sources[0];
            Machine oldDestination = facade.Destinations[0];

            string serialize =
                "Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +
                "\tImagine.Destination 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t}\n" +
                "}";

            facade.DeserializeGraph(serialize);
            Assert.AreNotSame(oldSource, facade.Sources[0]);
            Assert.AreNotSame(oldDestination, facade.Destinations[0]);
            Assert.AreSame(facade.Destinations[0], facade.Graph.GetNodeFor(facade.Sources[0]).Outports[0].RemotePort.Node.Machine);
        }

        [Test]
        public void that_we_can_load_an_unconnected_graph()
        {
            Machine oldSource = facade.Sources[0];
            Machine oldDestination = facade.Destinations[0];

            string serialize =
                "Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +
                "\tImagine.Destination 'machine1' {}\n" +
                "}";
            facade.DeserializeGraph(serialize);
            Assert.AreNotSame(oldSource, facade.Sources[0]);
            Assert.AreNotSame(oldDestination, facade.Destinations[0]);
            Assert.AreEqual(0, facade.Graph.GetNodeFor(facade.Sources[0]).OutputCount);
            Assert.AreEqual(0, facade.Graph.GetNodeFor(facade.Destinations[0]).InputCount);
            Assert.AreEqual(2, facade.Graph.NodeCount);
            Assert.AreEqual(0, facade.Graph.ConnectionCount);
        }

        [Test]
        public void that_we_can_load_a_complex_graph()
        {
            Machine oldSource = facade.Sources[0];
            Machine oldDestination = facade.Destinations[0];

            string serialize =
                "Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +

                "\tImagine.Img.Inverter 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t}\n" +

                "\tImagine.Img.RGBSplitter 'machine2' {\n" +
                "\t\t'machine1' -> \n" +
                "\t}\n" +

                "\tImagine.Branch4 'machine3' {\n" +
                "\t\t'machine2':r -> \n" +
                "\t}\n" +

                "\tImagine.Ctrl.RGBJoiner 'machine4' {\n" +
                "\t\t'machine3':1 -> r\n" +
                "\t\t'machine3':2 -> g\n" +
                "\t\t'machine2':b -> b\n" +
                "\t}\n" +

                "\tImagine.Destination 'machine5' {\n" +
                "\t\t'machine4' -> \n" +
                "\t}\n" +
                "}";
            facade.DeserializeGraph(serialize);
            Graph<Machine> graph = facade.Graph;
            Assert.AreNotSame(oldSource, facade.Sources[0]);
            Assert.AreNotSame(oldDestination, facade.Destinations[0]);
            Assert.AreEqual(1, graph.GetNodeFor(facade.Sources[0]).OutputCount);
            Assert.AreEqual(1, graph.GetNodeFor(facade.Destinations[0]).InputCount);
            Assert.AreEqual(6, graph.NodeCount);
            Assert.AreEqual(7, graph.ConnectionCount);
        }

        [Test]
        public void that_we_can_load_multiple_sources_and_destinations()
        {
            SourceMachine oldSource = facade.Sources[0];
            SinkMachine oldDestination = facade.Destinations[0];

            string serialize = 
                "Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +
                "\tImagine.Source 'machine1' {}\n" +
                "\tImagine.Destination 'machine2' {\n" +
                "\t\t'machine0' -> \n" +
                "\t}\n" +
                "\tImagine.Destination 'machine3' {\n" +
                "\t\t'machine1' -> \n" +
                "\t}\n" +
                "}";
            facade.DeserializeGraph(serialize);
            Graph<Machine> graph = facade.Graph;
            Assert.IsFalse(facade.Sources.Contains(oldSource));
            Assert.IsFalse(facade.Destinations.Contains(oldDestination));
            Assert.AreEqual(2, facade.Sources.Count);
            Assert.AreEqual(2, facade.Destinations.Count);
            Assert.AreEqual(1, graph.GetNodeFor(facade.Sources[0]).OutputCount);
            Assert.AreEqual(1, graph.GetNodeFor(facade.Sources[1]).OutputCount);
            Assert.AreEqual(1, graph.GetNodeFor(facade.Destinations[0]).InputCount);
            Assert.AreEqual(1, graph.GetNodeFor(facade.Destinations[1]).InputCount);
            Assert.AreEqual(4, graph.NodeCount);
            Assert.AreEqual(2, graph.ConnectionCount);
        }

        [Test]
        public void that_we_can_load_an_empty_graph()
        {
            facade.RemoveMachine(facade.Sources[0]);
            facade.RemoveMachine(facade.Destinations[0]);

            string serialize =
                "Graph {\n" +
                "}";
            facade.DeserializeGraph(serialize);
            Graph<Machine> graph = facade.Graph;
            Assert.AreEqual(0, facade.Sources.Count);
            Assert.AreEqual(0, facade.Destinations.Count);
            Assert.AreEqual(0, graph.NodeCount);
            Assert.AreEqual(0, graph.ConnectionCount);
        }

        [Test]
        public void that_we_can_load_from_correct_section()
        {
            Machine oldSource = facade.Sources[0];
            Machine oldDestination = facade.Destinations[0];

            string serialize =
                "Blah { hej { } hopp {\n hejsan {\n}} }\n" +

                "Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +

                "\tImagine.Img.Inverter 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t}\n" +

                "\tImagine.Img.RGBSplitter 'machine2' {\n" +
                "\t\t'machine1' -> \n" +
                "\t}\n" +

                "\tImagine.Branch4 'machine3' {\n" +
                "\t\t'machine2':r -> \n" +
                "\t}\n" +

                "\tImagine.Ctrl.RGBJoiner 'machine4' {\n" +
                "\t\t'machine3':1 -> r\n" +
                "\t\t'machine3':2 -> g\n" +
                "\t\t'machine2':b -> b\n" +
                "\t}\n" +

                "\tImagine.Destination 'machine5' {\n" +
                "\t\t'machine4' -> \n" +
                "\t}\n" +
                "}\n" +

                "Hoho { mjau { } hopp {\n hejsan {\n}} }\n";
            facade.DeserializeGraph(serialize);
            Graph<Machine> graph = facade.Graph;
            Assert.AreNotSame(oldSource, facade.Sources[0]);
            Assert.AreNotSame(oldDestination, facade.Destinations[0]);
            Assert.AreEqual(1, graph.GetNodeFor(facade.Sources[0]).OutputCount);
            Assert.AreEqual(1, graph.GetNodeFor(facade.Destinations[0]).InputCount);
            Assert.AreEqual(6, graph.NodeCount);
            Assert.AreEqual(7, graph.ConnectionCount);
        }

        [Test]
        public void that_we_can_load_simple_machine_settings_from_a_file()
        {
            facade.MachineTypes["Test.LoadSave"] = typeof(LoadSaveMachine);

            string serialize =
                "Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +

                "\tTest.LoadSave 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t\t[\n" +
                "\t\t\tValue = '2'\n" +
                "\t\t]\n" +
                "\t}\n" +

                "\tImagine.Destination 'machine2' {\n" +
                "\t\t'machine1' -> \n" +
                "\t}\n" +
                "}";
            facade.DeserializeGraph(serialize);

            Graph<Machine> graph = facade.Graph;
            // See that the settings section didn't destroy the rest of the parsing
            Assert.IsNotNull(facade.Sources[0], "facade.Sources[0]");
            Assert.IsNotNull(facade.Destinations[0], "facade.Destinations[0]");
            Assert.AreEqual(1, graph.GetNodeFor(facade.Sources[0]).OutputCount, "SourceMachine.OutputCount");
            Assert.AreEqual(1, graph.GetNodeFor(facade.Destinations[0]).InputCount, "DestinationMachine.InputCount");
            Assert.AreEqual(3, graph.NodeCount, "Graph.NodeCount");
            Assert.AreEqual(2, graph.ConnectionCount, "Graph.ConnectionCount");

            LoadSaveMachine machine = ((LoadSaveMachine)graph.GetTopologicalOrdering()[1].Machine);
            Assert.IsNotNull(machine.settings, "Dummy machine settings");
            Assert.AreEqual("Value = '2'", machine.settings.Trim());
            Assert.AreEqual(2, machine.DummyValue);
        }

        [Test]
        public void that_we_can_load_complex_machine_settings_from_a_file()
        {
            facade.MachineTypes["Test.ComplexLoadSave"] = typeof(ComplexLoadSaveMachine);

            string serialize =
                "Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +

                "\tTest.ComplexLoadSave 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t\t[\n" +
                "\t\t\tred = '2'\n" +
                "\t\t\tgreen = '50'\n" +
                "\t\t\tblue = '10'\n" +
                "\t\t\ttext = ' hello '\n" +
                "\t\t\tdouble = '0.2'\n" +
                "\t\t]\n" +
                "\t}\n" +

                "\tImagine.Destination 'machine2' {\n" +
                "\t\t'machine1' -> \n" +
                "\t}\n" +
                "}";
            facade.DeserializeGraph(serialize);

            Graph<Machine> graph = facade.Graph;
            // See that the settings section didn't destroy the rest of the parsing
            Assert.IsNotNull(facade.Sources[0], "facade.Sources[0]");
            Assert.IsNotNull(facade.Destinations[0], "facade.Destinations[0]");
            Assert.AreEqual(1, graph.GetNodeFor(facade.Sources[0]).OutputCount, "SourceMachine.OutputCount");
            Assert.AreEqual(1, graph.GetNodeFor(facade.Destinations[0]).InputCount, "DestinationMachine.InputCount");
            Assert.AreEqual(3, graph.NodeCount, "Graph.NodeCount");
            Assert.AreEqual(2, graph.ConnectionCount, "Graph.ConnectionCount");

            ComplexLoadSaveMachine machine = ((ComplexLoadSaveMachine)graph.GetTopologicalOrdering()[1].Machine);
            Assert.IsNotNull(machine.settings, "Dummy machine settings");
            Assert.IsTrue(machine.settings.Contains("red = '2'"), "red");
            Assert.IsTrue(machine.settings.Contains("green = '50'"), "green");
            Assert.IsTrue(machine.settings.Contains("blue = '10'"), "blue");
            Assert.IsTrue(machine.settings.Contains("text = ' hello '"), "text");
            Assert.IsTrue(machine.settings.Contains("double = '0.2'"), "double");
            Assert.AreEqual("2", machine.properties["red"], "red");
            Assert.AreEqual("50", machine.properties["green"], "green");
            Assert.AreEqual("10", machine.properties["blue"], "blue");
            Assert.AreEqual(" hello ", machine.properties["text"], "text");
            Assert.AreEqual("0.2", machine.properties["double"], "double");
        }

        [Test]
        public void that_we_can_save_machine_settings_to_a_file()
        {
            facade.MachineTypes["Test.ComplexLoadSave"] = typeof(ComplexLoadSaveMachine);

            Machine machine = facade.NewMachine("Test.ComplexLoadSave");
            facade.Connect(facade.Sources[0], 0, machine, 0);
            facade.Connect(machine, 0, facade.Destinations[0], 0);

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties["red"] = "-2";
            properties["green"] = "50.11";
            properties["blue"] = "3232432323";
            properties["text"] = "\tyahoo!";
            properties["double"] = "3.14159265";
            ((ComplexLoadSaveMachine)machine).properties = properties;

            string serialize = facade.SerializeGraph();
            Assert.AreEqual("Graph {\n" +
                "\tImagine.Source 'machine0' {}\n" +

                "\tTest.ComplexLoadSave 'machine1' {\n" +
                "\t\t'machine0' -> \n" +
                "\t\t[ red='-2' green='50.11' blue='3232432323' text='\tyahoo!' double='3.14159265' ]\n" +
                "\t}\n" +

                "\tImagine.Destination 'machine2' {\n" +
                "\t\t'machine1' -> \n" +
                "\t}\n" +
                "}", serialize);
        }
    }

    [UniqueName("Test.LoadSave")]
    public class LoadSaveMachine : DummyMachine
    {
        public LoadSaveMachine() : base() { }

        public string settings = null;

        public override void LoadSettings(string settings)
        {
            this.settings = settings;
            Dictionary<string, string> properties = ParseSettings(settings);
            int? value = GetInt(properties, "Value");
            if (value != null)
                DummyValue = value.Value;
        }
    }

    [UniqueName("Test.ComplexLoadSave")]
    public class ComplexLoadSaveMachine : DummyMachine
    {
        public ComplexLoadSaveMachine() : base() { }

        public string settings = null;
        public Dictionary<string, string> properties = null;


        public override void LoadSettings(string settings)
        {
            this.settings = settings;
            this.properties = ParseSettings(settings);
        }

        public override string SaveSettings()
        {
            return CompileSettings(this.properties);
        }
    }
}
