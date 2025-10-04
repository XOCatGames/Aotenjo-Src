using System;
using Aotenjo;

[Serializable]
public class MagnetGadget : Gadget
{
    public MagnetGadget() : base("magnet", 4, 3, 4)
    {
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (ShouldHighlightTile(tile))
        {
            tile.AddTransform(new TileTransformMagnet(), player);
            MessageManager.Instance.OnSoundEvent("Magnet");
            return true;
        }

        return false;
    }

    public override bool CanObtainBy(Player player, bool inShop)
    {
        if (player.deck.regName == MahjongDeck.ScarletDeck.regName) return false;
        return base.CanObtainBy(player, inShop);
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
        return tile.GetCategory() == Tile.Category.Feng;
    }
}