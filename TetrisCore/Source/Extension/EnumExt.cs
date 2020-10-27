using System;
using System.Drawing;
using static TetrisCore.Source.BlockUnit;

namespace TetrisCore.Source.Extension
{
    /// <see cref="BlockUnit.Directions"/>
    public static class DirectionsExt
    {
        public static Directions Rotate(this Directions self, int num)
        {
            return (Directions)Enum.ToObject(typeof(Directions), (int)self + num >= 0 ? (((int)self) + num) % 4 : ((int)self + num) % 4 + 4);
        }
    }

    /// <see cref="BlockUnit.Kind"/>
    public static class KindExt
    {
        private static readonly BlockUnit Object_I =
            new BlockUnit(
                    Color.Aqua,
                    new int[,]{
                        {0,0,0,0},
                        {1,1,1,1},
                        {0,0,0,0},
                        {0,0,0,0}
                    });

        private static readonly BlockUnit Object_O =
            new BlockUnit(
                    Color.Yellow,
                    new int[,]{
                        {1,1},
                        {1,1}
                    });

        private static readonly BlockUnit Object_T =
            new BlockUnit(
                    Color.Purple,
                    new int[,]{
                        {0,1,0},
                        {1,1,1},
                        {0,0,0}
                    });

        private static readonly BlockUnit Object_J =
            new BlockUnit(
                    Color.Blue,
                    new int[,]{
                        {1,0,0},
                        {1,1,1},
                        {0,0,0}
                    });

        private static readonly BlockUnit Object_L =
            new BlockUnit(
                    Color.Orange,
                    new int[,]{
                        {0,0,1},
                        {1,1,1},
                        {0,0,0}
                    });

        private static readonly BlockUnit Object_S =
            new BlockUnit(
                    Color.Green,
                    new int[,]{
                        {0,1,1},
                        {1,1,0},
                        {0,0,0}
                    });

        private static readonly BlockUnit Object_Z =
            new BlockUnit(
                    Color.Red,
                    new int[,]{
                        {1,1,0},
                        {0,1,1},
                        {0,0,0}
                    });

        public static BlockUnit GetObject(this Kind self)
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