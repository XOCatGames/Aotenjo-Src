using System;
using System.Collections.Generic;
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
        effects.Add(new TextEffect("effect_chrysanthemum_name").OnTile(this));
        Tile[] tiles = { perm.jiang.tile1, perm.jiang.tile2 };

        foreach (Tile tile in tiles)
        {
            effects.Add(new TileScoringEffectAppendEffect(player, tile, perm, player.playHandEffectStack));
        }

        used = true;
    }
}
