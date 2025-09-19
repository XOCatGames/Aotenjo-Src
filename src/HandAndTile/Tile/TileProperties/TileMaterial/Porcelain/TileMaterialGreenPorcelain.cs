using System;
using System.Collections.Generic;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialGreenPorcelain : TileMaterialPorcelain
    {
        private const int AMOUNT = 1;

        public TileMaterialGreenPorcelain(int ID) : base(ID, "green_porcelain")
        {
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), AMOUNT);
        }

        public override void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects)
        {
            base.AppendUnusedEffects(player, perm, effects);
            effects.Add(new EarnMoneyEffect(AMOUNT));
        }
    }
}