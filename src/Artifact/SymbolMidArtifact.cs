using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 整副面子都为四至六的数字牌时加番；单牌高亮与面子触发使用相同的数字范围。
    /// </summary>
    public sealed class SymbolMidArtifact : Artifact
    {
        public SymbolMidArtifact() : base("symbol_mid", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.IsNumbered() && tile.GetOrder() >= 4 && tile.GetOrder() <= 6;
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if (block.All(a => a.IsNumbered() && a.GetOrder() >= 4 && a.GetOrder() <= 6)) effects.Add(ScoreEffect.AddFan(8, this));
        }
    }
}
