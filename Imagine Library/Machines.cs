using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Globalization;

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

        public virtual void LoadSettings(string settings)
        {
            // By default do nothing
        }

        public virtual string SaveSettings()
        {
            // By default return null
            return null;
        }

        protected Dictionary<string, string> ParseSettings(string settings)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();

            Group settingsGroup = Regex.Match(settings, "^(?<setting>\\s*\\w+\\s*=\\s*'[^']*'\\s*)+$").Groups["setting"];
            foreach (Capture settingCapture in settingsGroup.Captures)
            {
                string settingData = settingCapture.Value;
                Match settingMatch = Regex.Match(settingData, "^\\s*(?<key>\\w+)\\s*=\\s*'(?<value>[^']*)'\\s*$");
                string key = settingMatch.Groups["key"].Value;
                string value = settingMatch.Groups["value"].Value;
                properties.Add(key, value);
            }

            return properties;
        }

        protected string CompileSettings(Dictionary<string, string> properties)
        {
            string settings = "";
            foreach (string property in properties.Keys)
                settings = String.Format("{0} {1}='{2}'", settings, property, properties[property]);

            return settings.Trim();
        }

        protected int? GetInt(Dictionary<string, string> properties, string property)
        {
            string value = null;
            int intValue;
            if (properties.TryGetValue(property, out value))
                if (int.TryParse(value, out intValue))
                    return intValue;

            return null;
        }

        protected double? GetDouble(Dictionary<string, string> properties, string property)
        {
            string value = null;
            double doubleValue;
            if (properties.TryGetValue(property, out value))
                if (double.TryParse(value, System.Globalization.NumberStyles.Float, new CultureInfo("en-US"), out doubleValue))
                    return doubleValue;

            return null;
        }

        protected Dictionary<string, string> Set(Dictionary<string, string> properties, string property, string value)
        {
            if (properties == null)
                properties = new Dictionary<string, string>();
            properties[property] = value;
            return properties;
        }

        protected Dictionary<string, string> Set(Dictionary<string, string> properties, string property, object value)
        {
            return Set(properties, property, value.ToString());
        }

        protected Dictionary<string, string> Set(Dictionary<string, string> properties, string property, double value)
        {
            return Set(properties, property, value.ToString(new CultureInfo("en-US")));
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
