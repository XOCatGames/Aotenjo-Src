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
            player.OnPostAddScoringAnimationEffectEvent += OnSettlePermutation;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.OnPostAddScoringAnimationEffectEvent -= OnSettlePermutation;
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