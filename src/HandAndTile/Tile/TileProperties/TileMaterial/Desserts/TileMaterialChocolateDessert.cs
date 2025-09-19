using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialChocolateDessert : TileMaterialDessert
    {
        private const double BASE_MUL = 1.5;
        private const int MAX_USES = 3;
        private const int TRANSFORM_THRESHOLD = 1; // 消耗3次后变化材质
        [SerializeField] private int materialID;

        public TileMaterialChocolateDessert(int ID) : base(ID, "dessert_chocolate", MAX_USES, Rarity.COMMON)
        {
            this.materialID = ID;
        }

        private TileMaterialChocolateDessert(int ID, int remainingUses)
            : base(ID, "dessert_chocolate", remainingUses, Rarity.COMMON) 
        { 
            this.materialID = ID;
        }

        private TileMaterialChocolateDessert(int ID, int remainingUses, int totalConsumed)
            : base(ID, "dessert_chocolate", remainingUses, Rarity.COMMON) 
        { 
            this.materialID = ID;
            this.totalUsesConsumed = totalConsumed;
        }

        protected override int GetMaterialTransformThreshold()
        {
            return TRANSFORM_THRESHOLD;
        }

        protected override void AddDessertEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            effects.Add(ScoreEffect.MulFan(BASE_MUL, null));
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialChocolateDessert(materialID, usesLeft, totalUsesConsumed);
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(localizer(GetDescriptionLocalizeKey()), usesLeft, maxUses, BASE_MUL);
        }
    }
}
