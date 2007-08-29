using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Imagine.Library;

namespace Imagine.AcceptanceTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void that_you_can_open_a_source_and_a_destination_and_open_and_save_a_picture_between_them()
        {
            string SRC_FILE = System.IO.Directory.GetCurrentDirectory() + "\\Leighton_Idyll.jpg";
            string DEST_FILE = System.IO.Directory.GetCurrentDirectory() + "\\test.jpg";

            ImagineFacade facade = new ImagineFacade();
            facade.OpenSource(SRC_FILE);
            facade.OpenDestination(DEST_FILE);

            try
            {
                facade.Generate();
                FileAssert.AreEqual(SRC_FILE, DEST_FILE);
            }
            finally
            {
                System.IO.File.Delete(DEST_FILE);
            }
        }
    }
}
