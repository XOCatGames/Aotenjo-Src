using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 整副面子都兼容风牌时追加番数，保留兼容类别而非原始类别的判断。
    /// </summary>
    public sealed class CompassArtifact : Artifact
    {
        public CompassArtifact() : base("compass", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.CompatWithCategory(Tile.Category.Feng);
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if (block.All(a => a.CompatWithCategory(Tile.Category.Feng))) effects.Add(ScoreEffect.AddFan(12, this));
        }
    }
}
