using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 整副面子都为一至三的数字牌时加番；字牌不能因点数较小而触发。
    /// </summary>
    public sealed class SymbolSmallArtifact : Artifact
    {
        public SymbolSmallArtifact() : base("symbol_small", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.IsNumbered() && tile.GetOrder() <= 3;
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if (block.All(a => a.IsNumbered() && a.GetOrder() <= 3)) effects.Add(ScoreEffect.AddFan(8, this));
        }
    }
}
