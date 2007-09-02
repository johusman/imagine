using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Imagine.Library
{
    [TestFixture]
    public class NodeTest
    {
        [Test]
        public void that_source_machines_can_be_instantiated_and_configured()
        {
            SourceMachine machine = new SourceMachine();
            machine.Filename = "Hejhopp";

            Assert.AreEqual("Hejhopp", machine.Filename);
            Assert.IsNotNull(machine as Machine);
        }

        [Test]
        public void that_destination_machines_can_be_instantiated_and_configured()
        {
            SinkMachine machine = new SinkMachine();
            machine.Filename = "Hejhopp";

            Assert.AreEqual("Hejhopp", machine.Filename);
            Assert.IsNotNull(machine as Machine);
        }
    }
}
