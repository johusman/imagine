using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Imagine.Library
{
    public abstract class ImagineImage
    {
        protected int width, height;
        
        public abstract void SetPixel(int x, int y, ImagineColor color);
        public abstract void SetPixel(int x, int y, int a, int r, int g, int b);
        public abstract ImagineColor GetPixel(int x, int y);
        
        public abstract Bitmap GetBitmap();
        
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        public abstract ImagineImage Copy();
    }

    public class ControlImage : ImagineImage
    {
        private int[,] image;

        public ControlImage(int width, int height)
        {
            image = new int[width, height];
            this.width = width;
            this.height = height;
        }

        public override void SetPixel(int x, int y, ImagineColor color)
        {
            SetValue(x, y, color.A);
        }

        public override void SetPixel(int x, int y, int a, int r, int g, int b)
        {
            SetValue(x, y, a);
        }

        public void SetValue(int x, int y, int a)
        {
            image[x, y] = (a > ImagineColor.MAX) ? ImagineColor.MAX : a;
        }

        public override ImagineColor GetPixel(int x, int y)
        {
            return new ImagineColor(image[x, y], 0, 0, 0);
        }

        public override ImagineImage Copy()
        {
            ControlImage copy = new ControlImage(width, height);
            copy.image = (int[,]) image.Clone();
            return copy;
        }

        public override Bitmap GetBitmap()
        {
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for(int x = 0; x < width; x++)
                for(int y = 0; y < height; y++)
                {
                    int a = image[x,y];
                    bitmap.SetPixel(x, y, (new ImagineColor(ImagineColor.MAX, a, a, a)).Color);
                }
            return bitmap;
        }
    }

    public class FullImage : ImagineImage
    {
        private ImagineColor[,] image;

        public FullImage(int width, int height)
        {
            image = new ImagineColor[width, height];
            this.width = width;
            this.height = height;
        }

        public FullImage(Bitmap bitmap) : this(bitmap.Width, bitmap.Height)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    image[x, y] = new ImagineColor(bitmap.GetPixel(x, y));
        }

        public override void SetPixel(int x, int y, ImagineColor color)
        {
            image[x, y] = color;
        }

        public override void SetPixel(int x, int y, int a, int r, int g, int b)
        {
            SetPixel(x, y, new ImagineColor(Clip(a), Clip(r), Clip(g), Clip(b)));
        }

        private int Clip(int value)
        {
            return (value > ImagineColor.MAX) ? ImagineColor.MAX : value;
        }

        public override ImagineColor GetPixel(int x, int y)
        {
            return image[x, y];
        }

        public override ImagineImage Copy()
        {
            FullImage copy = new FullImage(width, height);
            copy.image = (ImagineColor[,]) image.Clone();
            return copy;
        }

        public override Bitmap GetBitmap()
        {
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for(int x = 0; x < width; x++)
                for(int y = 0; y < height; y++)
                    bitmap.SetPixel(x, y, image[x, y].Color);

            return bitmap;
        }
    }

    public struct ImagineColor
    {
        public static int MAX = 256 * 256 - 1;

        private int a, r, g, b;

        public int B
        {
            get { return b; }
            set { b = value; }
        }

        public int G
        {
            get { return g; }
            set { g = value; }
        }

        public int R
        {
            get { return r; }
            set { r = value; }
        }

        public int A
        {
            get { return a; }
            set { a = value; }
        }

        public ImagineColor(Color color)
        {
            a = color.A * 256;
            r = color.R * 256;
            g = color.G * 256;
            b = color.B * 256;
        }

        public ImagineColor(int a, int r, int g, int b)
        {
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Color Color
        {
            get
            {
                return Color.FromArgb(a/256, r/256, g/256, b/256);
            }
        }
    }
}
