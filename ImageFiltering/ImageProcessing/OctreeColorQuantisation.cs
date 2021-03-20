using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFiltering.ImageProcessing
{
    class OctreeColorQuantisation : ImageFilter
    {
        public string FilterType { get { return "Color Quantisers"; } }

        public void Apply(Bitmap bitmap)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Octree Color Quantiser";
        }

        class Octree
        {
            uint MaxLeafs, nLeafs = 0;
            OctreeNode root;
            SortedSet<OctreeNode> s = new SortedSet<OctreeNode>();

            public Octree(uint MaxLeafs)
            {
                this.MaxLeafs = MaxLeafs;
                
            }
        }

        class OctreeNode : IComparable<OctreeNode>
        {
            int PixelCount = 0;
            public int CompareTo(OctreeNode other)
            {
                return PixelCount - other.PixelCount;
            }
        }
    }
}
