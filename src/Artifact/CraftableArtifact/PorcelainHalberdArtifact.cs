using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class PorcelainHalberdArtifact : CraftableArtifact
    {
        private const int FU = 40;

        public PorcelainHalberdArtifact() : base("porcelain_halberd", Rarity.RARE)
        {
            SetHighlightRequirement((tile, _) => tile.properties.material is TileMaterialPorcelain);
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
                effects.Add(new TextEffect("effect_porcelain_halberd"));
                effects.Add(ScoreEffect.AddFu(FU, this));
                effects.Add(new TileUnusedScoringEffectAppendEffect(player, tile, permutation, player.playHandEffectStack));
            }
        }
    }
}