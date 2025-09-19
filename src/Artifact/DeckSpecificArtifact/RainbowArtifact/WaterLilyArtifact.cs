using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class WaterLilyArtifact : Artifact
    {
        private const int FAN = 8;

        public WaterLilyArtifact() : base("water_lily", Rarity.RARE)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FAN);
        }

        public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendOnUnusedTileEffects(player, perm, tile, effects);
            if (tile is FlowerTile)
            {
                effects.Add(ScoreEffect.AddFan(FAN, this));
            }
        }
    }
}