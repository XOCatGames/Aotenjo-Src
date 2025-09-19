using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class VioletGoatHornArtifact : Artifact
    {
        private const double MUL_INCRE = 2;
        private const double MUL_DECRE = 0.5;

        public VioletGoatHornArtifact() : base("violet_goat_horn", Rarity.EPIC)
        {
            
            SetHighlightRequirement((t, p) => p.DetermineMaterialCompactbility(t, TileMaterial.Succubus()));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MUL_INCRE, MUL_DECRE);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (player.DetermineMaterialCompactbility(tile, TileMaterial.Succubus()))
            {
                effects.Add(ScoreEffect.MulFan(MUL_INCRE, this));
            }
        }

        public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendOnUnusedTileEffects(player, perm, tile, effects);
            if (player.DetermineMaterialCompactbility(tile, TileMaterial.Succubus()))
            {
                effects.Add(ScoreEffect.MulFan(MUL_DECRE, this));
            }
        }
    }
}