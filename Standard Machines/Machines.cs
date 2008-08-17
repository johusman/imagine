using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Imagine.Library;

namespace Imagine.StandardMachines
{
    [UniqueName("Imagine.Img.Inverter")]
    public class InverterMachine : Machine
    {
        public InverterMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Does an RGB invert of the image (leaves Alpha intact).";
        }

        public override string Caption
        {
            get { return "Invert"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            int MAX = ImagineColor.MAX;
            ImagineImage result = NewFull(inputs);
            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    ImagineColor color = inputs[0].GetPixel(x, y);
                    result.SetPixel(x, y, color.A, MAX - color.R, MAX - color.G, MAX - color.B);
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Ctrl.ControlInverter")]
    public class ControlInverterMachine : Machine
    {
        public ControlInverterMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Inverts a control channel.";
        }

        public override string Caption
        {
            get { return "[Invert]"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            int MAX = ImagineColor.MAX;
            ControlImage result = NewControl(inputs);
            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    result.SetValue(x, y, MAX - inputs[0].GetPixel(x, y).A);
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Img.RGBSplitter")]
    public class RGBSplitterMachine : Machine
    {
        public RGBSplitterMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "red (control)", "green (control)", "blue (control)" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { 'r', 'g', 'b' };
            description = "Deconstructs the R, G, and B channels of an image into three single-channel (control) images.";
        }

        public override string Caption
        {
            get { return "RGB Split"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            ImagineImage original = inputs[0];
            ControlImage[] controls = NewControlArray(inputs, 3);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    ImagineColor color = original.GetPixel(x, y);
                    controls[0].SetValue(x, y, color.R);
                    controls[1].SetValue(x, y, color.G);
                    controls[2].SetValue(x, y, color.B);
                }

                StandardCallback(x, original.Width, callback);
            }

            return new ImagineImage[] { controls[0], controls[1], controls[2] };
        }
    }
    
    [UniqueName("Imagine.Adder4")]
    public class Adder4Machine : Machine
    {
        public Adder4Machine()
        {
            inputNames = new string[] { "input 1", "input 2", "input 3", "input 4" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { '1', '2', '3', '4' };
            outputCodes = new char[] { ' ' };
            description = "Adds up to four input images by adding and clipping the separate channels (A, R, G, B).";
        }

        public override string Caption
        {
            get { return "Adder"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            ImagineImage result = NewFull(inputs);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    int a = 0, r = 0, g = 0, b = 0;
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        if (inputs[i] != null)
                        {
                            ImagineColor color = inputs[i].GetPixel(x, y);
                            a += color.A;
                            r += color.R;
                            g += color.G;
                            b += color.B;
                        }
                    }
                    result.SetPixel(x, y, a, r, g, b);
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Branch4")]
    public class Branch4Machine : Machine
    {
        public Branch4Machine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output1", "output2", "output3", "output4" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { '1', '2', '3', '4' };
            description = "Outputs up for four identical copies of the input image.";
        }

        public override string Caption
        {
            get { return "Branch"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            return new ImagineImage[] { CloneFirst(inputs), CloneFirst(inputs), CloneFirst(inputs), CloneFirst(inputs) };
        }

        private ImagineImage CloneFirst(ImagineImage[] inputs)
        {
            return (inputs[0] == null) ? null : inputs[0].Copy();
        }
    }

    [UniqueName("Imagine.Ctrl.RGBJoiner")]
    public class RGBJoinerMachine : Machine
    {
        public RGBJoinerMachine()
        {
            inputNames = new string[] { "red (control)", "green (control)", "blue (control)" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { 'r', 'g', 'b' };
            outputCodes = new char[] { ' ' };
            description = "Constructs an image from red, green and blue channels derived from control channel of respective input (alpha of output is fully opaque).";
        }

        public override string Caption
        {
            get { return "RGB Join"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            ImagineImage result = NewFull(inputs);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    int r = 0, g = 0, b = 0;
                    if (inputs[0] != null)
                        r = inputs[0].GetPixel(x, y).A;
                    if (inputs[1] != null)
                        g = inputs[1].GetPixel(x, y).A;
                    if (inputs[2] != null)
                        b = inputs[2].GetPixel(x, y).A;

                    result.SetPixel(x, y, ImagineColor.MAX, r, g, b);
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Img.HSLSplitter")]
    public class HSLSplitterMachine : Machine
    {
        public HSLSplitterMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "hue (control)", "saturation (control)", "lightness (control)" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { 'h', 's', 'l' };
            description = "Outputs the HSL (Hue/Saturation/Lightness) of each pixel, encoded in control channels.";
        }

        public override string Caption
        {
            get { return "HSL Split"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            ControlImage[] results = NewControlArray(inputs, 3);

            for (int x = 0; x < results[0].Width; x++)
            {
                for (int y = 0; y < results[0].Height; y++)
                {
                    ImagineColor color = inputs[0].GetPixel(x, y);
                    results[0].SetValue(x, y, (int)(color.Color.GetHue() / 360.0 * ImagineColor.MAX));
                    results[1].SetValue(x, y, (int)(color.Color.GetSaturation() * ImagineColor.MAX));
                    // This is wrong.. Microsofts model isn't HSB (a.k.a HSV) as claimed, it is actually HSL, which is quite different
                    results[2].SetValue(x, y, (int)(color.Color.GetBrightness() * ImagineColor.MAX));
                }

                StandardCallback(x, results[0].Width, callback);
            }

            return new ImagineImage[] { results[0], results[1], results[2] };
        }
    }

    [UniqueName("Imagine.Ctrl.HSLJoiner")]
    public class HSLJoinerMachine : Machine
    {
        public HSLJoinerMachine()
        {
            inputNames = new string[] { "hue (control)", "saturation (control)", "lightness (control)" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { 'h', 's', 'l' };
            outputCodes = new char[] { ' ' };
            description = "Constructs an image from HSL (Hue/Saturation/Lightness) derived from control channel of respective input (alpha of output is fully opaque).";
        }

        public override string Caption
        {
            get { return "HSL Join"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            int MAX = ImagineColor.MAX;

            ImagineImage result = NewFull(inputs);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    double h = 0, s = 0, l = 0;
                    if (inputs[0] != null)
                        h = (double)(inputs[0].GetPixel(x, y).A);
                    if (inputs[1] != null)
                        s = (double)(inputs[1].GetPixel(x, y).A);
                    if (inputs[2] != null)
                        l = (double)(inputs[2].GetPixel(x, y).A);

                    result.SetPixel(x, y, ImagineColor.FromHSL(h * 360.0 / MAX, s / MAX, l / MAX));
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Ctrl.ControlMultiplier4")]
    public class ControlMultiply4Machine : Machine
    {
        public ControlMultiply4Machine()
        {
            inputNames = new string[] { "input 1 (control)", "input 2 (control)", "input 3 (control)", "input 4 (control)" };
            outputNames = new string[] { "output (control)" };
            inputCodes = new char[] { '1', '2', '3', '4' };
            outputCodes = new char[] { ' ' };
            description = "Multiplies up to four control inputs, clipping as necessary.";
        }

        public override string Caption
        {
            get { return "[Multiply]"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            ControlImage result = NewControl(inputs);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    double alpha = 1;
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        if (inputs[i] != null)
                        {
                             alpha *= (((double)inputs[i].GetPixel(x, y).A) / ImagineColor.MAX);
                        }
                    }
                    result.SetValue(x, y, (int)(alpha * ImagineColor.MAX));
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Img.BadDynamicBlur")]
    public class BadDynamicBlurMachine : Machine
    {
        private int iterations = 20;

        public int Iterations
        {
            get { return iterations; }
            set { iterations = value; OnMachineChanged(); }
        }

        public BadDynamicBlurMachine()
        {
            inputNames = new string[] { "image", "control" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { 'I', 'c' };
            outputCodes = new char[] { ' ' };
            description = "Blurs the RGB image. The amount of blur at each pixel is determined by the single channel of the control input. Alpha becomes fully opaque.";
        }

        public override string Caption
        {
            get { return "Dyn Blur"; }
        }

        public override void LoadSettings(string settings)
        {
            Dictionary<string, string> properties = ParseSettings(settings);
            int? iterationsSetting = GetInt(properties, "iterations");
            if (iterationsSetting != null)
                iterations = iterationsSetting.Value;
        }

        public override string SaveSettings()
        {
            return CompileSettings(Set(null, "iterations", iterations));
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            FullImage result = NewFull(inputs);
            if (inputs[0] == null)
                return new ImagineImage[1];
            if (inputs[1] == null)
                return new ImagineImage[] { inputs[0].Copy() };
            ImagineImage source = inputs[0];

            for (int i = 0; i < iterations; i++)
            {
                result = NewFull(inputs[0]);
                
                for (int x = 1; x < result.Width - 1; x++)
                    for (int y = 1; y < result.Height - 1; y++)
                    {
                        int r, g, b;
                        
                        ImagineColor c, n, w, s, e;
                        c = source.GetPixel(x, y);
                        n = source.GetPixel(x, y - 1);
                        e = source.GetPixel(x + 1, y);
                        w = source.GetPixel(x - 1, y);
                        s = source.GetPixel(x, y + 1);

                        double a = ((double) inputs[1].GetPixel(x, y).A) / ImagineColor.MAX;
                        double cA = 1.0 - a * 0.8; 
                        double vA = (1.0 - cA) / 4.0;

                        r = (int) (c.R * cA + n.R * vA + w.R * vA + s.R * vA + e.R * vA);
                        g = (int) (c.G * cA + n.G * vA + w.G * vA + s.G * vA + e.G * vA);
                        b = (int) (c.B * cA + n.B * vA + w.B * vA + s.B * vA + e.B * vA);

                        result.SetPixel(x, y, ImagineColor.MAX, r, g, b);
                    }

                source = result;

                if (callback != null)
                    callback.Invoke(5 * i);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Ctrl.SoftControlContrast")]
    public class SoftControlContrastMachine : Machine
    {
        private double amount = 1.0;

        public double Amount
        {
            get { return amount; }
            set { amount = value; OnMachineChanged(); }
        }

        public SoftControlContrastMachine()
        {
            inputNames = new string[] { "input (control)" };
            outputNames = new string[] { "output (control)" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Increases contrast in the control input by a continuous, nonclipping function.";
        }

        public override string Caption
        {
            get { return "[SContrast]"; }
        }

        public override void LoadSettings(string settings)
        {
            Dictionary<string, string> properties = ParseSettings(settings);
            double? amountSetting = GetDouble(properties, "amount");
            if (amountSetting != null)
                amount = amountSetting.Value;
        }

        public override string SaveSettings()
        {
            return CompileSettings(Set(null, "amount", amount));
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            ControlImage result = NewControl(inputs);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    double value = ((double)inputs[0].GetPixel(x, y).A) / ImagineColor.MAX;
                    value = Math.Atan(Math.Tan((value - 0.5) * Math.PI) * amount) / Math.PI + 0.5;

                    result.SetValue(x, y, (int)(value * ImagineColor.MAX));
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Ctrl.HardControlContrast")]
    public class HardControlContrastMachine : Machine
    {
        private double amount = 1.0;

        public double Amount
        {
            get { return amount; }
            set { amount = value; OnMachineChanged(); }
        }

        public HardControlContrastMachine()
        {
            inputNames = new string[] { "input (control)" };
            outputNames = new string[] { "output (control)" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Increases contrast in the control input by a clipping, linear function.";
        }

        public override string Caption
        {
            get { return "[HContrast]"; }
        }

        public override void LoadSettings(string settings)
        {
            Dictionary<string, string> properties = ParseSettings(settings);
            double? amountSetting = GetDouble(properties, "amount");
            if (amountSetting != null)
                amount = amountSetting.Value;
        }

        public override string SaveSettings()
        {
            return CompileSettings(Set(null, "amount", amount));
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            ControlImage result = NewControl(inputs);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    double value = ((double)inputs[0].GetPixel(x, y).A) / ImagineColor.MAX;
                    value = (value - 0.5) * amount + 0.5;

                    result.SetValue(x, y, (int)(value * ImagineColor.MAX));
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Img.Gain")]
    public class GainMachine : Machine
    {
        private double gain = 1.0;

        public double Gain
        {
            get { return gain; }
            set { gain = value; OnMachineChanged(); }
        }

        public GainMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Applies gain (multiplication by value) to R, G, and B. 1.0 = no change. Does not affect alpha.";
        }

        public override string Caption
        {
            get { return "Gain"; }
        }

        public override void LoadSettings(string settings)
        {
            Dictionary<string, string> properties = ParseSettings(settings);
            double? gainSetting = GetDouble(properties, "gain");
            if (gainSetting != null)
                gain = gainSetting.Value;
        }

        public override string SaveSettings()
        {
            return CompileSettings(Set(null, "gain", gain));
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            FullImage result = NewFull(inputs);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    ImagineColor color = inputs[0].GetPixel(x, y);
                    result.SetPixel(x, y, color.A, (int)(color.R * gain), (int)(color.G * gain), (int)(color.B * gain));
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Ctrl.ControlGain")]
    public class ControlGainMachine : Machine
    {
        private double gain = 1.0;

        public double Gain
        {
            get { return gain; }
            set { gain = value; OnMachineChanged(); }
        }

        public ControlGainMachine()
        {
            inputNames = new string[] { "input (control)" };
            outputNames = new string[] { "output (control)" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Applies gain value to control input.";
        }

        public override string Caption
        {
            get { return "[Gain]"; }
        }

        public override void LoadSettings(string settings)
        {
            Dictionary<string, string> properties = ParseSettings(settings);
            double? gainSetting = GetDouble(properties, "gain");
            if (gainSetting != null)
                gain = gainSetting.Value;
        }

        public override string SaveSettings()
        {
            return CompileSettings(Set(null, "gain", gain));
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            ControlImage result = NewControl(inputs);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    result.SetValue(x, y, (int) (inputs[0].GetPixel(x, y).A * gain));
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Img.ColorProximity")]
    public class ColorProximityMachine : Machine
    {
        private ImagineColor targetColor = new ImagineColor(ImagineColor.MAX, ImagineColor.MAX, ImagineColor.MAX, ImagineColor.MAX);

        public ImagineColor TargetColor
        {
            get { return targetColor; }
            set { targetColor = value; OnMachineChanged(); }
        }

        public ColorProximityMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output (control)" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Contructs a control imagine based on the proximity of each pixel to the given color.";
        }

        public override string Caption
        {
            get { return "Color prox."; }
        }

        public override void LoadSettings(string settings)
        {
            Dictionary<string, string> properties = ParseSettings(settings);
            int? redSetting = GetInt(properties, "red");
            int? greenSetting = GetInt(properties, "green");
            int? blueSetting = GetInt(properties, "blue");
            if (redSetting != null && greenSetting != null && blueSetting != null)
                targetColor = new ImagineColor(ImagineColor.MAX,
                    ImagineColor.MAX / 256 * redSetting.Value,
                    ImagineColor.MAX / 256 * greenSetting.Value,
                    ImagineColor.MAX / 256 * blueSetting.Value);
        }

        public override string SaveSettings()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            Set(properties, "red", targetColor.R * 256 / ImagineColor.MAX);
            Set(properties, "green", targetColor.G * 256 / ImagineColor.MAX);
            Set(properties, "blue", targetColor.B * 256 / ImagineColor.MAX);
            return CompileSettings(properties);
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            ControlImage result = NewControl(inputs);

            double MAX = ImagineColor.MAX;
            double MAX_DISTANCE = Math.Sqrt(3);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    ImagineColor color = inputs[0].GetPixel(x, y);
                    double dR = (color.R - targetColor.R) / MAX;
                    double dG = (color.G - targetColor.G) / MAX;
                    double dB = (color.B - targetColor.B) / MAX;
                    result.SetValue(x, y, (int)((1.0 - Math.Sqrt(dR * dR + dG * dG + dB * dB) / MAX_DISTANCE) * MAX));
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Img.HueProximity")]
    public class HueProximityMachine : Machine
    {
        private ImagineColor targetColor = new ImagineColor(ImagineColor.MAX, ImagineColor.MAX, ImagineColor.MAX, ImagineColor.MAX);

        public ImagineColor TargetColor
        {
            get { return targetColor; }
            set { targetColor = value; OnMachineChanged(); }
        }

        public HueProximityMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output (control)" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Contructs a control imagine based on the proximity of the hue of each pixel to the hue of the given color.";
        }

        public override string Caption
        {
            get { return "Hue prox."; }
        }

        public override void LoadSettings(string settings)
        {
            Dictionary<string, string> properties = ParseSettings(settings);
            int? redSetting = GetInt(properties, "red");
            int? greenSetting = GetInt(properties, "green");
            int? blueSetting = GetInt(properties, "blue");
            if (redSetting != null && greenSetting != null && blueSetting != null)
                targetColor = new ImagineColor(ImagineColor.MAX,
                    ImagineColor.MAX / 256 * redSetting.Value,
                    ImagineColor.MAX / 256 * greenSetting.Value,
                    ImagineColor.MAX / 256 * blueSetting.Value);
        }

        public override string SaveSettings()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            Set(properties, "red", targetColor.R * 256 / ImagineColor.MAX);
            Set(properties, "green", targetColor.G * 256 / ImagineColor.MAX);
            Set(properties, "blue", targetColor.B * 256 / ImagineColor.MAX);
            return CompileSettings(properties);
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            ControlImage result = NewControl(inputs);

            double MAX = ImagineColor.MAX;
            double MAX_DISTANCE = Math.Sqrt(3);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    ImagineColor color = inputs[0].GetPixel(x, y);
                    double dH = Math.Abs(color.Color.GetHue() - targetColor.Color.GetHue()) / 180.0;
                    if (dH > 1.0)
                        dH = 2.0 - dH;
                    result.SetValue(x, y, (int) ((1.0 - dH) * MAX));
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Img.Blender")]
    public class BlendMachine : Machine
    {
        public BlendMachine()
        {
            inputNames = new string[] { "image 1", "image 2", "blend (control)" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { '1', '2', 'c' };
            outputCodes = new char[] { ' ' };
            description = "";
        }

        public override string Caption
        {
            get { return "Blend"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            if (inputs[0] == null && inputs[1] == null)
                return new ImagineImage[1];
            if (inputs[0] != null && inputs[1] == null)
                return new ImagineImage[] { inputs[0].Copy() };
            if (inputs[0] == null && inputs[1] != null)
                return new ImagineImage[] { inputs[1].Copy() };
            if(inputs[2] == null)
                return new ImagineImage[] { inputs[0].Copy() };
            
            FullImage result = NewFull(inputs);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    ImagineColor color1 = inputs[0].GetPixel(x, y);
                    ImagineColor color2 = inputs[1].GetPixel(x, y);
                    double factor = ((double)inputs[2].GetPixel(x, y).A) / ImagineColor.MAX;
                    result.SetPixel(x, y, ImagineColor.MAX,
                        (int)(color1.R * (1.0 - factor) + color2.R * factor),
                        (int)(color1.G * (1.0 - factor) + color2.G * factor),
                        (int)(color1.B * (1.0 - factor) + color2.B * factor));
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Img.Pixelator")]
    public class PixelateMachine : Machine
    {
        private int size = 1;

        public int Size
        {
            get { return size; }
            set { size = value; OnMachineChanged(); }
        }

        public PixelateMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "";
        }

        public override string Caption
        {
            get { return "Pixelate"; }
        }

        public override void LoadSettings(string settings)
        {
            Dictionary<string, string> properties = ParseSettings(settings);
            int? sizeSetting = GetInt(properties, "size");
            if (sizeSetting != null)
                size = sizeSetting.Value;
        }

        public override string SaveSettings()
        {
            return CompileSettings(Set(null, "size", size));
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            FullImage result = NewFull(inputs);

            for (int x = 0; x < result.Width; x += size)
            {
                for (int y = 0; y < result.Height; y += size)
                {
                    int ysize = (y + size > result.Height ? result.Height - y : size);
                    int xsize = (x + size > result.Width ? result.Width - x : size);

                    ImagineColor color = inputs[0].GetPixel(x, y);
                    for (int x1 = 0; x1 < xsize; x1++)
                        for (int y1 = 0; y1 < ysize; y1++)
                            result.SetPixel(x + x1, y + y1, color);
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Img.Crop")]
    public class CropMachine : Machine
    {
        public CropMachine()
        {
            inputNames = new string[] { "image", "reference" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { 'i', 'c' };
            outputCodes = new char[] { ' ' };
            description = "Crops the input image to the size specified by the reference image (control or full)";
        }

        public override string Caption
        {
            get { return "Crop"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            if (inputs[1] == null || inputs[0] == null)
                return new ImagineImage[1];
            
            FullImage result = NewFull(inputs[1]);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    result.SetPixel(x, y, inputs[0].GetPixel(x, y));
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Img.Resize")]
    public class ResizeMachine : Machine
    {
        public ResizeMachine()
        {
            inputNames = new string[] { "image", "reference" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { 'i', 'c' };
            outputCodes = new char[] { ' ' };
            description = "Resizes the input image to the size specified by the reference image (control or full). Uses nearest-neighbour.";
        }

        public override string Caption
        {
            get { return "Resize"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            if (inputs[1] == null || inputs[0] == null)
                return new ImagineImage[1];

            FullImage result = NewFull(inputs[1]);

            double dx = ((double)inputs[0].Width) / result.Width;
            double dy = ((double)inputs[0].Height) / result.Height;

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    result.SetPixel(x, y, inputs[0].GetPixel((int) Math.Round(x*dx), (int) Math.Round(y*dy)));
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }
}
