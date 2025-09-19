using System;
using Aotenjo;

[Serializable]
public class InkBrushGadget : ReusableGadget
{
    public InkBrushGadget() : base("ink_brush", 5, 1, 5)
    {
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (uses <= 0) return false;
        if (ShouldHighlightTile(tile))
        {
            int order = player.GenerateRandomInt(3) + 5;
            tile.AddTransform(new TileTransformInkBrush(order), player);
            return true;
        }

        return false;
    }

    public override bool CanObtainBy(Player player, bool inShop)
    {
        if (player.deck.regName == MahjongDeck.ScarletDeck.regName) return false;
        return base.CanObtainBy(player, inShop);
    }

    public override bool ShouldHighlightTile(Tile tile)
    {
        return (tile.GetCategory() == Tile.Category.Feng || tile.GetCategory() == Tile.Category.Wan);
    }
}