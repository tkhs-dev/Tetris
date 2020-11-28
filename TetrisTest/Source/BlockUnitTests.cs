using NUnit.Framework;
using System;
using TetrisCore.Source.Extension;
using static TetrisCore.Source.BlockUnit;

namespace TetrisCore.Source.Tests
{
    [TestFixture]
    public class BlockUnitTests
    {
        [TestCase(Kind.I, new int[] { 1, 4, 1, 4 })]
        [TestCase(Kind.J, new int[] { 2, 3, 2, 3 })]
        [TestCase(Kind.L, new int[] { 2, 3, 2, 3 })]
        [TestCase(Kind.O, new int[] { 2, 2, 2, 2 })]
        [TestCase(Kind.S, new int[] { 2, 3, 2, 3 })]
        [TestCase(Kind.T, new int[] { 2, 3, 2, 3 })]
        [TestCase(Kind.Z, new int[] { 2, 3, 2, 3 })]
        public void GetWidthTest(Kind kind, int[] expectedResults)
        {
            BlockUnit unit = kind.GetObject();
            foreach (Directions dir in Enum.GetValues(typeof(Directions)))
            {
                int result = unit.GetWidth(dir);
                Assert.AreEqual(expectedResults[(int)dir], result, $"Kind:{kind},Direction:{dir}");
            }
        }

        [TestCase(Kind.I, new int[] { 1, 4, 1, 4 })]
        [TestCase(Kind.J, new int[] { 2, 3, 2, 3 })]
        [TestCase(Kind.L, new int[] { 2, 3, 2, 3 })]
        [TestCase(Kind.O, new int[] { 2, 2, 2, 2 })]
        [TestCase(Kind.S, new int[] { 2, 3, 2, 3 })]
        [TestCase(Kind.T, new int[] { 2, 3, 2, 3 })]
        [TestCase(Kind.Z, new int[] { 2, 3, 2, 3 })]
        public void GetHeightTest(Kind kind, int[] expectedResults)
        {
            BlockUnit unit = kind.GetObject();
            foreach (Directions dir in Enum.GetValues(typeof(Directions)))
            {
                int result = unit.GetWidth(dir);
                Assert.AreEqual(expectedResults[(int)dir], result, $"Kind:{kind},Direction:{dir},{unit}");
            }
        }

        [TestCase(Kind.I, new int[] { 1, 0, 2, 0 })]
        [TestCase(Kind.J, new int[] { 0, 0, 1, 0 })]
        [TestCase(Kind.L, new int[] { 0, 0, 1, 0 })]
        [TestCase(Kind.O, new int[] { 0, 0, 0, 0 })]
        [TestCase(Kind.S, new int[] { 0, 0, 1, 0 })]
        [TestCase(Kind.T, new int[] { 0, 0, 1, 0 })]
        [TestCase(Kind.Z, new int[] { 0, 0, 1, 0 })]
        public void GetXGapTest(Kind kind, int[] expectedResults)
        {
            BlockUnit unit = kind.GetObject();
            foreach (Directions dir in Enum.GetValues(typeof(Directions)))
            {
                int result = unit.GetXGap(dir);
                Assert.AreEqual(expectedResults[(int)dir], result, $"Kind:{kind},Direction:{dir},{unit}");
            }
        }

        [TestCase(Kind.I, new int[] { 0, 2, 0, 1 })]
        [TestCase(Kind.J, new int[] { 0, 1, 0, 0 })]
        [TestCase(Kind.L, new int[] { 0, 1, 0, 0 })]
        [TestCase(Kind.O, new int[] { 0, 0, 0, 0 })]
        [TestCase(Kind.S, new int[] { 0, 1, 0, 0 })]
        [TestCase(Kind.T, new int[] { 0, 1, 0, 0 })]
        [TestCase(Kind.Z, new int[] { 0, 1, 0, 0 })]
        public void GetYGapTest(Kind kind, int[] expectedResults)
        {
            BlockUnit unit = kind.GetObject();
            foreach (Directions dir in Enum.GetValues(typeof(Directions)))
            {
                int result = unit.GetYGap(dir);
                Assert.AreEqual(expectedResults[(int)dir], result, $"Kind:{kind},Direction:{dir},{unit}");
            }
        }

        public void GetBlocksTest()
        {
            Assert.Fail();
        }

        public void ToStringTest()
        {
            Assert.Fail();
        }
    }
}