using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 整副面子都为七至九的数字牌时加番；高亮只检查单牌，仍由原面子计分入口决定是否触发。
    /// </summary>
    public sealed class SymbolBigArtifact : Artifact
    {
        public SymbolBigArtifact() : base("symbol_big", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.IsNumbered() && tile.GetOrder() >= 7;
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if (block.All(a => a.IsNumbered() && a.GetOrder() >= 7)) effects.Add(ScoreEffect.AddFan(8, this));
        }
    }
}
