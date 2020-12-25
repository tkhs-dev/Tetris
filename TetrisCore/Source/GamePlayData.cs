using System;
using System.Collections.Generic;
using System.Drawing;
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
        public List<SerializableBlockUnit> ObjectPool { get; set; }

        /// <summary>
        /// 出現したブロック
        /// </summary>
        public List<int> ObjectsQueue { get; set; }

        /// <summary>
        /// イベント
        /// </summary>
        public List<GamePlayEvent> Events { get; set; }

        [XmlIgnore]
        public const string Directory = "playdata";

        public GamePlayData()
        {
            ObjectPool = new List<SerializableBlockUnit>();
            ObjectsQueue = new List<int>();
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
            public Color Color { get; set; }

            public static SerializableBlockUnit FromBlockUnit(BlockUnit unit)
            {
                return new SerializableBlockUnit() { Data = unit.Data.ToJaggedArray(),Color = unit.Color};
            }
            public BlockUnit GetBlockUnit()
            {
                return new BlockUnit(Color,Data.ToDimensionalArray());
            }
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
                [XmlIgnore]
                public Type Type { get; set; }

                public string TypeString {
                    get { return Type.FullName; } 
                    set{
                        this.Type = Type.GetType(value);
                        this.TypeString = value; 
                    } 
                }

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