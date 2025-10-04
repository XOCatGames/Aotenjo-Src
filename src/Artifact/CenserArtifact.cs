using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CenserArtifact : Artifact
    {
        public CenserArtifact() : base("censer", Rarity.RARE)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), player.GetSkillSet().GetYakus().Count());
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            if (player.GetSkillSet().GetYakus().Count() < 24)
                effects.Add(ScoreEffect.MulFan(2f, this));
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            int count = player.GetSkillSet().GetYakus().Count();
            string format = count >= 24 ? "<style=\"black\">{0}</style>" : "{0}";
            return (format, count);
        }
    }
}