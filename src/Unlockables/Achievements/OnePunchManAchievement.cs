using System;

namespace Aotenjo
{
    public class OnePunchManAchievement : Achievement
    {
        private int playCount;

        public OnePunchManAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.PostRoundStartEvent += OnRoundStart;
            player.PostSettlePermutationEvent += OnSettlePermutation;
            playCount = 0;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PostRoundStartEvent -= OnRoundStart;
            player.PostSettlePermutationEvent -= OnSettlePermutation;
        }

        private void OnRoundStart(PlayerEvent playerEvent)
        {
            playCount = 0;
        }

        private void OnSettlePermutation(PlayerPermutationEvent permutationEvent)
        {
            playCount++;
            if (playCount == 1)
            {
                Player player = permutationEvent.player;
                if (Math.Floor(player.CurrentAccumulatedScore) >= Math.Floor(player.GetLevelTarget()))
                {
                    SetComplete();
                }
            }
        }
    }
}