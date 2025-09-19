using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialButter : TileMaterialDessert
    {
        private const int INITIAL_COINS = 4;   // 首次获得金币
        private const int DEC_PER_USE = 1;   // 每次递减
        private const int MAX_USES = 3;
        private const int TRANSFORM_THRESHOLD = 1; // 消耗1次后变化材质

        [SerializeField] private int materialID;

        public TileMaterialButter(int id): base(id, "dessert_butter", MAX_USES, Rarity.COMMON)
        {
            materialID = id;
        }

        private TileMaterialButter(int id, int remainingUses): base(id, "dessert_butter", remainingUses, Rarity.COMMON)
        {
            materialID = id;
        }

        private TileMaterialButter(int id, int remainingUses, int totalConsumed): base(id, "dessert_butter", remainingUses, Rarity.COMMON)
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
            if (player.Selecting(tile))
            {
                int currentCoins = INITIAL_COINS - DEC_PER_USE * (MAX_USES - usesLeft);
                effects.Add(new EarnMoneyEffect(currentCoins, null));
            }
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialButter(spriteID, usesLeft, totalUsesConsumed);
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            int currentCoins = INITIAL_COINS - DEC_PER_USE * (MAX_USES - usesLeft);
            return string.Format(localizer(GetDescriptionLocalizeKey()), usesLeft, maxUses, currentCoins);
        }
    }
}
