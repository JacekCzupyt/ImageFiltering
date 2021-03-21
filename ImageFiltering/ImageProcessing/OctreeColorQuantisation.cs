using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ImageFiltering.ImageProcessing
{
    public class OctreeColorQuantisation : ImageFilter
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

        /// <summary>
        /// Octree used for color quantisation
        /// </summary>
        class Octree
        {
            uint MaxLeafs, nLeafs = 0;
            OctreeNode root;
            List<OctreeNode>[] LeafsPerLayer = new List<OctreeNode>[8];

            public Octree(uint MaxLeafs/*Maximum leafs allowed in the octree*/)
            {
                this.MaxLeafs = MaxLeafs;
                for(int i = 0; i < 8; i++)
                    LeafsPerLayer[i] = new List<OctreeNode>();

                root = new OctreeNode(this, null, 0, new BitArray(24));
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
                //get leaf node of the tree
                OctreeNode leaf = root.GetCorespondingLeaf(new BitArray(new byte[3] { r, g, b }));

                //add a pixel to it
                leaf.PixelCount += count;

                //reduce the tree
                int ind = 8;
                while(nLeafs > MaxLeafs)
                {
                    while (LeafsPerLayer[ind].Count == 0)
                        ind--;
                    LeafsPerLayer[ind].Last().Reduce();
                }
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
                OctreeNode leaf = root.GetCorespondingLeaf(new BitArray(new byte[3] { r, g, b }));

                //change the node id to reflect the average color in the color range
                BitArray nodeId = new BitArray(leaf.NodeId);
                if (leaf.Layer != 8)
                {
                    nodeId[leaf.Layer] = true;
                    nodeId[leaf.Layer+8] = true;
                    nodeId[leaf.Layer+16] = true;
                }

                //copy and return the result
                byte[] res = new byte[3];
                nodeId.CopyTo(res, 0);
                return res;
            }

            /// <summary>
            /// A single node of the octree
            /// </summary>
            class OctreeNode
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
                        PixelCount = 0;
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
                    int index = 4 * (id[Layer] ? 1 : 0) + 2 * (id[8 + Layer] ? 1 : 0) + (id[16 + Layer] ? 1 : 0);

                    //if child does not exist, create it
                    if (Children[index] == null)
                    {
                        BitArray newNodeId = NodeId;
                        newNodeId[Layer] = id[Layer];
                        newNodeId[8 + Layer] = id[8 + Layer];
                        newNodeId[16 + Layer] = id[16 + Layer];
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
            }
        }

        
    }
}
