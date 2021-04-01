using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFiltering.ImageProcessing
{
    public partial class ConvolutionFilter : ImageFilter, ICloneable
    {
        public string FilterType { get { return "Convolution Filters"; } }

        public ConvolutionMatrix Matrix { get; set; }
        public string FilterName { get; set; }

        public ConvolutionFilter(ConvolutionMatrix Matrix, string FilterName)
        {
            this.Matrix = Matrix;
            this.FilterName = FilterName;
        }

        public Bitmap Apply(Bitmap bitmap)
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
            byte[] OriginalRgbValues = new byte[bytes];
            byte[] NewRgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, OriginalRgbValues, 0, bytes);

            int bytesPerPixel = bmpData.Stride / bmpData.Width;

            // Apply filter to byte
            for (int counter = 0; counter < OriginalRgbValues.Length; counter++)
            {
                int value = 0;
                int offset = counter % bytesPerPixel;
                int index = counter / bytesPerPixel;
                (int, int) pixelCoords = Unflatten(index, bmpData.Width);
                
                for(int x = 0; x <Matrix.Width; x++)
                {
                    for (int y = 0; y < Matrix.Height; y++)
                    {
                        (int, int) EvaluatedPixelCoords = GetBoundedPoint((
                            pixelCoords.Item1 + x - Matrix.AnchorX, pixelCoords.Item2 + y - Matrix.AnchorY), 
                            bmpData.Width, 
                            bmpData.Height);
                        int FlattenedPixelCoords = Flatten(EvaluatedPixelCoords, bmpData.Width) * bytesPerPixel + offset;
                        value += Matrix[x, y] * OriginalRgbValues[FlattenedPixelCoords];
                    }
                }
                NewRgbValues[counter] = (byte)Math.Max(0, Math.Min(255, Matrix.Offset + (int)Math.Round((float)value / Matrix.Divisor)));
            };

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(NewRgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);

            return bitmap;
        }

        public override string ToString()
        {
            return FilterName;
        }

        private int Flatten((int, int) coords, int Width)
        {
            return coords.Item1 + coords.Item2 * Width;
        }

        private (int, int) Unflatten(int ind, int Width)
        {
            return (ind % Width, ind / Width);
        }

        private (int, int) GetBoundedPoint((int, int) coords, int Width, int Height)
        {
            return (Math.Max(0, Math.Min(Width - 1, coords.Item1)), Math.Max(0, Math.Min(Height - 1, coords.Item2)));
        }

        public object Clone()
        {
            return new ConvolutionFilter((ConvolutionMatrix)Matrix.Clone(), FilterName);
        }
    }



    public class ConvolutionMatrix : ICloneable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int AnchorX { get; set; }//would have prefared a tuple, but they use fields, not properites, so I can't use them for bindings
        public int AnchorY { get; set; }
        public float Divisor { get; set; }
        public int Offset { get; set; }
        private int[,] Matrix;

        public ConvolutionMatrix(int Size)
        {
            if (Size <= 0)
                throw new ArgumentException();
            this.Width = Size;
            this.Height = Size;
            Matrix = new int[Size, Size];
            Divisor = 1;
            AnchorX = Size / 2;
            AnchorY = Size / 2;
        }

        public ConvolutionMatrix(int Width, int Height)
        {
            if (Width <= 0 || Height<=0)
                throw new ArgumentException();
            this.Width = Width;
            this.Height = Height;
            Matrix = new int[Width, Height];
            Divisor = 1;
            AnchorX = Width / 2; 
            AnchorY = Height / 2;
        }

        public ConvolutionMatrix(int[,] Matrix, float Divisor = 0, int Offset = 0, (int, int)? Anchor = null)
        {
            if (Matrix.GetLength(0) <= 0 || Matrix.GetLength(1) <= 0)
                throw new ArgumentException();
            this.Width = Matrix.GetLength(0);
            this.Height = Matrix.GetLength(1);
            this.Matrix = Matrix;
            this.Offset = Offset;
            if (Divisor == 0)
                ComputeDivisor();
            else
                this.Divisor = Divisor;
            if (Anchor.HasValue)
            {
                this.AnchorX = Anchor.Value.Item1;
                this.AnchorY = Anchor.Value.Item2;
            }
            else
            {
                this.AnchorX = Width / 2; 
                this.AnchorY = Height / 2;
            }
                
        }

        public int this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x >= Width || y >= Height)
                    return 0;//this should prefarably raise an exception, but I coldn't find a way to handle exeptions during databindings
                return Matrix[x, y];
            }
            set
            {
                if (x < 0 || y < 0 || x >= Width || y >= Height)
                    return;//same as above
                Matrix[x, y] = value;
            }
        }

        public void ComputeDivisor()
        {
            Divisor = 0;
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Divisor += Matrix[x, y];

            if (Divisor == 0)
                Divisor = 1;
        }

        public object Clone()
        {
            return new ConvolutionMatrix((int[,])Matrix.Clone(), Divisor, Offset, (AnchorX, AnchorY));
        }
    }
}
