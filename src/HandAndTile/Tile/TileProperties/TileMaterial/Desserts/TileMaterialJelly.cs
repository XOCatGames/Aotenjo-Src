using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialJelly : TileMaterialDessert
    {
        private const int MAX_USES = 4;
        private const int TRANSFORM_THRESHOLD = 1; // 消耗1次后变化材质
        private const double SCORE_MULTIPLIER = 1.5; // 甜品得分加成

        [SerializeField] private int materialID;

        public TileMaterialJelly(int id): base(id, "dessert_jelly", MAX_USES, Rarity.RARE)
        {
            materialID = id;
        }

        private TileMaterialJelly(int id, int remainingUses): base(id, "dessert_jelly", remainingUses, Rarity.RARE)
        {
            materialID = id;
        }

        private TileMaterialJelly(int id, int remainingUses, int totalConsumed): base(id, "dessert_jelly", remainingUses, Rarity.RARE)
        {
            materialID = id;
            totalUsesConsumed = totalConsumed;
        }

        protected override int GetMaterialTransformThreshold()
        {
            return TRANSFORM_THRESHOLD;
        }

        protected override void AddDessertEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            effects.Add(ScoreEffect.MulFan(SCORE_MULTIPLIER, null));
            if (player.Selecting(tile))
                effects.Add(new ShufflePlainCopyEffect(tile));
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialJelly(materialID, usesLeft, totalUsesConsumed);
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(localizer(GetDescriptionLocalizeKey()), usesLeft, maxUses, SCORE_MULTIPLIER);
        }

        [Serializable]
        private class ShufflePlainCopyEffect : Effect
        {
            private readonly Tile sourceTile;
            public ShufflePlainCopyEffect(Tile src) { sourceTile = src; }

            public override string GetEffectDisplay(Func<string, string> func)
                => func("effect_copied");

            public override Artifact GetEffectSource() => null;

            public override void Ingest(Player player)
            {
                Tile copy = new Tile(sourceTile.GetCategory(), sourceTile.GetOrder());
                player.AddNewTileToPool(copy);
            }
        }
    }
}
