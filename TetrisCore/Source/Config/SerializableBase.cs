using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TetrisCore.Source.Config
{
    public abstract class SerializableBase
    {
        [XmlIgnoreAttribute]
        protected XmlSerializer Serializer { get; }

        public SerializableBase()
        {
            Serializer = new XmlSerializer(GetType());
        }
        public bool Save(string path,string file_name)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            using(StreamWriter writer = new StreamWriter(path + "/" + file_name, false, Encoding.UTF8))
            {
                Serializer.Serialize(writer,this);
                writer.Flush();
            }
            return File.Exists(path + "/" + file_name);
        }
        public static SerializableBase Load(Type type,string path,string  file_name)
        {
            SerializableBase result;
            XmlReaderSettings setting = new XmlReaderSettings() { CheckCharacters =false };
            if (!File.Exists(path + "/" + file_name)) return null;
            using (StreamReader reader = new StreamReader(path + "/" + file_name,Encoding.UTF8))
            {
                using (XmlReader deserializer = XmlReader.Create(reader,setting))
                {
                    result =new XmlSerializer(type).Deserialize(deserializer) as SerializableBase;
                }
            }
            return result;
        }
        public override string ToString()
        {
            string result = GetType().Name+"{";
            var property = GetType().GetProperties()
                .Where(x => !x.CustomAttributes.Any(x => x.AttributeType == typeof(XmlIgnoreAttribute)))
                .Select(x => $"{ x.Name}={x.GetValue(this)}")
                .ToArray(); ;
            result += String.Join(",",property);
            result += "}";
            return result;
        }
    }
}
