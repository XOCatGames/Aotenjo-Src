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
            player.OnPostAddScoringAnimationEffectEvent += OnSettlePermutation;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.OnPostAddScoringAnimationEffectEvent -= OnSettlePermutation;
        }

        private void OnSettlePermutation(Permutation permutation, Player player, List<IAnimationEffect> list)
        {
            if (permutation.GetYakus(player, true).Contains(YakuType.LyuYiSe))
            {
                if (permutation.IsFullHand() && permutation.ToTiles().TrueForAll(t =>
                        t.GetCategory() != Tile.Category.Suo && !t.CompactWith(new Tile("6z"))))
                {
                    SetComplete();
                }
            }
        }
    }
}