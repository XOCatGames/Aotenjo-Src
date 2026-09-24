using System.Collections.Generic;

namespace Aotenjo
{
    public class IllusionAchievement : Achievement
    {
        public IllusionAchievement(string id) : base(id)
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
            if (permutation.GetYakus(player, true).Contains(FixedYakuType.LyuYiSe))
            {
                if (permutation.IsFullHand(player) && permutation.ToTiles().TrueForAll(t =>
                        t.GetCategory() != Tile.Category.Suo && !t.CompatWith(new Tile("6z"))))
                {
                    SetComplete();
                }
            }
        }
    }
}