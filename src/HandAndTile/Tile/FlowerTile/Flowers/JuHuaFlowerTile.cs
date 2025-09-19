using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class JuHuaFlowerTile : OneTimeUseFlowerTile
{
    public JuHuaFlowerTile() : base(Category.JunZi, 3)
    {
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);
        if (used) return;
        effects.Add(new OnTileAnimationEffect(this, new TextEffect("effect_chrysanthemum_name")));
        Tile[] tiles = { perm.jiang.tile1, perm.jiang.tile2 };

        foreach (Tile tile in tiles)
        {
            List<OnTileAnimationEffect> lst = effects
                .Where(e => e is OnTileAnimationEffect te && te.tile == tile && !te.isClone)
                .Select(e => ((OnTileAnimationEffect)e).Clone()).ToList();
            effects.AddRange(lst);
        }

        used = true;
    }
}