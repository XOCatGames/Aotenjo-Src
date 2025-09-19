using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class BicolorFeatherArtifact : Artifact
    {
        private const int AMOUNT = 2;

        public BicolorFeatherArtifact() : base("bicolor_feather", Rarity.EPIC)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation perm, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, perm, effects);

            if (player is not ScarletPlayer scarlet) return;

            int mainCount = 0;
            int subCount = 0;

            foreach (var block in perm.blocks)
            {
                var cat = block.GetCategory();

                if (scarlet.IsCompatibleWithMainCategory(cat)) mainCount++;
                if (scarlet.IsCompatibleWithSubCategory(cat)) subCount++;
            }

            if (mainCount > 0 && mainCount == subCount)
            {
                effects.Add(ScoreEffect.MulFan(AMOUNT, this));
            }
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), AMOUNT);
        }
    }
}