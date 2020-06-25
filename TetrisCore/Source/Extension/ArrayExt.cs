using System;
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
    }
}
