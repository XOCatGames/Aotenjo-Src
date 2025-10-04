using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class ScarletHourglassArtifact : LevelingArtifact
    {
        private const int INIT_FU = 40;
        private const int INIT_FAN = 10;

        private const int FU_PER_LEVEL = 4;
        private const int FAN_PER_LEVEL = 1;

        private const int MAX_LEVEL = 10;
        private const int MIN_LEVEL = -10;

        public ScarletHourglassArtifact() : base("scarlet_hourglass", Rarity.RARE, 0)
        {
            
            SetHighlightRequirement((t, p) => ((ScarletPlayer)p).IsCompatibleWithMainCategory(t.GetCategory()) ||
                                              ((ScarletPlayer)p).IsCompatibleWithSubCategory(t.GetCategory()));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            int fu = GetFu();
            int fan = GetFan();
            return string.Format(base.GetDescription(localizer), FU_PER_LEVEL, FAN_PER_LEVEL, fu, fan);
        }

        private int GetFan()
        {
            return INIT_FAN + Level * FAN_PER_LEVEL;
        }

        private int GetFu()
        {
            return INIT_FU - Level * FU_PER_LEVEL;
        }

        public override void AddOnSelfEffects(Player player, Permutation perm, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, perm, effects);
            effects.Add(ScoreEffect.AddFu(GetFu(), this));
            effects.Add(ScoreEffect.AddFan(GetFan(), this));
        }

        public override void AddOnBlockEffects(Player player, Permutation perm, Block block, List<Effect> effects)
        {
            base.AddOnBlockEffects(player, perm, block, effects);

            if (!block.Any(tile => player.Selecting(tile))) return;
            if (player is not ScarletPlayer scarlet) return;

            var cat = block.GetCategory();

            if (scarlet.IsCompatibleWithMainCategory(cat))
            {
                effects.Add(new UpgradeEffect(this));
            }

            if (scarlet.IsCompatibleWithSubCategory(cat))
            {
                effects.Add(new DowngradeEffect(this));
            }
        }

        private class UpgradeEffect : Effect
        {
            private readonly ScarletHourglassArtifact artifact;

            public UpgradeEffect(ScarletHourglassArtifact artifact)
            {
                this.artifact = artifact;
            }

            public override void Ingest(Player player)
            {
                if (artifact.Level < MAX_LEVEL)
                    artifact.Level++;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_scarlet_hourglass_upgrade");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }
        }

        private class DowngradeEffect : Effect
        {
            private readonly ScarletHourglassArtifact artifact;

            public DowngradeEffect(ScarletHourglassArtifact artifact)
            {
                this.artifact = artifact;
            }

            public override void Ingest(Player player)
            {
                if (artifact.Level > MIN_LEVEL)
                    artifact.Level--;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_scarlet_hourglass_downgrade");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }
        }
    }
}