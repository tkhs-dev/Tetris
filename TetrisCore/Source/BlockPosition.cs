using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static TetrisCore.Source.BlockUnit;

namespace TetrisCore.Source
{
    public class BlockPosition
    {
        public readonly Point Point;
        public readonly Directions Direction;

        public BlockPosition(Point point,Directions direction)
        {
            Point = point;
            Direction = direction;
        }
    }
}
