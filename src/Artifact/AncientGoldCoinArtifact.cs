using System.Collections.Generic;

namespace Aotenjo
{
    public class AncientGoldCoinArtifact : Artifact, IFuProvider
    {
        public AncientGoldCoinArtifact() : base("ancient_gold_coin", Rarity.RARE)
        {
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToAddFuFormat(GetFu(player));
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            base.AddOnBlockEffects(player, permutation, block, effects);
            if (player.GetMoney() < 0) return;
            effects.Add(ScoreEffect.AddFu(() => GetFu(player), this));
        }

        public double GetFu(Player player) => player.GetMoney();
    }
}