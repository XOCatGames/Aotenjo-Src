using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 每个面子固定追加符数；骰子的强制点数仍由骰子在触发时查询持有关系。
    /// </summary>
    public sealed class LeadBlockArtifact : Artifact
    {
        public LeadBlockArtifact() : base("lead_block", Rarity.RARE)
        {
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            effects.Add(ScoreEffect.AddFu(20, this));
        }
    }
}
