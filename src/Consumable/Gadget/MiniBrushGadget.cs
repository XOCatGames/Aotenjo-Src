using System;
using Aotenjo;

[Serializable]
public class MiniBrushGadget : ReusableGadget
{
    public MiniBrushGadget() : base("mini_brush", 7, 1, 7)
    {
    }

    public override Rarity GetRarity()
    {
        return Rarity.RARE;
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (uses <= 0) return false;
        if (ShouldHighlightTile(tile))
        {
            EventManager.Instance.OnUseMiniBrushEvent(this, tile);
            return true;
        }

        return false;
    }

    public override bool ShouldHighlightTile(Tile tile)
    {
        return tile.IsNumbered();
    }
}