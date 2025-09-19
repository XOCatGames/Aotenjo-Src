using System;
using System.Collections.Generic;
using Aotenjo;

[Serializable]
public class SummerFlowerTile : FlowerTile
{
    private const float FU = 30;
    private const int MONEY_LOSS = 1;

    public SummerFlowerTile() : base(Category.SiJi, 2)
    {
    }

    public override string GetFlowerDescription(Func<string, string> loc)
    {
        return string.Format(base.GetFlowerDescription(loc), FU, MONEY_LOSS);
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);

        effects.Add(new TextEffect("effect_summer_name").OnTile(this));
        effects.Add(ScoreEffect.AddFu(FU, null).OnTile(this));
    }

    public override void AppendRoundEndEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendRoundEndEffect(effects, player, perm);

        effects.Add(new EarnMoneyEffect(-MONEY_LOSS, null).OnTile(this));
    }
}