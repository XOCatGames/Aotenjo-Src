using System.Collections.Generic;
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
        EventBus.Subscribe<PlayerRoundEvent.Start.Pre>(IncreaseLevelTarget);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerRoundEvent.Start.Pre>(IncreaseLevelTarget);
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new UnrestReversedArtifact(name);
    }

    /// <summary>
    /// 旧版未实现的反转能力仍显式抛出异常，保留兼容身份；类拆分不擅自新增玩法。
    /// </summary>
    private sealed class UnrestReversedArtifact : Artifact
    {
        public UnrestReversedArtifact(string bossName) : base($"artifact_{bossName}_reversed", Rarity.COMMON)
        {
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            throw new NotImplementedException();
        }
    }

    private void IncreaseLevelTarget(PlayerEvent eventData)
    {
        Player player = eventData.player;
        player.IncreaseTargetMultiplier(multiplier);
    }
}