using System;
using Aotenjo;

[Serializable]
public class TileTransformSnakesEye : TileTransformTrivial
{
    public TileTransformSnakesEye(Tile tile) : base(tile.GetCategory(), tile.GetOrder(), "snaked")
    {
    }
}