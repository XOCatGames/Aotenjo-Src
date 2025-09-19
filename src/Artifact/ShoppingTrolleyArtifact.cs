using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class ShoppingTrolleyArtifact : Artifact
    {
        public ShoppingTrolleyArtifact() : base("shopping_trolley", Rarity.RARE)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GetCount(player));
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.AddFan(() => GetCount(player), this));
        }

        private int GetCount(Player player)
        {
            return player.stats.BoughtCount();
        }
    }
}