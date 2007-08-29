using System;
using System.Collections.Generic;
using System.Text;

namespace Imagine.Library
{
    public class ImagineFacade
    {
        private string sourceFileName;
        private string destinationFileName;

        public event System.EventHandler SourceChanged;
        public event System.EventHandler DestinationChanged;

        public void OpenSource(string fileName)
        {
            sourceFileName = fileName;
            SourceChanged.Invoke(this, new StringEventArg(fileName));
        }

        public void OpenDestination(string fileName)
        {
            destinationFileName = fileName;
            DestinationChanged.Invoke(this, new StringEventArg(fileName));
        }

        public void Generate()
        {
            System.IO.File.Copy(sourceFileName, destinationFileName, true);
        }

        public string GetSourceFilename()
        {
            return sourceFileName;
        }

        public string GetDestinationFilename()
        {
            return destinationFileName;
        }
    }

    public class StringEventArg : EventArgs
    {
        private string _string;

        public StringEventArg(string _string)
        {
            this._string = _string;
        }

        public override string ToString()
        {
            return _string;
        }
    }
}
