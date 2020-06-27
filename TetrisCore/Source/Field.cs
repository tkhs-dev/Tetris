using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisCore.Source
{
    public class Field
    {
        private int _row;
        public int Row { get => _row; }

        private int _column;
        public int Column { get => _column; }

        private Cell[,] _cells;
        public Cell[,] Cells
        {
            get { return _cells; }
        }

        private BlockObject _object;
        public BlockObject Object
        {
            get { return _object; }
        }

        public Field(int row,int column)
        {
            this._column = column;
            this._row = row;

            _cells = new Cell[row,column];
        }
    }
}
