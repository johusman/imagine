using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Drawing;
using Imagine.StandardMachines;

namespace Imagine.Library
{
    [TestFixture]
    public class MachineTest
    {
        string SRC_FILE = System.IO.Directory.GetCurrentDirectory() + "\\berry.png";
        string DEST_FILE = System.IO.Directory.GetCurrentDirectory() + "\\test.png";

        [Test]
        public void that_source_machines_can_be_instantiated_and_configured()
        {
            SourceMachine machine = new SourceMachine();
            machine.Filename = "Hejhopp";

            Assert.AreEqual("Hejhopp", machine.Filename);
            Assert.IsNotNull(machine as Machine);
        }

        [Test]
        public void that_source_machines_can_return_a_bitmap()
        {
            SourceMachine machine = new SourceMachine();
            machine.Filename = SRC_FILE;

            ImagineImage bitmapByLoad = machine.Load();
            Assert.IsNotNull(bitmapByLoad);

            // It should also be available by the general "Process" method
            ImagineImage bitmapByProcess = machine.Process(new ImagineImage[0], null)[0];
            TestUtil.AssertImagesAreEqual(bitmapByLoad, bitmapByProcess);
        }

        [Test]
        public void that_destination_machines_can_be_instantiated_and_configured()
        {
            SinkMachine machine = new SinkMachine();
            machine.Filename = "Hejhopp";

            Assert.AreEqual("Hejhopp", machine.Filename);
            Assert.IsNotNull(machine as Machine);
        }

        [Test]
        public void that_destination_machines_can_save_a_bitmap()
        {
            SinkMachine machine = new SinkMachine();
            machine.Filename = DEST_FILE;

            ImagineImage[] inputs = { new FullImage((Bitmap)Image.FromFile(SRC_FILE)) };

            try
            {
                ImagineImage[] bitmaps = machine.Process(inputs, null);
                Assert.AreEqual(0, bitmaps.Length);
                TestUtil.AssertBitmapFilesAreEqual(SRC_FILE, DEST_FILE);
            }
            finally
            {
                System.IO.File.Delete(DEST_FILE);
            }
        }

        [Test]
        public void that_we_can_ask_what_inputs_and_outputs_a_machine_works_with()
        {
            Machine machine;
            
            machine = new SourceMachine();
            Assert.AreEqual(0, machine.InputCount);
            Assert.AreEqual(1, machine.OutputCount);
            Assert.AreEqual("output", machine.OutputNames[0]);
            Assert.AreEqual(' ', machine.OutputCodes[0]);

            machine = new SinkMachine();
            Assert.AreEqual(1, machine.InputCount);
            Assert.AreEqual(0, machine.OutputCount);
            Assert.AreEqual("input", machine.InputNames[0]);
            Assert.AreEqual(' ', machine.InputCodes[0]);

            machine = new InverterMachine();
            Assert.AreEqual(1, machine.InputCount);
            Assert.AreEqual(1, machine.OutputCount);
            Assert.AreEqual("input", machine.InputNames[0]);
            Assert.AreEqual(' ', machine.InputCodes[0]);
            Assert.AreEqual("output", machine.OutputNames[0]);
            Assert.AreEqual(' ', machine.OutputCodes[0]);

            machine = new RGBSplitterMachine();
            Assert.AreEqual(1, machine.InputCount);
            Assert.AreEqual(3, machine.OutputCount);
            Assert.AreEqual("input", machine.InputNames[0]);
            Assert.AreEqual(' ', machine.InputCodes[0]);
            Assert.AreEqual("red (control)", machine.OutputNames[0]);
            Assert.AreEqual('r', machine.OutputCodes[0]);
            Assert.AreEqual("green (control)", machine.OutputNames[1]);
            Assert.AreEqual('g', machine.OutputCodes[1]);
            Assert.AreEqual("blue (control)", machine.OutputNames[2]);
            Assert.AreEqual('b', machine.OutputCodes[2]);
        }
    }
}
