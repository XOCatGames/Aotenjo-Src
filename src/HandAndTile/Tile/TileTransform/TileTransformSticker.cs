using System;
using Aotenjo;

[Serializable]
public class TileTransformSticker : TileTransformOrderShift
{
    public TileTransformSticker() : base(1, "sticked")
    {
    }

    public override int GetDisplayID(Tile t)
    {
        int i = GetStackCount(t);
        return t.GetOrder() - 1 + (i >= 2 ? 9 : 0);
    }
}