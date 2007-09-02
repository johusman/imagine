using System;
using System.Collections.Generic;
using System.Text;

namespace Imagine.Library
{
    public class Machine
    {
    }

    public class SourceMachine : Machine
    {
        private string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public override string ToString()
        {
            return String.Format("Source [{0}]", filename);
        }
    }

    public class SinkMachine : Machine
    {
        private string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public override string ToString()
        {
            return String.Format("Destination [{0}]", filename);
        }
    }
}
