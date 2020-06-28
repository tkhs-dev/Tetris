using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static TetrisCore.Source.BlockObject;

namespace TetrisCore.Source
{
    public class Field
    {
        private int _row;
        public int Row => _row;

        private int _column;
        public int Column => Column1;

        private Cell[,] _cells;
        public Cell[,] Cells => _cells;

        private BlockObject _object;
        public BlockObject Object => _object;

        public int Column1 { get => _column; set => _column = value; }

        private Point _objectPoint;
        public Point ObjectPoint => _objectPoint;

        public Field(int row,int column)
        {
            this.Column1 = column;
            this._row = row;

            _cells = new Cell[row,column];
        }
        public Cell GetCell(Point point)
        {
            return _cells[point.X, point.Y];
        }
        public bool Move(Directions direction)
        {
            Point point = _objectPoint;
            switch (direction)
            {
                case Directions.NORTH:
                    point.Offset(0,1);
                    break;
                case Directions.EAST:
                    point.Offset(1, 0);
                    break;
                case Directions.SOUTH:
                    point.Offset(0, -1);
                    //ブロックがあった時
                    if (GetCell(point).HasBlock())
                    {
                        PutAt(point);
                    }
                    break;
                case Directions.WEST:
                    point.Offset(-1, 0);
                    break;
            }
            return MoveTo(point);
        }
        public bool MoveTo(Point point)
        {
            if (CanMoveTo(point))
            {
                _objectPoint = point;
                return true;
            }
            return false;
        }
        public bool PutAt(Point point)
        {
            if (!CanMoveTo(point)) return false;
            foreach (Block block in _object.GetBlocks(point))
            {
                GetCell(block.Point).SetBlock(block);
            }
            _object = null;
            _objectPoint = Point.Empty;

            return true;
        }

        public bool CanMoveTo(Point point)
        {
            if (!(point.X < _row && point.Y < _column)) return false;
            foreach(Block block in _object.GetBlocks(point))
            {
                if (GetCell(block.Point).HasBlock()) return false;
            }
            return true;
        }
    }
}
