using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class TanghuluArtifact : LevelingArtifact, IFanProvider
    {
        public TanghuluArtifact() : base("tanghulu", Rarity.COMMON, 8)
        {
        }

        public override void ResetArtifactState()
        {
            Level = 8;
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToAddFanFormat(GetFan(player));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), Level);
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.AddFan(Level, this));
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            effects.Add(new DecreaseEffect(this));
        }

        public bool IsAffecting(Player player)
        {
            return Level > 0;
        }

        private class DecreaseEffect : Effect
        {
            private readonly TanghuluArtifact tanghulu;

            public DecreaseEffect(TanghuluArtifact artifact)
            {
                tanghulu = artifact;
            }

            public override void Ingest(Player player)
            {
                if (tanghulu.Level > 0)
                {
                    tanghulu.Level--;
                    player.stats.RecordCustomStats(PlayerStatsType.EAT_FOOD, 1);
                }
                    
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return tanghulu.Level <= 0 ? func("effect_tanghulu_used") : func("effect_tanghulu");
            }

            public override string GetSoundEffectName()
            {
                return "Food";
            }

            public override Artifact GetEffectSource()
            {
                return tanghulu;
            }
        }

        public double GetFan(Player player) => Level;
    }
}