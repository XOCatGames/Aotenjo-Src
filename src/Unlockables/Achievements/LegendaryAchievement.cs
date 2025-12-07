using System.Linq;

namespace Aotenjo
{
    public class LegendaryAchievement : Achievement
    {
        public LegendaryAchievement(string id) : base(id)
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
            if (!permutation.IsFullHand(permutationEvent.player)) return;

            if (permutation.GetYakus(permutationEvent.player)
                .Any(y => YakuTester.InfoMap[y].rarity == Rarity.LEGENDARY))
                SetComplete();
        }
    }
}