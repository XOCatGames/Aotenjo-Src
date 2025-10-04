using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class TaotiePactArtifact : Artifact
    {
        const double MUL_PER_LEVEL = 0.15D;
        public TaotiePactArtifact() : base("taotie_pact", Rarity.EPIC)
        {
            SetHighlightRequirement((tile, _) => tile.properties.material is TileMaterialDessert);
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(player, localizer), MUL_PER_LEVEL, GetMul(player));
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToMulFanFormat(GetMul(player));
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.MulFan(GetMul(player), this));
            
        }

        private static double GetMul(Player player)
        {
            int level = player.stats.GetCustomStats(PlayerStatsType.EAT_FOOD);
            return 1 + level * MUL_PER_LEVEL;
        }
    }
}