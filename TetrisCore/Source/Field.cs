﻿using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using TetrisCore.Source.Extension;
using static TetrisCore.Source.BlockObject;

namespace TetrisCore.Source
{
    public class Field
    {
        private int _row;
        public int Row => _row;

        private int _column;
        public int Column => _column;

        private Cell[,] _cells;
        public Cell[,] Cells => _cells;

        private BlockObject _object;
        public BlockObject Object => _object;

        private Point _objectPoint;
        public Point ObjectPoint => _objectPoint;

        public delegate void OnBlockChangedEvent(object sender, Point point);
        public event OnBlockChangedEvent OnBlockChanged;
        public delegate void OnBlockPlacedEvent(object sender, BlockObject obj);
        public event OnBlockPlacedEvent OnBlockPlaced;
        public delegate void OnLineRemoveEvent(object sender, int line);
        public event OnLineRemoveEvent OnLineRemove;

        private List<int> _lastRemovedLines;
        public List<int> LastRemovedLines => _lastRemovedLines;

        public Field(int row, int column)
        {
            this._column = column;
            this._row = row;

            _cells = new Cell[row, column];
            for (int d1 = 0; d1 < row; d1++)
            {
                for (int d2 = 0; d2 < column; d2++) _cells[d1, d2] = new Cell();
            }
            OnBlockPlaced += (object sender,BlockObject obj)=>{
                List<int> lines = FindFilledLines();
                _lastRemovedLines = lines;
                foreach (int i in lines) RemoveLine(i);
            };
        }
        public void SetObject(BlockObject o)
        {
            _object = o;
            _objectPoint = new Point(((int)(_row / 2)) - (int)(_object.GetWidth()/2), 0);
        }
        public Cell GetCell(Point point)
        {
            if (point.X < 0 || point.Y < 0) return null;
            if (point.X >= _cells.GetLength(0) || point.Y >= _cells.GetLength(1)) return null;
            return _cells[point.X, point.Y];
        }
        public int[,] ToArrays()
        {
            return _cells.Cols().Select(x => x.Select(y => y.HasBlock() ? 1 : 0).ToArray()).ToArray().ToDimensionalArray();
        }
        public Point GetImmediatePlacementPoint()
        {
            Point point = _objectPoint;
            while (CanMoveTo(point)) point.Offset(0, 1);
            point.Offset(0, -1);
            return point;
        }
        internal bool Move(Directions direction)
        {
            Point point = _objectPoint;
            switch (direction)
            {
                case Directions.NORTH:
                    point.Offset(0, -1);
                    break;
                case Directions.EAST:
                    point.Offset(1, 0);
                    break;
                case Directions.SOUTH:
                    point.Offset(0, 1);
                    //ブロックがあった時
                    if (!CanMoveTo(point))
                    {
                        PlaceAt(_objectPoint);
                        OnBlockPlaced?.Invoke(this, Object);
                        return true;
                    }
                    break;
                case Directions.WEST:
                    point.Offset(-1, 0);
                    break;
            }
            return MoveTo(point);
        }
        internal bool MoveTo(Point point)
        {
            if (CanMoveTo(point))
            {
                _objectPoint = point;
                return true;
            }
            return false;
        }
        internal void PlaceImmediately()
        {
            PlaceAt(GetImmediatePlacementPoint());
            OnBlockPlaced?.Invoke(this,_object);
        }
        internal bool Rotate(int rotation)
        {
            if (CanRotate(rotation))
            {
                _object.Rotate(rotation);
                return true;
            }
            return false;
        }
        private bool PlaceAt(Point point)
        {
            if (_object == null) return false;
            foreach (Block block in _object.GetBlocks(point))
            {
                GetCell(block.Point)?.SetBlock(block);
                OnBlockChanged?.Invoke(this, block.Point);
            }
            _object = null;
            _objectPoint = new Point(2,0);

            return true;
        }
        public bool CanRotate(int rotation)
        {
            return _objectPoint.X >= 0 && _objectPoint.Y >= 0 && _objectPoint.X + _object.GetWidth(_object.Direction.Rotate(rotation)) <= _row && _objectPoint.Y + _object.GetHeight(_object.Direction.Rotate(rotation)) <= _column;
        }
        public bool CanMoveTo(Point point)
        {
            return CanMoveTo(_object,point);
        }
        public bool CanMoveTo(BlockObject obj,Point point)
        {
            if (obj == null) return false;
            foreach (Block block in obj.GetBlocks(point))
            {
                Cell cell = GetCell(block.Point);
                if (cell == null || cell.HasBlock()) return false;
            }
            return true;
        }
        public List<int> FindFilledLines()
        {
            return _cells.Cols().Select((cells,index)=> new { Cells = cells,Index = index}).Where(x => x.Cells.All(c => c.HasBlock())).Select(x=>x.Index).ToList();
        }
        public void RemoveLine(int line)
        {
            foreach (Cell c in _cells.Cols(line)) c.RemoveBlock();
            for(int i = line - 1; i >= 0; i--)
            {
                var from = _cells.Cols(i);
                var to = _cells.Cols(i + 1);
                for(int i2 = 0; i2 < Row; i2++)
                {
                    if (from[i2].HasBlock())
                    {
                        to[i2].SetBlock(from[i2].Block);
                        from[i2].RemoveBlock();
                        OnBlockChanged?.Invoke(this, new Point(i, i2));
                    }
                }
            }
            OnLineRemove?.Invoke(this,line);
        }
    }
}
