using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class LantanaArtifact : Artifact
    {
        private const int FAN = 5;

        public LantanaArtifact() : base("lantana", Rarity.RARE)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FAN);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.AddFan(FAN * ((RainbowDeck.RainbowPlayer)player).PlayedFlowerTiles.Count, this));
        }
    }
}