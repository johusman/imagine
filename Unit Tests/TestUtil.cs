using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using NUnit.Framework;

namespace Imagine.Library
{
    public class TestUtil
    {
        public static void AssertBitmapsAreEqual(Bitmap bitmap1, Bitmap bitmap2)
        {
            Assert.AreEqual(bitmap1.Width, bitmap2.Width, "Width");
            Assert.AreEqual(bitmap1.Height, bitmap2.Height, "Height");

            for(int x = 0; x < bitmap1.Width; x++)
                for(int y = 0; y < bitmap1.Height; y++)
                    Assert.AreEqual(bitmap1.GetPixel(x, y), bitmap2.GetPixel(x, y), "Pixel at (" + x + ", " + y + ")");
        }

        public static void AssertBitmapFilesAreEqual(string filename1, string filename2)
        {
            using(Bitmap bitmap1 = (Bitmap)Image.FromFile(filename1))
            using(Bitmap bitmap2 = (Bitmap)Image.FromFile(filename2))
                AssertBitmapsAreEqual(bitmap1, bitmap2);
        }
    }
}
