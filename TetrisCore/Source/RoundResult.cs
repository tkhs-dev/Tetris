using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using TetrisCore.Source;

namespace TetrisCore
{
    public class RoundResult
    {
        public RoundResult(Field start, Field end, BlockObject obj, int[] removedLines, int erodedObjectCells)
        {
            this.FieldAtStart = start;
            this.FieldAtEnd = end;
            this.Object = obj;
            this.RemovedLines = removedLines;
            this.ErodedObjectCells = erodedObjectCells;
        }
        public Field FieldAtStart { get; }
        public Field FieldAtEnd { get; }
        public BlockObject Object { get; }
        public BlockPosition Position => Object.Position;
        public int[] RemovedLines { get; }
        public int ErodedObjectCells { get; }
        public int Score { get => RemovedLines.Length switch
        {
            0=>0,
            1=>40,
            2=>100,
            3=>300,
            4=>1200,
            _=> (RemovedLines.Length ^ 2 )*100
        }; }
    }
}