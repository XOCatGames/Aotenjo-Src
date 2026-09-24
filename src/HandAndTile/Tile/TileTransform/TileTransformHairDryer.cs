using System;
using Aotenjo;
using UnityEngine;

[Serializable]
public class TileTransformHairDryer : TileTransformTrivial
{
    private const int BlowRightSpriteId = 72;
    private const int BlowLeftSpriteId = 73;
    private const float FontOffsetRatio = 0.04f;

    [SerializeField] private Direction direction;

    public TileTransformHairDryer(Tile.Category category, int order, Direction direction)
        : base(category, order, "trivial")
    {
        this.direction = direction;
    }

    public override TileTransform Copy()
    {
        return new TileTransformHairDryer(cat, order, direction);
    }

    public override int GetDisplayID(Tile tile)
    {
        // The sprite shows the trailing air flow, opposite to the direction
        // in which the font itself is displaced.
        return direction == Direction.RIGHT ? BlowRightSpriteId : BlowLeftSpriteId;
    }

    public override float GetFontDisplayOffsetX()
    {
        return direction == Direction.RIGHT ? FontOffsetRatio : -FontOffsetRatio;
    }
}
