using System;
using Aotenjo;

[Serializable]
public class RiceGadget : Gadget
{
    protected override Gadget CreateCopy() => new RiceGadget();

    public RiceGadget() : base("rice", 0, 3, 4)
    {
    }

    protected override TileTransform CreateTileTransform(Player player, Tile tile) => new TileTransformRice();

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (!ShouldHighlightTile(tile, player)) return false;

        tile.AddTransform(CreateTileTransform(player, tile), player);
        MessageManager.Instance.OnSoundEvent("Rice");
        return true;
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
        TileTransform tileTransform = tile.GetLastTransform();
        bool isRedMarked = tileTransform != null &&
                           tileTransform.GetNameKey() == new TileTransformRedMarker().GetNameKey();
        return !isRedMarked && (tile.GetCategory() == Tile.Category.Suo || tile.GetCategory() == Tile.Category.Bing)
                            && tile.GetOrder() > 1;
    }
}