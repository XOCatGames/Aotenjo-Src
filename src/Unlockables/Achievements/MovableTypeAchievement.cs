using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class MovableTypeAchievement : Achievement
    {
        public MovableTypeAchievement(string id) : base(id)
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
            if (player.GetSelectedTilesCopy().Where(t => t.IsHonor(player) && t.GetLastTransform() != null).Count() > 3)
            {
                SetComplete();
            }
        }
    }
}