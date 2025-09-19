using System;

namespace Aotenjo
{
    public class PorcelainFishArtifact : Artifact
    {
        public const int GROW_FU_AMOUNT = 1;

        public PorcelainFishArtifact() : base("porcelain_fish", Rarity.RARE)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GROW_FU_AMOUNT);
        }
    }
}