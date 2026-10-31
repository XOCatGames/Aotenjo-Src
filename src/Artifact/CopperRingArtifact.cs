using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 将整副牌中数字牌的点数去重后计符，同点数跨类别仍只计算一次，字牌不计入。
    /// </summary>
    public sealed class CopperRingArtifact : Artifact
    {
        public CopperRingArtifact() : base("copper_ring", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.IsNumbered();
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            int count = permutation.ToTiles().Where(t => t.IsNumbered()).Select(t => t.GetOrder()).Distinct().Count();
            effects.Add(ScoreEffect.AddFu(count * 8, this));
        }
    }
}
