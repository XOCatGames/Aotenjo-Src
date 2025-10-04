using System;
using Aotenjo;

[Serializable]
public class WhiskGadget : Gadget
{
    public WhiskGadget() : base("whisk", 10, 3, 6)
    {
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        bool res = false;
        if (tile.properties.mask.IsDebuff())
        {
            tile.SetMask((TileMask.NONE), player);
            res = true;
        }

        if (tile.properties.font.IsDebuff())
        {
            tile.SetFont((TileFont.PLAIN), player);
            res = true;
        }

        if (res)
        {
            EventManager.Instance.OnSoundEvent("Whisk");
            return true;
        }

        return false;
    }

    public override bool CanUseOnSettledTiles()
    {
        return true;
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        return tile.properties.IsDebuffed();
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