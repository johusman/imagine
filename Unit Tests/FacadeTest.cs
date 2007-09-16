using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Drawing;

namespace Imagine.Library
{
    [TestFixture]
    public class FacadeTest
    {
        ImagineFacade facade;
        InmemorySourceMachine sourceMachine;
        InmemorySinkMachine sinkMachine;

        [SetUp]
        public void Init()
        {
            facade = new ImagineFacade();
            facade.RemoveMachine(facade.SourceMachine);
            facade.RemoveMachine(facade.DestinationMachine);

            facade.OverrideSource(sourceMachine = new InmemorySourceMachine());
            facade.OverrideDestination(sinkMachine = new InmemorySinkMachine());
        }

        [Test]
        public void that_we_can_remove_machines()
        {
            Machine machine = facade.NewMachine("Imagine.Inverter");
            Assert.AreEqual(3, facade.Graph.NodeCount);

            facade.RemoveMachine(machine);
            Assert.AreEqual(2, facade.Graph.NodeCount);
        }

        [Test]
        public void that_removing_unregistered_machines_does_nothing()
        {
            facade.RemoveMachine(new InverterMachine());
            Assert.AreEqual(2, facade.Graph.NodeCount);
        }

        [Test]
        public void that_we_can_generate_something_simple()
        {
            facade.Connect(facade.SourceMachine, 0, facade.DestinationMachine, 0);
            sourceMachine.source = new Bitmap(2, 2, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            facade.Generate();

            TestUtil.AssertBitmapsAreEqual(sourceMachine.source, sinkMachine.destination);
        }

        [Test]
        public void that_we_can_generate_to_multiple_outputs()
        {
            Machine rgbmachine = facade.NewMachine("Imagine.RGBSplitter");
            InmemorySinkMachine sinkMachine2 = new InmemorySinkMachine();
            facade.AddMachine(sinkMachine2);

            facade.Connect(sourceMachine, 0, rgbmachine, 0);
            facade.Connect(rgbmachine, 0, sinkMachine, 0);
            facade.Connect(rgbmachine, 1, sinkMachine2, 0);

            sourceMachine.source = new Bitmap(2, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            sourceMachine.source.SetPixel(0, 0, Color.FromArgb(255, 255, 0, 0));
            sourceMachine.source.SetPixel(1, 0, Color.FromArgb(255, 0, 255, 0));

            facade.Generate();
            Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), sinkMachine.destination.GetPixel(0, 0));
            Assert.AreEqual(Color.FromArgb(255, 0, 0, 0), sinkMachine.destination.GetPixel(1, 0));
            Assert.AreEqual(Color.FromArgb(255, 0, 0, 0), sinkMachine2.destination.GetPixel(0, 0));
            Assert.AreEqual(Color.FromArgb(255, 0, 255, 0), sinkMachine2.destination.GetPixel(1, 0));
        }

        [Test]
        public void that_we_cannot_use_ports_for_which_the_machine_does_not_have_inputs_or_outputs()
        {
            try
            {
                facade.Connect(facade.SourceMachine, 0, facade.DestinationMachine, 1);
                Assert.Fail("Expected MachineInputIndexOutOfRangeException");
            }
            catch(MachineInputIndexOutOfRangeException) { }

            try
            {
                facade.Connect(facade.SourceMachine, 1, facade.DestinationMachine, 0);
                Assert.Fail("Expected MachineInputIndexOutOfRangeException");
            }
            catch(MachineOutputIndexOutOfRangeException) { }
        }
    }

    public class InmemorySourceMachine : SourceMachine
    {
        public Bitmap source;

        public override Bitmap[] Process(Bitmap[] inputs)
        {
            return new Bitmap[] { source };
        }
    }

    public class InmemorySinkMachine : SinkMachine
    {
        public Bitmap destination;

        public override Bitmap[] Process(Bitmap[] inputs)
        {
            destination = inputs[0];
            return new Bitmap[0];
        }
    }

}
