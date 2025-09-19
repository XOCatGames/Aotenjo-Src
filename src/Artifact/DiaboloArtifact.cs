using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class DiaboloArtifact : Artifact
    {
        public int[] tiers = { 15, 30 };
        public double[] muls = { 1.2, 1.3 };

        public DiaboloArtifact() : base("diabolo", Rarity.EPIC)
        {
            SetHighlightRequirement((t, p) => p.GetBaseFuOfTile(t) >= tiers[0]);
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(
                base.GetDescription(player, localizer),
                string.Join("/", tiers),
                string.Join("/", muls)
            );
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            int j = -1;
            foreach (int t in tiers)
            {
                if (player.GetBaseFuOfTile(tile) >= t)
                {
                    j++;
                }
                else
                {
                    break;
                }
            }

            if (j == -1) return;

            effects.Add(ScoreEffect.MulFan(muls[j], this));
        }
    }
}