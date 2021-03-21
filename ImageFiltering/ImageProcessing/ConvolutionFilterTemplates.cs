using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFiltering.ImageProcessing
{
    public partial class ConvolutionFilter : ImageFilter
    {
        public static ConvolutionFilter Blur(int Size)
        {
            if (Size < 0 || Size % 2 == 0)
                throw new ArgumentException("Invalid blur matrix size");
            ConvolutionMatrix m = new ConvolutionMatrix(Size);
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    m[x, y] = 1;

            m.ComputeDivisor();
            return new ConvolutionFilter(m, $"Blur {Size}px");
        }
        public static ConvolutionFilter GaussSmooth3 { get
            {
                return new ConvolutionFilter(new ConvolutionMatrix(new int[3, 3]
                {
                    {0, 1, 0 },
                    {1, 4, 1 },
                    {0, 1, 0 }
                }), "Gaussian Smoothing 3px");
            } }

        public static ConvolutionFilter GaussSmooth5
        {
            get
            {
                return new ConvolutionFilter(new ConvolutionMatrix(new int[5, 5]
                {
                    {0, 1, 2, 1, 0 },
                    {1, 4, 8, 4, 1 },
                    {2, 8, 16, 8, 2 },
                    {1, 4, 8, 4, 1 },
                    {0, 1, 2, 1, 0 }
                }), "Gaussian Smoothing 5px");
            }
        }

        public static ConvolutionFilter GaussSmooth7
        {
            get
            {
                return new ConvolutionFilter(new ConvolutionMatrix(new int[7, 7]
                {
                    {0, 0, 1, 2, 1, 0, 0 },
                    {0, 3, 13, 22, 13, 3, 0 },
                    {1, 13, 59, 97, 59, 13, 1 },
                    {2, 22, 97, 159, 97, 22, 2 },
                    {1, 13, 59, 97, 59, 13, 1 },
                    {0, 3, 13, 22, 13, 3, 0 },
                    {0, 0, 1, 2, 1, 0, 0 }
                }), "Gaussian Smoothing 7px");
            }
        }

        public static ConvolutionFilter HighPassSharpen3(int divisor = 1)
        {
            return new ConvolutionFilter(new ConvolutionMatrix(new int[3, 3]
               {
                    {0, -1, 0 },
                    {-1, divisor+4, -1 },
                    {0, -1, 0 }
               }), "High-pass Sharpen 3px");
        }

        public static ConvolutionFilter MeanRemovalSharpen3(int divisor = 1)
        {
            return new ConvolutionFilter(new ConvolutionMatrix(new int[3, 3]
               {
                    {-1, -1, -1 },
                    {-1, divisor+8, -1 },
                    {-1, -1, -1 }
               }), "Mean Removal Sharpen 3px");
        }

        //note that the matricies below are effectivly displayed transposed
        public static ConvolutionFilter SmallHorizontalEdgeDetection(float sensitivity = 1, int offset = 0)
        {
            return new ConvolutionFilter(new ConvolutionMatrix(new int[2, 1]
               {
                    {-1},
                    {1 }
               }, 1/sensitivity, offset, Anchor: (1, 0)), "Small Horizontal Edge Detection");
        }

        public static ConvolutionFilter SmallVerticalEdgeDetection(float sensitivity = 1, int offset = 0)
        {
            return new ConvolutionFilter(new ConvolutionMatrix(new int[1, 2]
               {
                    {-1, 1}
               }, 1 / sensitivity, offset, Anchor: (0, 1)), "Small Vertical Edge Detection");
        }

        public static ConvolutionFilter LargeHorizontalEdgeDetection(float sensitivity = 1, int offset = 0)
        {
            return new ConvolutionFilter(new ConvolutionMatrix(new int[3, 3]
               {
                    {-1, -1, -1 },
                    {0, 0, 0 },
                    {1, 1, 1 }
               }, 1 / sensitivity, offset), "Large Horizontal Edge Detection");
        }

        public static ConvolutionFilter LargeVerticalEdgeDetection(float sensitivity = 1, int offset = 0)
        {
            return new ConvolutionFilter(new ConvolutionMatrix(new int[3, 3]
               {
                    {-1, 0, 1 },
                    {-1, 0, 1 },
                    {-1, 0, 1 }
               }, 1 / sensitivity, offset), "Large Vertical Edge Detection");
        }

        public static ConvolutionFilter SmallEdgeDetection(float sensitivity = 1)
        {
            return new ConvolutionFilter(new ConvolutionMatrix(new int[3, 3]
               {
                    {0, -1, 0 },
                    {-1, 4, -1 },
                    {0, -1, 0 }
               }, 1 / sensitivity), "Small Edge Detection");
        }

        public static ConvolutionFilter LargeEdgeDetection(float sensitivity = 1)
        {
            return new ConvolutionFilter(new ConvolutionMatrix(new int[3, 3]
               {
                    {-1, -1, -1 },
                    {-1, 8, -1 },
                    {-1, -1, -1 }
               }, 1 / sensitivity), "Large Edge Detection");
        }

        public static ConvolutionFilter EmbossFilterRight//or I guess east emboss? Why east? It's a matrix, not a map
        {
            get
            {
                {
                    return new ConvolutionFilter(new ConvolutionMatrix(new int[3, 3]
                       {
                            {-1, -1, -1 },
                            {0, 1, 0 },
                            {1, 1, 1 }
                       }), "Emboss Right");
                }
            }
        }

        public static ConvolutionFilter EmbossFilterLeft
        {
            get
            {
                {
                    return new ConvolutionFilter(new ConvolutionMatrix(new int[3, 3]
                       {
                            {1, 1, 1 },
                            {0, 1, 0 },
                            {-1, -1, -1 }
                       }), "Emboss Left");
                }
            }
        }

        public static ConvolutionFilter EmbossFilterUp
        {
            get
            {
                {
                    return new ConvolutionFilter(new ConvolutionMatrix(new int[3, 3]
                       {
                            {1, 0, -1 },
                            {1, 1, -1 },
                            {1, 0, -1 }
                       }), "Emboss Up");
                }
            }
        }

        public static ConvolutionFilter EmbossFilterDown
        {
            get
            {
                {
                    return new ConvolutionFilter(new ConvolutionMatrix(new int[3, 3]
                       {
                            {-1, 0, 1 },
                            {-1, 1, 1 },
                            {-1, 0, 1 }
                       }), "Emboss Down");
                }
            }
        }
    }
}
