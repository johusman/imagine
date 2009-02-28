using System;
using System.Collections.Generic;
using System.Text;
using Imagine.Library;
using System.Windows.Forms;
using Imagine.GUI;
using Standard_Machines;
using System.Drawing;

namespace Imagine.StandardMachines
{
    [GUIForMachine("Imagine.Ctrl.HardControlContrast")]
    public class HardControlContrastGUI : MachineGUI
    {
        public HardControlContrastGUI()
        {
            SetBitmap(Resources.Imagine_Ctrl_Contrast);
        }

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

    [GUIForMachine("Imagine.Ctrl.SoftControlContrast")]
    public class SoftControlContrastGUI : MachineGUI
    {
        public SoftControlContrastGUI()
        {
            SetBitmap(Resources.Imagine_Ctrl_Contrast);
        }

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

    [GUIForMachine("Imagine.Img.Gain")]
    public class GainGUI : MachineGUI
    {
        public GainGUI()
        {
            SetBitmap(Resources.Imagine_Gain);
        }

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

    [GUIForMachine("Imagine.Ctrl.ControlGain")]
    public class ContrastGainGUI : MachineGUI
    {
        public ContrastGainGUI()
        {
            SetBitmap(Resources.Imagine_Gain);
        }

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

    [GUIForMachine("Imagine.Img.ColorProximity")]
    public class ColorProximityGUI : MachineGUI
    {
        public ColorProximityGUI()
        {
            SetBitmap(Resources.Imagine_Img_Proximity);
        }

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

    [GUIForMachine("Imagine.Img.HueProximity")]
    public class HueProximityGUI : MachineGUI
    {
        public HueProximityGUI()
        {
            SetBitmap(Resources.Imagine_Img_Proximity);
        }

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

    [GUIForMachine("Imagine.Img.BadDynamicBlur")]
    public class BadDynamicBlurGUI : MachineGUI
    {
        public BadDynamicBlurGUI()
        {
            SetBitmap(Resources.Imagine_Img_Blur);
        }
    }

    [GUIForMachine("Imagine.Img.GaussianBlur")]
    public class GaussianBlurGUI : MachineGUI
    {
        public GaussianBlurGUI()
        {
            SetBitmap(Resources.Imagine_Img_Blur);
        }

        public GaussianBlurMachine MyMachine
        {
            get { return (GaussianBlurMachine)Node.Machine; }
        }

        public override void LaunchSettings(GraphArea graphArea)
        {
            NumberInput numberInput = new NumberInput();
            numberInput.Value = MyMachine.Size;
            numberInput.Caption = "Blur size (px)";
            if (numberInput.ShowDialog() == DialogResult.OK)
            {
                MyMachine.Size = numberInput.Value;
            }
            numberInput.Dispose();
        }
    }

    [GUIForMachine("Imagine.Img.GaussianBleed")]
    public class GaussianBleedGUI : GaussianBlurGUI
    {
        public GaussianBleedGUI() : base() { }
    }

    [GUIForMachine("Imagine.Ctrl.ControlMultiplier4")]
    public class ControlMultiplierGUI : MachineGUI
    {
        public ControlMultiplierGUI()
        {
            SetBitmap(Resources.Imagine_Ctrl_ControlMultiplier);
        }
    }

    [GUIForMachine("Imagine.Adder4")]
    public class AdderGUI : MachineGUI
    {
        public AdderGUI()
        {
            SetBitmap(Resources.Imagine_Adder);
        }
    }

    [GUIForMachine("Imagine.Img.Pixelator")]
    public class PixelateGUI : MachineGUI
    {
        public PixelateMachine MyMachine
        {
            get { return (PixelateMachine)Node.Machine; }
        }

        public override void LaunchSettings(GraphArea graphArea)
        {
            NumberInput numberInput = new NumberInput();
            numberInput.Value = MyMachine.Size;
            numberInput.Caption = "Size";
            if (numberInput.ShowDialog() == DialogResult.OK)
            {
                MyMachine.Size = (int) numberInput.Value;
            }
            numberInput.Dispose();
        }
    }

    [GUIForMachine("Imagine.Img.Color")]
    public class ColorGUI : MachineGUI
    {
        public ColorMachine MyMachine
        {
            get { return (ColorMachine)Node.Machine; }
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
