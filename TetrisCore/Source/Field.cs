using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using TetrisCore.Source.Extension;
using TetrisCore.Source.Util;
using static TetrisCore.Source.BlockUnit;

namespace TetrisCore.Source
{
    public class Field : ICloneable
    {
        private int _row;
        public int Row => _row;

        private int _column;
        public int Column => _column;

        private Cell[,] _cells;
        public Cell[,] Cells => _cells;

        private BlockObject _object;
        public BlockObject Object => _object;

        public delegate void OnBlockChangedEvent(object sender, Point point);

        public event OnBlockChangedEvent OnBlockChanged;

        public delegate void OnBlockPlacedEvent(object sender, BlockObject obj);

        public event OnBlockPlacedEvent OnBlockPlaced;

        public delegate void OnLineRemoveEvent(object sender, int line);

        public event OnLineRemoveEvent OnLineRemove;

        public delegate void OnLinesRemovedEvent(object sender, int[] lines, int erodedObjectCells);

        public event OnLinesRemovedEvent OnLinesRemoved;

        public delegate void OnRoundEndEvent(object sender, RoundResult result);

        public event OnRoundEndEvent OnRoundEnd;

        public delegate void OnRoundStartEvent(object sender);

        public event OnRoundStartEvent OnRoundStart;

        public delegate void OnGameOverEvent(object sender);

        public event OnGameOverEvent OnGameOver;

        private Field fieldStart;

        private bool _isGameOvered;
        public bool IsGameOvered { get => _isGameOvered; set => _isGameOvered = value; }

        public Field(int row, int column)
        {
            this._column = column;
            this._row = row;

            _cells = new Cell[row, column];
            for (int d1 = 0; d1 < row; d1++)
            {
                for (int d2 = 0; d2 < column; d2++) _cells[d1, d2] = new Cell();
            }

            OnBlockPlaced += (object sender, BlockObject obj) =>
            {
                List<int> lines = FindFilledLines();
                int eroded = 0;
                foreach (int i in lines)
                {
                    RemoveLine(i);
                    eroded += obj.GetBlocks().Where(x => x.Point.Y == i).Count();
                }
                OnLinesRemoved?.Invoke(this, lines.ToArray(), eroded);
                OnRoundEnd?.Invoke(this, new RoundResult(fieldStart, this, obj, lines.ToArray(), eroded));
            };
            OnRoundStart += (object sender) =>
              {
                  fieldStart = (Field)Clone();
              };
            OnGameOver += (object sender) =>
            {
                _isGameOvered = true;
            };
        }

        //フィールド操作系関数
        public void SetObject(BlockUnit o,Directions direction=Directions.NORTH)
        {
            Point point = new Point(((int)(_row / 2)) - (int)(o.GetWidth(direction) / 2), 0);
            if(!CanMoveTo(new BlockObject(o), point))
            {
                OnGameOver?.Invoke(this);
            }
            _object = new BlockObject(o,direction) { Point=point  };
        }
        public void StartRound()
        {
            if(!_isGameOvered)OnRoundStart?.Invoke(this);
        }

        public bool Move(Directions direction)
        {
            Point point = Object.Point;
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
                        PlaceAt(Object.Position);
                        return true;
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
                Object.Point = point;
                return true;
            }
            return false;
        }

        public void PlaceImmediately()
        {
            PlaceAt(new BlockPosition(GetImmediatePlacementPoint(),_object.Direction));
        }

        public bool Rotate(int rotation)
        {
            if (CanRotate(rotation))
            {
                _object.Rotate(rotation);
                return true;
            }
            return false;
        }

        public bool PlaceAt(BlockPosition position)
        {
            if (_object == null) return false;
            BlockObject clone = (BlockObject)_object.Clone();
            clone.Point = position.Point;
            foreach (Block block in _object.Unit.GetBlocks(position))
            {
                GetCell(block.Point)?.SetBlock(block);
                OnBlockChanged?.Invoke(this, block.Point);
            }
            OnBlockPlaced?.Invoke(this, clone);

            return true;
        }

