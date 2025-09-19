using System;
using System.Collections.Generic;
using Aotenjo;

[Serializable]
public class OrchidFlowerTile : OneTimeUseFlowerTile
{
    private const float MUL = 1.5f;


    public OrchidFlowerTile() : base(Category.JunZi, 2)
    {
    }

    public override string GetFlowerDescription(Func<string, string> loc)
    {
        return string.Format(base.GetFlowerDescription(loc), MUL);
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);
        if (used) return;
        effects.Add(new OnTileAnimationEffect(this, new TextEffect("effect_orchid_name")));
        effects.Add(new OnTileAnimationEffect(this, ScoreEffect.MulFan(MUL, null)));
        used = true;
    }
}