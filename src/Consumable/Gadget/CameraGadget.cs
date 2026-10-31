using System;
using System.Collections.Generic;
using Aotenjo;

[Serializable]
public class CameraGadget : Gadget
{
    protected override Gadget CreateCopy() => new CameraGadget();

    public CameraGadget() : base("camera", 21, 1, 5)
    {
    }

    protected override IReadOnlyList<GadgetTilePreview> BuildPreview(Player player, List<Tile> targets)
    {
        Tile copy = targets[0].Copy();
        copy.properties = copy.properties.CopyWithFont(TileFont.COLORLESS);
        return new[] { new GadgetTilePreview(targets[0], copy) };
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (!ShouldHighlightTile(tile, player)) return false;
        Tile copy = tile.Copy();
        copy.properties = copy.properties.CopyWithFont(TileFont.COLORLESS);
        player.AddNewTileToPool(copy);
        MessageManager.Instance.OnSoundEvent("Camera");
        return true;
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        return tile is FlowerTile || tile.IsNumbered() || tile.IsHonor(player);
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
