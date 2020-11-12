using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using TetrisCore.Source.Extension;

namespace TetrisCore.Source
{
    public class BlockUnit
    {
        /// <summary>
        /// デフォルトのブロック配置データ
        /// </summary>
        private int[,] _data;

        private Color _color;

        /// <summary>
        /// 色
        /// </summary>
        public Color Color
        {
            get { return _color; }
        }

        /// <summary>
        /// 回転済みデータのキャッシュ
        /// </summary>
        public ReadOnlyDictionary<Directions, int[,]> TransformedData { get; }

        private Dictionary<BlockPosition,IReadOnlyList<Block>> BlockCache { get;}

        public BlockUnit(Color color, int[,] data) : this(color, data, new ReadOnlyDictionary<Directions, int[,]>(Enum.GetValues(typeof(Directions)).Cast<Directions>().ToList()
                .ToDictionary(x => x, x => data.RotateClockwise((int)x))))
        { }

        private BlockUnit(Color color, int[,] data, ReadOnlyDictionary<Directions, int[,]> transformed)
        {
            this._color = color;
            this._data = data;

            TransformedData = transformed;
            BlockCache = new Dictionary<BlockPosition, IReadOnlyList<Block>>();
        }

        public int GetWidth(Directions direction)
        {
            int result = 0;
            for (int d1 = 0; d1 < TransformedData[direction].GetLength(0); d1++)
            {
                int count = 0;
                for (int d2 = 0; d2 < TransformedData[direction].GetLength(1); d2++)
                {
                    count += TransformedData[direction][d1, d2];
                }
                if (count != 0) result++;
            }
            return result;
        }

        public int GetHeight(Directions direction)
        {
            int result = 0;
            for (int d1 = 0; d1 < TransformedData[direction].GetLength(0); d1++)
            {
                int count = 0;
                for (int d2 = 0; d2 < TransformedData[direction].GetLength(1); d2++)
                {
                    count += TransformedData[direction][d2, d1];
                }
                if (count != 0) result++;
            }
            return result;
        }

        public int GetXGap(Directions direction)
        {
            int result = 0;
            for (int d1 = 0; d1 < TransformedData[direction].GetLength(0); d1++)
            {
                int count = 0;
                for (int d2 = 0; d2 < TransformedData[direction].GetLength(1); d2++)
                {
                    count += TransformedData[direction][d1, d2];
                }
                if (count == 0) result++;
                else break;
            }
            return result;
        }

        public int GetYGap(Directions direction)
        {
            int result = 0;
            for (int d1 = 0; d1 < TransformedData[direction].GetLength(0); d1++)
            {
                int count = 0;
                for (int d2 = 0; d2 < TransformedData[direction].GetLength(1); d2++)
                {
                    count += TransformedData[direction][d2, d1];
                }
                if (count == 0) result++;
                else break;
            }
            return result;
        }


        public IReadOnlyList<Block> GetBlocks(BlockPosition position)
        {
            if (BlockCache.ContainsKey(position))
            {
                return BlockCache[position];
            }
            var result = Enumerable.Range(0, TransformedData[position.Direction].GetLength(0))
                    .SelectMany(r => Enumerable.Range(0, _data.GetLength(1)).Select(c => new Point(r, c)))
                    .Where(x => TransformedData[position.Direction][x.X, x.Y] != 0)
                    .Select(x => new Point(x.X + position.Point.X, x.Y + position.Point.Y))
                    .Select(x => new Block(_color, x))
                    .ToArray();
            BlockCache.Add(position,result);
            return result;
        }
        public override string ToString()
        {
            return String.Join("\n", _data.Flatten(SquareDirection.Column).ToArray().Chunk(_data.GetLength(0)).Select(x => String.Join("", x)).ToArray());
        }

        //列挙子
        public enum Kind
        {
            I, O, T, J, L, S, Z
        }

        public enum Directions
        {
            NORTH,
            EAST,
            SOUTH,
            WEST
        }
    }
}