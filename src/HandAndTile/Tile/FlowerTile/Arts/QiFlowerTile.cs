using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class QiFlowerTile : OneTimeUseFlowerTile
{
    public QiFlowerTile() : base(Category.SiYi, 2)
    {
    }

    public override void OnPlayed(Player player, Permutation perm)
    {
        base.OnPlayed(player, perm);
        List<Tile> tiles = player.GetHandDeckCopy().Where(t => t.IsHonor(player) || t.IsNumbered()).ToList();
        if (!tiles.Any())
        {
            return;
        }

        int minCatTileCount = tiles.Select(x => x.GetCategory()).Select(c => tiles.Count(t => t.GetCategory() == c))
            .Min();
        List<Tile> tilesToDiscard = tiles
            .Where(t => tiles.Count(t2 => t2.GetCategory() == t.GetCategory()) == minCatTileCount).ToList();

        foreach (Tile t in tilesToDiscard)
        {
            EventManager.Instance.EnqueueToDiscard(t, true);
        }
    }
}