using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisAI.Source.util
{
    public class EvaluationItem
    {
        /// <summary>
        /// 設置したオブジェクトの高さ
        /// </summary>
        public readonly int ObjectHeight;

        /// <summary>
        /// 穴の数
        /// </summary>
        public readonly int NumHole;

        /// <summary>
        /// 消した行の数
        /// </summary>
        public readonly int NumRemovedLine;

        /// <summary>
        /// 穴のある行の数
        /// </summary>
        public readonly int NumRowWithHole;

        /// <summary>
        /// 横方向のブロックの変化の合計
        /// </summary>
        public readonly int NumRowTransition;

    }
}
