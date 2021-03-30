using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFiltering.ImageProcessing
{
    public class YCbCrFilter: ImageFilter, ICloneable
    {
        public uint KValue { get; set; }
        public string FilterName { get; set; }

        public YCbCrFilter(uint KValue, string FilterName = null)
        {
            this.KValue = KValue;
            this.FilterName = FilterName ?? $"YcbCr Filter K={KValue}";
        }

        public string FilterType { get { return "YCbCr Filters"; } }

        public Bitmap Apply(Bitmap previousBitmap)
        {
            new OctreeColorQuantisation(KValue).Apply(previousBitmap);

            var bitmap = new Bitmap(previousBitmap.Width, previousBitmap.Height * 3);

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData =
                bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            //lock previousBitmap
            Rectangle rect2 = new Rectangle(0, 0, previousBitmap.Width, previousBitmap.Height);
            BitmapData bmpData2 =
                previousBitmap.LockBits(rect2, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                previousBitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr2 = bmpData2.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes2 = Math.Abs(bmpData2.Stride) * previousBitmap.Height;
            byte[] prevrgbValues = new byte[bytes2];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr2, prevrgbValues, 0, bytes2);

            int bytesPerPixel = bmpData.Stride / bmpData.Width;

            // Fill in the color channels
            for (int i = 0; i < prevrgbValues.Length; i+=bytesPerPixel)
            {
                float Y = 16 + (1 / 256.0f) * (65.738f * prevrgbValues[i] + 129.057f * prevrgbValues[i + 1] + 25.064f * prevrgbValues[i + 2]);
                float t = (Y - 16) / (float)(235 - 16);
                rgbValues[i] = (byte)(Math.Min(t * 256, 255));
                rgbValues[i + 1] = rgbValues[i];
                rgbValues[i + 2] = rgbValues[i];
            }
            for (int i = 0; i < prevrgbValues.Length; i += bytesPerPixel)
            {
                float Cb = 128 + (1 / 256.0f) * (-37.934f * prevrgbValues[i] - 74.494f * prevrgbValues[i + 1] + 112.439f * prevrgbValues[i + 2]);
                float t = (Cb - 16) / (float)(240 - 16);
                rgbValues[prevrgbValues.Length + i] = 127;
                rgbValues[prevrgbValues.Length + i + 2] = (byte)(Math.Min(t * 256, 255));
                rgbValues[prevrgbValues.Length + i + 1] = (byte)(255 - rgbValues[prevrgbValues.Length + i + 2]);
            }
            for (int i = 0; i < prevrgbValues.Length; i += bytesPerPixel)
            {
                float Y = 128 + (1 / 256.0f) * (112.439f * prevrgbValues[i] - 94.154f * prevrgbValues[i + 1] - 18.285f * prevrgbValues[i + 2]);
                float t = (Y - 16) / (float)(240 - 16);
                rgbValues[2 * prevrgbValues.Length + i] = (byte)(Math.Min(t * 256, 255));
                rgbValues[2 * prevrgbValues.Length + i + 1] = (byte)(255 - rgbValues[2 * prevrgbValues.Length + i]);
                rgbValues[2 * prevrgbValues.Length + i + 2] = 127;
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);

            previousBitmap.UnlockBits(bmpData2);

            return bitmap;
        }

        public override string ToString()
        {
            return FilterName;
        }

        public object Clone()
        {
            return new YCbCrFilter(KValue, FilterName);
        }
    }
}
