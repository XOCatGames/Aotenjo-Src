using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 含五与兼容四五六是两个独立判定，可在同一面子分别追加加符效果；保留原有触发次数。
    /// </summary>
    public sealed class PeachWoodSwordArtifact : Artifact
    {
        public PeachWoodSwordArtifact() : base("peach_wood_sword", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.IsNumbered() && tile.GetOrder() == 5;
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if (block.Any(a => a.IsNumbered() && a.GetOrder() == 5)) effects.Add(ScoreEffect.AddFu(30, this));
            if (block.CompatWithNumbers("456")) effects.Add(ScoreEffect.AddFu(30, this));
        }
    }
}
