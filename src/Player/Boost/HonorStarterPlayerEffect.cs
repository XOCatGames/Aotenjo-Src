using System.Collections.Generic;
using Aotenjo;

public class HonorStarterPlayerEffect : StarterBoostEffect
{
    public HonorStarterPlayerEffect() : base("starter_honor")
    {
        BlockFromDeck(MahjongDeck.ScarletDeck);
    }

    public override void Boost(Player player)
    {
        List<Tile> tiles = new();
        for (int i = 1; i < 8; i++)
        {
            Tile tile = new Tile(i + "z");
            tile.properties = player.GenerateRandomTileProperties(60, 38, 2, 10);
            if (player.GenerateRandomInt(2) == 0)
                tile.properties.mask = player.GenerateRandomInt(3) == 0 ? TileMask.Fractured() : TileMask.Corrupted();
            player.AddNewTileToPool(new Tile(tile));
            tiles.Add(tile);
        }
    }

    public class More : StarterBoostEffect
    {
        public More() : base("starter_honor_plus")
        {
            BlockFromDeck(MahjongDeck.ScarletDeck);
        }

        public override void Boost(Player player)
        {
            List<Tile> tiles = new();
            for (int i = 1; i < 8; i++)
            {
                Tile tile = new Tile(i + "z");
                tile.properties = player.GenerateRandomTileProperties(70, 28, 2, 10);
                player.AddNewTileToPool(new Tile(tile));
                tiles.Add(tile);
            }
        }
    }
}