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

        public override string ToString()
        {
            return "Source";
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

    public class SinkMachine : Machine
    {
        private string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public override string ToString()
        {
            return "Destination";
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

    public class InverterMachine : Machine
    {
        public override string ToString()
        {
            return "Inverter";
        }

        public InverterMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Does an RGB invert of the image (leaves Alpha intact).";
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

    public class RGBSplitterMachine : Machine
    {
        public override string ToString()
        {
            return "RGB Split";
        }

        public RGBSplitterMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "red (alpha)", "green (alpha)", "blue (alpha)" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { 'r', 'g', 'b' };
            description = "Deconstructs the R, G, and B channels of an image into three single-channel (Alpha) images.";
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

    public class Adder4Machine : Machine
    {
        public override string ToString()
        {
            return "Adder4";
        }

        public Adder4Machine()
        {
            inputNames = new string[] { "input 1", "input 2", "input 3", "input 4" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { '1', '2', '3', '4' };
            outputCodes = new char[] { ' ' };
            description = "Adds up to four input images by adding and clipping the separate channels.";
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

    public class Branch4Machine : Machine
    {
        public override string ToString()
        {
            return "Branch4";
        }

        public Branch4Machine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output1", "output2", "output3", "output4" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { '1', '2', '3', '4' };
            description = "Outputs up for four identical copies of the input image.";
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

    public class ComposerMachine : Machine
    {
        public override string ToString()
        {
            return "Composer";
        }

        public ComposerMachine()
        {
            inputNames = new string[] { "red (alpha)", "green (alpha)", "blue (alpha)" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { 'r', 'g', 'b' };
            outputCodes = new char[] { ' ' };
            description = "Constructs an image from red, green and blue channels derives from alpha channel of respective input (alpha of output is fully opaque).";
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
                        r = (int) (inputs[0].GetPixel(x, y).A);
                    if (inputs[1] != null)
                        g = (int) (inputs[1].GetPixel(x, y).A);
                    if (inputs[2] != null)
                        b = (int) (inputs[2].GetPixel(x, y).A);

                    result.SetPixel(x, y, ImagineColor.MAX, r, g, b);
                }

            return new ImagineImage[] { result };
        }
    }

    public class HalverMachine : Machine
    {
        public override string ToString()
        {
            return "Halver";
        }

        public HalverMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Diminishes R, G and B channel by 50% (leaves alpha intact).";
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

    public class BrightnessMachine : Machine
    {
        public override string ToString()
        {
            return "Brightness";
        }

        public BrightnessMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Outputs the brightness of each pixel, encoded in the alpha channel.";
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            ControlImage result = NewControl(inputs[0]);
            if (result == null)
                return new ImagineImage[1];

            for(int x = 0; x < result.Width; x++)
                for(int y = 0; y < result.Height; y++)
                {
                    ImagineColor color = inputs[0].GetPixel(x, y);
                    result.SetValue(x, y, (int) (color.Color.GetBrightness() * ImagineColor.MAX));
                }

            return new ImagineImage[] { result };
        }
    }

    public class HueMachine : Machine
    {
        public override string ToString()
        {
            return "Hue";
        }

        public HueMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Outputs the hue of each pixel, encoded in the alpha channel.";
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            ControlImage result = NewControl(inputs[0]);
            if (result == null)
                return new ImagineImage[1];

            for(int x = 0; x < result.Width; x++)
                for(int y = 0; y < result.Height; y++)
                {
                    ImagineColor color = inputs[0].GetPixel(x, y);
                    result.SetValue(x, y, (int) (color.Color.GetHue() * ImagineColor.MAX));
                }

            return new ImagineImage[] { result };
        }
    }

    public class SaturationMachine : Machine
    {
        public override string ToString()
        {
            return "Saturation";
        }

        public SaturationMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { ' ' };
            description = "Outputs the saturation of each pixel, encoded in the alpha channel.";
        }

        public override ImagineImage[] Process(ImagineImage[] inputs)
        {
            ControlImage result = NewControl(inputs[0]);
            if (result == null)
                return new ImagineImage[1];

            for(int x = 0; x < result.Width; x++)
                for(int y = 0; y < result.Height; y++)
                {
                    ImagineColor color = inputs[0].GetPixel(x, y);
                    result.SetValue(x, y, (int) (color.Color.GetSaturation() * ImagineColor.MAX));
                }

            return new ImagineImage[] { result };
        }
    }
}
