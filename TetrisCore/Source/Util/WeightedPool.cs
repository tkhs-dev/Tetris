using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TetrisCore.Source.Config;
using static TetrisCore.Source.GamePlayData;

namespace TetrisCore.Source.Util
{
    public class WeightedPool<T>
    {
        public List<WeightedItem> WeightedItems { get => _weightedItems; }
        private List<WeightedItem> _weightedItems; 

        public List<T> Items { get => _weightedItems.Select(x => x.Value).ToList(); }

        public int Count { get => _weightedItems.Count; }
        public WeightedPool()
        {
            _weightedItems = new List<WeightedItem>();
        }
        public WeightedPool(List<WeightedItem> list)
        {
            _weightedItems = list;
        }
        public void Add(WeightedItem item)
        {
            _weightedItems.Add(item);
        }
        public void Add(int weight, T value)
        {
            Add(new WeightedItem(weight,value));
        }
        public T Take()
        {
            return _weightedItems[TakeIndex()].Value;
        }
        public int TakeIndex()
        {
            if (_weightedItems == null || _weightedItems.Count == 0) return 0;
            Random rnd = new Random();
            int num = rnd.Next(0, _weightedItems.Select(x => x.Weight).Sum());
            int weight = 0;
            foreach (var (w, index) in _weightedItems.Select((x, index) => (x.Weight, index)))
            {
                weight += w;
                if (num - weight < 0) return index;
            }
            return 0;
        }
        public class WeightedItem
        {
            public int Weight { get; set; }
            public T Value { get; set; }

            public WeightedItem()
            {
                Weight = 1;
                Value = default;
            }
            public WeightedItem(int weight,T value)
            {
                Weight = weight;
                Value = value;
            }
        }
    }
}
