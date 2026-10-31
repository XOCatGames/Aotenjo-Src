using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class TweezerGadget : Gadget
{
    protected override Gadget CreateCopy() => new TweezerGadget();

    public TweezerGadget() : base("tweezer", 12, 3, 5)
    {
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        List<Tile> selectingTiles = player.GetSelectedTilesCopy();
        return UseOnTiles(player, selectingTiles).Success;
    }

    protected override IReadOnlyList<GadgetTilePreview> BuildPreview(Player player, List<Tile> targets)
    {
        return new[]
        {
            GadgetTilePreview.WithTransform(targets[0], SwapTransform(targets[0], targets[1])),
            GadgetTilePreview.WithTransform(targets[1], SwapTransform(targets[1], targets[0]))
        };
    }

    private static TileTransform SwapTransform(Tile target, Tile other) =>
        new TileTransformTweezed(other.GetOrder(), target.GetCategory());

    public override GadgetUseResult UseOnTiles(Player player, List<Tile> tiles)
    {
        if (tiles == null || tiles.Count == 0 || uses <= 0) return GadgetUseResult.Failed;
        if (!CanUseOnTiles(tiles, player)) return GadgetUseResult.Failed;
        Tile tile1 = tiles[0];
        Tile tile2 = tiles[1];
        TileTransform first = SwapTransform(tile1, tile2);
        TileTransform second = SwapTransform(tile2, tile1);
        tile1.AddTransform(first, player);
        tile2.AddTransform(second, player);
        return GadgetUseResult.Succeeded(tiles);
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
        return tile.IsNumbered();
    }

    public override int GetMaxOnUseNum(Player player)
    {
        return 2;
    }

    public override bool CanUseOnTiles(List<Tile> tiles, Player player)
    {
        return tiles.Count == 2 && tiles.All(t => t.IsNumbered());
    }
}