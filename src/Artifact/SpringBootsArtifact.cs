using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class SpringBootArtifact : Artifact, IPersistantAura
    {
        public SpringBootArtifact() : base("spring_boot", Rarity.RARE)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            int skipCount;
            if (player.GetAccumulatedPermutation() != null)
            {
                skipCount = player.CurrentPlayingStage - player.GetAccumulatedPermutation().blocks.Count();
            }
            else
            {
                skipCount = player.CurrentPlayingStage;
            }

            return string.Format(base.GetDescription(player, localizer), 1 + skipCount);
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            int skipCount;
            if (player.GetAccumulatedPermutation() != null)
            {
                skipCount = player.CurrentPlayingStage - player.GetAccumulatedPermutation().blocks.Count();
            }
            else
            {
                skipCount = player.CurrentPlayingStage;
            }

            if (skipCount > 0)
                effects.Add(ScoreEffect.MulFan(1 + (skipCount), this));
        }


        public bool IsAffecting(Player player)
        {
            int skipCount;
            if (player.GetAccumulatedPermutation() != null)
            {
                skipCount = player.CurrentPlayingStage - player.GetAccumulatedPermutation().blocks.Count();
            }
            else
            {
                skipCount = player.CurrentPlayingStage;
            }

            return skipCount > 0;
        }
    }
}