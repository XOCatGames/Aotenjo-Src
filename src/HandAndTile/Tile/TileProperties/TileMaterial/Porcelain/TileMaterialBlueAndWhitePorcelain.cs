using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialBlueAndWhite : TileMaterialPorcelain
    {
        private const int MAX_LEVEL = 3;
        private const float MULT_PER_LEVEL = 1f;

        [SerializeField] private int level;

        public TileMaterialBlueAndWhite(int ID) : base(ID, "blue_and_white_porcelain")
        {
            level = 0;
        }

        private TileMaterialBlueAndWhite(int ID, int level) : this(ID)
        {
            this.level = level;
        }

        public override Rarity GetRarity()
        {
            return Rarity.RARE;
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialBlueAndWhite(spriteID, level);
        }

        protected override int GetSpriteID()
        {
            if (level < MAX_LEVEL)
                return base.GetSpriteID();
            return base.GetSpriteID() + 3;
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), 1f + MULT_PER_LEVEL * level, MULT_PER_LEVEL,
                1f + MAX_LEVEL * MULT_PER_LEVEL);
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            effects.Add(ScoreEffect.MulFan(1f + MULT_PER_LEVEL * level, null));
            if (!player.Selecting(tile)) return;
            effects.Add(new FractureEffect(null, tile));
        }

        public override void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects)
        {
            base.AppendUnusedEffects(player, perm, effects);
            if (level < MAX_LEVEL)
            {
                effects.Add(new UpgradeEffect(this));
            }
        }

        private class UpgradeEffect : Effect
        {
            private TileMaterialBlueAndWhite mat;

            public UpgradeEffect(TileMaterialBlueAndWhite m)
            {
                mat = m;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_blue_and_white_porcelain_upgrade_name");
            }

            public override Artifact GetEffectSource()
            {
                return null;
            }

            public override void Ingest(Player player)
            {
                if (mat.level < MAX_LEVEL)
                {
                    mat.level++;
                }
            }
        }
    }
}