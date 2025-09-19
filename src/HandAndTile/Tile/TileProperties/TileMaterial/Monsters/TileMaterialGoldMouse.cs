using System;
using System.Collections.Generic;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialGoldMouse : TileMaterial
    {
        private const double MUL = 1.5f;
        private const int COST = 2;

        public TileMaterialGoldMouse(int ID) : base(ID, "gold_mouse", null)
        {
        }

        public override Rarity GetRarity()
        {
            return Rarity.COMMON;
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            return string.Format(base.GetDescription(localizer), COST, MUL);
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            if (player.Selecting(tile))
            {
                effects.Add(new EarnMoneyEffect(-COST, null));
            }

            effects.Add(ScoreEffect.MulFan(MUL, null));
        }
    }
}