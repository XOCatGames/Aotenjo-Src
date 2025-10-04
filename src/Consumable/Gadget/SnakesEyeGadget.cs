using System.Linq;
using Aotenjo;

public class SnakesEyeGadget : Gadget
{
    public SnakesEyeGadget() : base("snakes_eye", 17, 2, 5)
    {
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (uses <= 0) return false;
        if (!ShouldHighlightTile(tile, player)) return false;
        Permutation perm = player.GetAccumulatedPermutation();
        Block block = perm.blocks.First(b => b.tiles.Contains(tile));
        foreach (var memberTile in block.tiles)
        {
            if (memberTile == tile) continue;
            memberTile.AddTransform(new TileTransformSnakesEye(tile), player);
        }

        MessageManager.Instance.OnSoundEvent("SnakesEye");
        return true;
    }

    public override Rarity GetRarity()
    {
        return Rarity.COMMON;
    }

    public override bool IsConsumable()
    {
        return true;
    }

    public override int GetStackLimit()
    {
        return 5;
    }

    public override bool CanUseOnSettledTiles()
    {
        return true;
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        Permutation perm = player.GetAccumulatedPermutation();
        if (perm == null) return false;
        return perm.blocks.Any(b => b.IsABC() && b.tiles.Contains(tile));
    }
}