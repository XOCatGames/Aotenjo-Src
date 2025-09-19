using System;
using System.Collections.Generic;
using Aotenjo;

public class UnaidedBoss : Boss
{
    protected virtual double RareMul  => 0.8;
    protected virtual double EpicMul  => 0.7;

    public UnaidedBoss() : base("Unaided")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnPostAddScoringAnimationEffectEvent += OnPostAddScoringAnimationEffect;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddScoringAnimationEffectEvent -= OnPostAddScoringAnimationEffect;
    }
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return Artifact.CreateOnSelfEffectArtifact($"{this.name}_reversed", 
            Rarity.COMMON,
            (player, perm, effects) =>
            {
                List<Artifact> artifacts = player.GetArtifacts();
                
                effects.Add(new TextEffect("effect_boss_unaided_reversed", baseArtifact));
                foreach (var item in artifacts)
                {
                    if (item.GetRarity() == Rarity.RARE)
                    {
                        effects.Add(ScoreEffect.MulFan(1.2, item));
                    }
                    else if (item.GetRarity() == Rarity.EPIC)
                    {
                        effects.Add(ScoreEffect.MulFan(1.5, item));
                    }
                }
            });
    }
    private void OnPostAddScoringAnimationEffect(Permutation permutation, Player player, List<IAnimationEffect> list)
    {
        foreach (var item in player.GetArtifacts())
        {
            if (item.GetRarity() == Rarity.RARE)
            {
                list.Add(ScoreEffect.MulFan(RareMul, item));
            }
            else if (item.GetRarity() == Rarity.EPIC)
            {
                list.Add(ScoreEffect.MulFan(EpicMul, item));
            }
        }
    }

    public override Boss GetHarderBoss() => new UnaidedHarderBoss();
}

[Serializable]
[HarderBoss]
public class UnaidedHarderBoss : UnaidedBoss
{
    protected override double RareMul => 0.8;
    protected override double EpicMul => 0.6;

    public UnaidedHarderBoss() : base() { }

    public override Boss GetHarderBoss() => this;
}