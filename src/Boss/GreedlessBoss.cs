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
        player.OnPostAddOnTileAnimationEffectEvent += Snatch;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddOnTileAnimationEffectEvent -= Snatch;
    }

    private void Snatch(Permutation permutation, Player player, List<OnTileAnimationEffect> list)
    {
        if (permutation == null) return;

        foreach (Tile tile in player.GetSelectedTilesCopy()){
            list.Add(new SnatchEffect(tile, GetAmount(player), DebtMultiplier).OnTile(tile));
        }
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return Artifact.CreateOnTileEffectArtifact($"{this.name}_reversed", 
            Rarity.COMMON,
            (player, perm, tile, effects) =>
            {
                if (!player.Selecting(tile)) return;
                effects.Add(new EarnMoneyEffect(GetAmount(player)));
                effects.Add(new IncreaseTargetEffect(1 + DebtMultiplier, "desireless_reversed"));
            });
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
    }
}