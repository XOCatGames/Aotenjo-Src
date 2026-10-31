using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 只处理本次打出的七至九万，且不重复转换已有铜牌；变换效果仍绑定触发的本局对象。
    /// </summary>
    public sealed class CopperStatueArtifact : Artifact
    {
        public CopperStatueArtifact() : base("copper_statue", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.GetOrder() >= 7 && tile.GetCategory() == Tile.Category.Wan;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.IsPlayingTile(tile)) return;
            if(tile.GetOrder() >= 7 && tile.GetCategory() == Tile.Category.Wan && tile.properties.material.GetRegName() != TileMaterial.COPPER.GetRegName())
            {
                effects.Add(new TransformMaterialEffect(TileMaterial.COPPER, this, tile, "effect_transform_copperize_name"));
            }
        }
    }
}
