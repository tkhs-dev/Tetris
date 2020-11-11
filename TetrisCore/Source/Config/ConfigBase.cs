using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TetrisCore.Source.Config
{
    public abstract class ConfigBase : ISerializeable
    {
        [XmlIgnoreAttribute]
        protected XmlSerializer Serializer { get; }

        public ConfigBase()
        {
            Serializer = new XmlSerializer(GetType());
        }
        public abstract Type GetType();
        public bool Save(string path,string file_name)
        {
            using(StreamWriter writer = new StreamWriter(path + "/" + file_name, false, Encoding.UTF8))
            {
                Serializer.Serialize(writer,this);
                writer.Flush();
            }
            return File.Exists(path + "/" + file_name);
        }
        public static ConfigBase Load(Type type,string path,string  file_name)
        {
            ConfigBase result;
            XmlReaderSettings setting = new XmlReaderSettings() { CheckCharacters =false };
            if (!File.Exists(path + "/" + file_name)) return null;
            using (StreamReader reader = new StreamReader(path + "/" + file_name,Encoding.UTF8))
            {
                using (XmlReader deserializer = XmlReader.Create(reader,setting))
                {
                    result =new XmlSerializer(type).Deserialize(deserializer) as ConfigBase;
                }
            }
            return result;
        }
    }
}
