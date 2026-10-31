using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 逐牌按索子兼容性加符，高亮保留同一类别兼容规则。
    /// </summary>
    public sealed class BambooPenContainerArtifact : Artifact
    {
        public BambooPenContainerArtifact() : base("bamboo_pen_container", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.CompatWithCategory(Tile.Category.Suo);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (tile.CompatWithCategory(Tile.Category.Suo)) effects.Add(ScoreEffect.AddFu(10, this));
        }
    }
}
