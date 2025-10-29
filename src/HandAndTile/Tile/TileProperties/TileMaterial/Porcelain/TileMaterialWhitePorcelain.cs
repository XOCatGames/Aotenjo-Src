using System;
using System.Collections.Generic;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialWhitePorcelain : TileMaterialPorcelain
    {
        private const double FU_PER_TILE = 10;

        public TileMaterialWhitePorcelain(int ID) : base(ID, "white_porcelain")
        {
        }

        public override Rarity GetRarity()
        {
            return Rarity.COMMON;
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            return string.Format(base.GetDescription(localizer), FU_PER_TILE);
        }

        public override void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects)
        {
            base.AppendUnusedEffects(player, perm, effects);
            effects.Add(new TextEffect("effect_appreciate_white_porcelain"));
        }

        public override void AppendToListOnTileUnusedEffect(Player player, Permutation perm, List<Effect> effects,
            Tile scoringTile, Tile onEffectTile)
        {
            base.AppendToListOnTileUnusedEffect(player, perm, effects, scoringTile, onEffectTile);
            if (!player.DetermineTileCompatibility(scoringTile, (int)onEffectTile.GetCategory(), -1)) return;
            effects.Add(ScoreEffect.AddFu(FU_PER_TILE, null));
        }
    }
}