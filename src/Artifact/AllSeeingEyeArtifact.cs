using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 按整副牌的原始类别去重，恰好五种类别时追加一次倍率。
    /// </summary>
    public sealed class AllSeeingEyeArtifact : Artifact
    {
        public AllSeeingEyeArtifact() : base("all_seeing_eye", Rarity.EPIC)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            if (permutation.ToTiles().Select(t => t.GetCategory()).Distinct().Count() == 5) effects.Add(ScoreEffect.MulFan(5, this));
        }
    }
}
