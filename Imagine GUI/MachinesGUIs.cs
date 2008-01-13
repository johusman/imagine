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

        private Bitmap bitmap = null;

        public Bitmap Bitmap
        {
            get { return bitmap; }
            set { bitmap = value; }
        }

        public virtual Brush Background
        {
            get { return Brushes.Bisque; }
        }

        protected void SetBitmap(Bitmap newBitmap)
        {
            Bitmap alphaBitmap = (Bitmap) newBitmap.Clone();
            for(int x = 0; x < alphaBitmap.Width; x++)
                for(int y = 0; y < alphaBitmap.Height; y++)
                {
                    double b = 1.0 - alphaBitmap.GetPixel(x, y).GetBrightness();
                    alphaBitmap.SetPixel(x, y, Color.FromArgb((int)(20 * b), Color.Black)); 
                }
            bitmap = alphaBitmap;
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
                //graphArea.Facade.OpenSource(dialog.FileName);
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
                //graphArea.Facade.OpenDestination(dialog.FileName);
            dialog.Dispose();
        }
    }
}
