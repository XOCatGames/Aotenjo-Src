using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Aotenjo
{
    public class VoidTeapotArtifact : LevelingArtifact, IActivable, IMultiplierProvider
    {
        private const double FAN_BONUS_MULTIPLIER = 0.5f;
        private const double FAN_INIT_MULTIPLIER = 2f;
        private bool first = true;

        public VoidTeapotArtifact() : base("void_teapot", Rarity.EPIC, 0)
        {
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToMulFanFormat(GetMul(player));
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            first = true;
        }
        
        public double GetMul(Player player)
        {
            return (FAN_INIT_MULTIPLIER + FAN_BONUS_MULTIPLIER * (Level));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostRoundEndEvent += OnRoundEnd;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostRoundEndEvent -= OnRoundEnd;
        }

        private void OnRoundEnd(PlayerEvent data)
        {
            first = true;
        }

        public override string GetDescription(Player p, Func<string, string> localize)
        {
            return string.Format(base.GetDescription(localize), FAN_BONUS_MULTIPLIER.ToShortString(), GetMul(p).ToShortString());
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            IEnumerable<YakuType> activatedYakus =
                permutation.GetYakus(player).Where(y => player.GetSkillSet().GetLevel(y) > 0);
            if (activatedYakus.Count() > 2) return;

            effects.Add(ScoreEffect.MulFan(GetMul(player), this));
            if (first)
            {
                effects.Add(new UpgradeEffect(this));
                first = false;
            }
        }

        public bool IsActivating()
        {
            return first;
        }

        private class UpgradeEffect : Effect
        {
            private readonly VoidTeapotArtifact artifact;

            public UpgradeEffect(VoidTeapotArtifact artifact)
            {
                this.artifact = artifact;
            }

            public override void Ingest(Player player)
            {
                artifact.Level++;
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