        public void RemoveLine(int line)
        {
            foreach (Cell c in _cells.Cols(line)) c.RemoveBlock();
            for (int i = line - 1; i >= 0; i--)
            {
                var from = _cells.Cols(i);
                var to = _cells.Cols(i + 1);
                for (int i2 = 0; i2 < Row; i2++)
                {
                    if (from[i2].HasBlock())
                    {
                        to[i2].SetBlock(from[i2].Block);
                        from[i2].RemoveBlock();
                        OnBlockChanged?.Invoke(this, new Point(i, i2));
                    }
                }
            }
            OnLineRemove?.Invoke(this, line);
        }

        //Can
        public bool CanRotate(int rotation)
        {
            BlockObject dist = (BlockObject)_object.Clone();
            dist.Rotate(rotation);
            return (_object.Point.X >= 0 && _object.Point.Y >= 0 && _object.Point.X + _object.Unit.GetWidth(_object.Direction.Rotate(rotation)) <= _row && _object.Point.Y + _object.Unit.GetHeight(_object.Direction.Rotate(rotation)) <= _column) && CanMoveTo(dist, _object.Point);
        }

        public bool CanMoveTo(Point point)
        {
            return CanMoveTo(_object, point);
        }

        public bool CanMoveTo(BlockObject obj, Point point)
        {
            if (obj == null) return false;
            foreach (Block block in obj.Unit.GetBlocks(new BlockPosition(point,obj.Direction)))
            {
                Cell cell = GetCell(block.Point);
                if (cell == null || cell.HasBlock()) return false;
            }
            return true;
        }

        //取得
        public Cell GetCell(Point point)
        {
            if (point.X < 0 || point.Y < 0) return null;
            if (point.X >= _cells.GetLength(0) || point.Y >= _cells.GetLength(1)) return null;
            return _cells[point.X, point.Y];
        }

        public Point GetImmediatePlacementPoint()
        {
            Point point = _object.Point;
            while (CanMoveTo(point)) point.Offset(0, 1);
            point.Offset(0, -1);
            return point;
        }

        public List<int> FindFilledLines()
        {
            return _cells.Cols().Select((cells, index) => new { Cells = cells, Index = index }).Where(x => x.Cells.All(c => c.HasBlock())).Select(x => x.Index).ToList();
        }

        public List<Point> GetHoles()
        {
            int[,] data = ToArrays();
            Point[] sp = data.Cols(0).Select((x, index) => new { X = x, Index = index }).Where((x) => x.X == 0).Select(x => new Point(x.Index, 0)).ToArray();
            foreach (Point p in sp)
            {
                List<Point> points = GetAdjacentEmptyCell(data, p);
                while (points.Count != 0)
                {
                    List<Point> pp = new List<Point>(); ;
                    foreach (Point point in points)
                    {
                        if (data[point.X, point.Y] != 0) continue;
                        pp.AddRange(GetAdjacentEmptyCell(data, point));
                        data[point.X, point.Y] = 2;
                    }
                    points = pp;
                }
                points = GetAdjacentEmptyCell(data, p);
            }
            List<Point> result = new List<Point>();
            foreach (var d in data.Cols().Select((x, index) => new { X = x.ToArray(), Index = index }))
            {
                int y = d.Index;
                foreach (var i in d.X.Select((x, index) => new { X = x, Index = index }))
                {
                    if (i.X == 0) result.Add(new Point(i.Index, y));
                }
            }
            return result;
        }

        public List<List<Point>> GetWells()
        {
            int[,] data = ToArrays();
            List<int[]> cols = data.Rows().Select(x => x.ToArray()).ToList();
            List<List<Point>> result = new List<List<Point>>();
            for (int index = 0; index < cols.Count; index++)
            {
                int[] col = cols.ToArray()[index];
                List<Point> well = new List<Point>();
                int y = GetSurfacePoint(col);
                for (int i = y; i >= 0; i--)
                {
                    int d = col[i];
                    if (d == 0)
                    {
                        int left = index - 1;
                        int right = index + 1;
                        if ((left < 0 && data[right, i] != 0) || (right >= cols.Count && data[left, i] != 0) || (!(left < 0) && !(right >= cols.Count) && data[left, i] != 0 && data[right, i] != 0))
                        {
                            well.Add(new Point(index, i));
                        }
                    }
                }
                result.Add(well);
            }
            return result;
        }

