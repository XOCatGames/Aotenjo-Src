using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class SewingKitGadget : Gadget
{
    public SewingKitGadget() : base("sewing_kit", 16, 2, 9)
    {
    }

    public override Rarity GetRarity()
    {
        return Rarity.RARE;
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        List<Tile> selectingTiles = player.GetSelectedTilesCopy();
        if (!ShouldHighlightTile(tile, player) || selectingTiles.Count != 2) return false;

        Tile tile1 = selectingTiles[0];
        Tile tile2 = selectingTiles[1];

        if (tile1.GetCategory() == tile2.GetCategory()) return false;

        Tile tile3 = player.RandomlyMergeTwoTile(tile1, tile2);
        player.AddNewTileToPool(tile3);
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
        return true;
    }

    public override int GetMaxOnUseNum()
    {
        return 2;
    }

    public override bool CanUseOnTiles(List<Tile> tiles, Player player)
    {
        if (tiles.Count != 2 || tiles.Any(t => !(t.IsHonor(player) || t.IsNumbered()))) return false;
        return tiles[0].GetCategory() != tiles[1].GetCategory();
    }

    public override bool UseOnTiles(Player player, List<Tile> tiles)
    {
        if (!CanUseOnTiles(tiles, player)) return false;

        Tile tile1 = tiles[0];
        Tile tile2 = tiles[1];

        Tile tile3 = player.RandomlyMergeTwoTile(tile1, tile2);
        player.AddNewTileToPool(tile3);
        return true;
    }
}