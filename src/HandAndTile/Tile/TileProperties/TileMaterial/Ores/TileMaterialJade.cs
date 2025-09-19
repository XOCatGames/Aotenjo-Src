using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialJade : TileMaterial, IJade
    {
        private const double INCREMENT = 0.4f;

        private const int cap = 5;

        [SerializeField] public int level;
        public int GetLevel(Player player)
        {
            return level;
        }

        public TileMaterialJade(int ID) : base(ID, "jade", null)
        {
        }

        private TileMaterialJade(int ID, int level) : this(ID)
        {
            this.level = level;
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialJade(6, level);
        }

        public override Rarity GetRarity()
        {
            return Rarity.RARE;
        }

        protected override int GetSpriteID()
        {
            if (level < cap)
                return base.GetSpriteID();
            return 53;
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            if (level > 0)
            {
                effects.Add(ScoreEffect.MulFan(1d + INCREMENT * level, null));
            }

            if (level < cap && player.Selecting(tile))
                effects.Add(new UpgradeEffect(this));
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), (1d + INCREMENT * level).ToShortString(),
                INCREMENT.ToShortString(),
                (1d + INCREMENT * cap).ToShortString());
        }

        private class UpgradeEffect : Effect
        {
            private TileMaterialJade mat;

            public UpgradeEffect(TileMaterialJade mat)
            {
                this.mat = mat;
            }

            public override string GetSoundEffectName()
            {
                return "JadeSound";
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_jade_mirror_level_up");
            }

            public override Artifact GetEffectSource()
            {
                return null;
            }

            public override void Ingest(Player player)
            {
                if (mat.level >= cap)
                {
                    return;
                }

                mat.level++;
            }
        }

    }
}