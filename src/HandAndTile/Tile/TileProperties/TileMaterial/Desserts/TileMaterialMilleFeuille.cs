using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialMilleFeuille : TileMaterialDessert
    {
        private const int BASE_FU = 50;  //初符数
        private const int MAX_USES = 5;   //可用次数
        private const int TRANSFORM_THRESHOLD = 1; // 消耗1次后变化材质
        [SerializeField] private int materialID;

        public TileMaterialMilleFeuille(int id): base(id, "dessert_mille_feuille", MAX_USES, Rarity.COMMON)
        {
            materialID = id;
        }

        private TileMaterialMilleFeuille(int id, int remainingUses): base(id, "dessert_mille_feuille", remainingUses, Rarity.COMMON)
        {
            materialID = id;
        }

        private TileMaterialMilleFeuille(int id, int remainingUses, int totalConsumed): base(id, "dessert_mille_feuille", remainingUses, Rarity.COMMON)
        {
            materialID = id;
            this.totalUsesConsumed = totalConsumed;
        }

        protected override int GetMaterialTransformThreshold()
        {
            return TRANSFORM_THRESHOLD;
        }

        protected override void AddDessertEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            effects.Add(ScoreEffect.AddFu(BASE_FU, null));
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialMilleFeuille(materialID, usesLeft, totalUsesConsumed);
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(localizer(GetDescriptionLocalizeKey()), usesLeft, maxUses, BASE_FU);  
        }
    }
}
