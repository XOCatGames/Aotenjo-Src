using System;
using Aotenjo;

[Serializable]
public class CameraGadget : Gadget
{
    public CameraGadget() : base("camera", 21, 1, 5)
    {
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (!ShouldHighlightTile(tile, player)) return false;
        Tile copy = new(tile);
        copy.properties = copy.properties.CopyWithFont(TileFont.COLORLESS);
        player.AddNewTileToPool(copy);
        MessageManager.Instance.OnSoundEvent("Camera");
        return true;
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        return tile.IsNumbered() || tile.IsHonor(player);
    }

    public override Rarity GetRarity()
    {
        return Rarity.RARE;
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