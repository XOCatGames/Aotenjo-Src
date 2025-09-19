using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialBonePorcelain : TileMaterialPorcelain
    {
        private const int FAN_PER_LEVEL = 1;
        [SerializeField] private int level = 3;

        public TileMaterialBonePorcelain(int ID) : base(ID, "bone_porcelain")
        {
            level = 3;
        }

        public TileMaterialBonePorcelain(int ID, int level) : base(ID, "bone_porcelain")
        {
            this.level = level;
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialBonePorcelain(spriteID, level);
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), level * FAN_PER_LEVEL, FAN_PER_LEVEL);
        }

        public override void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects)
        {
            base.AppendUnusedEffects(player, perm, effects);
            effects.Add(ScoreEffect.AddFan(level * FAN_PER_LEVEL, null));
            effects.Add(new UpgradeEffect(this));
        }

        private class UpgradeEffect : Effect
        {
            private TileMaterialBonePorcelain mat;

            public UpgradeEffect(TileMaterialBonePorcelain m)
            {
                mat = m;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_bone_procelain_upgrade_name");
            }

            public override Artifact GetEffectSource()
            {
                return null;
            }

            public override void Ingest(Player player)
            {
                mat.level++;
            }
        }
    }
}