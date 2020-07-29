using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisCore.Source.Extension
{
    public static class ListExtensions
    {
        /// <summary>
        /// 先頭にあるオブジェクトを削除し、返します
        /// </summary>
        public static T Dequeue<T>(this IList<T> self)
        {
            var result = self[0];
            self.RemoveAt(0);
            return result;
        }

        /// <summary>
        /// 末尾にオブジェクトを追加します
        /// </summary>
        public static void Enqueue<T>(this IList<T> self, T item)
        {
            self.Add(item);
        }

        /// <summary>
        /// 先頭にあるオブジェクトを削除せずに返します
        /// </summary>
        public static T Peek<T>(this IList<T> self)
        {
            return self[0];
        }
        ///<summary>
        ///リストからランダムに一つ取り出す
        /// </summary>
        public static T GetRandom<T>( this IList<T> list )
	    {
            Random random = new Random();
		    return list[random.Next(0, list.Count)];
	    }
}
}
