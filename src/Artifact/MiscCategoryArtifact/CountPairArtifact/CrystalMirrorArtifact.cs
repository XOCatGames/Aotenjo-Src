using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CrystalMirrorArtifact : CountPairArtifact
    {
        private static readonly int FAN = 12;

        public CrystalMirrorArtifact() : base("crystal_mirror", Rarity.RARE)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FAN);
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);

            int count = CountPairsWithDiff(permutation, 0, player);

            if (count != 0)
                effects.AddRange(Enumerable.Repeat(ScoreEffect.AddFan(FAN, this), count));
        }
    }
}