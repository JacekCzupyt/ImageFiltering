using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFiltering.ImageProcessing
{
    public interface ImageFilter
    {
        Bitmap Apply(Bitmap bitmap);
        string FilterType { get; }
    }
}
