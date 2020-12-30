using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisPlayerWPF.Source.SettingElement
{
    public abstract class SettingElementBase<T>
    {
        public T Default;
        public T Value;
        public SettingElementBase(T value)
        {
            Value = Default = value;
        }
    }
}
