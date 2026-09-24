using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class TricolorLianPuAchievement : Achievement
    {
        private bool flower = false;
        private bool lianpu = false;
        public TricolorLianPuAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, PreAppendSettleScoringEffects);
            EventBus.Subscribe<PlayerEvents.PostIngestEffectEvent>(player, PostIngestEffect);
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, PreAppendSettleScoringEffects);
            EventBus.Unsubscribe<PlayerEvents.PostIngestEffectEvent>(player, PostIngestEffect);
        }

        private void PreAppendSettleScoringEffects(PlayerPermutationEvent permutationEvent)
        {
            flower = false;
            lianpu = false;
        }

        private void PostIngestEffect(Permutation permutation, Player player,
            Effect effect)
        {
            List<Artifact> artifacts = player.GetArtifacts();
            if (!artifacts.Contains(Artifacts.TricolorFlower) || !artifacts.Contains(Artifacts.SichuanLianPu))
            {
                return;
            }
            
            if (effect.GetEffect() is TricolorFlowerArtifact.UpgradeEffect)
            {
                flower = true;
            }
            else if (effect.GetEffect() is SichuanLianPuArtifact.UpgradeEffect)
            {
                lianpu = true;
            }
            
            if (flower && lianpu)
            {
                SetComplete();
            }
        }
    }
}