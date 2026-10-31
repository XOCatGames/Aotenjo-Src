using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 只对全部为数字牌的刻子追加面子符数，字牌面子不触发。
    /// </summary>
    public sealed class StoneLionArtifact : Artifact
    {
        public StoneLionArtifact() : base("stone_lion", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.IsNumbered();
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if ((block.IsAAA()) && block.All(t => t.IsNumbered()))
                effects.Add(ScoreEffect.AddFu(30, this));
        }
    }
}
