using System.Collections.Generic;
using System;
using Aotenjo;

public class UndefeatableBoss : Boss
{
    protected virtual double Multiplier => 3.0;

    public UndefeatableBoss() : base("Undefeatable")
    {
    }

    public override double GetStartingTarget(Player player, double basicTarget) => basicTarget * Multiplier;

    public override void SubscribeToPlayerEvents(Player player)
    {
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new UndefeatableReversedArtifact(name, baseArtifact);
    }

    /// <summary>
    /// 每次反转创建独立遗物类型；保留原触发条件、结算顺序及来源引用，不在结算时按 ID 重查吸收者。
    /// </summary>
    private sealed class UndefeatableReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;

        public UndefeatableReversedArtifact(string bossName, Artifact baseArtifact) : base($"{bossName}_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            effects.Add(ScoreEffect.MulFan(3.0, baseArtifact));
        }
    }
    
    public override Boss GetHarderBoss() => new UndefeatableHarderBoss();
}

[HarderBoss]
public class UndefeatableHarderBoss : UndefeatableBoss
{
    protected override double Multiplier => 5.0;
}