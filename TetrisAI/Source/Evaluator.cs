using KelpNet;
using KelpNet.CL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TetrisCore;
using TetrisCore.Source;
using TetrisCore.Source.Config;
using TetrisCore.Source.Extension;

using Real = System.Single;

namespace TetrisAI.Source
{
    public class Evaluator
    {
        public const int NumInput = 9;
        public const int NumMiddle = 5;
        public const int NumOutput = 1;

        private EvaluationNNParameter _parameter;

        public Evaluator(EvaluationNNParameter parameter)
        {
            _parameter = parameter;
        }

        public Task<EvaluationResult> EvaluateAsync(EvaluationItem item)
        {
            return Task.Run(() => { return Evaluate(item); });
        }

        public EvaluationResult Evaluate(EvaluationItem item)
        {
            FunctionStack<Real> nn = new FunctionStack<Real>(
                new Linear<Real>(NumInput, NumMiddle, false, _parameter.MiddleLayerWeight),
                new Linear<Real>(NumMiddle, NumOutput, false, _parameter.OutputLayerWeight)
                );
            NdArray<Real> result = nn.Predict(item.GetReal())[0];
            return new EvaluationResult(result.Data[0]);
        }

        public class EvaluationNNParameter : SerializableBase
        {
            public float[] MiddleLayerWeight { get; set; }
            public float[] OutputLayerWeight { get; set; }

            public EvaluationNNParameter() : this(new float[0], new float[0])
            {
            }

            public EvaluationNNParameter( float[] mw, float[] ow)
            {
                if ((mw != null && ow != null) && (mw.Length >= NumInput * NumMiddle && ow.Length >= NumMiddle * NumOutput))
                {
                    MiddleLayerWeight = mw;
                    OutputLayerWeight = ow;
                }
                else
                {
                    MiddleLayerWeight = new float[NumInput * NumMiddle];
                    OutputLayerWeight = new float[NumMiddle * NumOutput];
                    CreateParameter();
                }
            }

            public static EvaluationNNParameter CreateNew()
            {
                return new EvaluationNNParameter();
            }

            public static XmlSerializer Serialize()
            {
                return new XmlSerializer(typeof(EvaluationNNParameter));
            }

            private void CreateParameter()
            {
                Random rnd = new Random();
                float MIN_VALUE = -1f;
                float MAX_VALUE = 1f;

                for (int i = 0; i < NumInput * NumMiddle; i++)
                {
                    MiddleLayerWeight[i] = (float)rnd.NextDouble() * (MAX_VALUE - MIN_VALUE) + MIN_VALUE;
                }
                for (int i = 0; i < NumMiddle * NumOutput; i++)
                {
                    OutputLayerWeight[i] = (float)rnd.NextDouble() * (MAX_VALUE - MIN_VALUE) + MIN_VALUE;
                }
            }
        }

        public class EvaluationItem
        {
            public EvaluationItem(int objectHeight, int numHole, int holeDepth, int numDeadSpace, int cumulativeWells, int erodedPieceCells, int numRowWithHole, int numRowTransition, int numColTransition)
            {
                ObjectHeight = objectHeight;
                NumHole = numHole;
                HoleDepth = holeDepth;
                NumDeadSpace = numDeadSpace;
                CumulativeWells = cumulativeWells;
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
            /// デッドスペースの数
            /// </summary>
            public readonly int NumDeadSpace;

            /// <summary>
            /// 井戸の高さの階乗の和
            /// </summary>
            public readonly int CumulativeWells;

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

            public override string ToString()
            {
                return $"ObjectHeight:{ObjectHeight},Holes:{NumHole},HoleDepth:{HoleDepth},DeadSpace:{NumDeadSpace},CumulativeWells:{CumulativeWells},ErodedPieceOfCells:{ErodedPieceCells},RowsWithHoles:{NumRowWithHole},RowTransition:{NumRowTransition},ColTransition:{NumColTransition}";
            }

            public static EvaluationItem GetEvaluationItem(RoundResult result)
            {
                Field field = result.FieldAtEnd;
                List<Point> holes = field.GetHoles();
                int holeDepth = holes.Count == 0 ? 0 : holes.Select(x => x.X)
                    .Distinct()
                    .Select(x => holes.Where(y => y.X == x).OrderBy(y => y.Y).First())
                    .Select(x =>
                    {
                        int r = 0;
                        int[] col = field.ToArrays().Rows(x.X).ToArray();
                        for (int i = x.Y; i >= 0; i--) if (col[i] != 0) r++;
                        return r;
                    }).Aggregate((x, y) => x + y);
                int numRowWithHoles = Enumerable.Range(0, field.Row - 1)
                    .Select(x => holes.Where(y => y.X == x)
                    .ToList().Count != 0)
                    .Where(x => x)
                    .Count();
                int rowTransition = GetTotalElementChange(
                    field.ToArrays()
                .Flatten(SquareDirection.Column)
                .ToArray());
                int colTransition = GetTotalElementChange(
                    field.ToArrays()
                .Flatten(SquareDirection.Row)
                .ToArray());

                EvaluationItem ev = new EvaluationItem(result.Object.GetHeight(), holes.Count, holeDepth, field.GetDeadSpace().Count, field.GetWells().Select(x => Enumerable.Range(1, x.ToArray().Count()).Aggregate(1, (p, item) => p * item)).Aggregate((x, y) => x + y), result.ErodedObjectCells, numRowWithHoles, rowTransition, colTransition);
                return ev;
            }

            private static int GetTotalElementChange(int[] array)
            {
                int result = 0;
                for (int i = 0; i < array.Length - 1; i++)
                {
                    if (array[i] != array[i + 1]) result++;
                }
                return result;
            }

            internal Real[] GetReal()
            {
                return new Real[] { ObjectHeight, NumHole, HoleDepth, NumDeadSpace, CumulativeWells, ErodedPieceCells, NumRowWithHole, NumRowTransition, NumColTransition };
            }
        }

        public class EvaluationResult
        {
            public readonly float EvaluationValue;

            public EvaluationResult(float value)
            {
                EvaluationValue = value;
            }
        }
    }
}