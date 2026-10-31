using System;
using Aotenjo;

[Serializable]
public class OrderMirrorGadget : Gadget
{
    protected override Gadget CreateCopy() => new OrderMirrorGadget();

    public OrderMirrorGadget() : base("order_mirror", 13, 3, 6)
    {
    }

    public override Rarity GetRarity()
    {
        return Rarity.COMMON;
    }

    protected override TileTransform CreateTileTransform(Player player, Tile tile) => new TileTransformMirrored();

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (ShouldHighlightTile(tile, player))
        {
            tile.AddTransform(CreateTileTransform(player, tile), player);
            return true;
        }

        return false;
    }

    public override bool IsConsumable()
    {
        return true;
    }

    public override int GetStackLimit()
    {
        return 5;
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        return tile.IsNumbered();
    }
}