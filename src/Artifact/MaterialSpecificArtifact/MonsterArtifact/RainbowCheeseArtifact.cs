using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class RainbowCheeseArtifact : Artifact
    {
        public RainbowCheeseArtifact() : base("rainbow_cheese", Rarity.RARE)
        {
            SetHighlightRequirement((t, p) => p.DetermineMaterialCompatibility(t, TileMaterial.GoldMouse()));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            Permutation permutation = player.GetCurrentSelectedPerm();
            List<Tile> tiles = player.GetScoringTiles(permutation);

            return string.Format(base.GetDescription(localizer), tiles.Select(t => t.GetCategory()).Distinct().Count());
        }


        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (!player.IsPlayingTile(tile)) return;
            if (player.DetermineMaterialCompatibility(tile, TileMaterial.GoldMouse()))
            {
                List<Tile> tiles = player.GetScoringTiles(permutation);

                effects.Add(new EarnMoneyEffect(tiles.Select(t => t.GetCategory()).Distinct().Count(), this));
            }
        }
    }
}
