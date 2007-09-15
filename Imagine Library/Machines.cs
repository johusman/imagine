using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Imagine.Library
{
    public abstract class Machine
    {
        public abstract Bitmap Process(Bitmap[] inputs);
    }

    public class SourceMachine : Machine
    {
        private string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public override string ToString()
        {
            return "Source";
        }

        public Bitmap Load()
        {
            return (Bitmap)Image.FromFile(filename);
        }

        public override Bitmap Process(Bitmap[] inputs)
        {
            return Load();
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

        public override Bitmap Process(Bitmap[] inputs)
        {
            ImageCodecInfo codec = FindPngCodec();
            EncoderParameters parameters = new EncoderParameters(0);
            inputs[0].Save(filename, codec, parameters);

            return null;
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

        public override Bitmap Process(Bitmap[] inputs)
        {
            Bitmap bitmap = (Bitmap) inputs[0].Clone();
            for(int x = 0; x < bitmap.Width; x++)
                for(int y = 0; y < bitmap.Height; y++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    Color newColor = Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
                    bitmap.SetPixel(x, y, newColor);
                }

            return bitmap;
        }
    }
}
