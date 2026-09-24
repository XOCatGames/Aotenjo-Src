using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class VioletGoatHornArtifact : Artifact
    {
        private const double MUL_INCRE = 1.5;
        private const double MUL_DECRE = 0.5;
        private const int GIFT_COUNT = 5;

        public VioletGoatHornArtifact() : base("violet_goat_horn", Rarity.EPIC)
        {
            SetHighlightRequirement((t, p) => p.DetermineMaterialCompatibility(t, TileMaterial.Succubus()));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MUL_INCRE, MUL_DECRE, GIFT_COUNT);
        }

        public override void OnObtain(Player player)
        {
            base.OnObtain(player);
            foreach (var tile in player.DrawPlainTilesFromPool(GIFT_COUNT))
            {
                tile.SetMaterial(TileMaterial.Succubus(), player);
            }
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (player.DetermineMaterialCompatibility(tile, TileMaterial.Succubus()))
            {
                effects.Add(ScoreEffect.MulFan(MUL_INCRE, this));
            }
        }

        public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendOnUnusedTileEffects(player, perm, tile, effects);
            if (player.DetermineMaterialCompatibility(tile, TileMaterial.Succubus()))
            {
                effects.Add(ScoreEffect.MulFan(MUL_DECRE, this));
            }
        }
    }
}