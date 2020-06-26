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
        public enum Kind
        {
            I,O,T,J,L,S,Z
        }
    }
}
