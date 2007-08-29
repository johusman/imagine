using System;
using System.Collections.Generic;
using System.Text;

namespace Imagine.Library
{
    public class ImagineFacade
    {
        private string sourceFileName;
        private string destinationFileName;

        public void OpenSource(string fileName)
        {
            sourceFileName = fileName;
        }

        public void OpenDestination(string fileName)
        {
            destinationFileName = fileName;
        }

        public void Generate()
        {
            System.IO.File.Copy(sourceFileName, destinationFileName);
        }
    }
}
