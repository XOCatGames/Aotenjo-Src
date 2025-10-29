using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class TrimmedDeckPlayerEffect : StarterBoostEffect
{
    public TrimmedDeckPlayerEffect() : base("starter_trimmed")
    {
    }

    public override void Boost(Player player)
    {
        List<Tile> tiles = new List<Tile>();
        foreach (Tile bp in DistinctBy(player.GetAllTiles().Where(t => t.IsHonor(player) || t.IsNumbered()),
                     (t => t.ToString())))
        {
            Tile toRemove = player.GetAllTiles().Where(t => bp.CompatWith(t)).First();
            player.RemoveTileFromPool(toRemove);
            tiles.Add(toRemove);
        }

        tiles.Sort();

        MessageManager.Instance.OnRemoveTileEvent(tiles);
    }

    public static IEnumerable<T> DistinctBy<T, TKey>(IEnumerable<T> items, Func<T, TKey> property)
    {
        return items.GroupBy(property).Select(x => x.First());
    }
}