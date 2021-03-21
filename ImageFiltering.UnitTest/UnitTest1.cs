using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ImageFiltering.ImageProcessing;
using System.Collections.Generic;

namespace ImageFiltering.UnitTest
{
    [TestClass]
    public class OctreeTest
    {
        [TestMethod]
        public void SmallTest()
        {
            OctreeColorQuantisation.Octree tree = new OctreeColorQuantisation.Octree(3);
            tree.AddColor(0, 0, 0);
            tree.AddColor(255, 0, 0);
            tree.AddColor(0, 255, 0);
            tree.AddColor(255, 255, 0);

            byte[] expectedAnswer = new byte[3] { 128, 128, 128 };

            byte[] bytes = new byte[3];

            bytes = tree.GetQuantisedColor(0, 0, 0);
            Assert.IsTrue(bytes[0] == expectedAnswer[0] && bytes[1] == expectedAnswer[1] && bytes[2] == expectedAnswer[2]);
            bytes = tree.GetQuantisedColor(255, 0, 0);
            Assert.IsTrue(bytes[0] == expectedAnswer[0] && bytes[1] == expectedAnswer[1] && bytes[2] == expectedAnswer[2]);
            bytes = tree.GetQuantisedColor(0, 255, 0);
            Assert.IsTrue(bytes[0] == expectedAnswer[0] && bytes[1] == expectedAnswer[1] && bytes[2] == expectedAnswer[2]);
            bytes = tree.GetQuantisedColor(0, 0, 255);
            Assert.IsTrue(bytes[0] == expectedAnswer[0] && bytes[1] == expectedAnswer[1] && bytes[2] == expectedAnswer[2]);

            var rand = new Random();
            for(int i = 0; i < 100; i++)
            {
                rand.NextBytes(bytes);
                bytes = tree.GetQuantisedColor(bytes[0], bytes[1], bytes[2]);
                Assert.IsTrue(bytes[0] == expectedAnswer[0] && bytes[1] == expectedAnswer[1] && bytes[2] == expectedAnswer[2]);
            }
        }

        [TestMethod]
        public void ComplexTest()
        {
            OctreeColorQuantisation.Octree tree = new OctreeColorQuantisation.Octree(2);
            tree.AddColor(64, 64, 64);
            tree.AddColor(64, 64, 64);

            tree.AddColor(192, 64, 64);
            tree.AddColor(192, 64, 64);

            tree.AddColor(194, 98, 72);

            byte[] expectedAnswer = new byte[3] { 192+32, 64+32, 64+32 };

            byte[] bytes = tree.GetQuantisedColor(196, 99, 72);

            Assert.IsTrue(bytes[0] == expectedAnswer[0] && bytes[1] == expectedAnswer[1] && bytes[2] == expectedAnswer[2]);

            tree.AddColor(66, 64, 64, 3);

            expectedAnswer = new byte[3] { 64+16, 64+16, 64+16 };

            bytes = tree.GetQuantisedColor(64, 64, 64);
            Assert.IsTrue(bytes[0] == expectedAnswer[0] && bytes[1] == expectedAnswer[1] && bytes[2] == expectedAnswer[2]);
            bytes = tree.GetQuantisedColor(64, 65, 67);
            Assert.IsTrue(bytes[0] == expectedAnswer[0] && bytes[1] == expectedAnswer[1] && bytes[2] == expectedAnswer[2]);
        }
    }
}
