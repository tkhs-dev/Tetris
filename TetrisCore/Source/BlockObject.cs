using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TetrisCore.Source.Extension;
using static TetrisCore.Source.BlockUnit;

namespace TetrisCore.Source
{
    public class BlockObject : ICloneable
    {
        private BlockUnit _unit;
        private Point _point;
        private Directions _direction;

        public Directions Direction { get => _direction; set => _direction = value; }
        public Point Point { get => _point; set => _point = value; }
        public BlockUnit Unit { get => _unit;}
        public BlockPosition Position { get => new BlockPosition(Point, Direction); }

        public BlockObject(BlockUnit unit,Directions direction = Directions.NORTH)
        {
            this._unit = unit;
        }
        public IReadOnlyList<Block> GetBlocks()
        {
            return _unit.GetBlocks(Position);
        }
        public int GetWidth()
        {
            return _unit.GetWidth(_direction);
        }
        public int GetHeight()
        {
            return _unit.GetHeight(_direction);
        }
        public int GetXGap()
        {
            return _unit.GetXGap(_direction);
        }
        public int GetYGap()
        {
            return _unit.GetYGap(_direction);
        }

        //操作系メソッド
        /// <summary>
        /// オブジェクトを回転させる
        /// </summary>
        /// <param name="rotate">回転する回数</param>
        public void Rotate(int rotate)
        {
            _direction = _direction.Rotate(rotate);
        }

        public object Clone()
        {
            return new BlockObject(_unit) { _point = this._point,_direction=this._direction };
        }
        public override string ToString()
        {
            int[,] _data = _unit.TransformedData[_direction];
            return String.Join("\n", _data.Flatten(SquareDirection.Column).ToArray().Chunk(_data.GetLength(0)).Select(x => String.Join("",x)).ToArray());
        }
    }
}
