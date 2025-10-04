using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class BigEncyclopediaArtifact : CraftableArtifact
    {
        public BigEncyclopediaArtifact() : base("big_encyclopedia", Rarity.RARE)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), player.GetNonNativeYaku().Count());
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.AddFan(player.GetNonNativeYaku().Count(), this));
        }
    }
}