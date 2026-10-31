using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 只对偶数数字牌逐牌追加符数；字牌的偶数点数不能触发。
    /// </summary>
    public sealed class MobiusRingArtifact : Artifact
    {
        public MobiusRingArtifact() : base("mobius_ring", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.IsNumbered() && tile.GetOrder() % 2 == 0;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (tile.IsNumbered() && tile.GetOrder() % 2 == 0)
            {
                effects.Add(ScoreEffect.AddFu(12, this));
            }
        }
    }
}
