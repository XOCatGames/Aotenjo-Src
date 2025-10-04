using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class EncyclopediaArtifact : Artifact
    {
        public EncyclopediaArtifact() : base("encyclopedia", Rarity.RARE)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), Math.Min(30, player.GetNonNativeYaku().Count()));
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.AddFan(Math.Min(30, player.GetNonNativeYaku().Count()), this));
        }
    }
}