using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Imagine.Library
{
    public abstract class Machine
    {
        public abstract Bitmap[] Process(Bitmap[] inputs);
        protected string[] inputNames;
        protected string[] outputNames;
        protected char[] inputCodes;
        protected char[] outputCodes;

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
        }

        public override string ToString()
        {
            return "Source";
        }

        public Bitmap Load()
        {
            return (Bitmap)Image.FromFile(filename);
        }

        public override Bitmap[] Process(Bitmap[] inputs)
        {
            return new Bitmap[] { Load() };
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
        }

        public override Bitmap[] Process(Bitmap[] inputs)
        {
            ImageCodecInfo codec = FindPngCodec();
            EncoderParameters parameters = new EncoderParameters(0);
            inputs[0].Save(filename, codec, parameters);

            return new Bitmap[0];
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
        }

        public override Bitmap[] Process(Bitmap[] inputs)
        {
            Bitmap bitmap = (Bitmap) inputs[0].Clone();
            for(int x = 0; x < bitmap.Width; x++)
                for(int y = 0; y < bitmap.Height; y++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    Color newColor = Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
                    bitmap.SetPixel(x, y, newColor);
                }

            return new Bitmap[] { bitmap };
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
            outputNames = new string[] { "red", "green", "blue" };
            inputCodes = new char[] { ' ' };
            outputCodes = new char[] { 'R', 'G', 'B' };
        }

        public override Bitmap[] Process(Bitmap[] inputs)
        {
            Bitmap original = inputs[0];
            Bitmap[] bitmaps = { (Bitmap)original.Clone(), (Bitmap)original.Clone(), (Bitmap)original.Clone() };
            for(int x = 0; x < original.Width; x++)
                for(int y = 0; y < original.Height; y++)
                {
                    Color color = original.GetPixel(x, y);
                    Color rColor = Color.FromArgb(color.A, color.R, 0, 0);
                    Color gColor = Color.FromArgb(color.A, 0, color.G, 0);
                    Color bColor = Color.FromArgb(color.A, 0, 0, color.B);
                    bitmaps[0].SetPixel(x, y, rColor);
                    bitmaps[1].SetPixel(x, y, gColor);
                    bitmaps[2].SetPixel(x, y, bColor);
                }

            return bitmaps;
        }
    }

    public class AdderMachine : Machine
    {
        public override string ToString()
        {
            return "Adder";
        }

        public AdderMachine()
        {
            inputNames = new string[] { "input 1", "input 2" };
            outputNames = new string[] { "output" };
            inputCodes = new char[] { '1', '2' };
            outputCodes = new char[] { ' ' };
        }

        public override Bitmap[] Process(Bitmap[] inputs)
        {
            Bitmap[] bitmaps = { (Bitmap)inputs[0].Clone() };
            for(int x = 0; x < inputs[0].Width; x++)
                for(int y = 0; y < inputs[0].Height; y++)
                {
                    Color color1 = inputs[0].GetPixel(x, y);
                    Color color2 = inputs[1].GetPixel(x, y);
                    bitmaps[0].SetPixel(x, y, Color.FromArgb((int) Math.Min(color1.A + color2.A, 255), (int) Math.Min(color1.R + color2.R, 255), (int) Math.Min(color1.G + color2.G, 255), (int) Math.Min(color1.B + color2.B, 255)));
                }

            return bitmaps;
        }
    }
}
