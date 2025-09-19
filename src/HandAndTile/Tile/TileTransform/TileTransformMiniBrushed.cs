using System;
using Aotenjo;

[Serializable]
public class TileTransformMiniBrushed : TileTransformTrivial
{
    public TileTransformMiniBrushed(Tile.Category cat, int order) : base(cat, order, "mini_brushed")
    {
    }
}