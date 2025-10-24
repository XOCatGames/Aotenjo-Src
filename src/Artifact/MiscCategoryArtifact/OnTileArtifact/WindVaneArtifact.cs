using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class WindVaneArtifact : Artifact
    {
        public const int MONEY = 1;
        public const double FAN = 2;
        public const double FU = 30;

        public WindVaneArtifact() : base("wind_vane", Rarity.COMMON)
        {
            SetHighlightRequirement((tile, player) => tile.IsPlayerWind(player));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MONEY, FAN, FU);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!tile.CompatWith(player.GetPlayerWind() + "z")) return;
            if (player.Selecting(tile))
            {
                effects.Add(new EarnMoneyEffect(MONEY, this));
            }

            effects.Add(ScoreEffect.AddFu(FU, this));
            effects.Add(ScoreEffect.AddFan(FAN, this));
        }
    }
}