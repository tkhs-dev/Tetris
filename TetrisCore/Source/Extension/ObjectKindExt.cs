using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static TetrisCore.Source.BlockObject;

namespace TetrisCore.Source.Extension
{
    public static class KindExt
    {
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
    }
}
