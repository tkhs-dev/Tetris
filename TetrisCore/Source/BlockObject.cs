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
        public BlockObject(int[,] data, Color color)
        {
            _data = data;
            _color = color;
        }
    }
}
