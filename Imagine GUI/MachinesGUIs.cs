using System;
using System.Collections.Generic;
using System.Text;
using Imagine.Library;
using System.Windows.Forms;
using System.Drawing;
using Imagine.GUI.Properties;
using Imagine.Library.Machines.Core;

namespace Imagine.GUI
{
    public class MachineGUI
    {
        private GraphNode<Machine> node = null;

        public GraphNode<Machine> Node
        {
            get { return node; }
            set { node = value; }
        }

        private Bitmap halfDimmedBitmap = null;

        public Bitmap HalfDimmedBitmap
        {
            get { return halfDimmedBitmap; }
            set { halfDimmedBitmap = value; }
        }

        private Bitmap dimmedBitmap = null;

        public Bitmap DimmedBitmap
        {
            get { return dimmedBitmap; }
            set { dimmedBitmap = value; }
        }

        public virtual Brush Background
        {
            get { return Brushes.Bisque; }
        }

        protected void SetBitmap(Bitmap newBitmap)
        {
            Bitmap newDimmedBitmap = (Bitmap) newBitmap.Clone();
            Bitmap newHalfDimmedBitmap = (Bitmap)newBitmap.Clone();
            for(int x = 0; x < newDimmedBitmap.Width; x++)
                for(int y = 0; y < newDimmedBitmap.Height; y++)
                {
                    double b = 1.0 - newDimmedBitmap.GetPixel(x, y).GetBrightness();
                    newDimmedBitmap.SetPixel(x, y, Color.FromArgb((int)(20 * b), Color.Black));
                    newHalfDimmedBitmap.SetPixel(x, y, Color.FromArgb((int)(128 * b), Color.Black)); 
                }
            dimmedBitmap = newDimmedBitmap;
            halfDimmedBitmap = newHalfDimmedBitmap;
        }

        public virtual void LaunchSettings(GraphArea graphArea) { }
        public virtual string SerializeSettings() { return "";  }
        public virtual void DeserializeSettings(string input) {}
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class GUIForMachine : Attribute
    {
        private string value;
        public GUIForMachine(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }
    }
}
