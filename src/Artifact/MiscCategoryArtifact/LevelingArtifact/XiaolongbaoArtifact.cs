using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class XiaolongbaoArtifact : LevelingArtifact
    {
        private const int INIT_LEVEL = 25;

        public XiaolongbaoArtifact() : base("xiaolongbao", Rarity.COMMON, INIT_LEVEL)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), Level);
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToAddFanFormat(Level);
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block,
            List<Effect> effects)
        {
            base.AddOnBlockEffects(player, permutation, block, effects);
            effects.Add(ScoreEffect.AddFu(Level, this));
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            effects.Add(new DecreaseEffect(this));
        }

        private class DecreaseEffect : Effect
        {
            private readonly XiaolongbaoArtifact xiaolongbao;

            public DecreaseEffect(XiaolongbaoArtifact artifact)
            {
                xiaolongbao = artifact;
            }

            public override void Ingest(Player player)
            {
                if (xiaolongbao.Level > 0)
                {
                    xiaolongbao.Level -= Math.Min(xiaolongbao.Level, 2);
                    player.stats.RecordCustomStats(PlayerStatsType.EAT_FOOD, 1);
                }
                    
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return xiaolongbao.Level <= 0 ? func("effect_tanghulu_used") : func("effect_tanghulu");
            }

            public override string GetSoundEffectName()
            {
                return "Food";
            }

            public override Artifact GetEffectSource()
            {
                return xiaolongbao;
            }
        }
    }
}