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
        public bool Load()
        {
            var value = SerializableBase.Load(GetType(), ConfigDirectory, Name + ".xml") as ConfigBase;
            SetValue(value ?? GetDefault());
            return value!=null;
        }
        protected void SetValue(ConfigBase value)
        {
            var type = value.GetType();
            foreach(var prop in type.GetProperties().Where(x=>!x.CustomAttributes.Any(x=>x.AttributeType==typeof(XmlIgnoreAttribute))))
            {
                this.GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(value));
            }
        }
        public abstract ConfigBase GetDefault();
    }
}
