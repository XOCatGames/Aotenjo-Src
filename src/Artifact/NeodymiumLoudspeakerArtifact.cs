using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class NeodymiumLoudspeakerArtifact : LoudspeakerArtifact
    {
        private const double FU_PER_EXTRA_TILE = 10D;

        public NeodymiumLoudspeakerArtifact() : base("neodymium_loudspeaker", Rarity.EPIC, 4)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(player, localizer), FU_PER_EXTRA_TILE * GetExtraTileCount(player));
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            if (player.GetAllTiles().Count > player.GetInitialWall().Count)
            {
                int amount = GetExtraTileCount(player);
                effects.Add(ScoreEffect.AddFu(FU_PER_EXTRA_TILE * amount, this));
            }
        }

        private static int GetExtraTileCount(Player player)
        {
            return Math.Max(0, player.GetAllTiles().Count - player.GetInitialWall().Count);
        }
    }
}