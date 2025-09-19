using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Aotenjo
{
    public class JadeRingArtifact : LevelingArtifact, IActivable, IMultiplierProvider, IJade
    {
        private const double FAN_BONUS_MULTIPLIER = 0.3f;
        private bool first = true;

        public JadeRingArtifact() : base("jade_ring", Rarity.EPIC, 0)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToMulFanFormat(GetMul(player));
        }

        private void PostRoundEnd(PlayerEvent data)
        {
            first = true;
        }

        public override string GetDescription(Player player, Func<string, string> localize)
        {
            return string.Format(base.GetDescription(localize), FAN_BONUS_MULTIPLIER.ToShortString(),
                GetMul(player).ToShortString());
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            first = true;
        }

        public double GetMul(Player player)
        {
            return 1.5f + FAN_BONUS_MULTIPLIER * this.GetEffectiveJadeStack(player);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            if (permutation.ToTiles().Where(t => t.IsNumbered()).Select(t => t.GetOrder()).Distinct().Count() == 9)
            {
                effects.Add(ScoreEffect.MulFan(GetMul(player), this));
                if (first)
                    effects.Add(new UpgradeEffect(this));
            }
        }

        public bool IsActivating()
        {
            return first;
        }

        private class UpgradeEffect : Effect
        {
            private readonly JadeRingArtifact artifact;

            public UpgradeEffect(JadeRingArtifact artifact)
            {
                this.artifact = artifact;
            }

            public override void Ingest(Player player)
            {
                artifact.Level++;
                artifact.first = false;
            }

            public override string GetSoundEffectName()
            {
                return "JadeSound";
            }


            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_jade_mirror_level_up");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }
        }
    }
}