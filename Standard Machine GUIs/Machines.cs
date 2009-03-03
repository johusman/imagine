using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Imagine.Library;

namespace Imagine.StandardMachines
{
    [UniqueName("Imagine.Img.Inverter")]
    [InputNames("input")]
    [InputCodes(' ')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Does an RGB invert of the image (leaves Alpha intact).")]
    public class InverterMachine : Machine
    {
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
    [InputNames("input")]
    [InputCodes(' ')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Inverts a control channel.")]
    public class ControlInverterMachine : Machine
    {
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
    [InputNames("input")]
    [InputCodes(' ')]
    [OutputNames("red (control)", "green (control)", "blue (control)")]
    [OutputCodes('r', 'g', 'b')]
    [Description("Deconstructs the R, G, and B channels of an image into three single-channel (control) images.")]
    public class RGBSplitterMachine : Machine
    {
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
    [InputNames("input 1", "input 2", "input 3", "input 4")]
    [InputCodes('1', '2', '3', '4')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Adds up to four input images by adding and clipping the separate channels (A, R, G, B).")]
    public class Adder4Machine : Machine
    {
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

    [UniqueName("Imagine.Ctrl.RGBJoiner")]
    [InputNames("red (control)", "green (control)", "blue (control)")]
    [InputCodes('r', 'g', 'b')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Constructs an image from red, green and blue channels derived from control channel of respective input (alpha of output is fully opaque).")]
    public class RGBJoinerMachine : Machine
    {
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
    [InputNames("input")]
    [InputCodes(' ')]
    [OutputNames("hue (control)", "saturation (control)", "lightness (control)")]
    [OutputCodes('h', 's', 'l')]
    [Description("Outputs the HSL (Hue/Saturation/Lightness) of each pixel, encoded in control channels.")]
    public class HSLSplitterMachine : Machine
    {
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
    [InputNames("hue (control)", "saturation (control)", "lightness (control)")]
    [InputCodes('h', 's', 'l')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Constructs an image from HSL (Hue/Saturation/Lightness) derived from control channel of respective input (alpha of output is fully opaque).")]
    public class HSLJoinerMachine : Machine
    {
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
    [InputNames("input 1 (control)", "input 2 (control)", "input 3 (control)", "input 4 (control)")]
    [InputCodes('1', '2', '3', '4')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Multiplies up to four control inputs, clipping as necessary.")]
    public class ControlMultiply4Machine : Machine
    {
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

    public class Convolutor
    {
        public static FullImage Convolute(FramedImage source, double[] vector, bool vertically, ProgressCallback callback, double callbackOffset, double callbackFactor)
        {
            int cb_percentOffset = (int) (callbackOffset * 100);
            double cb_factor = 100.0 * callbackFactor / source.Width;

            FullImage result = new FullImage(source.Width, source.Height);

            int offset = vector.Length / 2;
            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    double r = 0.0, g = 0.0, b = 0.0, a = 0.0;

                    for (int i = 0; i < vector.Length; i++)
                    {
                        double factor = vector[i];
                        ImagineColor col =
                            vertically ?
                                source.GetPixel(x, y - offset + i) :
                                source.GetPixel(x - offset + i, y);
                        r += col.R * factor;
                        g += col.G * factor;
                        b += col.B * factor;
                        a += col.A * factor;
                    }

                    result.SetPixel(x, y, (int)a, (int)r, (int)g, (int)b);
                }

                callback.Invoke(cb_percentOffset + (int) (x * cb_factor));
            }

            return result;
        }

        public static FullImage Convolute(FramedImage source, FramedImage mask, double[] vector, bool vertically, ProgressCallback callback, double callbackOffset, double callbackFactor)
        {
            int cb_percentOffset = (int)(callbackOffset * 100);
            double cb_factor = 100.0 * callbackFactor / source.Width;

            FullImage result = new FullImage(source.Width, source.Height);

            int offset = vector.Length / 2;
            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    double r = 0.0, g = 0.0, b = 0.0, a = 0.0;

                    for (int i = 0; i < vector.Length; i++)
                    {
                        double factor = vector[i];
                        double maskFactor =
                            vertically ?
                                mask.GetPixel(x, y - offset + i).A :
                                mask.GetPixel(x - offset + i, y).A;

                        factor = (factor * maskFactor) / ImagineColor.MAX;

                        ImagineColor col =
                            vertically ?
                                source.GetPixel(x, y - offset + i) :
                                source.GetPixel(x - offset + i, y);
                        r += col.R * factor;
                        g += col.G * factor;
                        b += col.B * factor;
                        a += col.A * factor;
                    }

                    result.SetPixel(x, y, (int)a, (int)r, (int)g, (int)b);
                }

                callback.Invoke(cb_percentOffset + (int)(x * cb_factor));
            }

            return result;
        }
    }

    [UniqueName("Imagine.Img.GaussianBlur")]
    [InputNames("input")]
    [InputCodes(' ')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Applies a gaussian blur to the input image.")]
    public class GaussianBlurMachine : Machine
    {
        protected double size = 5.0;

        public double Size
        {
            get { return size; }
            set { size = value; OnMachineChanged(); }
        }

        public override string Caption
        {
            get { return "GaussBlur"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            if (inputs[0] == null)
                return new ImagineImage[1];
            FramedImage source = new EdgeRepeatFramedImage(inputs[0]);

            double[] vector = GenerateGaussVector(size);
            FullImage result = Convolutor.Convolute(source, vector, false, callback, 0.0, 0.5);
            source = new EdgeRepeatFramedImage(result);
            result = Convolutor.Convolute(source, vector, true, callback, 0.5, 0.5);

            return new ImagineImage[] { result };
        }

        public override void LoadSettings(string settings)
        {
            Dictionary<string, string> properties = ParseSettings(settings);
            double? sizeSetting = GetDouble(properties, "size");
            if (sizeSetting != null)
                size = sizeSetting.Value;
        }

        public override string SaveSettings()
        {
            return CompileSettings(Set(null, "size", size));
        }

        protected double[] GenerateGaussVector(double pixelSize)
        {
            double sigma = pixelSize / 3.0;
            int limit = (int) pixelSize;
            int length = 1 + limit * 2;
            double[] vector = new double[length];
            
            double offset = (length - 1) / 2.0;
            double denominator = 2.0*sigma*sigma;
            double totalEnergy = 0.0;
            for (int i = 0; i < length; i++)
            {
                double x = i - offset;
                double value = Math.Exp(-(x * x) / denominator);
                vector[i] = value;
                totalEnergy += value;
            }

            for (int i = 0; i < length; i++)
                vector[i] /= totalEnergy;

            return vector;
        }
    }

    [UniqueName("Imagine.Img.GaussianBleed")]
    [InputNames("image", "control")]
    [InputCodes('I', 'c')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Gaussian Bleed is like Gaussian Blur, but the control channel determines how much each pixel contribute to the blur.")]
    public class GaussianBleedMachine : GaussianBlurMachine
    {
        public override string Caption
        {
            get { return "GaussBleed"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            if (inputs[0] == null)
                return new ImagineImage[1];
            FramedImage source = new EdgeRepeatFramedImage(inputs[0]);

            double[] vector = GenerateGaussVector(size);
            FullImage result;
            if (inputs[1] == null)
            {
                result = Convolutor.Convolute(source, vector, false, callback, 0.0, 0.5);
            }
            else
            {
                FramedImage mask = new EdgeRepeatFramedImage(inputs[1]);
                result = Convolutor.Convolute(source, mask, vector, false, callback, 0.0, 0.5);
            }
            source = new EdgeRepeatFramedImage(result);
            result = Convolutor.Convolute(source, vector, true, callback, 0.5, 0.5);

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Ctrl.SoftControlContrast")]
    [InputNames("input (control)")]
    [InputCodes(' ')]
    [OutputNames("output (control)")]
    [OutputCodes(' ')]
    [Description("Increases contrast in the control input by a continuous, nonclipping function.")]
    public class SoftControlContrastMachine : Machine
    {
        private double amount = 1.0;

        public double Amount
        {
            get { return amount; }
            set { amount = value; OnMachineChanged(); }
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
    [InputNames("input (control)")]
    [InputCodes(' ')]
    [OutputNames("output (control)")]
    [OutputCodes(' ')]
    [Description("Increases contrast in the control input by a clipping, linear function.")]
    public class HardControlContrastMachine : Machine
    {
        private double amount = 1.0;

        public double Amount
        {
            get { return amount; }
            set { amount = value; OnMachineChanged(); }
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
    [InputNames("input")]
    [InputCodes(' ')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Applies gain (multiplication by value) to R, G, and B. 1.0 = no change. Does not affect alpha.")]
    public class GainMachine : Machine
    {
        private double gain = 1.0;

        public double Gain
        {
            get { return gain; }
            set { gain = value; OnMachineChanged(); }
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
    [InputNames("input (control)")]
    [InputCodes(' ')]
    [OutputNames("output (control)")]
    [OutputCodes(' ')]
    [Description("Applies gain value to control input.")]
    public class ControlGainMachine : Machine
    {
        private double gain = 1.0;

        public double Gain
        {
            get { return gain; }
            set { gain = value; OnMachineChanged(); }
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
    [InputNames("input")]
    [InputCodes(' ')]
    [OutputNames("output (control)")]
    [OutputCodes(' ')]
    [Description("Contructs a control imagine based on the proximity of each pixel to the given color.")]
    public class ColorProximityMachine : Machine
    {
        private ImagineColor targetColor = new ImagineColor(ImagineColor.MAX, ImagineColor.MAX, ImagineColor.MAX, ImagineColor.MAX);

        public ImagineColor TargetColor
        {
            get { return targetColor; }
            set { targetColor = value; OnMachineChanged(); }
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
    [InputNames("input")]
    [InputCodes(' ')]
    [OutputNames("output (control)")]
    [OutputCodes(' ')]
    [Description("Contructs a control imagine based on the proximity of the hue of each pixel to the hue of the given color.")]
    public class HueProximityMachine : Machine
    {
        private ImagineColor targetColor = new ImagineColor(ImagineColor.MAX, ImagineColor.MAX, ImagineColor.MAX, ImagineColor.MAX);

        public ImagineColor TargetColor
        {
            get { return targetColor; }
            set { targetColor = value; OnMachineChanged(); }
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
    [InputNames("image 1", "image 2", "blend (control)")]
    [InputCodes('1', '2', 'c')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Blends two images; the blend input determines how much from image 1 and 2 to use at each pixel.")]
    public class BlendMachine : Machine
    {
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
    [InputNames("input")]
    [InputCodes(' ')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Pixelates the image to the size in the settings.")]
    public class PixelateMachine : Machine
    {
        private int size = 1;

        public int Size
        {
            get { return size; }
            set { size = value; OnMachineChanged(); }
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
    [InputNames("image", "reference")]
    [InputCodes('i', 'c')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Crops the input image to the size specified by the reference image (control or full).")]
    public class CropMachine : Machine
    {
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
    [InputNames("image", "reference")]
    [InputCodes('i', 'c')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Resizes the input image to the size specified by the reference image (control or full). Uses nearest-neighbour.")]
    public class ResizeMachine : Machine
    {
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

    [UniqueName("Imagine.Img.Color")]
    [InputNames("reference")]
    [InputCodes(' ')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Produces an image filled with a single color, having the size specified by the reference image (control or full).")]
    public class ColorMachine : Machine
    {
        private ImagineColor targetColor = new ImagineColor(ImagineColor.MAX, ImagineColor.MAX, ImagineColor.MAX, ImagineColor.MAX);

        public ImagineColor TargetColor
        {
            get { return targetColor; }
            set { targetColor = value; OnMachineChanged(); }
        }

        public override string Caption
        {
            get { return "Color"; }
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
            ImagineImage result = NewFull(inputs);
            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    result.SetPixel(x, y, targetColor);
                }

                StandardCallback(x, result.Width, callback);
            }

            return new ImagineImage[] { result };
        }
    }
}
