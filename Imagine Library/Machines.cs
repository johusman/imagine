using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Imagine.Library
{
    public abstract class Machine
    {
        public virtual ImagineImage[] Process(ImagineImage[] inputs, ProgressCallback callback)
        {
            if(callback != null)
                callback.Invoke(0);

            try
            {
                if (inputs.Length != InputCount)
                    throw new IncorrectNumberOfMachineInputsException();
                if (InputCount > 0 && FindFirstImage(inputs) == null)
                    return new ImagineImage[OutputCount];

                return DoProcess(inputs, callback);
            }
            finally
            {
                if (callback != null)
                    callback.Invoke(100);
            }
        }

        public delegate void ProgressCallback(int percent);

        protected abstract ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback);

        public event System.EventHandler MachineChanged;
        
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

        protected void StandardCallback(int index, int outOf, ProgressCallback callback)
        {
            if (callback != null && (index & 0x3F) == 0)
                callback.Invoke(100 * index / outOf);
        }

        protected virtual void OnMachineChanged()
        {
            EventHandler handler = MachineChanged;
            if (handler != null)
                handler(this, null);
        }

    }

    public class IncorrectNumberOfMachineInputsException : Exception
    {
        public IncorrectNumberOfMachineInputsException() : base() { }
        public IncorrectNumberOfMachineInputsException(string message) : base(message) { }
        public IncorrectNumberOfMachineInputsException(string message, Exception innerException) : base(message, innerException) { }
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
            set { filename = value; OnMachineChanged(); }
        }

        private bool preview = false;

        public bool Preview
        {
            get { return preview; }
            set { preview = value; }
        }

        private ImagineImage lastPreviewImage;

        public ImagineImage LastPreviewImage
        {
            get { return lastPreviewImage; }
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
            ImagineImage image = null;
            if (filename != null)
            {
                Bitmap bitmap = (Bitmap)Image.FromFile(filename, false);
                if (!preview)
                    image = new FullImage(bitmap);
                else
                    image = FullImage.CreatePreview(bitmap);
                bitmap.Dispose();
            }
            lastPreviewImage = image;
            return image;
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
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
            set { filename = value; OnMachineChanged();  }
        }

        public override string Caption
        {
            get { return "Destination"; }
        }

        private bool preview = false;

        public bool Preview
        {
            get { return preview; }
            set { preview = value; }
        }

        private ImagineImage lastPreviewImage;

        public ImagineImage LastPreviewImage
        {
            get { return lastPreviewImage; }
        }

        public SinkMachine()
        {
            inputNames = new string[] { "input" };
            outputNames = new string[0];
            inputCodes = new char[] { ' ' };
            outputCodes = new char[0];
            description = "Writes the input image to a file.";
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            if (!preview)
            {
                if (filename != null)
                {
                    Bitmap bitmap = inputs[0].GetBitmap();
                    ImageCodecInfo codec = FindPngCodec();
                    EncoderParameters parameters = new EncoderParameters(0);
                    bitmap.Save(filename, codec, parameters);
                    bitmap.Dispose();
                }
            }
            else
                lastPreviewImage = inputs[0];

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
}
