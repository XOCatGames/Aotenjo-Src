using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 以整副牌的原始类别去重计数，恰好三类时才追加倍率，不把类别条件改为至少三类。
    /// </summary>
    public sealed class KaleidoscopeArtifact : Artifact
    {
        public KaleidoscopeArtifact() : base("kaleidoscope", Rarity.EPIC)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            if (permutation.ToTiles().Select(t => t.GetCategory()).Distinct().Count() == 3) effects.Add(ScoreEffect.MulFan(3, this));
        }
    }
}
