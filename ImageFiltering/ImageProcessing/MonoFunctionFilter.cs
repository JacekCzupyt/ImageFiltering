using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFiltering.ImageProcessing
{
    public partial class MonoFunctionFilter : ImageFilter
    {
        FunctionFilterTemplate TransformFunction;
        string FilterName;

        public MonoFunctionFilter(FunctionFilterTemplate TransformFunction, string FilterName)
        {
            this.TransformFunction = TransformFunction;
            this.FilterName = FilterName;
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

            // Apply transformation function
            for (int counter = 0; counter < rgbValues.Length; counter += bytesPerPixel)
            {
                int inputValue = 0;
                for (int i = 0; i < 3; i++)
                    inputValue += rgbValues[counter+i];
                byte outputValue = TransformFunction((byte)(inputValue/(3)));
                for (int i = 0; i < 3; i++)
                    rgbValues[counter + i] = outputValue;
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

        public string FilterType { get { return "Monochrome Function Filters"; } }

        public static MonoFunctionFilter Identity
        {
            get
            {
                return new MonoFunctionFilter((byte input) => input, "Monochrome Identity");
            }
        }
    }
}
