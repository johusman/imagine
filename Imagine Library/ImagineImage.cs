using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Imagine.Library
{
    public abstract class ImagineImage
    {
        protected int width, height;
        
        public abstract void SetPixel(int x, int y, ImagineColor color);
        public abstract void SetPixel(int x, int y, int a, int r, int g, int b);
        public abstract ImagineColor GetPixel(int x, int y);
        
        public abstract Bitmap GetBitmap(ProgressCallback callback);
        public Bitmap GetBitmap() { return GetBitmap(null); }

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
            if (x < width && y < height)
                image[x, y] = (a > ImagineColor.MAX) ? ImagineColor.MAX : (a < 0 ? 0 : a);
        }

        public override ImagineColor GetPixel(int x, int y)
        {
            if (x < width && y < height)
                return new ImagineColor(image[x, y], 0, 0, 0);
            else
                return new ImagineColor(0, 0, 0, 0);
        }

        public override ImagineImage Copy()
        {
            ControlImage copy = new ControlImage(width, height);
            copy.image = (int[,]) image.Clone();
            return copy;
        }

        public unsafe override Bitmap GetBitmap(ProgressCallback callback)
        {
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            BitmapData bmd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

            for (int y = 0; y < bitmap.Height; y++)
            {
                byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int idx = x << 2;
                    int a = image[x, y];
                    Color color = (new ImagineColor(ImagineColor.MAX, a, a, a)).Color;
                    row[idx] = color.B;
                    row[idx + 1] = color.G;
                    row[idx + 2] = color.R;
                    row[idx + 3] = color.A;
                }

                if (callback != null)
                    callback.Invoke(100 * y / bitmap.Height);
            }

            bitmap.UnlockBits(bmd);

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

        public FullImage(Bitmap bitmap) : this(bitmap, null) { }

        public unsafe FullImage(Bitmap bitmap, ProgressCallback callback) : this(bitmap.Width, bitmap.Height)
        {
            RowDecoder decoder = RowDecoder.GetDecoder(bitmap.PixelFormat);
            if (decoder == null)
            {
                //throw new Exception("Found unsupported pixel format: " + bitmap.PixelFormat);
                //FIXME: This one should throw some kind of warning since processing will be slow.
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                        image[x, y] = new ImagineColor(bitmap.GetPixel(x, y));
                    if (callback != null)
                        callback.Invoke(100 * y / bitmap.Height);
                }
            }
            else
            {
                BitmapData bmd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

                for (int y = 0; y < bitmap.Height; y++)
                {
                    byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                    for (int x = 0; x < bitmap.Width; x++)
                        image[x, y] = decoder.GetColor(row, x);

                    if (callback != null)
                        callback.Invoke(100 * y / bitmap.Height);
                }

                bitmap.UnlockBits(bmd);
            }

            bitmap.Dispose();

            if (callback != null)
                callback.Invoke(100);
        }

        public override void SetPixel(int x, int y, ImagineColor color)
        {
            if (x < width && y < height)
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
            if (x < width && y < height)
                return image[x, y];
            else
                return new ImagineColor(0, 0, 0, 0);
        }

        public override ImagineImage Copy()
        {
            FullImage copy = new FullImage(width, height);
            copy.image = (ImagineColor[,]) image.Clone();
            return copy;
        }

        public unsafe override Bitmap GetBitmap(ProgressCallback callback)
        {
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            BitmapData bmd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

            for (int y = 0; y < bitmap.Height; y++)
            {
                byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int idx = x << 2;
                    Color color = image[x, y].Color;
                    row[idx] = color.B;
                    row[idx + 1] = color.G;
                    row[idx + 2] = color.R;
                    row[idx + 3] = color.A;
                }

                if (callback != null)
                    callback.Invoke(100 * y / bitmap.Height);
            }

            bitmap.UnlockBits(bmd);

            return bitmap;
        }

        public static ImagineImage CreatePreview(Bitmap bitmap)
        {
            int width, height;
            if (bitmap.Height > bitmap.Width)
            {
                height = 100;
                width = bitmap.Width * 100 / bitmap.Height;
            }
            else
            {
                width = 100;
                height = bitmap.Height * 100 / bitmap.Width;
            }

            Bitmap thumbnail = (Bitmap) bitmap.GetThumbnailImage(width, height, new Image.GetThumbnailImageAbort(delegate() { return false; }), System.IntPtr.Zero);
            FullImage image = new FullImage(thumbnail);
            thumbnail.Dispose();
            return image;
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

        public static ImagineColor FromARGB255(int a, int r, int g, int b)
        {
            return new ImagineColor(a * 256, r * 256, g * 256, b * 256);
        }

        public static ImagineColor FromHSV(double h, double s, double v)
        {
            double r = 0, g = 0, b = 0;
            double temp1, temp2;

            if (v == 0)
                return new ImagineColor(MAX, 0, 0, 0);

            if (s == 0)
                return new ImagineColor(MAX, (int) (v*MAX), (int) (v*MAX), (int) (v*MAX));

            temp2 = ((v <= 0.5) ? v * (1.0 + s) : v + s - (v * s));
            temp1 = 2.0 * v - temp2;

            double[] t3 = new double[] { h + 1.0 / 3.0, h, h - 1.0 / 3.0 };
            double[] clr = new double[] { 0, 0, 0 };

            for (int i = 0; i < 3; i++)
            {
                if (t3[i] < 0)
                    t3[i] += 1.0;
                if (t3[i] > 1)
                    t3[i] -= 1.0;

                if (6.0 * t3[i] < 1.0)
                    clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
                else if (2.0 * t3[i] < 1.0)
                    clr[i] = temp2;
                else if (3.0 * t3[i] < 2.0)
                    clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);
                else
                    clr[i] = temp1;
            }

            r = clr[0];
            g = clr[1];
            b = clr[2];

            return new ImagineColor(MAX, (int)(r * MAX), (int)(g * MAX), (int)(b * MAX));
        }

        public static ImagineColor FromHSL(double h, double s, double l)
        {
            double var_1, var_2;
            double r, g, b;

            if (s == 0)
                return new ImagineColor(MAX, (int) (l*MAX), (int) (l*MAX), (int) (l*MAX));

            h = h / 360.0;
            
            if ( l < 0.5 )
                var_2 = l * ( 1.0 + s );
            else
                var_2 = ( l + s ) - ( s * l );

            var_1 = 2.0 * l - var_2;

            r = Hue_2_RGB( var_1, var_2, h + ( 1.0 / 3.0 ) );
            g = Hue_2_RGB( var_1, var_2, h );
            b = Hue_2_RGB( var_1, var_2, h - ( 1.0 / 3.0 ) );

            return new ImagineColor(MAX, (int)(r * MAX), (int)(g * MAX), (int)(b * MAX));
        }

        private static double Hue_2_RGB( double v1, double v2, double vH )
        {
           if (vH < 0.0) vH += 1.0;
           if (vH > 1.0) vH -= 1.0;
           if ((6.0 * vH) < 1) return (v1 + (v2 - v1) * 6.0 * vH);
           if ( 2.0 * vH  < 1) return ( v2 );
           if ( 3.0 * vH  < 2) return ( v1 + ( v2 - v1 ) * ( 2.0 / 3.0 - vH ) * 6.0 );
           return v1;
        }

        public Color Color
        {
            get
            {
                return Color.FromArgb(a >= 0 ? a/256 : 0, r >= 0 ? r/256 : 0, g >= 0 ? g/256 : 0, b >= 0 ? b/256 : 0);
            }
        }
    }

    abstract class RowDecoder
    {
        public unsafe abstract ImagineColor GetColor(byte* row, int x);
        public static RowDecoder GetDecoder(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Format32bppPArgb:
                    return new Format32bppPArgbRowDecoder();
                case PixelFormat.Format32bppArgb:
                    return new Format32bppArgbRowDecoder();
                case PixelFormat.Format24bppRgb:
                    return new Format24bppRgbRowDecoder();
                default:
                    return null;
            }
        }
    }

    class Format32bppPArgbRowDecoder : RowDecoder
    {
        public override unsafe ImagineColor GetColor(byte* row, int x)
        {
            int idx = x << 2;
            int alpha = row[idx + 3];
            return ImagineColor.FromARGB255(alpha, row[idx + 2] * 255 / alpha, row[idx + 1] * 255 / alpha, row[idx] * 255 / alpha);
        }
    }

    class Format32bppArgbRowDecoder : RowDecoder
    {
        public override unsafe ImagineColor GetColor(byte* row, int x)
        {
            int idx = x << 2;
            return ImagineColor.FromARGB255(row[idx + 3], row[idx + 2], row[idx + 1], row[idx]);
        }
    }

    class Format24bppRgbRowDecoder : RowDecoder
    {
        public override unsafe ImagineColor GetColor(byte* row, int x)
        {
            int idx = x * 3;
            return ImagineColor.FromARGB255(255, row[idx + 2], row[idx + 1], row[idx]);
        }
    }
}
