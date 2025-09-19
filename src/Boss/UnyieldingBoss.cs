using System;
using System.Collections.Generic;
using Aotenjo;

public class UnyieldingBoss : Boss
{
    private static readonly double multiplier = 0.03D;

    public UnyieldingBoss() : base("Unyielding")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.PostDiscardTileEvent += IncreaseLevelTarget;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.PostDiscardTileEvent -= IncreaseLevelTarget;
    }

    private void IncreaseLevelTarget(PlayerEvent eventData)
    {
        Player player = eventData.player;
        player.levelTarget *= (1D + multiplier);
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new UnyieldingBossArtifact(baseArtifact);
    }

    public override Boss GetHarderBoss() => new UnyieldingHarderBoss();
}

public class UnyieldingBossArtifact : Artifact
{
    private readonly Artifact baseArtifact;

    public UnyieldingBossArtifact(Artifact baseArtifact) : base("Unyielding_reversed", Rarity.COMMON)
    {
        this.baseArtifact = baseArtifact;
    }

    public override void AddDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone)
    {
        base.AddDiscardTileEffects(player, tile, onDiscardTileEffects, withForce, isClone);
        onDiscardTileEffects.Add(new GrowFuEffect(baseArtifact, tile, 3).OnTile(tile));
    }
}

[Serializable]
[HarderBoss]
public class UnyieldingHarderBoss : UnyieldingBoss
{
    public UnyieldingHarderBoss() : base() { }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnAddSingleDiscardTileAnimationEffectEvent += OnSingleDiscardAnim;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnAddSingleDiscardTileAnimationEffectEvent -= OnSingleDiscardAnim;
    }

    private void OnSingleDiscardAnim(Player player, List<IAnimationEffect> list, Tile tile, bool forceDiscard)
    {
        list.Add(new IncreaseTargetEffect(1.03D, "unyield").OnTile(tile));
        list.Add(new CorruptEffect(tile).OnTile(tile));
    }

    public override Boss GetHarderBoss() => this; 
}