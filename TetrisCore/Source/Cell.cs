using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisCore.Source
{
    public class Cell
    {
        private Block _block;
        public Block Block
        {
            get { return _block; }
            set { this._block = value; }
        }
    }
}
