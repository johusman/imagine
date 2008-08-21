using System;
using System.Collections.Generic;
using System.Text;
using Imagine.Library;
using System.Windows.Forms;
using System.Drawing;
using Imagine.GUI.Properties;

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

    [GUIForMachine("Imagine.Source")]
    public class SourceMachineGUI : MachineGUI
    {
        public SourceMachineGUI()
        {
            SetBitmap(Resources.Imagine_Source);
        }

        public override Brush Background
        {
            get { return Brushes.AliceBlue; }
        }

        public SourceMachine MyMachine
        {
            get { return (SourceMachine)Node.Machine; }
        }

        public override void LaunchSettings(GraphArea graphArea)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Images|*.jpg;*.png;*.gif;*.bmp|JPEG (*.jpg)|*.jpg|Ping (*.png)|*.png|GIF (*.gif)|*.gif|Bitmap (*.bmp)|*.bmp";
            if (MyMachine.Filename != null)
                dialog.FileName = MyMachine.Filename;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
                MyMachine.Filename = dialog.FileName;
            dialog.Dispose();
        }
    }

    [GUIForMachine("Imagine.Destination")]
    public class SinkMachineGUI : MachineGUI
    {
        public SinkMachineGUI()
        {
            SetBitmap(Resources.Imagine_Destination);
        }

        public override Brush Background
        {
            get { return Brushes.AliceBlue; }
        }

        public SinkMachine MyMachine
        {
            get { return (SinkMachine)Node.Machine; }
        }

        public override void LaunchSettings(GraphArea graphArea)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "png";
            dialog.Filter = "Ping (*.png)|*.png";
            if (MyMachine.Filename != null)
                dialog.FileName = MyMachine.Filename;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
                MyMachine.Filename = dialog.FileName;
            dialog.Dispose();
        }
    }

    [GUIForMachine("Imagine.Branch4")]
    public class BranchGUI : MachineGUI
    {
        public BranchGUI()
        {
            SetBitmap(Resources.Imagine_Branch);
        }

        public override System.Drawing.Brush Background
        {
            get { return Brushes.AntiqueWhite; }
        }
    }
}
