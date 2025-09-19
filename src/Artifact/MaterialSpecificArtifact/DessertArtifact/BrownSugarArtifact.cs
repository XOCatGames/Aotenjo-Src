using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class BrownSugarArtifact : Artifact
    {
        private const int CHANCE = 7; // 1/7 几率

        public BrownSugarArtifact() : base("brown_sugar", Rarity.COMMON)
        {
            SetHighlightRequirement((tile, player) => tile.ContainsRed(player));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), CHANCE, GetChanceMultiplier(player));
        }

        public override void AppendOnTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, perm, tile, effects);

            if (!tile.ContainsRed(player)) return;
            if (!player.Selecting(tile)) return;

            if (player.GenerateRandomDeterminationResult(CHANCE))
                effects.Add(new TransformMaterialEffect(TileMaterial.SugarCube(), this, tile, "effect_brown_sugar"));
        }
    }
} 