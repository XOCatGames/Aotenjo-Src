using System;
using Aotenjo;

[Serializable]
public class CarrotStampGadget : Gadget
{
    public CarrotStampGadget() : base("carrot_stamp", 27, 3, 9)
    {
    }

    public override Rarity GetRarity()
    {
        return Rarity.RARE;
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (!ShouldHighlightTile(tile)) return false;

        Tile.Category cat = tile.GetCategory();
        int ord = tile.GetOrder();

        tile.ModifyCarvedDesign(cat, ord, player);

        tile.ClearTransform(player);
        MessageManager.Instance.OnSoundEvent("Rice");

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
        return tile.GetTransforms().Count > 0;
    }

    public override bool CanUseOnSettledTiles()
    {
        return true;
    }
}