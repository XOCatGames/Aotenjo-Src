using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class ChiselGadget : ReusableGadget
{
    protected override Gadget CreateCopy() => new ChiselGadget();

    public ChiselGadget() : base("chisel", 2, 1, 7)
    {
    }

    public override bool UseOnBlock(Player player, Block block)
    {
        if (uses == 0 || !block.IsNumbered()) return false;
        if (block.Any(t => t.GetOrder() == 1)) return false;

        ApplyTransform(player, block);

        MessageManager.Instance.OnSoundEvent("Chisel");
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

        MessageManager.Instance.OnSoundEvent("Chisel");
        return true;
    }

    private static void ApplyTransform(Player player, Block block)
    {
        foreach (var item in block.tiles)
        {
            item.AddTransform(new TileTransformChisel(item.GetOrder() - 1, item.GetCategory()), player);
        }
    }

    public override bool CanUseOnSettledTiles(Player player)
    {
        return false;
    }

    public override int GetMaxOnUseNum(Player player)
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

    private static TileTransform PreviousFace(Player player, Tile tile)
    {
        Tile previous = player.GetUniqueFullDeck().FirstOrDefault(t => player.GetCombinator().ASuccB(tile, t));
        return previous == null ? null : new TileTransformChisel(previous.GetOrder(), previous.GetCategory());
    }

    protected override IReadOnlyList<GadgetTilePreview> BuildPreview(Player player, List<Tile> targets)
    {
        var result = new List<GadgetTilePreview>();
        foreach (Tile target in targets)
        {
            TileTransform transform = PreviousFace(player, target);
            if (transform == null) return Array.Empty<GadgetTilePreview>();
            result.Add(GadgetTilePreview.WithTransform(target, transform));
        }
        return result;
    }

    public override GadgetUseResult UseOnTiles(Player player, List<Tile> tiles)
    {
        if (tiles == null || tiles.Count == 0 || uses <= 0) return GadgetUseResult.Failed;
        if (!CanUseOnTiles(tiles, player)) return GadgetUseResult.Failed;

        var transforms = tiles.Select(t => PreviousFace(player, t)).ToList();
        if (transforms.Any(t => t == null)) return GadgetUseResult.Failed;
        for (int i = 0; i < tiles.Count; i++)
            tiles[i].AddTransform(transforms[i], player);

        MessageManager.Instance.OnSoundEvent("Chisel");
        return GadgetUseResult.Succeeded(tiles);
    }
}