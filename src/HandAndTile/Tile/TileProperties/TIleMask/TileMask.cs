using System;

namespace Aotenjo
{
    [Serializable]
    public class TileMask : TileAttribute
    {
        public TileMask(int id, string name, Effect effect) : base(id, name + "_mask", effect)
        {
        }

        public virtual TileMask Copy()
        {
            return this;
        }

        public static TileMask NONE = new TileMask(-1, "none", null);
        public static TileMask Corrupted(Tile tile = null) => new TileMaskCorrupted(0);
        public static TileMask Suppressed(TileMask mask) => new TileMaskSuppressed(1, mask);
        public static TileMask BLESSED;
        public static TileMask Fractured() => new FracturedMask(3);

        public static TileMask Grow() => new TileMaskGrow(6);
        public static TileMask Frozen() => new TileMaskFrozen(5);
    }
}