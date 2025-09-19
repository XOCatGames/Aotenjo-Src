using System;
using Aotenjo;

[Serializable]
public class StimulantGadget : Gadget
{
    public StimulantGadget() : base("stimulant", 9, 2, 6)
    {
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (!ShouldHighlightTile(tile, player)) return false;
        TileProperties properties = player.GenerateRandomTileProperties(0, 80, 20, 100);
        tile.MergeAndSetProperties(properties.CopyWithMask(TileMask.Fractured()), player);
        MessageManager.Instance.OnSoundEvent("Stimulant");
        return true;
    }

    public override Rarity GetRarity()
    {
        return Rarity.COMMON;
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        return (tile.IsNumbered() || tile.IsHonor(player)) && tile.CompactWithMaterial(TileMaterial.PLAIN, player);
    }

    public override bool IsConsumable()
    {
        return true;
    }

    public override int GetStackLimit()
    {
        return 5;
    }
}