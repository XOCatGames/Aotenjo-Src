using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class EraserGadget : ReusableGadget
{
    public EraserGadget() : base("eraser", 18, 1, 30)
    {
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (uses <= 0) return false;
        if (!ShouldHighlightTile(tile, player)) return false;
        Permutation perm = player.GetAccumulatedPermutation();
        Block block = perm.blocks.First(b => b.tiles.Contains(tile));
        bool res = player.EraseBlock(block);

        return res;
    }

    public override Rarity GetRarity()
    {
        return Rarity.RARE;
    }

    public override bool CanObtainBy(Player player, bool inShop)
    {
        return false;
    }

    public override bool CanUseOnSettledTiles()
    {
        return true;
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        List<Tile> tiles = player.GetSettledTiles();
        if (tiles.Count == 0) return false;
        return tiles.Contains(tile) && player.GetAccumulatedPermutation().JiangFulfillAll((t => t != tile));
    }
}