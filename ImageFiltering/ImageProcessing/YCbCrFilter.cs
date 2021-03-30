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

            // Build Octree
            for(int i = 0; i < rgbValues.Length; i++)
            {
                rgbValues[i] = prevrgbValues[i % prevrgbValues.Length];
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
