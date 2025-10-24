using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class AgateDaggerArtifact : Artifact
    {
        private const float MUL_PER_AGATE = 0.1f;

        public AgateDaggerArtifact() : base("crystal_dagger", Rarity.EPIC)
        {
            SetHighlightRequirement((tile, player) => tile.CompatWithMaterial(TileMaterial.Agate(), player));
            SetPrerequisite(player =>
                player.GetAllTiles().Any(tile => tile.CompatWithMaterial(TileMaterial.Agate(), player)));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(player, localizer), MUL_PER_AGATE,
                Utils.NumberToFormat(1f + AgateEffect.GetAgateCount(player) * MUL_PER_AGATE));
        }

        public override void AppendOnTileEffects(Player player, Permutation perm, Tile tile, List<Effect> lst)
        {
            base.AppendOnTileEffects(player, perm, tile, lst);
            if (tile.CompatWithMaterial(TileMaterial.Agate(), player))
            {
                perm.ToTiles().Count(a => a.CompatWithMaterial(TileMaterial.Agate(), player));
                lst.Add(new AgateDaggerEffect(MUL_PER_AGATE, this));
            }
        }

        private class AgateDaggerEffect : ScoreEffect
        {
            private float mul;
            private Artifact artifact;

            public AgateDaggerEffect(float mul, Artifact artifact)
            {
                this.mul = mul;
                this.artifact = artifact;
            }

            public override string GetEffectDisplay(Player player, Func<string, string> func)
            {
                return string.Format(func("effect_mul_fan_format"), 1f + mul * AgateEffect.GetAgateCount(player));
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }

            public override void Ingest(Player player)
            {
                int count = AgateEffect.GetAgateCount(player);
                player.RoundAccumulatedScore = player.RoundAccumulatedScore.MultiplyFan(1f + mul * count);
            }
        }
    }
}