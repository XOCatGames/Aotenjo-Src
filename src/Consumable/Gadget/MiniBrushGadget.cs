using System;
using Aotenjo;

[Serializable]
public class MiniBrushGadget : ReusableGadget
{
    protected override Gadget CreateCopy() => new MiniBrushGadget();

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
        if (ShouldHighlightTile(tile, player))
        {
            MessageManager.Instance.OnUseMiniBrushEvent(this, tile);
            return true;
        }

        return false;
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        return tile.IsNumbered();
    }
}