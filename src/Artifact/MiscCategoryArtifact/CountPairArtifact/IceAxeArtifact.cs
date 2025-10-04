using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class IceAxeArtifact : CountPairArtifact
    {
        private const int ADD_FU = 40;

        public IceAxeArtifact() : base("ice_axe", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localize)
        {
            return string.Format(base.GetDescription(localize), ADD_FU);
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);

            int count = CountPairsWithDiff(permutation, 1, player);

            if (count != 0)
                effects.Add(ScoreEffect.AddFu(ADD_FU * count, this));
        }
    }
}