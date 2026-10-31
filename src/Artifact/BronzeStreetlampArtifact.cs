using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 逐牌检查数字牌的点数是否为三的倍数，符合时追加本实例来源的加番效果。
    /// </summary>
    public sealed class BronzeStreetlampArtifact : Artifact
    {
        public BronzeStreetlampArtifact() : base("bronze_street_lamp", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.IsNumbered() && tile.GetOrder() % 3 == 0;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (tile.IsNumbered() && tile.GetOrder() % 3 == 0) effects.Add(ScoreEffect.AddFan(1, this));
        }
    }
}
