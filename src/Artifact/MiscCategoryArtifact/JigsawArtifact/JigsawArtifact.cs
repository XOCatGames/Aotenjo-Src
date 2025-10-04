using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public abstract class JigsawArtifact : LevelingArtifact, ICountable
    {
        private const int INIT_LEVEL = 3;
        public const float FU = 20f;
        public const float FAN = 12f;
        public const float MUL = 2f;

        public JigsawArtifact(string name, Rarity rarity) : base(name, rarity, INIT_LEVEL)
        {
        }


        public override string GetName(Func<string, string> localizer)
        {
            if (Level == 0)
                return localizer("artifact_omnijigsaw_name");
            return base.GetName(localizer);
        }

        protected override int GetSpriteID()
        {
            if (Level == 0)
                return 107;
            return base.GetSpriteID();
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            if (Level == 0)
            {
                string format = localizer("artifact_omnijigsaw_description");
                string fu = localizer("artifact_omnijigsaw_fu");
                string fan = localizer("artifact_omnijigsaw_fan");
                string mul = localizer("artifact_omnijigsaw_mul");

                string redFormat = "<style=\"yellow\">{0}</style>";

                if (player.GetArtifacts().Contains(Artifacts.Jigsaw20) && Artifacts.Jigsaw20.Level == 0)
                {
                    fu = string.Format(redFormat, fu);
                }

                if (player.GetArtifacts().Contains(Artifacts.Jigsaw12) && Artifacts.Jigsaw12.Level == 0)
                {
                    fan = string.Format(redFormat, fan);
                }

                if (player.GetArtifacts().Contains(Artifacts.Jigsaw2) && Artifacts.Jigsaw2.Level == 0)
                {
                    mul = string.Format(redFormat, mul);
                }

                return string.Format(format, fu, fan, mul);
            }

            return string.Format(base.GetDescription(localizer), Level);
        }

        protected abstract bool EligibleToDowngrade(Block block, Block.Jiang jiang);
        protected abstract Effect GetPlainEffect();

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            if (Level != 0) return;
            if (player.GetArtifacts().Contains(Artifacts.Jigsaw12) && Artifacts.Jigsaw12.Level == 0)
            {
                effects.Add(ScoreEffect.AddFan(FAN, this));
            }

            if (player.GetArtifacts().Contains(Artifacts.Jigsaw2) && Artifacts.Jigsaw2.Level == 0)
            {
                effects.Add(ScoreEffect.MulFan(MUL, this));
            }
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block,
            List<Effect> effects)
        {
            if (Level == 0 && player.GetArtifacts().Contains(Artifacts.Jigsaw20) && Artifacts.Jigsaw20.Level == 0)
            {
                effects.Add(ScoreEffect.AddFu(FU, this));
                return;
            }

            if (!EligibleToDowngrade(block, permutation.jiang) || !block.All(t => player.Selecting(t))) return;
            effects.Add(GetPlainEffect());
            effects.Add(new DowngradeEffect(this));
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
        }

        public int GetMaxCounter()
        {
            return 4;
        }

        public int GetCurrentCounter()
        {
            return Level == 0 ? -1 : Level;
        }

        private class DowngradeEffect : Effect
        {
            private LevelingArtifact source;

            public DowngradeEffect(LevelingArtifact source)
            {
                this.source = source;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_jigsaw_upgrade");
            }

            public override Artifact GetEffectSource()
            {
                return source;
            }

            public override void Ingest(Player player)
            {
                if (source.Level > 0)
                    source.Level--;
            }
        }
    }
}