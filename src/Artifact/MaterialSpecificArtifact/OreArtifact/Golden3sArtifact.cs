using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class Golden3sArtifact : Artifact
    {
        public Golden3sArtifact() : base("golden_3s", Rarity.COMMON)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GetChanceMultiplier(player));
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.Selecting(tile)) return;
            if (tile.CompatWith("3p") ||
                (tile.GetCategory() == Tile.Category.Bing && player.GenerateRandomInt(18) == 0))
            {
                effects.Add(new TransformMaterialEffect(TileMaterial.GOLDEN, this, tile, "effect_golden", "Agate"));
            }
        }
    }
}