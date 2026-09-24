using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class ExpertCrookAchievement : Achievement
    {
        public ExpertCrookAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerEvents.OnPostAddScoringAnimationEffectEvent>(player, OnSettlePermutation);
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerEvents.OnPostAddScoringAnimationEffectEvent>(player, OnSettlePermutation);
        }

        private void OnSettlePermutation(Permutation permutation, Player player, List<IAnimationEffect> list)
        {
            if (permutation.ToTiles().Where(t => t.GetLastTransform() != null).Count() >= 8)
            {
                SetComplete();
            }
        }
    }
}