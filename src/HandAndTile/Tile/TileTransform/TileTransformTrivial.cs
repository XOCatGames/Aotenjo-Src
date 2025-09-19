using System;
using Aotenjo;
using UnityEngine;

[Serializable]
public class TileTransformTrivial : TileTransform
{
    [SerializeField] public Tile.Category cat;
    [SerializeField] public int order;

    public TileTransformTrivial(Tile.Category cat, int order, string name = "trivial") : base(name)
    {
        this.cat = cat;
        this.order = order;
    }

    public override TileTransform Copy()
    {
        return new TileTransformTrivial(cat, order);
    }

    public override bool ChangeBaseDisplay()
    {
        return true;
    }

    public override Tile.Category GetTransformedCategory(Tile tile)
    {
        return cat;
    }

    public override int GetTransformedOrder(Tile tile)
    {
        return order;
    }
}