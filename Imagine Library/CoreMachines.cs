using System.Drawing.Imaging;
using System.Drawing;
namespace Imagine.Library.Machines.Core
{
    [UniqueName("Imagine.Source")]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Provides a source image from file.")]
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

        public override string Caption
        {
            get { return "Source"; }
        }

        public ImagineImage Load()
        {
            return Load(null);
        }

        public ImagineImage Load(ProgressCallback callback)
        {
            ImagineImage image = null;
            if (filename != null)
            {
                Bitmap bitmap = (Bitmap)Image.FromFile(filename, false);
                if (!preview)
                    image = new FullImage(bitmap, callback);
                else
                {
                    image = FullImage.CreatePreview(bitmap);
                    lastPreviewImage = image;
                }
                bitmap.Dispose();
            }

            return image;
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            return new ImagineImage[] { Load(callback) };
        }
    }


    [UniqueName("Imagine.Destination")]
    [InputNames("input")]
    [InputCodes(' ')]
    [Description("Writes the input image to a file.")]
    public class SinkMachine : Machine
    {
        private string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; OnMachineChanged(); }
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

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            if (!preview)
            {
                if (filename != null)
                {
                    Bitmap bitmap = inputs[0].GetBitmap(callback);
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
            foreach (ImageCodecInfo codec in codecs)
                foreach (string ext in codec.FilenameExtension.Split(';'))
                    if (ext.ToLower().Equals("*.png"))
                        return codec;

            return null;
        }
    }

    [UniqueName("Imagine.Branch4")]
    [InputNames("input")]
    [InputCodes(' ')]
    [OutputNames("output1", "output2", "output3", "output4")]
    [OutputCodes('1', '2', '3', '4')]
    [Description("Outputs up to four identical copies of the input image.")]
    public class Branch4Machine : Machine
    {
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
}