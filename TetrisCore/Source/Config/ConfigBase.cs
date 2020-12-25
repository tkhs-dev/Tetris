using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TetrisCore.Source.Config
{
    public abstract class ConfigBase : SerializableBase
    {
        [XmlIgnore]
        public const string Directory = "config";

        [XmlIgnore]
        public string Name { get; }
        public ConfigBase(string name)
        {
            Name = name;
        }
        public bool Save()
        {
            return base.Save(Directory,Name+".xml");
        }
        public bool Load()
        {
            var value = SerializableBase.Load(GetType(), Directory, Name + ".xml") as ConfigBase;
            SetValue(value ?? GetDefault());
            return value!=null;
        }
        public abstract ConfigBase GetDefault();
    }
}
