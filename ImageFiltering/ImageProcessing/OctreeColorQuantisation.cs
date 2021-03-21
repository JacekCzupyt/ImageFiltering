using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing.Imaging;

namespace ImageFiltering.ImageProcessing
{
    
    public class OctreeColorQuantisation : ImageFilter
    {
        public uint KValue { get; set; }

        public OctreeColorQuantisation(uint KValue)
        {
            this.KValue = KValue;
        }

        public string FilterType { get { return "Color Quantisers"; } }

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
            // Build Octree
            Octree tree = new Octree(KValue);
            for (int counter = 0; counter < rgbValues.Length; counter += bytesPerPixel)
                tree.AddColor(rgbValues[counter], rgbValues[counter + 1], rgbValues[counter + 2]);

            // Quantize colors
            for (int counter = 0; counter < rgbValues.Length; counter += bytesPerPixel)
                tree.GetQuantisedColor(rgbValues[counter], rgbValues[counter + 1], rgbValues[counter + 2]).CopyTo(rgbValues, counter);
                
            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);
        }

        public override string ToString()
        {
            return $"Octree Color Quantiser K={KValue}";
        }

        /// <summary>
        /// Octree used for color quantisation
        /// </summary>
        internal class Octree
        {
            uint MaxLeafs, nLeafs = 0;
            OctreeNode root;
            List<OctreeNode>[] LeafsPerLayer = new List<OctreeNode>[9];

            public Octree(uint MaxLeafs/*Maximum leafs allowed in the octree*/)
            {
                this.MaxLeafs = MaxLeafs;
                for(int i = 0; i <= 8; i++)
                    LeafsPerLayer[i] = new List<OctreeNode>();

                root = new OctreeNode(this, null, 0, new BitArray(24));
            }

            /// <summary>
            /// Finds the leaf coresponding to the id, if none exists, it is created, then the tree is reduced if nessesary
            /// </summary>
            /// <returns>Corresponding leaf node</returns>
            OctreeNode GetCorespondingLeaf(BitArray id)
            {
                //get leaf node of the tree
                OctreeNode leaf = root.GetCorespondingLeaf(id);

                //reduce the tree
                int ind = 8;
                while (nLeafs > MaxLeafs)
                {
                    while (LeafsPerLayer[ind].Count == 0)
                        ind--;
                    //reduce the leaf with minimum pixel count value
                    LeafsPerLayer[ind].Min().Reduce();
                }

                //the leaf may have changed due to the reductions preformed. 
                //However this time we are guaranted that the coresponding leaf already exists, therfore no reductions will be nessesary
                return root.GetCorespondingLeaf(id);
            }

            /// <summary>
            /// Adds a new color to the octree
            /// </summary>
            /// <param name="r">Red pixel value</param>
            /// <param name="g">Green pixel value</param>
            /// <param name="b">Blue pixel value</param>
            /// <param name="count">Number of pixles to add of this color</param>
            public void AddColor(byte r /*Red pixel value*/, byte g/*Green pixel value*/, byte b/*Blue pixel value*/, int count = 1/*Number of pixles to add of this color*/)
            {
                //get leaf node of the tree corresponding to the rgb value
                OctreeNode leaf = GetCorespondingLeaf(new BitArray(new byte[3] { r, g, b }));

                //add a pixel to it
                leaf.PixelCount += count;
            }

            /// <summary>
            /// Returns the quantisation of the input color using the octree
            /// </summary>
            /// <param name="r">Red pixel value</param>
            /// <param name="g">Green pixel value</param>
            /// <param name="b">Blue pixel value</param>
            /// <returns>Byte array representing the returned colors</returns>
            public byte[] GetQuantisedColor(byte r /*Red pixel value*/, byte g/*Green pixel value*/, byte b/*Blue pixel value*/)
            {
                //get leaf node of the tree
                OctreeNode leaf = GetCorespondingLeaf(new BitArray(new byte[3] { r, g, b }));

                //change the node id to reflect the average color in the color range
                BitArray nodeId = new BitArray(leaf.NodeId);
                if (leaf.Layer != 8)
                {
                    nodeId[7-leaf.Layer] = true;
                    nodeId[15-leaf.Layer] = true;
                    nodeId[23-leaf.Layer] = true;
                }

                //copy and return the result
                byte[] res = new byte[3];
                nodeId.CopyTo(res, 0);
                return res;
            }

            /// <summary>
            /// A single node of the octree
            /// </summary>
            class OctreeNode : IComparable<OctreeNode>
            {
                Octree Tree;
                OctreeNode Parent;
                
                /// <summary>Number of pixels coresponding to this leaf, null if not leaf</summary>
                public int? PixelCount;
                public BitArray NodeId { get; private set; }
                public int Layer { get; private set; }
                OctreeNode[] Children = new OctreeNode[8];

                public OctreeNode(Octree Tree, OctreeNode Parent, int Layer, BitArray NodeId, int? PixelCount = null)
                {
                    this.Tree = Tree;
                    this.Parent = Parent;
                    this.Layer = Layer;
                    this.NodeId = NodeId;
                    this.PixelCount = PixelCount;


                    //ensure that any node on layer 8 is a leaf
                    if (Layer == 8 && PixelCount == null)
                    {
                        this.PixelCount = 0;
                        Tree.nLeafs++;
                        Tree.LeafsPerLayer[Layer].Add(this);
                    }

                }

                
                /// <summary>
                /// Recursivly searches for the leaf corresponding to the provided color, creates it if it doesn't exist
                /// </summary>
                /// <param name="id">BitArray representing the rgb values of the pixel</param>
                /// <returns>The leaf octree node coresponding to the provided color</returns>
                public OctreeNode GetCorespondingLeaf(BitArray id)
                {
                    //if this is a leaf, return this
                    if (PixelCount.HasValue)
                        return this;

                    //get index of child that should be entered
                    int index = 4 * (id[7-Layer] ? 1 : 0) + 2 * (id[15-Layer] ? 1 : 0) + (id[23-Layer] ? 1 : 0);

                    //if child does not exist, create it
                    if (Children[index] == null)
                    {
                        BitArray newNodeId = new BitArray(NodeId);
                        newNodeId[7-Layer] = id[7-Layer];
                        newNodeId[15-Layer] = id[15-Layer];
                        newNodeId[23-Layer] = id[23-Layer];
                        Children[index] = new OctreeNode(Tree, this, Layer + 1, newNodeId);
                    }

                    //search recursivly
                    return Children[index].GetCorespondingLeaf(id);
                }

                /// <summary>
                /// Reduces this node by calling the gather method of its parent
                /// </summary>
                public void Reduce()
                {
                    if (Parent == null)
                        throw new InvalidOperationException();
                    Parent.Gather();
                }

                /// <summary>
                /// Recursivly gathers the pixel counts of all its children and becomes a leaf
                /// </summary>
                private void Gather()
                {
                    //if is leaf, do nothing
                    if (PixelCount.HasValue)
                        return;

                    PixelCount = 0;

                    //for each child
                    for(int i=0;i<8;i++)
                    {
                        var Node = Children[i];
                        //if child exists
                        if (Node != null)
                        {
                            //recursivly gather it, collect its pixel count and remove if from children and LeafsPerLayer array
                            Node.Gather();
                            PixelCount += Node.PixelCount;
                            Tree.LeafsPerLayer[Node.Layer].Remove(Node);
                            Tree.nLeafs--;
                            Children[i] = null;
                        }
                    }

                    //mark this as leaf
                    Tree.nLeafs++;
                    Tree.LeafsPerLayer[Layer].Add(this);
                }

                //used for the min function when figuring out which leaf to reduce
                public int CompareTo(OctreeNode other)
                {
                    //elimitanes the double null scenario
                    if (PixelCount == other.PixelCount)
                        return 0;
                    //if pixel count is null, it's not a leaf, therefore should be heigher (technically shouldn't be comapred at all, but may as well extend this)
                    if (!PixelCount.HasValue)
                        return 1;

                    if (!other.PixelCount.HasValue)
                        return -1;
                    //if both have value, return their difference
                    return PixelCount.Value - other.PixelCount.Value;
                }
            }
        }

        
    }
}
