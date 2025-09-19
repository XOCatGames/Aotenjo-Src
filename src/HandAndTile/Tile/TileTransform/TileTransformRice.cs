using System;
using Aotenjo;

[Serializable]
public class TileTransformRice : TileTransformOrderShift
{
    public TileTransformRice() : base(-1, "rice_sticked")
    {
    }

    public override int GetDisplayID(Tile t)
    {
        int i = GetStackCount(t);
        int order = t.GetOrder() + i - 1;
        int catShift = 9 * 8 * (Tile.CategoryToInteger(t.GetCategory()) - 2);
        int stackShift = ((i - 1) * 9);
        int v = 9 * 9 + stackShift + order + catShift;
        return v;
    }
}