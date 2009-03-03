using System;
using System.Collections.Generic;
using System.Text;
using Imagine.Library;
using System.Windows.Forms;
using System.Drawing;
using Imagine.GUI.Properties;
using Imagine.Library.Machines.Core;

namespace Imagine.GUI.Machines.Core
{
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
