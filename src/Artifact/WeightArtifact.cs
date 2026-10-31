using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 只累加兼容万子牌的点数，严格大于六十时追加倍率，等于六十仍不触发。
    /// </summary>
    public sealed class WeightArtifact : Artifact
    {
        public WeightArtifact() : base("weight", Rarity.RARE)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.CompatWithCategory(Tile.Category.Wan);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            int num = permutation.ToTiles().Where(t => t.CompatWithCategory(Tile.Category.Wan)).Select(t => t.GetOrder()).Sum();
            if (num > 60)
            {
                effects.Add(ScoreEffect.MulFan(2, this));
            }
        }
    }
}
