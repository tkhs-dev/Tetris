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
        private Directions _direction;
        public Directions Direction
        {
            get { return _direction; }
        }

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
            return Enumerable.Range(0, _data.GetLength(0))
                    .SelectMany(r => Enumerable.Range(0, _data.GetLength(1)).Select(c => new Point(r, c)))
                    .Where(x => _data[x.X, x.Y] != 0)
                    .Select(x => new Point(x.X + offset.X, x.Y + offset.Y))
                    .Select(x => new Block(_color, x))
                    .ToArray();
        }
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
