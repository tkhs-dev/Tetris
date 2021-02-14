using System.Drawing;
using static TetrisCore.Source.BlockUnit;

namespace TetrisCore.Source
{
    public class BlockPosition
    {
        public readonly Point Point;
        public readonly Directions Direction;

        public BlockPosition(Point point, Directions direction)
        {
            Point = point;
            Direction = direction;
        }
        public override bool Equals(object obj)
        {
            BlockPosition pos = (BlockPosition)obj;
            return this.Point.Equals(pos) && this.Direction == pos.Direction;
        }
    }
}
