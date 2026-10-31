using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 逐牌按万子兼容性加符，高亮保留同一类别兼容规则。
    /// </summary>
    public sealed class BankCardArtifact : Artifact
    {
        public BankCardArtifact() : base("bank_card", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.CompatWithCategory(Tile.Category.Wan);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (tile.CompatWithCategory(Tile.Category.Wan)) effects.Add(ScoreEffect.AddFu(12, this));
        }
    }
}
