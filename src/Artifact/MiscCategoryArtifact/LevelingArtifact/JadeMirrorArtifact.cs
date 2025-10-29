using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Aotenjo
{
    public class JadeMirrorArtifact : LevelingArtifact, IMultiplierProvider, IJade
    {
        private const double FAN_BONUS_MULTIPLIER = 0.2f;

        public JadeMirrorArtifact() : base("jade_mirror", Rarity.EPIC, 0)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localize)
        {
            return string.Format(base.GetDescription(localize), FAN_BONUS_MULTIPLIER.ToShortString(),
                GetMul(player).ToShortString());
        }

        public double GetMul(Player player)
        {
            return 1f + FAN_BONUS_MULTIPLIER * this.GetEffectiveJadeStack(player);
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToMulFanFormat(GetMul(player));
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.MulFan(GetMul(player), this));
            if (permutation is SevenPairsPermutation or ThirteenOrphansPermutation)
            {
                return;
            }

            var block = player.GetCurrentSelectedBlocks()[0];
            if (permutation.blocks.Any(o => o != block && o.IsABC() &&
                    player.DetermineShiftedPair(o, block, 0, false)))
            {
                effects.Add(new UpgradeEffect(this));
            }
        }

        private class UpgradeEffect : Effect
        {
            private readonly JadeMirrorArtifact artifact;

            public UpgradeEffect(JadeMirrorArtifact artifact)
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

            public override string GetSoundEffectName()
            {
                return "JadeSound";
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }
        }
    }
}