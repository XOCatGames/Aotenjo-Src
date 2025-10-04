using System;
using Aotenjo;

public class UnrestBoss : Boss
{
    private static readonly double multiplier = 0.3;

    public UnrestBoss() : base("Unrest")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.PreRoundStartEvent += IncreaseLevelTarget;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.PreRoundStartEvent -= IncreaseLevelTarget;
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return Artifact.CreateOnTileEffectArtifact($"artifact_{this.name}_reversed", 
            Rarity.COMMON,
            (player, perm, tile, effects) =>
            {
                throw new NotImplementedException();
            });
    }

    private void IncreaseLevelTarget(PlayerEvent eventData)
    {
        Player player = eventData.player;
        player.IncreaseTargetMultiplier(multiplier);
    }
}