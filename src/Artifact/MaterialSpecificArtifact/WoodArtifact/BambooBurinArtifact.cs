using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class BambooBurinArtifact : Artifact
    {
        private static readonly int CHANCE = 18;

        public BambooBurinArtifact() : base("bamboo_burin", Rarity.COMMON)
        {
            SetHighlightRequirement((t, _) => t.GetOrder() == 7 && t.GetCategory() == Tile.Category.Suo);
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GetChanceMultiplier(player));
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.Selecting(tile)) return;
            if (tile.CompactWith(new("7s")) || (tile.GetCategory() == Tile.Category.Suo &&
                                                player.GenerateRandomDeterminationResult(CHANCE)))
            {
                effects.Add(new TransformMaterialEffect(TileMaterial.EmeraldWood(), this, tile, "effect_bamboo_burin",
                    "Grow"));
            }
        }
    }
}