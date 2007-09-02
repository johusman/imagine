using System;
using System.Collections.Generic;
using System.Text;

namespace Imagine.Library
{
    public class Machine
    {
    }

    public class SourceNode : Machine
    {
        private string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }
    }

    public class SinkNode : Machine
    {
        private string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }
    }
}
