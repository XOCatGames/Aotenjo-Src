using System;
using Aotenjo;
using UnityEngine;

[Serializable]
public class TileTransformInkBrush : TileTransform
{
    [SerializeField] public int order;

    public TileTransformInkBrush(int order) : base("ink_brush")
    {
        this.order = order;
    }

    public override TileTransform Copy()
    {
        return new TileTransformInkBrush(order);
    }

    public override int GetTransformedOrder(Tile t)
    {
        return order;
    }

    public override Tile.Category GetTransformedCategory(Tile tile)
    {
        return Tile.Category.Jian;
    }

    public override int GetDisplayID(Tile t)
    {
        return 45;
    }

    public override bool ChangeBaseDisplay()
    {
        return true;
    }
}