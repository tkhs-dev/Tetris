using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TetrisCore.Source
{
    public class BlockObject
    {
        private int[,] _data;
        public int[,] Data
        {
            get { return _data; }
        }
        private Color _color;
        public Color Color
        {
            get { return _color; }
        }

        /// <param name="data">ObjectData 0:empty 1:block 2:block_point 3:empty_point</param>
        /// <param name="color">Object Color</param>
        public BlockObject(int[,] data, Color color)
        {
            _data = data;
            _color = color;
        }

        public static class Kind
        {
            public static BlockObject I =
                new BlockObject(
                    new int[,]{
                        {0,0,0,0},
                        {1,2,1,1},
                        {0,0,0,0},
                        {0,0,0,0}
                    },Color.Aqua);
            public static BlockObject T =
                new BlockObject(
                    new int[,]{
                        {0,1,0},
                        {1,1,1},
                        {0,0,0}
                    }, Color.Purple);
            public static BlockObject S =
                new BlockObject(
                    new int[,]{
                        {0,1,1},
                        {1,1,0},
                        {0,0,0}
                    }, Color.Green);
            public static BlockObject Z =
                new BlockObject(
                    new int[,]{
                        {1,1,0},
                        {0,1,1},
                        {0,0,0}
                    }, Color.Red);
            public static BlockObject L =
                new BlockObject(
                    new int[,]{
                        {0,0,1},
                        {1,1,1},
                        {0,0,0}
                    }, Color.Orange);
        }
        public static BlockObject J =
                new BlockObject(
                    new int[,]{
                        {1,0,0},
                        {1,1,1},
                        {0,0,0}
                    }, Color.Blue);
        public static BlockObject O =
                new BlockObject(
                    new int[,]{
                        {1,1},
                        {1,1}
                    }, Color.Orange);


    }
}
