using System;
using System.Collections.Generic;
using Aotenjo;

public class GreedlessBoss : Boss
{
    protected virtual double DebtMultiplier => 0.05D;
    protected virtual int GetAmount(Player player) => (player.Level - 1) / 8 + 1;  

    public GreedlessBoss() : base("Greedless")
    {
    }

    public override string GetDescription(Player player, Func<string, string> loc)
    {
        return string.Format(base.GetDescription(player, loc), GetAmount(player));
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        EventBus.Subscribe<PlayerEvents.OnPostAddOnTileAnimationEffectEvent>(player, Snatch);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerEvents.OnPostAddOnTileAnimationEffectEvent>(player, Snatch);
    }

    private void Snatch(Permutation permutation, Player player, List<OnTileAnimationEffect> list)
    {
        if (permutation == null) return;

        foreach (Tile tile in player.GetPlayingTiles()){
            list.Add(new SnatchEffect(tile, GetAmount(player), DebtMultiplier).OnTile(tile));
        }
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new GreedlessReversedArtifact(this);
    }

    /// <summary>
    /// 保留遭遇对象以读取其虚方法和难度参数，金额仍在每次打牌时按当前关卡计算；不在构造时冻结收益。
    /// </summary>
    private sealed class GreedlessReversedArtifact : Artifact
    {
        private readonly GreedlessBoss boss;

        public GreedlessReversedArtifact(GreedlessBoss boss) : base($"{boss.name}_reversed", Rarity.COMMON)
        {
            this.boss = boss;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.IsPlayingTile(tile)) return;
            effects.Add(new EarnMoneyEffect(boss.GetAmount(player)));
            effects.Add(new IncreaseTargetEffect(1 + boss.DebtMultiplier, "Greedless_reversed"));
        }
    }
    
    protected class SnatchEffect : Effect
    {
        private Tile tile;
        private readonly int cost;
        private readonly double debtMulPerDollar;

        public SnatchEffect(Tile tile, int cost, double debtMulPerDollar)
        {
            this.tile = tile;
            this.cost = cost;
            this.debtMulPerDollar = debtMulPerDollar;
        }

        public override string GetEffectDisplay(Player player, Func<string, string> func)
        {
            return $"-<style=\"money\">{cost}</style>";
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return "ERROR";
        }

        public override Artifact GetEffectSource()
        {
            return null;
        }

        public override void Ingest(Player player)
        {
            int preMoney = player.GetMoney();
            player.SpendMoney(cost);
            double originalTarget = player.levelTarget / (1D + debtMulPerDollar * (-Math.Min(0, preMoney)));

            player.levelTarget = originalTarget * (1D + debtMulPerDollar * (-Math.Min(0, player.GetMoney())));
        }

        public override string GetSoundEffectName()
        {
            return "SpendMoney";
        }
    }
}