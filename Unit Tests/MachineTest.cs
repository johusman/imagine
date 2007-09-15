using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Drawing;

namespace Imagine.Library
{
    [TestFixture]
    public class NodeTest
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

            Bitmap bitmapByLoad = machine.Load();
            Assert.IsNotNull(bitmapByLoad);

            // It should also be available by the general "Process" method
            Bitmap bitmapByProcess = machine.Process(null);
            AssertBitmapsAreEqual(bitmapByLoad, bitmapByProcess);
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

            Bitmap[] inputs = { (Bitmap)Image.FromFile(SRC_FILE) };

            try
            {
                Bitmap bitmap = machine.Process(inputs);
                Assert.IsNull(bitmap);
                AssertBitmapFilesAreEqual(SRC_FILE, DEST_FILE);
            }
            finally
            {
                inputs[0].Dispose();
                System.IO.File.Delete(DEST_FILE);
            }
        }


        private void AssertBitmapsAreEqual(Bitmap bitmap1, Bitmap bitmap2)
        {
            Assert.AreEqual(bitmap1.Width, bitmap2.Width, "Width");
            Assert.AreEqual(bitmap1.Height, bitmap2.Height, "Height");

            for(int x = 0; x < bitmap1.Width; x++)
                for(int y = 0; y < bitmap1.Height; y++)
                    Assert.AreEqual(bitmap1.GetPixel(x, y), bitmap2.GetPixel(x, y), "Pixel at (" + x + ", " + y + ")");
        }

        private void AssertBitmapFilesAreEqual(string filename1, string filename2)
        {
            using(Bitmap bitmap1 = (Bitmap)Image.FromFile(filename1))
                using(Bitmap bitmap2 = (Bitmap)Image.FromFile(filename2))
                    AssertBitmapsAreEqual(bitmap1, bitmap2);
        }
    }
}
