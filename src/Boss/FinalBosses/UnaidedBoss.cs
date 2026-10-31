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
        EventBus.Subscribe<PlayerEvents.OnPostAddScoringAnimationEffectEvent>(player, OnPostAddScoringAnimationEffect);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerEvents.OnPostAddScoringAnimationEffectEvent>(player, OnPostAddScoringAnimationEffect);
    }
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new UnaidedReversedArtifact(name, baseArtifact);
    }

    /// <summary>
    /// 提示归属于吸收者，各次倍率仍归属于对应的已持有遗物，保留原来的触发顺序和来源。
    /// </summary>
    private sealed class UnaidedReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;

        public UnaidedReversedArtifact(string bossName, Artifact baseArtifact) : base($"{bossName}_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
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
        }
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