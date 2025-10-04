using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class FallenCrystalBallArtifact : CraftableArtifact, IJade
    {
        private const double MUL_PER_NEST = 0.15f;

        public FallenCrystalBallArtifact() : base("fallen_crystal_ball", Rarity.EPIC)
        {
            SetHighlightRequirement((t, p) =>
                t.properties.IsDebuffed() || p.DetermineMaterialCompactbility(t, TileMaterial.Nest()));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(player, localizer), Utils.NumberToFormat(MUL_PER_NEST),
                Utils.NumberToFormat(GetMul(player)));
        }

        public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendOnUnusedTileEffects(player, perm, tile, effects);
            if (tile.properties.IsDebuffed())
            {
                if (player.GenerateRandomInt(3) == 0)
                {
                    effects.Add(new TransformMaterialEffect(TileMaterial.Nest(), this, tile, "effect_corrupt"));
                }
            }
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.MulFan(() => GetMul(player), this));
        }

        private double GetMul(Player player)
        {
            return 1 + MUL_PER_NEST * this.GetEffectiveJadeStack(player);
        }

        private static int GetNestCount(Player player)
        {
            return player
                .GetAllTiles().Count(t => t != null && player.DetermineMaterialCompactbility(t, TileMaterial.Nest()));
        }

        public int GetLevel(Player player)
        {
            return GetNestCount(player);
        }
    }
}