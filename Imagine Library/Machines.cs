using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Imagine.Library
{
    public abstract class Machine
    {
        public abstract ImagineImage[] Process(ImagineImage[] inputs);
        protected string[] inputNames;
        protected string[] outputNames;
        protected char[] inputCodes;
        protected char[] outputCodes;
        protected string description = "";

        public int InputCount
        {
            get { return inputNames.Length; }
        }

        public int OutputCount
        {
            get { return outputNames.Length; }
        }

        public string[] InputNames
        {
            get { return inputNames; }
        }

        public string[] OutputNames
        {
            get { return outputNames; }
        }

        public char[] InputCodes
        {
            get { return inputCodes; }
        }

        public char[] OutputCodes
        {
            get { return outputCodes; }
        }

        public string Description
        {
            get { return description; }
        }

        public abstract string Caption
        {
            get;
        }

        public override string ToString()
        {
            return ((UniqueName) GetType().GetCustomAttributes(typeof(UniqueName), false)[0]).Value;
        }

        protected ImagineImage FindFirstImage(ImagineImage[] images)
        {
            foreach (ImagineImage image in images)
                if (image != null)
                    return image;

            return null;
        }

        protected FullImage NewFull(ImagineImage image)
        {
            return (image == null) ? null : new FullImage(image.Width, image.Height);
        }

        protected ControlImage NewControl(ImagineImage image)
        {
            return (image == null) ? null : new ControlImage(image.Width, image.Height);
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class UniqueName : Attribute
    {
        private string value;
        public UniqueName(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }
    }

    [UniqueName("Imagine.Source")]
    public class SourceMachine : Machine
    {
        private string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public SourceMachine()
        {
            inputNames = new string[0];
            outputNames = new string[] { "output" };
            inputCodes = new char[0];
            outputCodes = new char[] { ' ' };
            description = "Provides a source image from file.";
        }

        public override string Caption
        {
            get { return "Source"; }
        }

        public ImagineImage Load()
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(filename, false);
            ImagineImage image = new FullImage(bitmap);
            bitmap.Dispose();
            return image;
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            return new ImagineImage[] { Load() };
        }
    }
    
    [UniqueName("Imagine.Destination")]
    public class SinkMachine : Machine
    {
        private string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public override string Caption
        {
            get { return "Destination"; }
        }

        public SinkMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[0];
            inputCodes = new char[] { ' ' };
            outputCodes = new char[0];
            description = "Writes the input image to a file.";
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            Bitmap bitmap = inputs[0].GetBitmap();
            ImageCodecInfo codec = FindPngCodec();
            EncoderParameters parameters = new EncoderParameters(0);
            bitmap.Save(filename, codec, parameters);
            bitmap.Dispose();

            return new ImagineImage[0];
        }

        private ImageCodecInfo FindPngCodec()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach(ImageCodecInfo codec in codecs)
                foreach(String ext in codec.FilenameExtension.Split(';'))
                    if(ext.ToLower().Equals("*.png"))
                        return codec;

            return null;
        }
    }

    [UniqueName("Imagine.Inverter")]
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
            get { return "Invert -a"; }
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            int MAX = ImagineColor.MAX;
            ImagineImage result = NewFull(inputs[0]);
            for(int x = 0; x < result.Width; x++)
                for(int y = 0; y < result.Height; y++)
                {
                    ImagineColor color = inputs[0].GetPixel(x, y);
                    result.SetPixel(x, y, color.A, MAX - color.R, MAX - color.G, MAX - color.B);
                }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.RGBSplitter")]
    public class RGBSplitterMachine : Machine
    {
        public RGBSplitterMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "red (alpha)", "green (alpha)", "blue (alpha)" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { 'r', 'g', 'b' };
            description = "Deconstructs the R, G, and B channels of an image into three single-channel (Alpha) images.";
        }

        public override string Caption
        {
            get { return "RGB Split"; }
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            ImagineImage original = inputs[0];
            ControlImage[] controls = { NewControl(original), NewControl(original), NewControl(original) };
            if (original == null)
                return controls;

            for(int x = 0; x < original.Width; x++)
                for(int y = 0; y < original.Height; y++)
                {
                    ImagineColor color = original.GetPixel(x, y);
                    controls[0].SetValue(x, y, color.R);
                    controls[1].SetValue(x, y, color.G);
                    controls[2].SetValue(x, y, color.B);
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
            description = "Adds up to four input images by adding and clipping the separate channels.";
        }

        public override string Caption
        {
            get { return "Adder"; }
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            ImagineImage result = NewFull(FindFirstImage(inputs));
            if (result == null)
                return new ImagineImage[1];

            for(int x = 0; x < result.Width; x++)
                for(int y = 0; y < result.Height; y++)
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

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            return new ImagineImage[] { CloneFirst(inputs), CloneFirst(inputs), CloneFirst(inputs), CloneFirst(inputs) };
        }

        private ImagineImage CloneFirst(ImagineImage[] inputs)
        {
            return (inputs[0] == null) ? null : inputs[0].Copy();
        }
    }

    [UniqueName("Imagine.RGBJoiner")]
    public class RGBJoinerMachine : Machine
    {
        public RGBJoinerMachine()
        {
            inputNames = new string[] { "red (alpha)", "green (alpha)", "blue (alpha)" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { 'r', 'g', 'b' };
            outputCodes = new char[] { ' ' };
            description = "Constructs an image from red, green and blue channels derived from alpha channel of respective input (alpha of output is fully opaque).";
        }

        public override string Caption
        {
            get { return "RGB Join"; }
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            ImagineImage result = NewFull(FindFirstImage(inputs));
            if (result == null)
                return new ImagineImage[1];

            for(int x = 0; x < result.Width; x++)
                for(int y = 0; y < result.Height; y++)
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

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.Halver")]
    public class HalverMachine : Machine
    {
        public HalverMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Diminishes R, G and B channel by 50% (leaves alpha intact).";
        }

        public override string Caption
        {
            get { return "Halver -a"; }
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            ImagineImage result = NewFull(inputs[0]);
            if (result == null)
                return new ImagineImage[1];

            for(int x = 0; x < result.Width; x++)
                for(int y = 0; y < result.Height; y++)
                {
                    ImagineColor color = inputs[0].GetPixel(x, y);
                    result.SetPixel(x, y, color.A, color.R / 2, color.G / 2, color.B / 2);
                }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.HSLSplitter")]
    public class HSLSplitterMachine : Machine
    {
        public HSLSplitterMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "hue (alpha)", "saturation (alpha)", "lightness (alpha)" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { 'h', 's', 'l' };
            description = "Outputs the HSL (Hue/Saturation/Lightness) of each pixel, encoded in the alpha channel of respective output.";
        }

        public override string Caption
        {
            get { return "HSL Split"; }
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            ControlImage[] results = { NewControl(inputs[0]), NewControl(inputs[0]), NewControl(inputs[0]) };
            if (inputs[0] == null)
                return new ImagineImage[3];

            for(int x = 0; x < results[0].Width; x++)
                for(int y = 0; y < results[0].Height; y++)
                {
                    ImagineColor color = inputs[0].GetPixel(x, y);
                    results[0].SetValue(x, y, (int) (color.Color.GetHue()/360.0 * ImagineColor.MAX));
                    results[1].SetValue(x, y, (int) (color.Color.GetSaturation() * ImagineColor.MAX));
                    // This is wrong.. Microsofts model isn't HSB (a.k.a HSV) as claimed, it is actually HSL, which is quite different
                    results[2].SetValue(x, y, (int) (color.Color.GetBrightness() * ImagineColor.MAX));
                }

            return new ImagineImage[] { results[0], results[1], results[2] };
        }
    }

    [UniqueName("Imagine.HSLJoiner")]
    public class HSLJoinerMachine : Machine
    {
        public HSLJoinerMachine()
        {
            inputNames = new string[] { "hue (alpha)", "saturation (alpha)", "lightness (alpha)" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { 'h', 's', 'l' };
            outputCodes = new char[] { ' ' };
            description = "Constructs an image from HSL (Hue/Saturation/Lightness) derived from alpha channel of respective input (alpha of output is fully opaque).";
        }

        public override string Caption
        {
            get { return "HSL Join"; }
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            int MAX = ImagineColor.MAX;

            ImagineImage result = NewFull(FindFirstImage(inputs));
            if (result == null)
                return new ImagineImage[1];

            for (int x = 0; x < result.Width; x++)
                for (int y = 0; y < result.Height; y++)
                {
                    double h = 0, s = 0, l = 0;
                    if (inputs[0] != null)
                        h = (double)(inputs[0].GetPixel(x, y).A);
                    if (inputs[1] != null)
                        s = (double)(inputs[1].GetPixel(x, y).A);
                    if (inputs[2] != null)
                        l = (double)(inputs[2].GetPixel(x, y).A);

                    result.SetPixel(x, y, ImagineColor.FromHSL(h*360.0/MAX, s/MAX, l/MAX));
                }

            return new ImagineImage[] { result };
        }
    }

    [UniqueName("Imagine.AlphaMultiplier4")]
    public class AlphaMultiply4Machine : Machine
    {
        public AlphaMultiply4Machine()
        {
            inputNames = new string[] { "input 1", "input 2", "input 3", "input 4" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { '1', '2', '3', '4' };
            outputCodes = new char[] { ' ' };
            description = "Multiplies up to four alpha channels from inputs, clipping as necessary.";
        }

        public override string Caption
        {
            get { return "Multiply -a"; }
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            ControlImage result = NewControl(FindFirstImage(inputs));
            if (result == null)
                return new ImagineImage[1];

            for (int x = 0; x < result.Width; x++)
                for (int y = 0; y < result.Height; y++)
                {
                    double dividend = 0;
                    double alpha = 1;
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        if (inputs[i] != null)
                        {
                            dividend += 1.0;
                            alpha *= (((double)inputs[i].GetPixel(x, y).A) / ImagineColor.MAX);
                        }
                    }
                    result.SetValue(x, y, (int) (alpha/dividend * ImagineColor.MAX));
                }

            return new ImagineImage[] { result };
        }
    }
}
