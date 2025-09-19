using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class RulerArtifact : Artifact
    {
        public RulerArtifact() : base("ruler", Rarity.EPIC)
        {
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            Permutation permutation = player.GetCurrentSelectedPerm() ?? player.GetAccumulatedPermutation();
            return permutation == null ? base.GetAdditionalDisplayingInfo(player) : ToMulFanFormat(GetMul(player, permutation));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            Permutation permutation = player.GetCurrentSelectedPerm() ?? player.GetAccumulatedPermutation();
            if (permutation == null)
                return string.Format(base.GetDescription(player, localizer), 1, GetOrizuruCount(player));
            double mul = GetMul(player, permutation);
            return string.Format(base.GetDescription(player, localizer), Utils.NumberToFormat(mul),
                GetOrizuruCount(player));
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            double ml = GetMul(player, permutation);
            for (int i = 0; i <= GetOrizuruCount(player); i++)
            {
                effects.Add(ScoreEffect.MulFan(ml, this));
            }
        }

        private static double GetMul(Player player, Permutation permutation)
        {
            int levelCount = GetLevelCount(player, permutation);

            return (1 + (levelCount / 20) * 0.4f);
        }

        private static int GetLevelCount(Player player, Permutation permutation)
        {
            return permutation.GetYakus(player).Select(y => player.GetSkillSet().GetLevel(y)).Sum();
        }

        public static int GetOrizuruCount(Player player)
        {
            List<Artifact> artifacts = player.GetArtifacts();
            return artifacts.Where(a => a.GetNameID().Contains("orizuru") || a.GetNameID().Contains("orizulu"))
                .Sum(a => a == Artifacts.TwinOrizulu ? 2 : 1);
        }
    }
}