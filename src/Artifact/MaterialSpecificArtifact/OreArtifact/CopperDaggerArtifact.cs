using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class CopperDaggerArtifact : Artifact
    {
        private const int FU = 30;

        public CopperDaggerArtifact() : base("copper_dagger", Rarity.COMMON)
        {
            SetHighlightRequirement((tile, player) => tile.CompactWithMaterial(TileMaterial.COPPER, player));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (tile.CompactWithMaterial(TileMaterial.COPPER, player))
            {
                effects.Add(ScoreEffect.AddFu(FU, this));
            }
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);

            foreach (Artifact a in player.GetArtifacts())
            {
                if (a.GetNameKey().Contains("copper") || a.GetNameKey().Contains("bronze") || a == Artifacts.WindVane ||
                    a == Artifacts.Censer)
                {
                    effects.Add(ScoreEffect.AddFu(FU, a));
                }
            }
        }
    }
}