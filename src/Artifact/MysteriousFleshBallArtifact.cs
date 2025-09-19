using System.Collections.Generic;

namespace Aotenjo
{
    public class MysteriousFleshBallArtifact : Artifact
    {
        public MysteriousFleshBallArtifact() : base("mysterious_flesh_ball", Rarity.COMMON)
        {
            SetHighlightRequirement((t, _) => t.properties.IsDebuffed());
        }

        public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendOnUnusedTileEffects(player, perm, tile, effects);
            if (tile.properties.IsDebuffed())
            {
                if (player.GenerateRandomInt(4) == 0)
                {
                    effects.Add(new TransformMaterialEffect(TileMaterial.Nest(), this, tile, "effect_corrupt"));
                }
            }
        }
    }
}