using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class AugmentStarterPlayerEffect : StarterBoostEffect
{
    public AugmentStarterPlayerEffect() : base("starter_augment")
    {
    }

    public override void Boost(Player player)
    {
        List<Tile> tiles = player.GetUniqueFullDeck().Where(t => t.IsNumbered()).ToList();

        List<Tile> tilesAdd = new List<Tile>();

        for (int i = 0; i < 12; i++)
        {
            Tile tile = tiles[player.GenerateRandomInt(tiles.Count)];
            tiles.Remove(tile);
            tile.properties = player.GenerateRandomTileProperties(0, 96, 4, 20);
            tilesAdd.Add(tile);
        }

        tilesAdd.Sort();

        tilesAdd.ForEach(tile => player.AddNewTileToPool(new Tile(tile)));
    }
}