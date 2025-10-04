using System.Collections.Generic;
using Aotenjo;

public class SuppressTileGroupAnimationEffect : OnMultipleTileAnimationEffect
{
    private List<Tile> tiles;
    private readonly Tile tile;

    public SuppressTileGroupAnimationEffect(List<Tile> tiles, Tile tile) : base(new SimpleEffect("effect_suppress",
        null, p => tiles.ForEach(t => t.Suppress(p)), soundName: "Suppress"))
    {
        this.tiles = tiles;
        this.tile = tile;
    }

    public override List<Tile> GetAffectedTiles(Player player)
    {
        return tiles;
    }

    public override Tile GetMainTile(Player player)
    {
        return tile;
    }
}