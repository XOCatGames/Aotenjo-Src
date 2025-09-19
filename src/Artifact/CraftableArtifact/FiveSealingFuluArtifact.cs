using System.Collections.Generic;

namespace Aotenjo
{
    public class FiveSealingFuluArtifact : CraftableArtifact
    {
        public FiveSealingFuluArtifact() : base("ghost_of_five_fulu", Rarity.RARE)
        {
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (tile.IsNumbered() && tile.GetOrder() == 5 &&
                tile.properties.material.GetRegName() == TileMaterial.PLAIN.GetRegName())
            {
                effects.Add(new TransformMaterialEffect(TileMaterial.Ghost(), this, tile, "effect_ghost_fulu_name"));
            }
        }
    }
}