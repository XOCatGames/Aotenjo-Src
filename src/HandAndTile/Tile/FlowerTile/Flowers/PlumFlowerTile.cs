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
        effects.Add(new OnTileAnimationEffect(this, new TextEffect("effect_plum_name")));
        foreach (Tile tile in player.GetSelectedTilesCopy().OrderBy(t => player.TileSettlingOrder(t, perm)))
        {
            effects.Add(new OnTileAnimationEffect(tile, new CleanseEffect(null, tile)));
        }

        used = true;
    }
}