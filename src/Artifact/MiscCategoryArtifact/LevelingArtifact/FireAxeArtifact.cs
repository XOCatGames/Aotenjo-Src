using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class FireAxeArtifact : LevelingArtifact, IFanProvider
    {
        public FireAxeArtifact() : base("fire_axe", Rarity.RARE, 5)
        {
            SetHighlightRequirement((t, player) => !t.IsYaoJiu(player));
        }

        public override string GetDescription(Player p, Func<string, string> localize)
        {
            return string.Format(base.GetDescription(localize), GetFan(p));
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToAddFanFormat(GetFan(player));
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.AddFan(Level, this));
            if (Level > 20)
                effects.Add(ScoreEffect.AddFu(80, this));
            if (permutation.ToTiles().All(t => !t.IsYaoJiu(player))) effects.Add(new UpgradeEffect(this));
        }

        private class UpgradeEffect : Effect
        {
            private readonly FireAxeArtifact axe;

            public UpgradeEffect(FireAxeArtifact artifact)
            {
                axe = artifact;
            }

            public override void Ingest(Player player)
            {
                axe.Level++;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_fire_axe_level_up");
            }

            public override Artifact GetEffectSource()
            {
                return axe;
            }

            public override string GetSoundEffectName()
            {
                return "FireAxe";
            }
        }

        public double GetFan(Player player) => Level;
    }
}