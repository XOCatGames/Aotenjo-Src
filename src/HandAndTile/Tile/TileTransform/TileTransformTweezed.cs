using System;
using Aotenjo;

[Serializable]
public class TileTransformTweezed : TileTransformTrivial
{
    public TileTransformTweezed(int order, Tile.Category cat) : base(cat, order, "tweezed")
    {
    }

    public override TileTransform Copy()
    {
        return new TileTransformTweezed(order, cat);
    }
}