using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class PorcelainSpearArtifact : Artifact
    {
        private const int FU = 30;

        public PorcelainSpearArtifact() : base("porcelain_spear", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (tile.properties.material is TileMaterialPorcelain)
            {
                effects.Add(ScoreEffect.AddFu(FU, this));
                if (player.Selecting(tile))
                {
                    effects.Add(new FractureEffect(this, tile));
                }
            }
        }
    }
}