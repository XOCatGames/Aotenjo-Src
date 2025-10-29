using System;
using System.Collections.Generic;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialPinkPorcelain : TileMaterialPorcelain
    {
        private const double FAN_PER_TILE = 1;

        public TileMaterialPinkPorcelain(int ID) : base(ID, "pink_porcelain")
        {
        }

        public override Rarity GetRarity()
        {
            return Rarity.COMMON;
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            return string.Format(base.GetDescription(localizer), FAN_PER_TILE * player.GetPrevalentWind());
        }

        public override void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects)
        {
            base.AppendUnusedEffects(player, perm, effects);
            effects.Add(new TextEffect("effect_appreciate_pink_porcelain"));
        }

        public override void AppendToListOnTileUnusedEffect(Player player, Permutation perm, List<Effect> effects,
            Tile scoringTile, Tile onEffectTile)
        {
            base.AppendToListOnTileUnusedEffect(player, perm, effects, scoringTile, onEffectTile);
            if (player.DetermineTileCompatibility(scoringTile, (int)onEffectTile.GetCategory(), -1)) return;
            effects.Add(ScoreEffect.AddFan(FAN_PER_TILE * (1 + ((player.Level - 1) / 4)), null));
            if (player.GetArtifacts().Contains(Artifacts.PorcelainFish) && onEffectTile.ContainsRed(player))
            {
                effects.Add(new GrowFuEffect(Artifacts.PorcelainFish, onEffectTile,
                    PorcelainFishArtifact.GROW_FU_AMOUNT));
            }
        }
    }
}