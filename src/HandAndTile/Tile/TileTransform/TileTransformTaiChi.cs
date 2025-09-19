using System;
using Aotenjo;

[Serializable]
public class TileTransformTaiChi : TileTransformTrivial
{
    public TileTransformTaiChi(int order, Tile.Category category) : base(category, order, "tai_chi")
    {
    }

    public override TileTransform Copy()
    {
        return new TileTransformTaiChi(order, cat);
    }

    public override int GetDisplayID(Tile t)
    {
        return 46;
    }

    public override bool ChangeBaseDisplay()
    {
        return true;
    }
}