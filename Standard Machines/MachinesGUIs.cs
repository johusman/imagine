using System;
using System.Collections.Generic;
using System.Text;
using Imagine.Library;
using System.Windows.Forms;
using Imagine.GUI;

namespace Imagine.StandardMachines
{
    [GUIForMachine("Imagine.HardControlContrast")]
    public class HardControlContrastGUI : MachineGUI
    {
        public HardControlContrastMachine MyMachine
        {
            get { return (HardControlContrastMachine)Node.Machine; }
        }

        public override void LaunchSettings(GraphArea graphArea)
        {
            NumberInput numberInput = new NumberInput();
            numberInput.Value = MyMachine.Amount;
            numberInput.Caption = "Contrast";
            if (numberInput.ShowDialog() == DialogResult.OK)
            {
                MyMachine.Amount = numberInput.Value;
            }
            numberInput.Dispose();
        }
    }

    [GUIForMachine("Imagine.SoftControlContrast")]
    public class SoftControlContrastGUI : MachineGUI
    {
        public SoftControlContrastMachine MyMachine
        {
            get { return (SoftControlContrastMachine)Node.Machine; }
        }

        public override void LaunchSettings(GraphArea graphArea)
        {
            NumberInput numberInput = new NumberInput();
            numberInput.Value = MyMachine.Amount;
            numberInput.Caption = "Contrast";
            if (numberInput.ShowDialog() == DialogResult.OK)
            {
                MyMachine.Amount = numberInput.Value;
            }
            numberInput.Dispose();
        }
    }

    [GUIForMachine("Imagine.Gain")]
    public class GainGUI : MachineGUI
    {
        public GainMachine MyMachine
        {
            get { return (GainMachine)Node.Machine; }
        }

        public override void LaunchSettings(GraphArea graphArea)
        {
            NumberInput numberInput = new NumberInput();
            numberInput.Value = MyMachine.Gain;
            numberInput.Caption = "Gain";
            if (numberInput.ShowDialog() == DialogResult.OK)
            {
                MyMachine.Gain = numberInput.Value;
            }
            numberInput.Dispose();
        }
    }

    [GUIForMachine("Imagine.ControlGain")]
    public class ContrastGainGUI : MachineGUI
    {
        public ControlGainMachine MyMachine
        {
            get { return (ControlGainMachine)Node.Machine; }
        }

        public override void LaunchSettings(GraphArea graphArea)
        {
            NumberInput numberInput = new NumberInput();
            numberInput.Value = MyMachine.Gain;
            numberInput.Caption = "Gain";
            if (numberInput.ShowDialog() == DialogResult.OK)
            {
                MyMachine.Gain = numberInput.Value;
            }
            numberInput.Dispose();
        }
    }

    [GUIForMachine("Imagine.ColorProximity")]
    public class ColorProximityGUI : MachineGUI
    {
        public ColorProximityMachine MyMachine
        {
            get { return (ColorProximityMachine)Node.Machine; }
        }

        public override void LaunchSettings(GraphArea graphArea)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.FullOpen = true;
            dialog.AnyColor = true;
            dialog.AllowFullOpen = true;
            dialog.Color = MyMachine.TargetColor.Color;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MyMachine.TargetColor = new ImagineColor(dialog.Color);
            }
            dialog.Dispose();
        }
    }

    [GUIForMachine("Imagine.HueProximity")]
    public class HueProximityGUI : MachineGUI
    {
        public HueProximityMachine MyMachine
        {
            get { return (HueProximityMachine)Node.Machine; }
        }

        public override void LaunchSettings(GraphArea graphArea)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.FullOpen = true;
            dialog.AnyColor = true;
            dialog.AllowFullOpen = true;
            dialog.Color = MyMachine.TargetColor.Color;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MyMachine.TargetColor = new ImagineColor(dialog.Color);
            }
            dialog.Dispose();
        }
    }
}
