using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 所有面子都不是刻子时追加符数；保持对整副牌的单次判断，不改动特殊牌型规则。
    /// </summary>
    public sealed class LeafArtifact : Artifact
    {
        public LeafArtifact() : base("leaf", Rarity.COMMON)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            if (permutation.blocks.All(b => !b.IsAAA())) effects.Add(ScoreEffect.AddFu(60, this));
        }
    }
}
