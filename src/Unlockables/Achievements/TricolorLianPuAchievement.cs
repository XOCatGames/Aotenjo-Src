using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class TricolorLianPuAchievement : Achievement
    {
        public TricolorLianPuAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.OnPostAddScoringAnimationEffectEvent += OnPostAddScoringAnimationEffect;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.OnPostAddScoringAnimationEffectEvent -= OnPostAddScoringAnimationEffect;
        }

        private void OnPostAddScoringAnimationEffect(Permutation permutation, Player player,
            List<IAnimationEffect> list)
        {
            List<Artifact> artifacts = player.GetArtifacts();
            if (!artifacts.Contains(Artifacts.TricolorFlower) || !artifacts.Contains(Artifacts.SichuanLianPu))
            {
                return;
            }

            if (list.Any(e => e.GetEffect() is TricolorFlowerArtifact.UpgradeEffect) &&
                list.Any(e => e.GetEffect() is SichuanLianPuArtifact.UpgradeEffect))
            {
                SetComplete();
            }
        }
    }
}