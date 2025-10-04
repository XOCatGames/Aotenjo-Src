using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialSugarCube : TileMaterialDessert
    {
        private const int TRANSFORM_THRESHOLD = 1; // 消耗1次后变化材质
        private static readonly int[] FAN_TABLE = { 5, 12, 18, 36 };

        [SerializeField] private int materialID;
        [SerializeField] private bool isInitialized = false; // 标记是否已经根据牌的底番初始化

        public TileMaterialSugarCube(int id) : base(id, "dessert_sugar_cube", 5, Rarity.COMMON) // 临时设置为1，实际会在设置到牌上时调整
        {
            materialID = id;
            isInitialized = false;
        }

        private TileMaterialSugarCube(int id, int maxUses, int remainingUses) : base(id, "dessert_sugar_cube", maxUses, Rarity.COMMON)
        {
            materialID = id;
            usesLeft = remainingUses;
            isInitialized = true;
        }

        private TileMaterialSugarCube(int id, int maxUses, int remainingUses, int totalConsumed) : base(id, "dessert_sugar_cube", maxUses, Rarity.COMMON)
        {
            materialID = id;
            usesLeft = remainingUses;
            this.totalUsesConsumed = totalConsumed;
            isInitialized = true;
        }

        protected override int GetMaterialTransformThreshold()
        {
            return TRANSFORM_THRESHOLD;
        }

        protected override void AddDessertEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            int idx = ((player.Level - 1) / 4) % 4;
            int fanBonus = FAN_TABLE[idx];
            effects.Add(ScoreEffect.AddFan(fanBonus, null));
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialSugarCube(materialID, maxUses, usesLeft, totalUsesConsumed);
        }



        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            int idx = ((player.Level - 1) / 4) % 4;
            return string.Format(localizer(GetDescriptionLocalizeKey()), usesLeft, maxUses, FAN_TABLE[idx]);
        }
    }
}
