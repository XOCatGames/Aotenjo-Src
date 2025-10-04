using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class OrigamiBearArtifact : LevelingArtifact, ICountable
    {
        private const int INIT_FU = 40;
        private const int UPGRADED_FU = 70;

        public OrigamiBearArtifact() : base("origami_bear", Rarity.COMMON, 4)
        {
            SetHighlightRequirement((a, _) => a.CompactWithCategory(Tile.Category.Wan));
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToAddFuFormat(Level == 0 ? UPGRADED_FU : INIT_FU);
        }

        protected override int GetSpriteID()
        {
            return Level == 0 ? 172 : base.GetSpriteID();
        }

        public override string GetDescription(Func<string, string> localize)
        {
            if (Level == 0) return string.Format(localize("artifact_origami_bear_description_upgraded"), UPGRADED_FU);
            return string.Format(base.GetDescription(localize), Level, INIT_FU);
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);

            int count = player.GetCurrentSelectedBlocks().Count(b => b.OfCategory(Tile.Category.Wan));

            for (int i = 0; i < Math.Min(Level, count); i++)
            {
                effects.Add(new UpgradeEffect(this, Level == 1));
            }

            if (count >= Level)
            {
                effects.Add(ScoreEffect.AddFu(UPGRADED_FU, this));
            }
            else
            {
                effects.Add(ScoreEffect.AddFu(INIT_FU, this));
            }
        }

        public int GetMaxCounter()
        {
            return 4;
        }

        public int GetCurrentCounter()
        {
            return Level == 0 ? -1 : Level;
        }

        private class UpgradeEffect : Effect
        {
            private readonly OrigamiBearArtifact bear;
            private readonly bool upgrade;

            public UpgradeEffect(OrigamiBearArtifact artifact, bool upgrade)
            {
                bear = artifact;
                this.upgrade = upgrade;
            }

            public override void Ingest(Player player) => bear.Level--;

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return upgrade ? func("effect_bear_growl") : func("effect_bear_upgrade");
            }

            public override Artifact GetEffectSource()
            {
                return bear;
            }
        }
    }
}