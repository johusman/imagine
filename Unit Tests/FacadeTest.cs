using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Drawing;
using Imagine.StandardMachines;

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
            facade = new ImagineFacade(".");
            facade.RemoveMachine(facade.Sources[0]);
            facade.RemoveMachine(facade.Destinations[0]);

            facade.AddMachine(sourceMachine = new InmemorySourceMachine());
            facade.AddMachine(sinkMachine = new InmemorySinkMachine());
        }

        [Test]
        public void that_we_can_remove_machines()
        {
            Machine machine = facade.NewMachine("Imagine.Img.Inverter");
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
            facade.Connect(facade.Sources[0], 0, facade.Destinations[0], 0);
            sourceMachine.source = new FullImage(2, 2);
            facade.Generate();

            TestUtil.AssertImagesAreEqual(sourceMachine.source, sinkMachine.destination);
        }

        [Test]
        public void that_we_can_generate_to_multiple_outputs()
        {
            int MAX = ImagineColor.MAX;
            Machine rgbmachine = facade.NewMachine("Imagine.Img.RGBSplitter");
            InmemorySinkMachine sinkMachine2 = new InmemorySinkMachine();
            facade.AddMachine(sinkMachine2);

            facade.Connect(sourceMachine, 0, rgbmachine, 0);
            facade.Connect(rgbmachine, 0, sinkMachine, 0);
            facade.Connect(rgbmachine, 1, sinkMachine2, 0);

            sourceMachine.source = new FullImage(2, 1);
            sourceMachine.source.SetPixel(0, 0, new ImagineColor(MAX, MAX, 0, 0));
            sourceMachine.source.SetPixel(1, 0, new ImagineColor(MAX, 0, MAX, 0));

            facade.Generate();
            AssertSameColor(new ImagineColor(MAX, 0, 0, 0), sinkMachine.destination.GetPixel(0, 0));
            AssertSameColor(new ImagineColor(0, 0, 0, 0), sinkMachine.destination.GetPixel(1, 0));
            AssertSameColor(new ImagineColor(0, 0, 0, 0), sinkMachine2.destination.GetPixel(0, 0));
            AssertSameColor(new ImagineColor(MAX, 0, 0, 0), sinkMachine2.destination.GetPixel(1, 0));
        }

        private void AssertSameColor(ImagineColor expected, ImagineColor actual)
        {
            if (expected.A != actual.A
                || expected.R != actual.R
                || expected.G != actual.G
                || expected.B != actual.B)
                Assert.Fail(String.Format("Colors did not match. Expected ({0}, {1}, {2}, {3}) but was ({4}, {5}, {6}, {7})",
                    expected.A, expected.R, expected.G, expected.B, actual.A, actual.R, actual.G, actual.B));
        }

        [Test]
        public void that_we_cannot_use_ports_for_which_the_machine_does_not_have_inputs_or_outputs()
        {
            try
            {
                facade.Connect(facade.Sources[0], 0, facade.Destinations[0], 1);
                Assert.Fail("Expected MachineInputIndexOutOfRangeException");
            }
            catch(MachineInputIndexOutOfRangeException) { }

            try
            {
                facade.Connect(facade.Sources[0], 1, facade.Destinations[0], 0);
                Assert.Fail("Expected MachineInputIndexOutOfRangeException");
            }
            catch(MachineOutputIndexOutOfRangeException) { }
        }
    }

    public class InmemorySourceMachine : SourceMachine
    {
        public ImagineImage source;

        public override ImagineImage[] Process(ImagineImage[] inputs, ProgressCallback callback)
        {
            return new ImagineImage[] { source };
        }
    }

    public class InmemorySinkMachine : SinkMachine
    {
        public ImagineImage destination;

        public override ImagineImage[] Process(ImagineImage[] inputs, ProgressCallback callback)
        {
            destination = inputs[0];
            return new ImagineImage[0];
        }
    }

}
