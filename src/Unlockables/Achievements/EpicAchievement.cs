﻿using System.Linq;

namespace Aotenjo
{
    public class EpicAchievement : Achievement
    {
        public EpicAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.PostSettlePermutationEvent += Player_PostSettlePermutationEvent;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PostSettlePermutationEvent -= Player_PostSettlePermutationEvent;
        }

        private void Player_PostSettlePermutationEvent(PlayerPermutationEvent permutationEvent)
        {
            Permutation permutation = permutationEvent.permutation;
            if (!permutation.IsFullHand()) return;

            if (permutation.GetYakus(permutationEvent.player).Any(y => YakuTester.InfoMap[y].rarity == Rarity.EPIC))
                SetComplete();
        }
    }
}