using System.Collections.Generic;
using Aotenjo;
using System;
using System.Linq;

public class HeartlessBoss : Boss
{
    public HeartlessBoss() : base("Heartless")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        EventBus.Subscribe<PlayerEvents.OnPostAddOnTileAnimationEffectEvent>(player, Corrupt);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerEvents.OnPostAddOnTileAnimationEffectEvent>(player, Corrupt);
    }

    private void Corrupt(Permutation perm, Player player, List<OnTileAnimationEffect> list)
    {
        if (perm == null) return;

        if (!NeedCorrupt(perm, player)) return;

        foreach (Tile t in player.GetPlayingTiles().OrderBy(t => player.TileSettlingOrder(t, player.GetAccumulatedPermutation())))
            list.Add(new CorruptEffect(t).OnTile(t));
    }

    protected virtual bool NeedCorrupt(Permutation perm, Player player)
        => !perm.JiangFulfillAll(t => t.IsYaoJiu(player));

    public override Boss GetHarderBoss() => new HeartlessHarderBoss();
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new HeartlessReversedArtifact(name, baseArtifact);
    }

    /// <summary>
    /// 每次反转创建独立遗物类型；保留原触发条件、结算顺序及来源引用，不在结算时按 ID 重查吸收者。
    /// </summary>
    private sealed class HeartlessReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;

        public HeartlessReversedArtifact(string bossName, Artifact baseArtifact) : base($"{bossName}_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!permutation.JiangFulfillAll(t => !t.IsYaoJiu(player))) return;
            if (!tile.properties.IsDebuffed()) return;
            effects.Add(new CleanseEffect(baseArtifact, tile));
        }
    }
}

[Serializable]
[HarderBoss]
public class HeartlessHarderBoss : HeartlessBoss
{
    protected override bool NeedCorrupt(Permutation perm, Player player)
    {
        return perm.JiangFulfillAll(t =>
                   t.IsHonor(player)
                || (t.IsNumbered() && t.GetOrder() % 2 == 1));
    }

    public override Boss GetHarderBoss() => this;
}