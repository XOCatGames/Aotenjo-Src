using System;
using System.Collections.Generic;
using Aotenjo;

[Serializable]
public class WhiskGadget : Gadget
{
    protected override Gadget CreateCopy() => new WhiskGadget();

    public WhiskGadget() : base("whisk", 10, 3, 6)
    {
    }

    protected override IReadOnlyList<GadgetTilePreview> BuildPreview(Player player, List<Tile> targets)
    {
        Tile display = targets[0].Copy();
        if (display.properties.mask.IsDebuff()) display.properties.ChangeMask(TileMask.NONE);
        if (display.properties.font.IsDebuff()) display.properties.ChangeFont(TileFont.PLAIN);
        return new[] { new GadgetTilePreview(targets[0], display) };
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (tile?.properties == null) return false;
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
            MessageManager.Instance.OnSoundEvent("Whisk");
            return true;
        }

        return false;
    }

    public override bool CanUseOnSettledTiles(Player player)
    {
        return true;
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        return tile?.properties != null && tile.properties.IsDebuffed();
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
