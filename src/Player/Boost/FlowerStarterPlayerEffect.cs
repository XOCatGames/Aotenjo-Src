using System.Collections.Generic;
using Aotenjo;

public class FlowerStarterPlayerEffect : StarterBoostEffect
{
    public FlowerStarterPlayerEffect() : base("starter_flower")
    {
    }

    public override bool IsAvailable(Player player)
    {
        return player.deck.id == MahjongDeck.RainbowDeck.id;
    }

    public override void Boost(Player player)
    {
        List<Tile> toAdd = new Hand($"1234{(char)('f' + player.GenerateRandomInt(4))}").tiles;
        foreach (Tile tile in toAdd)
        {
            player.AddNewTileToPool(tile);
        }
    }

    public class Plus : StarterBoostEffect
    {
        public Plus() : base("starter_flower_plus")
        {
        }

        public override bool IsAvailable(Player player)
        {
            return player.deck.id == MahjongDeck.RainbowDeck.id;
        }

        public override void Boost(Player player)
        {
            List<Tile> toAdd = new Hand($"1234{(char)('f' + player.GenerateRandomInt(4))}").tiles;
            foreach (Tile tile in toAdd)
            {
                player.AddNewTileToPool(tile);
            }

            toAdd = new Hand($"1234{(char)('f' + player.GenerateRandomInt(4))}").tiles;
            foreach (Tile tile in toAdd)
            {
                player.AddNewTileToPool(tile);
            }
        }
    }
}