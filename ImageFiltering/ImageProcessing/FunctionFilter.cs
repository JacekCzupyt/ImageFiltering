using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFiltering.ImageProcessing
{
    public delegate byte FunctionFilterTemplate(byte input);
    public partial class FunctionFilter : ImageFilter
    {
        FunctionFilterTemplate TransformFunction;
        string FilterName;

        public FunctionFilter(FunctionFilterTemplate TransformFunction, string FilterName)
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

            // Apply transformation function
            for (int counter = 0; counter < rgbValues.Length; counter++)
                rgbValues[counter] = TransformFunction(rgbValues[counter]);

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);
        }

        public override string ToString()
        {
            return FilterName;
        }

        public string FilterType { get { return "Function Filters"; } }
    }
}
