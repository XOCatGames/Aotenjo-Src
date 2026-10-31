using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 只处理本次打出的红色图案牌，已染红的牌不重复变色；效果引用当前对象和原牌。
    /// </summary>
    public sealed class CinnabarPenArtifact : Artifact
    {
        public CinnabarPenArtifact() : base("cinnabar_pen", Rarity.EPIC)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.ContainsRed(player);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.IsPlayingTile(tile)) return;
            if (tile.ContainsRed(player) && tile.properties.font.GetRegName() != TileFont.RED.GetRegName())
            {
                effects.Add(new TransformColorEffect(TileFont.RED, this, tile, "effect_dyed_name"));
            }
        }
    }
}
