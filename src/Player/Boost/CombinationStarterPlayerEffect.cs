using System;

namespace Aotenjo
{
    public class CombinationStarterPlayerEffect : StarterBoostEffect
    {
        private StarterBoostEffect effect1;
        private StarterBoostEffect effect2;

        public CombinationStarterPlayerEffect(StarterBoostEffect effect1, StarterBoostEffect effect2) : base("comb")
        {
            this.effect1 = effect1;
            this.effect2 = effect2;
        }

        public override void Boost(Player player)
        {
            effect1.Boost(player);
            effect2.Boost(player);
        }

        public override string GetLocalizedName(Player player, Func<string, string> loc)
        {
            return effect1.GetLocalizedName(player, loc);
        }

        public override string GetLocalizedDesc(Player player, Func<string, string> loc)
        {
            string format = loc("player_effect_starter_combination_format");
            return string.Format(format, effect1.GetLocalizedDesc(player, loc), effect2.GetLocalizedDesc(player, loc));
        }
    }
}