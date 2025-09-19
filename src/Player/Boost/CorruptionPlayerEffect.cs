using System;
using System.Collections.Generic;
using Aotenjo;

public class CorruptionPlayerEffect : StarterBoostEffect
{
    public CorruptionPlayerEffect(int v) : base("starter_corruption")
    {
        V = v;
    }

    public int V { get; }

    public override string GetLocalizedDesc(Player _, Func<string, string> loc)
    {
        return string.Format(base.GetLocalizedDesc(_, loc), V);
    }

    public override void Boost(Player player)
    {
        List<Tile> cands = player.GetUniqueFullDeck();
        List<Tile> res = new LotteryPool<Tile>().AddRange(cands).DrawRange(player.GenerateRandomInt, V, false);
        foreach (Tile tile in res)
        {
            tile.properties.mask = TileMask.Corrupted();
            player.AddNewTileToPool(tile);
        }
    }
}