using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 仅雀头原始类别全部为万子时追加番数，保留与类别兼容判定的原有区别。
    /// </summary>
    public sealed class RedOrizuruArtifact : Artifact
    {
        public RedOrizuruArtifact() : base("red_orizulu", Rarity.COMMON)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            if (permutation.JiangFulfillAll(t => t.GetCategory() == Tile.Category.Wan))
                effects.Add(ScoreEffect.AddFan(5, this));
        }
    }
}
