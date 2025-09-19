using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class RainbowCheeseArtifact : Artifact
    {
        public RainbowCheeseArtifact() : base("rainbow_cheese", Rarity.RARE)
        {
            
            SetPrerequisite(p =>
                p.GetAllTiles().Any(t => p.DetermineMaterialCompactbility(t, TileMaterial.GoldMouse())));
            SetHighlightRequirement((t, p) => p.DetermineMaterialCompactbility(t, TileMaterial.GoldMouse()));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            Permutation permutation = player.GetCurrentSelectedPerm();
            List<Tile> tiles = new List<Tile>();
            if (permutation != null)
            {
                tiles = new(permutation.ToTiles());
            }

            if (player is RainbowDeck.RainbowPlayer rainbowPlayer)
            {
                tiles = tiles.Union(rainbowPlayer.PlayedFlowerTiles).ToList();
            }

            return string.Format(base.GetDescription(localizer), tiles.Select(t => t.GetCategory()).Distinct().Count());
        }


        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (!player.Selecting(tile)) return;
            if (player.DetermineMaterialCompactbility(tile, TileMaterial.GoldMouse()))
            {
                List<Tile> tiles = new List<Tile>(permutation.ToTiles());
                if (player is RainbowDeck.RainbowPlayer rainbowPlayer)
                {
                    tiles = tiles.Union(rainbowPlayer.PlayedFlowerTiles).ToList();
                }

                effects.Add(new EarnMoneyEffect(tiles.Select(t => t.GetCategory()).Distinct().Count()));
            }
        }
    }
}