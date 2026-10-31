using System;
using Aotenjo;

/// <summary>
/// 无视在每个实际计分效果结算后扣符，首轮东、南、西、北风圈分别扣除 1、2、4、6 符；此后每圈多扣 2 符。
/// 与观察者共用效果触发事件，但只作用于出牌计分阶段，不影响弃置、商店或小局结束效果。
/// </summary>
[Serializable]
public sealed class SightlessBoss : Boss
{
    [NonSerialized] private Player subscribedPlayer;

    public SightlessBoss() : base("Sightless") { }

    private static int GetFuAmount(Player player)
    {
        // 每四关为一圈，按总关数累计圈数，避免圈风每十六关循环时重置扣符。
        // 首轮依次扣除 1、2、4、6 符；第十七关起依次为 8、10、12……，空玩家预览按第一圈处理。
        int circle = (Math.Max(1, player?.Level ?? 1) - 1) / 4;
        return circle < 3 ? 1 << circle : 6 + (circle - 3) * 2;
    }

    public override string GetDescription(Player player, Func<string, string> loc)
    {
        // {0} 是当前圈风对应的扣符数值，预览和实战共用同一计算。
        return string.Format(base.GetDescription(player, loc), GetFuAmount(player));
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        // 同一实例重绑或读档恢复时先退订，防止一次计分重复扣符。
        if (subscribedPlayer != null) UnsubscribeFromPlayerEvents(subscribedPlayer);
        subscribedPlayer = player;
        EventBus.Subscribe<PlayerEvents.PostIngestEffectEvent>(player, OnScoringEffect);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        if (subscribedPlayer == null) return;
        EventBus.Unsubscribe<PlayerEvents.PostIngestEffectEvent>(subscribedPlayer, OnScoringEffect);
        subscribedPlayer = null;
    }

    private void OnScoringEffect(PlayerEvents.PostIngestEffectEvent evt)
    {
        if (!evt.player.IsResolvingScoringEffects) return;
        // 直接修正已结算的符数，不追加新的计分效果；否则扣符会再次触发自己，
        // 反转能力也可能与寄生虫相互触发。保留原计分次数，并沿用 Score 的零下限。
        evt.player.RoundAccumulatedScore = evt.player.RoundAccumulatedScore.AddFu(-GetFuAmount(evt.player));
    }

    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new SightlessReversedArtifact(baseArtifact);
    }

    private sealed class SightlessReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;
        private Player subscribedPlayer;

        public SightlessReversedArtifact(Artifact baseArtifact) : base("Sightless_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override string GetDescription(Player player, Func<string, string> loc)
        {
            // {0} 是当前圈风对应的加符数值，随玩家换圈更新。
            return string.Format(base.GetDescription(player, loc), GetFuAmount(player));
        }

        public override void SubscribeToPlayer(Player player)
        {
            // 反转能力也限定到所属玩家，葫芦重绑或读档时不能残留上一份订阅。
            if (subscribedPlayer != null) UnsubscribeToPlayer(subscribedPlayer);
            base.SubscribeToPlayer(player);
            subscribedPlayer = player;
            EventBus.Subscribe<PlayerEvents.PostIngestEffectEvent>(player, OnScoringEffect);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            if (subscribedPlayer != null)
                EventBus.Unsubscribe<PlayerEvents.PostIngestEffectEvent>(subscribedPlayer, OnScoringEffect);
            subscribedPlayer = null;
            base.UnsubscribeToPlayer(player);
        }

        private void OnScoringEffect(PlayerEvents.PostIngestEffectEvent evt)
        {
            if (!evt.player.IsResolvingScoringEffects ||
                (baseArtifact != null && evt.player.IsArtifactDebuffed(baseArtifact))) return;
            // 葫芦反转只修改这次计分的结果，不产生额外触发；同样受葫芦压制状态约束。
            evt.player.RoundAccumulatedScore = evt.player.RoundAccumulatedScore.AddFu(GetFuAmount(evt.player));
        }
    }
}
