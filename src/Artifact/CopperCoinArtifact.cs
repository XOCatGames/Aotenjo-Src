using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 逐牌按饼子兼容性加符，保留旧钱币注册名，确保存档、资源和配方仍解析同一身份。
    /// </summary>
    public sealed class CopperCoinArtifact : Artifact
    {
        public CopperCoinArtifact() : base("ancient_copper_coin", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.CompatWithCategory(Tile.Category.Bing);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (tile.CompatWithCategory(Tile.Category.Bing)) effects.Add(ScoreEffect.AddFu(8, this));
        }
    }
}
