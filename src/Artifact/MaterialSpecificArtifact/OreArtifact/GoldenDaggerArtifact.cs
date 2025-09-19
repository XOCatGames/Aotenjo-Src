using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class GoldenDaggerArtifact : Artifact
    {
        private const int FAN = 5;

        public GoldenDaggerArtifact() : base("golden_dagger", Rarity.RARE)
        {
            SetHighlightRequirement((tile, player) => tile.CompactWithMaterial(TileMaterial.GOLDEN, player));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FAN);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (tile.CompactWithMaterial(TileMaterial.GOLDEN, player))
            {
                effects.Add(ScoreEffect.AddFan(FAN, this));
            }
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);

            foreach (Artifact a in player.GetArtifacts())
            {
                if (a.GetNameKey().Contains("gold") || a == Artifacts.MobiusRing)
                {
                    effects.Add(ScoreEffect.AddFan(FAN, a));
                }
            }
        }
    }
}