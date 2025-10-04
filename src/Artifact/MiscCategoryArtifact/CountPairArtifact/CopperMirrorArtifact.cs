using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CopperMirrorArtifact : CountPairArtifact
    {
        private static readonly int FU = 40;

        public CopperMirrorArtifact() : base("copper_mirror", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU);
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);

            int count = CountPairsWithDiff(permutation, 0, player);

            if (count != 0)
                effects.AddRange(Enumerable.Repeat(ScoreEffect.AddFu(FU, this), count));
        }
    }
}