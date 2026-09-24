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

    public override GadgetUseResult UseOnTiles(Player player, List<Tile> tiles)
    {
        if (tiles == null || tiles.Count == 0 || uses <= 0) return GadgetUseResult.Failed;
        if (!CanUseOnTiles(tiles, player)) return GadgetUseResult.Failed;
        Tile tile1 = tiles[0];
        Tile tile2 = tiles[1];
        int order = tile2.GetOrder();
        Tile.Category cat = tile1.GetCategory();
        int order1 = tile1.GetOrder();
        Tile.Category cat1 = tile2.GetCategory();
        tile1.AddTransform(new TileTransformTweezed(order, cat), player);
        tile2.AddTransform(new TileTransformTweezed(order1, cat1), player);
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