using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 逐牌使用当前玩家的红色图案判定追加符数，高亮遵守同一规则。
    /// </summary>
    public sealed class BloodFeatherArtifact : Artifact
    {
        public BloodFeatherArtifact() : base("blood_feather", Rarity.RARE)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.ContainsRed(player);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (tile.ContainsRed(player)) effects.Add(ScoreEffect.AddFu(12, this));
        }
    }
}
