using System;
using Aotenjo;

[Serializable]
public class TileTransformChisel : TileTransformTrivial
{
    public TileTransformChisel(int order, Tile.Category cat) : base(cat, order, "chiseled")
    {
        this.order = order;
        this.cat = cat;
    }

    public override TileTransform Copy()
    {
        return new TileTransformChisel(order, cat);
    }
}