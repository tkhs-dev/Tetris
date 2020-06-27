using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;

using TetrisCore.Source.Extension;

namespace TetrisCore.Source
{
    public class BlockObject
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


        private Directions _direction;
        /// <summary>
        /// 向き
        /// </summary>
        public Directions Direction
        {
            get { return _direction; }
        }

        /// <summary>
        /// 回転済みデータのキャッシュ
        /// </summary>
        private ReadOnlyDictionary<Directions, int[,]> TransformedData;

        public BlockObject(Color color,int[,] data)
        {
            this._color = color;
            this._data = data;

            TransformedData = new ReadOnlyDictionary<Directions, int[,]>(Enum.GetValues(typeof(Directions)).Cast<Directions>().ToList()
                .ToDictionary(x => x,x => data.RotateClockwise((int)x)));
        }

        public IReadOnlyList<Block> GetBlocks(Point offset)
        {
            return GetBlocks(offset, _direction);
        }
        public IReadOnlyList<Block> GetBlocks(Point offset,Directions direction)
        {
            return Enumerable.Range(0, TransformedData[direction].GetLength(0))
                    .SelectMany(r => Enumerable.Range(0, _data.GetLength(1)).Select(c => new Point(r, c)))
                    .Where(x => TransformedData[direction][x.X, x.Y] != 0)
                    .Select(x => new Point(x.X + offset.X, x.Y + offset.Y))
                    .Select(x => new Block(_color, x))
                    .ToArray();
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

        //列挙子
        public enum Kind
        {
            I,O,T,J,L,S,Z
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
