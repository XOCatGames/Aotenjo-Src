using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class BambooSegmentArtifact : Artifact
    {
        private const int ADD_ON_FU = 3;

        public BambooSegmentArtifact() : base("bamboo_segment", Rarity.COMMON)
        {
            SetHighlightRequirement((t, p) => t.ContainsGreen(p));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), ADD_ON_FU);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.Selecting(tile)) return;
            if (tile.ContainsGreen(player))
            {
                effects.Add(new GrowFuEffect(this, tile, ADD_ON_FU, "grow_bamboo_segment"));
            }
        }
    }
}