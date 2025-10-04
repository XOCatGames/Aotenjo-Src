using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class NylonRopeArtifact : CountPairArtifact
    {
        private const double MUL_FAN = 1.5;

        public NylonRopeArtifact() : base("nylon_rope", Rarity.EPIC)
        {
        }

        public override string GetDescription(Func<string, string> localize)
        {
            return string.Format(base.GetDescription(localize), MUL_FAN);
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);

            int count = CountPairsWithDiff(permutation, 1, player);

            if (count != 0)
                effects.AddRange(Enumerable.Repeat(ScoreEffect.MulFan(MUL_FAN, this), count));
        }
    }
}