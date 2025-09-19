using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CakeHandAchievement : Achievement
    {
        public CakeHandAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.OnPostAddScoringAnimationEffectEvent += OnPostAddScoringAnimationEffectEvent;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.OnPostAddScoringAnimationEffectEvent -= OnPostAddScoringAnimationEffectEvent;
        }

        private void OnPostAddScoringAnimationEffectEvent(Permutation permutation, Player player,
            List<IAnimationEffect> list)
        {
            if (list.Where(ie => ie is OnTileAnimationEffect e && e.effect.GetEffectSource() == Artifacts.CakeExpert)
                    .Count() >= 7)
            {
                SetComplete();
            }
        }
    }
}