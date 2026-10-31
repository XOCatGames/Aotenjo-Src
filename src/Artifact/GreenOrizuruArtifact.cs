using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 仅雀头原始类别全部为索子时追加符数，仍在自身阶段触发一次。
    /// </summary>
    public sealed class GreenOrizuruArtifact : Artifact
    {
        public GreenOrizuruArtifact() : base("green_orizulu", Rarity.COMMON)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            if (permutation.JiangFulfillAll(t => t.GetCategory() == Tile.Category.Suo))
                effects.Add(ScoreEffect.AddFu(60, this));
        }
    }
}
