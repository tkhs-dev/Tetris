using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;
using TetrisCore.Source.Config;
using TetrisCore.Source.Extension;
using static TetrisCore.Source.TetrisGame;

namespace TetrisCore.Source
{
    public class GamePlayData : SerializableBase
    {
        /// <summary>
        /// プレイ開始時刻
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// ゲームの設定
        /// </summary>
        public GameSetting Setting { get; set; }

        /// <summary>
        /// オブジェクトプール
        /// </summary>
        public List<SerializableBlockUnit> SerializableObjectPool { get; set; }
        [XmlIgnore]
        public IReadOnlyList<BlockUnit> ObjectPool { get => SerializableObjectPool.Select(x => x.GetBlockUnit()).ToList(); set => SerializableObjectPool = value.Select((x, Index) => { var u = GamePlayData.SerializableBlockUnit.FromBlockUnit(x); u.ID = Index; return u; }).ToList(); }

        /// <summary>
        /// 出現したブロック
        /// </summary>
        public List<SerializableQueue> SerializableObjectQueue { get; set; }
        [XmlIgnore]
        public Queue<BlockUnit> ObjectQueue { get => new Queue<BlockUnit>(SerializableObjectQueue.OrderBy(x=>x.ID).Select(x => ObjectPool[x.Value])); }

        /// <summary>
        /// イベント
        /// </summary>
        public List<GamePlayEvent> Events { get; set; }

        [XmlIgnore]
        public const string Directory = "playdata";

        public GamePlayData()
        {
            SerializableObjectPool = new List<SerializableBlockUnit>();
            SerializableObjectQueue = new List<SerializableQueue>();
            Events = new List<GamePlayEvent>();
        }

        public bool Save()
        {
            return base.Save("playdata", Date.ToString("yyyy-MM-dd-HHmmss")+".xml");
        }
        public class SerializableBlockUnit
        {
            public int ID { get; set; }
            public int[][] Data { get; set; }
            public int ColorARGB { get; set; }
            [XmlIgnore]
            public Color Color { get => Color.FromArgb(ColorARGB); set => ColorARGB = value.ToArgb(); }

            public static SerializableBlockUnit FromBlockUnit(BlockUnit unit)
            {
                return new SerializableBlockUnit() { Data = unit.Data.ToJaggedArray(),Color = unit.Color};
            }
            public BlockUnit GetBlockUnit()
            {
                return new BlockUnit(Color,Data.ToDimensionalArray());
            }
        }
        public class SerializableQueue
        {
            public int ID { get; set; }
            public int Value { get; set; }
        }
        public class GamePlayEvent
        {
            public GamePlayEvent()
            {
            }

            public enum EventType
            {
                PLACE,
                MOVE,
                ROTATE
            }

            public int Round { get; set; }
            public TimeSpan Time { get; set; }
            public EventType Event { get; set; }
            public Argument Arg { get; set; }

            public class Argument
            {
                [XmlElement(typeof(BlockUnit.Directions)),
                 XmlElement(typeof(bool))
                    ]
                public Object Object { get; set; }

                public Argument()
                {
                }
            }
        }
    }
}