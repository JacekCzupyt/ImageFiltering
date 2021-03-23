using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFiltering.ImageProcessing
{
    public partial class RandomDitheringFilter : ImageFilter, ICloneable
    {
        public uint KValue { get; set; }
        public string FilterName { get; set; }

        public RandomDitheringFilter(uint KValue, string FilterName = null)
        {
            if (KValue < 2)
                throw new ArgumentException();
            this.KValue = KValue;
            this.FilterName = FilterName ?? $"Random Dithering Filter K={KValue}";
        }

        public void Apply(Bitmap bitmap)
        {
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

            int bytesPerPixel = bmpData.Stride / bmpData.Width;

            Random rand = new Random();
            byte[] randomByte = new byte[KValue];

            // Generate thresholds and check how many are smaller then the color value, apply the appropiate color
            for (int counter = 0; counter < rgbValues.Length; counter += bytesPerPixel)
            {
                rand.NextBytes(randomByte);
                for(int i = 0; i < 3; i++)
                {
                    rgbValues[counter + i] = (byte)(255*randomByte.Count((byte b) => (b < rgbValues[counter + i]))/KValue);
                }
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);
        }

        public override string ToString()
        {
            return FilterName;
        }

        public object Clone()
        {
            return new RandomDitheringFilter(KValue, FilterName);
        }

        public string FilterType { get { return "Dithering Filters"; } }
    }
}
