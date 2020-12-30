using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisPlayerWPF.Source.SettingElement
{
    public class BoolSettingElement:SettingElementBase<bool>
    {
        public BoolSettingElement(bool default_value) : base(default_value)
        {
        }
    }
}
