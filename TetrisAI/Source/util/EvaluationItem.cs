using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisAI.Source.util
{
    public class EvaluationItem
    {
        public EvaluationItem(int objectHeight,int numHole,int holeDepth,int erodedPieceCells,int numRowWithHole,int numRowTransition,int numColTransition)
        {
            ObjectHeight = objectHeight;
            NumHole = numHole;
            HoleDepth = holeDepth;
            ErodedPieceCells = erodedPieceCells;
            NumRowWithHole = numRowWithHole;
            NumRowTransition = numRowTransition;
            NumColTransition = numColTransition;
        }
        /// <summary>
        /// 設置したオブジェクトの高さ
        /// </summary>
        public readonly int ObjectHeight;

        /// <summary>
        /// 穴の数
        /// </summary>
        public readonly int NumHole;

        /// <summary>
        /// 穴の上のブロック数の和
        /// </summary>
        public readonly int HoleDepth;

        /// <summary>
        /// 消した行の数xオブジェクトの中の消えたブロックの数
        /// </summary>
        public readonly int ErodedPieceCells;

        /// <summary>
        /// 穴のある行の数
        /// </summary>
        public readonly int NumRowWithHole;

        /// <summary>
        /// 横方向のブロックの変化の合計
        /// </summary>
        public readonly int NumRowTransition;

        /// <summary>
        /// 縦方向のブロックの変化の合計
        /// </summary>
        public readonly int NumColTransition;

    }
}
