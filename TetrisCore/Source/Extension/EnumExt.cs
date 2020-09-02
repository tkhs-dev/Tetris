using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using static TetrisCore.Source.BlockObject;

namespace TetrisCore.Source.Extension
{
    /// <see cref="BlockObject.Directions"/>
    public static class DirectionsExt
    {
        public static Directions Rotate(this Directions self,int num)
        {
            return (Directions)Enum.ToObject(typeof(Directions),num>=0 ? (((int)self)+num) % 4 : ((((int)self) + num+444444) % 4));            
        }
    }
    /// <see cref="BlockObject.Kind"/>
    public static class KindExt
    {
        private static readonly BlockObject Object_I =
            new BlockObject(
                    Color.Aqua,
                    new int[,]{
                        {0,0,0,0},
                        {1,1,1,1},
                        {0,0,0,0},
                        {0,0,0,0}
                    });
        private static readonly BlockObject Object_O =
            new BlockObject(
                    Color.Yellow,
                    new int[,]{
                        {1,1},
                        {1,1}
                    });
        private static readonly BlockObject Object_T =
            new BlockObject(
                    Color.Purple,
                    new int[,]{
                        {0,1,0},
                        {1,1,1},
                        {0,0,0}
                    });
        private static readonly BlockObject Object_J =
            new BlockObject(
                    Color.Blue,
                    new int[,]{
                        {1,0,0},
                        {1,1,1},
                        {0,0,0}
                    });
        private static readonly BlockObject Object_L =
            new BlockObject(
                    Color.Orange,
                    new int[,]{
                        {0,0,1},
                        {1,1,1},
                        {0,0,0}
                    });
        private static readonly BlockObject Object_S =
            new BlockObject(
                    Color.Green,
                    new int[,]{
                        {0,1,1},
                        {1,1,0},
                        {0,0,0}
                    });
        private static readonly BlockObject Object_Z =
            new BlockObject(
                    Color.Red,
                    new int[,]{
                        {1,1,0},
                        {0,1,1},
                        {0,0,0}
                    });

        public static BlockObject GetObject(this Kind self)
        {
            switch (self)
            {
                case Kind.I:
                    return Object_I;
                case Kind.O:
                    return Object_O;
                case Kind.T:
                    return Object_T;
                case Kind.J:
                    return Object_J;
                case Kind.L:
                    return Object_L;
                case Kind.S:
                    return Object_S;
                case Kind.Z:
                    return Object_Z;
            }
            throw new InvalidOperationException("Unknown Kind");
        }
    }
}
