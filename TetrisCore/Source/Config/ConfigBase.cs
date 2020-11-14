using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TetrisCore.Source.Config
{
    public abstract class ConfigBase : SerializableBase
    {
        [XmlIgnore]
        public const string ConfigDirectory = "config";

        [XmlIgnore]
        public string Name { get; }
        public ConfigBase(string name)
        {
            Name = name;
        }
        public bool Save()
        {
            return base.Save(ConfigDirectory,Name+".xml");
        }
        public ConfigBase Load()
        {
            return SerializableBase.Load(GetType(),ConfigDirectory,Name+".xml") as ConfigBase;
        }
    }
}
