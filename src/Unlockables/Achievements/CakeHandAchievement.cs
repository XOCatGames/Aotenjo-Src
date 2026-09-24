using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CakeHandAchievement : Achievement
    {
        private int cakeCount = 0;
        public CakeHandAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, PreAppendSettleScoringEffects);
            EventBus.Subscribe<PlayerEvents.PostIngestEffectEvent>(player, PostIngestEffect);
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, PreAppendSettleScoringEffects);
            EventBus.Unsubscribe<PlayerEvents.PostIngestEffectEvent>(player, PostIngestEffect);
        }

        private void PreAppendSettleScoringEffects(PlayerPermutationEvent permutationEvent)
        {
            cakeCount = 0;
        }

        private void PostIngestEffect(Permutation permutation, Player player,
            Effect effect)
        {
            if (effect.GetEffectSource() == Artifacts.CakeExpert)
            {
                cakeCount++;
            }
            if (cakeCount >= 7)
            {
                SetComplete();
            }
        }
    }
}