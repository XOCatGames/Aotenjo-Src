using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class PlumFlowerTile : OneTimeUseFlowerTile
{
    public PlumFlowerTile() : base(Category.JunZi, 1)
    {
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);
        if (used) return;
        effects.Add(new TextEffect("effect_plum_name").OnTile(this));
        foreach (Tile tile in player.GetSelectedTilesCopy().OrderBy(t => player.TileSettlingOrder(t, perm)))
        {
            effects.Add(new CleanseEffect(null, tile).OnTile(tile));
        }

        used = true;
    }
}