using System;
using System.Drawing;
using TetrisCore.Source;

namespace TetrisCore
{
    public class RoundResult
    {
        public RoundResult(Field start, Field end, BlockObject obj, int[] removedLines, int erodedObjectCells)
        {
            ID = Guid.NewGuid();
            this.FieldAtStart = start;
            this.FieldAtEnd = end;
            this.Object = obj;
            this.RemovedLines = removedLines;
            this.ErodedObjectCells = erodedObjectCells;
        }
        public Guid ID { get; }
        public Field FieldAtStart { get; }
        public Field FieldAtEnd { get; }
        public BlockObject Object { get; }
        public int[] RemovedLines { get; }
        public int ErodedObjectCells { get; }
    }
}