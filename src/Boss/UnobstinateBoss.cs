using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;
using UnityEngine;

[Serializable]
public class UnobstinateBoss : Boss
{
    protected virtual double Multiplier => 1.04D;

    public UnobstinateBoss() : base("Unobstinate")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        EventBus.Subscribe<PlayerEvents.OnPostAddOnTileAnimationEffectEvent>(player, Persist);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerEvents.OnPostAddOnTileAnimationEffectEvent>(player, Persist);
    }

    private void Persist(Permutation permutation, Player player, List<OnTileAnimationEffect> list)
    {
        if (permutation == null) return;
        List<Tile> tiles = new(player.GetSelectedTilesCopy());
        foreach (var tile in tiles)
        {
            if (permutation.ToTiles().Any(a => a != tile && a.CompatWith(tile)))
                list.Add(new PersistEffect(tile, Multiplier).OnTile(tile));
        }
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new UnobstinateReversedArtifact(name, baseArtifact);
    }

    /// <summary>
    /// 每次反转创建独立遗物类型；保留原触发条件、结算顺序及来源引用，不在结算时按 ID 重查吸收者。
    /// </summary>
    private sealed class UnobstinateReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;

        public UnobstinateReversedArtifact(string bossName, Artifact baseArtifact) : base($"{bossName}_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            int count = permutation.ToTiles().Count(a => a != tile && a.CompatWith(tile));
            if (count > 0)
                effects.Add(ScoreEffect.AddFu(count * 5, baseArtifact));
        }
    }

    public override Boss GetHarderBoss() => new UnobstinateHarderBoss();

    [Serializable]
    [HarderBoss]
    private class PersistEffect : Effect
    {
        [SerializeField] private Tile tile;
        private readonly double mul;

        public PersistEffect(Tile tile, double mul)
        {
            this.tile = tile;
            this.mul = mul;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_persist");
        }

        public override Artifact GetEffectSource()
        {
            return null;
        }

        public override void Ingest(Player player)
        {
            player.levelTarget = player.GetLevelTarget() * mul;
        }
    }
}

[Serializable]
[HarderBoss]
public class UnobstinateHarderBoss : UnobstinateBoss
{
    protected override double Multiplier => 1.08D;

    public UnobstinateHarderBoss() : base() { }

    public override Boss GetHarderBoss() => this;
}