using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TetrisCore.Source.Extension
{
    public static class ArrayExt
    {
        // 時計回りに 90 度回転
        public static T[,] RotateClockwise<T>(this T[,] self)
        {
            int rows = self.GetLength(0);
            int columns = self.GetLength(1);
            var result = new T[columns, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[j, rows - i - 1] = self[i, j];
                }
            }

            return result;
        }

        // 時計回りに 90 * count 度回転
        public static T[,] RotateClockwise<T>(this T[,] self, int count)
        {
            for (int i = 0; i < count; i++)
            {
                self = self.RotateClockwise();
            }

            return self;
        }

        // 反時計回りに 90 度回転
        public static T[,] RotateAnticlockwise<T>(this T[,] self)
        {
            int rows = self.GetLength(0);
            int columns = self.GetLength(1);
            var result = new T[columns, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[columns - j - 1, i] = self[i, j];
                }
            }

            return result;
        }

        // 反時計回りに 90 * count 度回転
        public static T[,] RotateAnticlockwise<T>(this T[,] self, int count)
        {
            for (int i = 0; i < count; i++)
            {
                self = self.RotateAnticlockwise();
            }

            return self;
        }
        /// <summary>
        /// 2次元配列向け列クラス
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class SquareArrayColumn<T> : IEnumerable<T>
        {
            /// <summary>
            /// 2次元配列
            /// </summary>
            private T[,] _array;
            /// <summary>
            /// 列インデックス
            /// </summary>
            private int _colIndex;

            /// <summary>
            /// インデクサ
            /// </summary>
            /// <param name="rowIndex">行インデックス</param>
            /// <returns>要素</returns>
            public T this[int rowIndex]
            {
                get => _array[rowIndex, _colIndex];
                set => _array[rowIndex, _colIndex] = value;
            }

            /// <summary>
            /// Length
            /// </summary>
            public int Length => _array.GetLength(0);

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="array">2次元配列</param>
            /// <param name="rowIndex">行インデックス</param>
            public SquareArrayColumn(T[,] array, int colIndex)
            {
                _array = array;
                _colIndex = colIndex;
            }

            /// <summary>
            /// GetEnumerator()の実装
            /// </summary>
            /// <returns>IEnumerator<T></returns>
            public IEnumerator<T> GetEnumerator()
            {
                for (var i = 0; i < Length; i++)
                    yield return this[i];
            }

            /// <summary>
            /// GetEnumerator()の実装
            /// </summary>
            /// <returns>IEnumerator</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// 2次元配列向け行クラス
        /// </summary>
        /// <typeparam name="T">型パラメータ</typeparam>
        public class SquareArrayRow<T> : IEnumerable<T>
        {
            /// <summary>
            /// 2次元配列
            /// </summary>
            private T[,] _array;
            /// <summary>
            /// 行インデックス
            /// </summary>
            private int _rowIndex;

            /// <summary>
            /// インデクサ
            /// </summary>
            /// <param name="colIndex">列インデックス</param>
            /// <returns>要素</returns>
            public T this[int colIndex]
            {
                get => _array[_rowIndex, colIndex];
                set => _array[_rowIndex, colIndex] = value;
            }

            /// <summary>
            /// Length
            /// </summary>
            public int Length => _array.GetLength(1);

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="array">2次元配列</param>
            /// <param name="rowIndex">行インデックス</param>
            public SquareArrayRow(T[,] array, int rowIndex)
            {
                _array = array;
                _rowIndex = rowIndex;
            }

            /// <summary>
            /// GetEnumerator()の実装
            /// </summary>
            /// <returns>IEnumerator<T></returns>
            public IEnumerator<T> GetEnumerator()
            {
                for (var i = 0; i < Length; i++)
                    yield return this[i];
            }

            /// <summary>
            /// GetEnumerator()の実装
            /// </summary>
            /// <returns>IEnumerator</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// 2次元配列から行を取得する
        /// </summary>
        /// <typeparam name="T">型パラメータ</typeparam>
        /// <param name="array">this 2次元配列</param>
        /// <param name="rowIndex">行インデックス</param>
        /// <returns>行オブジェクト</returns>
        public static SquareArrayRow<T> Rows<T>(this T[,] array, int rowIndex)
        {
            if (rowIndex < 0 || array.GetLength(0) <= rowIndex)
                throw new IndexOutOfRangeException();
            return new SquareArrayRow<T>(array, rowIndex);
        }

        /// <summary>
        /// 2次元配列から行を列挙する
        /// </summary>
        /// <typeparam name="T">型パラメータ</typeparam>
        /// <param name="array">this 2次元配列</param>
        /// <returns>行オブジェクトの列挙子</returns>
        public static IEnumerable<SquareArrayRow<T>> Rows<T>(this T[,] array)
        {
            for (var row = 0; row < array.GetLength(0); row++)
                yield return array.Rows(row);
        }

        /// <summary>
        /// 2次元配列から列を取得する
        /// </summary>
        /// <typeparam name="T">型パラメータ</typeparam>
        /// <param name="array">this 2次元配列</param>
        /// <param name="colIndex">列インデックス</param>
        /// <returns>行オブジェクト</returns>
        public static SquareArrayColumn<T> Cols<T>(this T[,] array, int colIndex)
        {
            if (colIndex < 0 || array.GetLength(1) <= colIndex)
                throw new IndexOutOfRangeException();
            return new SquareArrayColumn<T>(array, colIndex);
        }

        /// <summary>
        /// 2次元配列から列を列挙する
        /// </summary>
        /// <typeparam name="T">型パラメータ</typeparam>
        /// <param name="array">this 2次元配列</param>
        /// <returns>行オブジェクトの列挙子</returns>
        public static IEnumerable<SquareArrayColumn<T>> Cols<T>(this T[,] array)
        {
            for (var col = 0; col < array.GetLength(1); col++)
                yield return array.Cols(col);
        }

        /// <summary>
        /// 2次元配列をまとめて列挙する
        /// </summary>
        /// <typeparam name="T">型パラメータ</typeparam>
        /// <param name="array">2次元配列</param>
        /// <param name="direction">方向</param>
        /// <returns>列挙子</returns>
        public static IEnumerable<T> Flatten<T>(this T[,] array, SquareDirection direction = SquareDirection.Row)
        {
            IEnumerable<T> rowDirection()
            {
                for (var row = 0; row < array.GetLength(0); row++)
                {
                    for (var col = 0; col < array.GetLength(1); col++)
                    {
                        yield return array[row, col];
                    }
                }
            }
            IEnumerable<T> colDirection()
            {
                for (var col = 0; col < array.GetLength(1); col++)
                {
                    for (var row = 0; row < array.GetLength(0); row++)
                    {
                        yield return array[row, col];
                    }
                }
            }

            if (direction == SquareDirection.Row)
                return rowDirection();
            else
                return colDirection();
        }
    }
    /// <summary>
    /// 2次元方向
    /// </summary>
    public enum SquareDirection { Row = 0, Column = 1 }
}
