using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class ChiselGadget : ReusableGadget
{
    public ChiselGadget() : base("chisel", 2, 1, 7)
    {
    }

    public override bool UseOnBlock(Player player, Block block)
    {
        if (uses == 0 || !block.IsNumbered()) return false;
        if (block.Any(t => t.GetOrder() == 1)) return false;

        ApplyTransform(player, block);

        EventManager.Instance.OnSoundEvent("Chisel");
        return true;
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (uses <= 0) return false;
        if (!ShouldHighlightTile(tile, player)) return false;
        Permutation perm = player.GetAccumulatedPermutation();
        if (perm == null) return false;

        Block block = perm.blocks.FirstOrDefault(b => b.tiles.Contains(tile));
        if (block == null) return false;

        if (block.Any(t => !t.IsNumbered() || t.GetOrder() == 1))
        {
            return false;
        }

        ApplyTransform(player, block);

        EventManager.Instance.OnSoundEvent("Chisel");
        return true;
    }

    private static void ApplyTransform(Player player, Block block)
    {
        foreach (var item in block.tiles)
        {
            item.AddTransform(new TileTransformChisel(item.GetOrder() - 1, item.GetCategory()), player);
        }
    }

    public override bool CanUseOnSettledTiles()
    {
        return false;
    }

    public override int GetMaxOnUseNum()
    {
        return 4;
    }

    public override bool CanUseOnTiles(List<Tile> tiles, Player player)
    {
        if (tiles.Count < 3) return false;
        if (tiles.Any(t => !t.IsNumbered() || t.GetOrder() == 1) &&
            player.deck.regName != MahjongDeck.GalaxyDeck.regName)
        {
            return false;
        }

        Block formedBlock = Block.FormValidBlock(tiles.ToArray(), player);

        return formedBlock != null;
    }

    public override bool UseOnTiles(Player player, List<Tile> tiles)
    {
        if (!CanUseOnTiles(tiles, player)) return false;

        foreach (var memberTile in tiles)
        {
            Tile predTile = player.GetUniqueFullDeck()
                .FirstOrDefault(t => player.GetCombinator().ASuccB(memberTile, t));
            memberTile.AddTransform(new TileTransformChisel(predTile.GetOrder(), predTile.GetCategory()), player);
        }

        EventManager.Instance.OnSoundEvent("Chisel");
        return true;
    }
}