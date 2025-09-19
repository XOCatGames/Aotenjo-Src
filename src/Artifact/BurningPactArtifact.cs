using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class BurningPactArtifact : Artifact
    {
        public int[] tiers = { 600, 400, 300 };
        public double[] muls = { 1.5, 2, 3 };

        public BurningPactArtifact() : base("burning_pact", Rarity.EPIC)
        {
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToMulFanFormat(GetMul(player));
        }

        private static int GetCount(Player player)
        {
            return player.GetAllTiles().Where(t => t != null).Select(t => t.IsNumbered() ? t.GetOrder() : 10).Sum();
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            int j = -1;
            int sum = GetCount(player);

            for (int i = 0; i < tiers.Length; i++)
            {
                if (sum <= tiers[i])
                {
                    j++;
                }
                else
                {
                    break;
                }
            }

            return string.Format(
                base.GetDescription(player, localizer),
                string.Join("/",
                    tiers.Select(t => (j != -1 && t == tiers[j]) ? $"<style=\"red\">{t}</style>" : t.ToString())),
                string.Join("/",
                    muls.Select(t => (j != -1 && t == muls[j]) ? $"<style=\"red\">{t}</style>" : t.ToString())),
                GetCount(player).ToString()
            );
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            double mul = GetMul(player);
            if(mul <= 1.0001D) return;
            effects.Add(ScoreEffect.MulFan(mul, this));
        }

        private double GetMul(Player player)
        {
            int j = -1;
            int sum = GetCount(player);
            foreach (int t in tiers)
            {
                if (sum <= t)
                {
                    j++;
                }
                else
                {
                    break;
                }
            }

            return j == -1 ? 1D : muls[j];
        }
    }
}