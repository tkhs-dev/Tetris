using System;
using System.Collections.Generic;
using System.Linq;

namespace TetrisCore.Source.Extension
{
    public static class LinqExt
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> self, int size)
        {
            if (size <= 0)
                throw new ArgumentException("Chunk size must be greater than 0.", nameof(size));

            return self.Select((v, i) => new { v, i })
                .GroupBy(x => x.i / size)
                .Select(g => g.Select(x => x.v));
        }
    }
}