using System;

namespace Aotenjo
{
    public class PrecisionAchievement : Achievement
    {
        public PrecisionAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.PostSettlePermutationEvent += OnSettlePermutation;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PostSettlePermutationEvent -= OnSettlePermutation;
        }

        private void OnSettlePermutation(PlayerPermutationEvent permutationEvent)
        {
            Player player = permutationEvent.player;
            if (Math.Floor(player.CurrentAccumulatedScore) == Math.Floor(player.GetLevelTarget()))
            {
                SetComplete();
            }
        }
    }
}