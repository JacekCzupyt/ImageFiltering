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

        class Octree
        {
            uint MaxLeafs, nLeafs = 0;
            OctreeNode root;
            List<OctreeNode>[] LeafsPerLayer = new List<OctreeNode>[8];

            public Octree(uint MaxLeafs)
            {
                this.MaxLeafs = MaxLeafs;
                for(int i = 0; i < 8; i++)
                    LeafsPerLayer[i] = new List<OctreeNode>();

                root = new OctreeNode(this, null, 0, new BitArray(24));
            }

            ///Add a color of a pixel to the tree
            public void AddColor(byte r, byte g, byte b, int count = 1)
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

            public byte[] GetQuantisedColor(byte r, byte g, byte b)
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






            class OctreeNode
            {
                Octree Tree;
                OctreeNode Parent;
                //if pixel count is null, it means that the node is not a leaf
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

                
                public OctreeNode GetCorespondingLeaf(BitArray id)
                {
                    if (PixelCount.HasValue)
                        return this;
                    int index = 4 * (id[Layer] ? 1 : 0) + 2 * (id[8 + Layer] ? 1 : 0) + (id[16 + Layer] ? 1 : 0);
                    if (Children[index] == null)
                    {
                        BitArray newNodeId = NodeId;
                        newNodeId[Layer] = id[Layer];
                        newNodeId[8 + Layer] = id[8 + Layer];
                        newNodeId[16 + Layer] = id[16 + Layer];
                        Children[index] = new OctreeNode(Tree, this, Layer + 1, newNodeId);
                    }
                    return Children[index].GetCorespondingLeaf(id);
                }

                public void Reduce()
                {
                    if (Parent == null)
                        throw new InvalidOperationException();
                    Parent.Gather();
                }

                private void Gather()
                {
                    if (PixelCount.HasValue)
                        return;

                    PixelCount = 0;

                    for(int i=0;i<8;i++)
                    {
                        var Node = Children[i];
                        if (Node != null)
                        {
                            Node.Gather();
                            PixelCount += Node.PixelCount;
                            Tree.LeafsPerLayer[Node.Layer].Remove(Node);
                            Tree.nLeafs--;
                            Children[i] = null;
                        }
                    }

                    Tree.nLeafs++;
                    Tree.LeafsPerLayer[Layer].Add(this);
                }
            }
        }

        
    }
}
