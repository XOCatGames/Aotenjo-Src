using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 整副牌含雀头且原始类别只有一种时追加倍率；悬停高亮继续按已有面子的类别判断。
    /// </summary>
    public sealed class GoldIngotArtifact : Artifact
    {
        public GoldIngotArtifact() : base("gold_ingot", Rarity.EPIC)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            Permutation perm = player.GetAccumulatedPermutation();
            return perm == null || perm.blocks.All(b => b.OfCategory(tile.GetCategory()));
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            if (permutation.ToTiles().Select(t => t.GetCategory()).Distinct().Count() == 1) effects.Add(ScoreEffect.MulFan(2.5, this));
        }
    }
}
