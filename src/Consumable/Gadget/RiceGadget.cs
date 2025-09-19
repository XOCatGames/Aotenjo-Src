using System;
using Aotenjo;

[Serializable]
public class RiceGadget : Gadget
{
    public RiceGadget() : base("rice", 0, 3, 4)
    {
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (!ShouldHighlightTile(tile)) return false;

        tile.AddTransform(new TileTransformRice(), player);
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

    public override bool ShouldHighlightTile(Tile tile)
    {
        TileTransform tileTransform = tile.GetLastTransform();
        bool isRedMarked = tileTransform != null &&
                           tileTransform.GetNameKey() == new TileTransformRedMarker().GetNameKey();
        return !isRedMarked && (tile.GetCategory() == Tile.Category.Suo || tile.GetCategory() == Tile.Category.Bing)
                            && tile.GetOrder() > 1;
    }
}