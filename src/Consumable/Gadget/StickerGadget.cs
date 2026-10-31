using System;
using Aotenjo;

[Serializable]
public class StickerGadget : Gadget
{
    protected override Gadget CreateCopy() => new StickerGadget();

    public StickerGadget() : base("sticker", 3, 3, 4)
    {
    }

    protected override TileTransform CreateTileTransform(Player player, Tile tile) => new TileTransformSticker();

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (!ShouldHighlightTile(tile, player)) return false;

        tile.AddTransform(CreateTileTransform(player, tile), player);
        MessageManager.Instance.OnSoundEvent("Sticker");
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

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        return tile.GetCategory() == Tile.Category.Wan && tile.GetOrder() < 9;
    }
}