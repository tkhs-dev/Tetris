using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisPlayer.Source.Util
{
    public class ColorConverter
    {
        public static SharpDX.Color GetDXColor(System.Drawing.Color color)
        {
            return new SharpDX.Color(color.ToArgb());
        }
    }
}
