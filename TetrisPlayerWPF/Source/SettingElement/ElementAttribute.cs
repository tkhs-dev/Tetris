using System;

namespace TetrisPlayerWPF.Source.SettingElement
{
    public class ElementAttribute : Attribute
    {
        public string Unit;//単位
        public string Tips;//説明
        public string Name { get; }
        public ElementAttribute(string name)
        {
            Name = name;
        }
    }
}
