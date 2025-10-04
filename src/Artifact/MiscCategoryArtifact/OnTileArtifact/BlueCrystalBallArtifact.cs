using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class BlueCrystalBallArtifact : LevelingArtifact, IFuProvider, IJade
    {
        private const int FU_PER_LEVEL = 1;

        public BlueCrystalBallArtifact() : base("blue_crystal_ball", Rarity.RARE, 0)
        {
            SetHighlightRequirement((t, player) => !t.IsYaoJiu(player));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU_PER_LEVEL, GetFu(player));
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);

            effects.Add(ScoreEffect.AddFu(() => GetFu(player), this));
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player) => ToAddFuFormat(GetFu(player));


        public double GetFu(Player player)
        {
            return this.GetEffectiveJadeStack(player) * FU_PER_LEVEL;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (!player.Selecting(tile)) return;


            if (tile.IsYaoJiu(player))
            {
                effects.Add(new DowngradeEffect(this));
            }
            else
            {
                effects.Add(new UpgradeEffect(this));
            }
        }

        private class UpgradeEffect : Effect
        {
            private readonly BlueCrystalBallArtifact artifact;

            public UpgradeEffect(BlueCrystalBallArtifact artifact)
            {
                this.artifact = artifact;
            }

            public override void Ingest(Player player)
            {
                artifact.Level++;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_blue_crystal_ball_upgrade");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }
        }

        private class DowngradeEffect : Effect
        {
            private readonly BlueCrystalBallArtifact artifact;

            public DowngradeEffect(BlueCrystalBallArtifact artifact)
            {
                this.artifact = artifact;
            }

            public override void Ingest(Player player)
            {
                if (artifact.Level > 0)
                    artifact.Level--;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_blue_crystal_ball_downgrade");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }
        }

    }
}