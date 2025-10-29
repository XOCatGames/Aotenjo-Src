using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CorruptedCrystalBallArtifact : Artifact, IJade
    {
        private const double MUL_PER_NEST = 0.1f;

        public CorruptedCrystalBallArtifact() : base("corrupted_crystal_ball", Rarity.EPIC)
        {
            SetHighlightRequirement((t, p) => p.DetermineMaterialCompatibility(t, TileMaterial.Nest()));
            
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToMulFanFormat(GetMul(player));
        }

        public override bool IsAvailableInShops(Player player)
        {
            return GetNestCount(player) > 0;
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(player, localizer), Utils.NumberToFormat(MUL_PER_NEST),
                Utils.NumberToFormat(GetMul(player)));
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            int nestCount = this.GetEffectiveJadeStack(player);
            if (nestCount > 0)
            {
                effects.Add(ScoreEffect.MulFan(GetMul(player), this));
            }
        }

        private double GetMul(Player player)
        {
            return 1 + MUL_PER_NEST * this.GetEffectiveJadeStack(player);
        }
        
        public int GetLevel(Player player)
        {
            return GetNestCount(player);
        }

        private static int GetNestCount(Player player)
        {
            return player
                .GetAllTiles().Count(t => t != null && player.DetermineMaterialCompatibility(t, TileMaterial.Nest()));
        }

    }
}