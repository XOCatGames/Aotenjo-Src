using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class IndigoPenArtifact : Artifact
    {
        private const double FU_DECREASE = 20;
        private static readonly int CHANCE = 3;

        public IndigoPenArtifact() : base("indigo_pen", Rarity.EPIC)
        {
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            return tile.ContainsNoColor(player);
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(localizer("artifact_indigo_pen_description"), FU_DECREASE,
                GetChanceMultiplier(player));
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (tile.ContainsNoColor(player) && player.Selecting(tile))
            {
                effects.Add(new TryDyeBlueEffect(tile, this));
            }

            if (tile.ContainsBlue(player))
            {
                effects.Add(ScoreEffect.AddFu(-FU_DECREASE, this));
            }
        }

        private class TryDyeBlueEffect : Effect
        {
            private Tile tile;
            private bool success;
            private IndigoPenArtifact indigoPenArtifact;

            public TryDyeBlueEffect(Tile tile, IndigoPenArtifact indigoPenArtifact)
            {
                this.tile = tile;
                this.indigoPenArtifact = indigoPenArtifact;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return success ? func("effect_indigo_pen_success") : func("effect_indigo_pen_fail");
            }

            public override Artifact GetEffectSource()
            {
                return indigoPenArtifact;
            }

            public override void Ingest(Player player)
            {
                success = player.GenerateRandomDeterminationResult(CHANCE);

                if (success)
                {
                    tile.SetFont(TileFont.BLUE, player);
                }
            }
        }
    }
}