using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TetrisCore.Source.Config;
using static TetrisCore.Source.GamePlayData;

namespace TetrisCore.Source.Util
{
    public class SerializableObjectPool : SerializableBase
    {
        public List<WeightedPool<SerializableBlockUnit>.WeightedItem> SerializablePool { get; set; }
        [XmlIgnore]
        public WeightedPool<BlockUnit> ObjectPool { get => new WeightedPool<BlockUnit>(SerializablePool.Select(x => new WeightedPool<BlockUnit>.WeightedItem(x.Weight, x.Value.GetBlockUnit())).ToList()); set => SerializablePool = value.WeightedItems.Select((x, Index) => { var u = GamePlayData.SerializableBlockUnit.FromBlockUnit(x.Value); u.ID = Index; return new WeightedPool<SerializableBlockUnit>.WeightedItem(x.Weight, u); }).ToList(); }

    }
}
