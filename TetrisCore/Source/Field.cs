using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisCore.Source
{
    public class Field
    {
        //フィールド
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

        public Field(int row,int colmn)
        {
            _cells = new Cell[row,colmn];
        }
    }
}
