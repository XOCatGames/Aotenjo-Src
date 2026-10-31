using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 逐牌按三元牌兼容性加符，高亮沿用同一兼容规则，避免排除可兼容该类别的特殊牌。
    /// </summary>
    public sealed class TeaCupArtifact : Artifact
    {
        public TeaCupArtifact() : base("tea_cup", Rarity.RARE)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.CompatWithCategory(Tile.Category.Jian);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (tile.CompatWithCategory(Tile.Category.Jian)) effects.Add(ScoreEffect.AddFu(30, this));
        }
    }
}
