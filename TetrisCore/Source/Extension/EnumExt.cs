using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using static TetrisCore.Source.BlockObject;

namespace TetrisCore.Source.Extension
{
    public static class DirectionsExt
    {
        public static Directions Rotate(this Directions self,int num)
        {
            return (Directions)Enum.ToObject(typeof(Directions),(((int)self)+num) % 4);            
        }
    }
}
