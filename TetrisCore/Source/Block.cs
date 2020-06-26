using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TetrisCore.Source
{
    public class Block
    {
        private Color _color;
        public Color Color {
            get { return _color; }
            set { this._color = value; }
        }
        private Point _point;
        public Point Point
        {
            get { return _point; }
            set { this._point = value; }
        }

        public Block(Color color,Point point)
        {
            _color = color;
            _point = point;
        }

    }
}
