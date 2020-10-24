using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TetrisCore.Source.Util
{
    public class ENumDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : struct, IConvertible
    {
        private TValue[] array = null;

        public ENumDictionary()
        {
            int length = Enum.GetNames(typeof(TKey)).Length;
            array = new TValue[length];
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new MyEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TValue this[TKey key]
        {
            set
            {
                array[KeyToIndex(key)] = value;
            }
            get
            {
                return array[KeyToIndex(key)];
            }
        }

        private static int KeyToIndex(TKey key)
        {
            return (int)(object)key;
        }

        private static TKey IndexToKey(int index)
        {
            return (TKey)(object)index;
        }

        public TValue this[int i]
        {
            set { array[i] = value; }
            get { return array[i]; }
        }

        class MyEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private ENumDictionary<TKey, TValue> dictionary;
            private int getIndex;
            private KeyValuePair<TKey, TValue> currentValue;

            public MyEnumerator(ENumDictionary<TKey, TValue> enumDictionary)
            {
                dictionary = enumDictionary;
                getIndex = 0;
            }

            object IEnumerator.Current => currentValue;

            KeyValuePair<TKey, TValue> IEnumerator<KeyValuePair<TKey, TValue>>.Current => currentValue;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (getIndex >= dictionary.array.Length)
                {
                    return false;
                }

                TKey key = IndexToKey(getIndex);
                TValue value = dictionary.array[getIndex];
                currentValue = new KeyValuePair<TKey, TValue>(key, value);
                getIndex++;
                return true;
            }

            public void Reset() { getIndex = 0; }
        }

        public void Add(TKey key, TValue v0)
        {
            array[KeyToIndex(key)] = v0;
        }
    }
}
