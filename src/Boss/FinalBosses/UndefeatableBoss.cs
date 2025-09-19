using System;
using Aotenjo;

public class UndefeatableBoss : Boss
{
    protected virtual double Multiplier => 3.0;

    public UndefeatableBoss() : base("Undefeatable")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        EventBus.Subscribe<PlayerRoundEvent.Start.Post>(PostRoundStart);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerRoundEvent.Start.Post>(PostRoundStart);
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return Artifact.CreateOnSelfEffectArtifact($"{this.name}_reversed", 
            Rarity.COMMON,
            (player, perm, effects) =>
            {
                effects.Add(ScoreEffect.MulFan(3.0, baseArtifact));
            });
    }
    
    private void PostRoundStart(PlayerEvent eventData)
    {
        Player player = eventData.player;
        player.levelTarget *= Multiplier;
    }

    public override Boss GetHarderBoss() => new UndefeatableHarderBoss();
}

[HarderBoss]
public class UndefeatableHarderBoss : UndefeatableBoss
{
    protected override double Multiplier => 5.0;
}