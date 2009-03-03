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

        protected abstract ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback);

        public event System.EventHandler MachineChanged;
        
        public Machine()
        {
            inputNames = HasAttribute<InputNamesAttribute>() ? GetAttribute<InputNamesAttribute>().Values : new string[0];
            outputNames = HasAttribute<OutputNamesAttribute>() ? GetAttribute<OutputNamesAttribute>().Values : new string[0];
            inputCodes = HasAttribute<InputCodesAttribute>() ? GetAttribute<InputCodesAttribute>().Values : new char[0];
            outputCodes = HasAttribute<OutputCodesAttribute>() ? GetAttribute<OutputCodesAttribute>().Values : new char[0];

            if (inputNames.Length != inputCodes.Length)
                throw new Exception("Number of input names does not match number of input codes for machine type " + ToString() + " (" + GetType().AssemblyQualifiedName + ")");
            if (outputNames.Length != outputCodes.Length)
                throw new Exception("Number of output names does not match number of output codes for machine type " + ToString() + " (" + GetType().AssemblyQualifiedName + ")");
        }
        
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

        public string Description
        {
            get { return HasAttribute<DescriptionAttribute>() ? GetAttribute<DescriptionAttribute>().Value : ""; }
        }

        public abstract string Caption
        {
            get;
        }

        public override string ToString()
        {
            return ((UniqueNameAttribute) GetType().GetCustomAttributes(typeof(UniqueNameAttribute), false)[0]).Value;
        }

        private bool HasAttribute<T>()
        {
            return GetType().GetCustomAttributes(typeof(T), true).Length > 0;
        }

        private T GetAttribute<T>()
        {
            return (T)GetType().GetCustomAttributes(typeof(T), true)[0];
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

        protected FullImage NewFull(ImagineImage[] images)
        {
            ImageSize max = GetMaxDimensions(images);
            if (max.height == 0 && max.width == 0)
                return null;
            return new FullImage(max.width, max.height);
        }

        protected ControlImage NewControl(ImagineImage[] images)
        {
            ImageSize max = GetMaxDimensions(images);
            if (max.height == 0 && max.width == 0)
                return null;
            return new ControlImage(max.width, max.height);
        }

        protected FullImage[] NewFullArray(ImagineImage[] images, int arraySize)
        {
            ImageSize max = GetMaxDimensions(images);
            FullImage[] array = new FullImage[arraySize];
            if (max.height == 0 && max.width == 0)
                return array;
            for (int i = 0; i < arraySize; i++)
                array[i] = new FullImage(max.width, max.height);
            return array;
        }

        protected ControlImage[] NewControlArray(ImagineImage[] images, int arraySize)
        {
            ImageSize max = GetMaxDimensions(images);
            ControlImage[] array = new ControlImage[arraySize];
            if (max.height == 0 && max.width == 0)
                return array;
            for (int i = 0; i < arraySize; i++)
                array[i] = new ControlImage(max.width, max.height);
            return array;
        }

        private static ImageSize GetMaxDimensions(ImagineImage[] images)
        {
            ImageSize max = new ImageSize();

            foreach (ImagineImage image in images)
                if (image is FullImage)
                {
                    max.width = image.Width > max.width ? image.Width : max.width;
                    max.height = image.Height > max.height ? image.Height : max.height;
                }

            if (max.width == 0 && max.height == 0)
                foreach (ImagineImage image in images)
                    if (image is ControlImage)
                    {
                        max.width = image.Width > max.width ? image.Width : max.width;
                        max.height = image.Height > max.height ? image.Height : max.height;
                    }
            return max;
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

    public struct ImageSize
    {
        public int width;
        public int height;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class UniqueNameAttribute : Attribute
    {
        private string value;
        public UniqueNameAttribute(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NamesAttribute : Attribute
    {
        private string[] values;
        public NamesAttribute(params string[] values)
        {
            this.values = values;
        }

        public string[] Values
        {
            get { return values; }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CodesAttribute : Attribute
    {
        private char[] values;
        public CodesAttribute(params char[] values)
        {
            this.values = values;
        }

        public char[] Values
        {
            get { return values; }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class InputNamesAttribute : NamesAttribute
    {
        public InputNamesAttribute(params string[] values) : base(values) {}
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class OutputNamesAttribute : NamesAttribute
    {
        public OutputNamesAttribute(params string[] values) : base(values) {}
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class InputCodesAttribute : CodesAttribute
    {
        public InputCodesAttribute(params char[] values) : base(values) {}
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class OutputCodesAttribute : CodesAttribute
    {
        public OutputCodesAttribute(params char[] values) : base(values) {}
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptionAttribute : Attribute
    {
        private string value;
        public DescriptionAttribute(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }
    }
}
