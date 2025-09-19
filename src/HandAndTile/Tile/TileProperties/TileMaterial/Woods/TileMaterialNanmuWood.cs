using System;
using System.Collections.Generic;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialNanmuWood : TileMaterialWood
    {
        private const int MONEY_BASE = 1;
        private const int MONEY_EXTRA = 2;

        public TileMaterialNanmuWood(int ID) : base(ID, "nanmu_wood")
        {
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MONEY_BASE, MONEY_EXTRA);
        }

        public override void AppendToListDiscardEffect(Player player, Permutation perm, List<IAnimationEffect> effects,
            Tile tile, bool withForce, bool isClone)
        {
            base.AppendToListDiscardEffect(player, perm, effects, tile, withForce, isClone);
            effects.Add(new EarnMoneyEffect(MONEY_BASE, null).OnTile(tile));
            if (withForce)
            {
                effects.Add(new EarnMoneyEffect(MONEY_EXTRA, null).OnTile(tile));
            }
        }
    }
}