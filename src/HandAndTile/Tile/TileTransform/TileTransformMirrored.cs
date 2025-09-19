using System;
using Aotenjo;

[Serializable]
public class TileTransformMirrored : TileTransform
{
    public TileTransformMirrored() : base("mirrored")
    {
    }

    public override bool ChangeBaseDisplay()
    {
        return true;
    }

    public override int GetTransformedOrder(Tile t)
    {
        return 10 - t.GetBaseOrder();
    }
}