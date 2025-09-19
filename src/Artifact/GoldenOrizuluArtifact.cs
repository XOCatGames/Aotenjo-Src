using System;
using System.Collections.Generic;
using Aotenjo;

public class GoldenOrizuluArtifact : Artifact
{
    private const int AMOUNT = 4;

    public GoldenOrizuluArtifact() : base("golden_orizulu", Rarity.RARE)
    {
    }

    public override string GetDescription(Func<string, string> localizer)
    {
        return string.Format(base.GetDescription(localizer), AMOUNT);
    }

    public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
    {
        base.AddOnRoundEndEffects(player, permutation, effects);
        effects.Add(new EarnMoneyEffect(AMOUNT, this));
    }
}