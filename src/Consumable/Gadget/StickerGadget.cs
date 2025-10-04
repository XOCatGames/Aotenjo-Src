using System;
using Aotenjo;

[Serializable]
public class StickerGadget : Gadget
{
    public StickerGadget() : base("sticker", 3, 3, 4)
    {
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (!ShouldHighlightTile(tile)) return false;

        tile.AddTransform(new TileTransformSticker(), player);
        EventManager.Instance.OnSoundEvent("Sticker");
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

    public override bool ShouldHighlightTile(Tile tile)
    {
        return tile.GetCategory() == Tile.Category.Wan && tile.GetOrder() < 9;
    }
}