        public List<BlockPosition> GetPlaceablePositions(BlockUnit block)
        {
            List<BlockPosition> result = new List<BlockPosition>();
            foreach (Directions direction in Enum.GetValues(typeof(Directions)))
            {
                result.AddRange(GetPlaceablePosition(block, direction));
            }
            return result;
        }

        private List<BlockPosition> GetPlaceablePosition(BlockUnit block, Directions direction)
        {
            List<BlockPosition> result = new List<BlockPosition>();
            int width = block.GetWidth(direction);
            int height = block.GetHeight(direction);
            int x_gap = block.GetXGap(direction);
            int y_gap = block.GetYGap(direction);

            for (int i = -x_gap; i <= Row - width - x_gap; i++)
            {
                //オブジェクトの下のブロックの高さの合計
                int sum_height = 0;
                for (int i2 = 0; i2 < width; i2++)
                {
                    int[] col = ToArrays().Rows(i + x_gap + i2).ToArray();
                    int h = Column - (GetSurfacePoint(col) + 1);
                    sum_height += h;
                }
                int start_y = Column > sum_height + height ? Column - sum_height - height - 1 : 3;
                for (int y = start_y; start_y < Column; start_y++)
                {
                    Point start_point = new Point(i, start_y);
                    BlockObject obj = new BlockObject(block) { Direction = direction};
                    if (!CanMoveTo(obj, start_point)) break;
                }
                result.Add(new BlockPosition(new Point(i, start_y - 1),direction));
            }
            return result;
        }

        /// <summary>
        /// 表面のemptyブロックのY座標を返す
        /// </summary>
        /// <param name="col_data">列のデータ</param>
        /// <returns>y座標</returns>
        private static int GetSurfacePoint(int[] col_data)
        {
            for (int i = 0; i < col_data.Length; i++)
            {
                if (col_data[i] == 1) return i;
            }
            return col_data.Length - 1;
        }

        private static List<Point> GetAdjacentEmptyCell(int[,] data, Point p)
        {
            List<Point> result = new List<Point>();
            //上
            if (p.Y - 1 >= 0 && data[p.X, p.Y - 1] == 0) result.Add(new Point(p.X, p.Y - 1));
            //右
            if (data.GetLength(0) > p.X + 1 && data[p.X + 1, p.Y] == 0) result.Add(new Point(p.X + 1, p.Y));
            //下
            if (data.GetLength(1) > p.Y + 1 && data[p.X, p.Y + 1] == 0) result.Add(new Point(p.X, p.Y + 1));
            //左
            if (p.X - 1 >= 0 && data[p.X - 1, p.Y] == 0) result.Add(new Point(p.X - 1, p.Y));

            return result;
        }

        //変換
        public int[,] ToArrays()
        {
            return _cells.Rows().Select(x => x.Select(y => y.HasBlock() ? 1 : 0).ToArray()).ToArray().ToDimensionalArray();
        }

        public object Clone()
        {
            Cell[,] cells_new = new Cell[Row,Column];
            for(int d1 = 0; d1 < Row; d1++)
            {
                for(int d2 = 0; d2 < Column; d2++)
                {
                    cells_new[d1, d2] = (Cell)_cells[d1, d2].Clone();
                }
            }
            return new Field(_row, _column) { _cells = cells_new, _object = (BlockObject)this.Object.Clone()};
        }
        public override string ToString()
        {
            string result = "";
            foreach(var i in ToArrays().Flatten(SquareDirection.Column).ToArray().Chunk(Row).Select(x => x.ToArray()).ToList())
            {
                foreach (var ii in i) result = result + ii;
                result = result + "\n";
            }
            
            return result;
        }
    }
}