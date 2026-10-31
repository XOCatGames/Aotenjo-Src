using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 按整副牌的数字牌类别判断加番，但仍对每个面子分别触发；无累计牌型或悬停字牌时保留高亮。
    /// </summary>
    public sealed class SilverIngotArtifact : Artifact
    {
        public SilverIngotArtifact() : base("silver_ingot", Rarity.RARE)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            Permutation perm = player.GetAccumulatedPermutation();
            if (tile.IsHonor(player) || perm == null) return true;
            return perm.blocks.All(b => b.OfCategory(tile.GetCategory()));
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if (permutation.ToTiles().Where(t => t.IsNumbered()).Select(t => t.GetCategory()).Distinct().Count() == 1)
                effects.Add(ScoreEffect.AddFan(6, this));
        }
    }
}
