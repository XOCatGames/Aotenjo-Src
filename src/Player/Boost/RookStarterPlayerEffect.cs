using System.Linq;
using Aotenjo;

public class RookStarterPlayerEffect : StarterBoostEffect
{
    public RookStarterPlayerEffect() : base("starter_rook")
    {
    }

    public override bool IsAvailable(Player player)
    {
        return player.deck.id == MahjongDeck.BambooDeck.id;
    }

    public override void Boost(Player player)
    {
        player.AddGadget(Gadgets.Rook);
        foreach (Tile tile in player.GetAllTiles().Where(t => t.IsNumbered() && t.GetOrder() == 5))
        {
            tile.SetFont(TileFont.RED, player);
        }
    }
}