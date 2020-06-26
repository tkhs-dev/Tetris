using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using static TetrisCore.Source.BlockObject;

namespace TetrisCore.Source.Extension
{
    public static class KindExt
    {
        //テンプレート
        //            switch (self)
        //    {
        //        case Kind.I: return ;
        //        case Kind.O: return ;
        //        case Kind.T: return ;
        //        case Kind.J: return ;
        //        case Kind.L: return ;
        //        case Kind.S: return ;
        //        case Kind.Z: return ;

        //    }
        //    throw new InvalidOperationException("Unknown Kind");
        //

        /// <summary>
        /// ブロックの色を取得
        /// </summary>
        /// <param name="self">オブジェクトの種類</param>
        /// <returns>色</returns>
        public static Color BlockColor(this Kind self)
        {
            switch (self)
            {
                case Kind.I: return Color.Aqua;
                case Kind.O: return Color.Yellow;
                case Kind.T: return Color.Purple;
                case Kind.J: return Color.Blue;
                case Kind.L: return Color.Orange;
                case Kind.S: return Color.Green;
                case Kind.Z: return Color.Red;
            }
            throw new InvalidOperationException("Unknown Kind");
        }
        public static int[,] Data(this Kind self)
        {
            switch (self)
            {
                case Kind.I: return 
                    new int[,]{
                        {0,0,0,0},
                        {1,2,1,1},
                        {0,0,0,0},
                        {0,0,0,0}
                    };
                case Kind.O: return
                    new int[,]{
                        {1,1},
                        {1,1}
                    };
                case Kind.T: return
                    new int[,]{
                        {0,1,0},
                        {1,1,1},
                        {0,0,0}
                    };
                case Kind.J: return
                    new int[,]{
                        {1,0,0},
                        {1,1,1},
                        {0,0,0}
                    };
                case Kind.L: return
                    new int[,]{
                        {0,0,1},
                        {1,1,1},
                        {0,0,0}
                    };
                case Kind.S: return
                    new int[,]{
                        {0,1,1},
                        {1,1,0},
                        {0,0,0}
                    };
                case Kind.Z: return
                    new int[,]{
                        {1,1,0},
                        {0,1,1},
                        {0,0,0}
                    };

            }
            throw new InvalidOperationException("Unknown Kind");
        }
        public static IReadOnlyList<Block> GetBlocks(this Kind self,Point offset)
        {
            int[,] data = self.Data();
            return Enumerable.Range(0, data.GetLength(0))
                    .SelectMany(r => Enumerable.Range(0, data.GetLength(1)).Select(c => new Point(r, c)))
                    .Where(x => data[x.X, x.Y] != 0)
                    .Select(x => new Point(x.X + offset.X, x.Y + offset.Y))
                    .Select(x => new Block(self.BlockColor(), x))
                    .ToArray();
        }
    }
}
