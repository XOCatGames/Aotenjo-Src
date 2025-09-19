using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public abstract class TileMaterialDessert : TileMaterial
    {
        [SerializeField] public int maxUses;
        [SerializeField] public int usesLeft;
        [SerializeField] public int totalUsesConsumed = 0;
        [SerializeField] private Rarity rarity;

        protected TileMaterialDessert(int ID, string name, int initialUses, Rarity rarity = Rarity.COMMON) : base(ID, name, null)
        {
            maxUses = initialUses;
            usesLeft = initialUses;
            this.rarity = rarity;
        }

        protected abstract void AddDessertEffects(Player player, Permutation perm, Tile tile, List<Effect> effects);

        protected abstract int GetMaterialTransformThreshold();

        protected string GetConsumeEffectDisplay()
        {
            return "effect_consume";
        }

        // 获取剩余使用次数
        public int GetRemainingUses()
        {
            return usesLeft;
        }

        // 获取总消耗次数
        public int GetTotalUsesConsumed()
        {
            return maxUses - usesLeft;
        }

        // 恢复1点新鲜度
        public void RestoreFreshness()
        {
            if (GetTotalUsesConsumed() > 0 && usesLeft < maxUses)
            {
                usesLeft++;
            }
        }

        // 完全消耗甜品（将剩余使用次数一次性消耗完）
        public void ConsumeCompletely(Player player)
        {
            Tile target = player.GetAllTiles()
                                .FirstOrDefault(t => t.properties.material == this);
            if (target == null) return;

            // 将剩余使用次数全部消耗
            usesLeft = 0;

            // 触发甜品消耗完毕事件并转换为普通牌
            player.TriggerDessertTileConsumedEvent(target, this);
            target.SetMaterial(PLAIN, player, true);
            player.stats.RecordCustomStats(PlayerStatsType.EAT_FOOD, 1);
        }

        // 创建满新鲜度的同类型甜品
        public virtual TileMaterialDessert CreateFreshCopy()
        {
            var freshCopy = (TileMaterialDessert)Copy();
            freshCopy.usesLeft = freshCopy.maxUses;
            return freshCopy;
        }

        public override int GetSpriteID(Player player)
        {
            int threshold = GetMaterialTransformThreshold();
            if (threshold <= 0) return base.GetSpriteID(player);
            
            int transformations = Math.Min(3, GetTotalUsesConsumed() / threshold);
            return spriteID - (transformations * 10);
        }

        public abstract override TileMaterial Copy();

        public override Rarity GetRarity() => rarity;

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);

            if (usesLeft <= 0) return;

            //只触发加分等效果，不再在打出时消耗
            AddDessertEffects(player, perm, tile, effects);
        }
        
        public override void AppendToListRoundEndEffect(Player player, Permutation perm, List<IAnimationEffect> effects,
            Tile tile)
        {
            if (perm == null) return;
            if (perm.ToTiles().Contains(tile))
            {
                // 如果这是最后一口，使用特殊的LastBiteEffect
                if (usesLeft == 1)
                {
                    effects.Add(new LastBiteEffect(this).OnTile(tile));
                }
                else
                {
                    effects.Add(new ConsumeUseEffect(this).OnTile(tile));
                }
            }
        }

        protected void ConsumeUse(Player player)
        {
            Tile target = player.GetAllTiles()
                                .FirstOrDefault(t => t.properties.material == this);
            if (target == null) return;
            
            // 触发消耗尝试事件，如果被阻止则不消耗
            if (!player.TriggerDessertTileConsumeAttemptEvent(target, this))
            {
                return; // 消耗被阻止
            }
            
            usesLeft--;
            totalUsesConsumed++;
            
            // 如果使用次数耗尽，变为 PLAIN
            if (usesLeft <= 0)
            {
                // 在变为普通牌之前触发甜品消耗完毕事件
                player.TriggerDessertTileConsumedEvent(target, this);
                target.SetMaterial(PLAIN, player, true);
                player.stats.RecordCustomStats(PlayerStatsType.EAT_FOOD, 1);
            }
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), usesLeft, maxUses);
        }


        [Serializable]
        public class ConsumeUseEffect : Effect
        {
            private readonly TileMaterialDessert dessert;
            public ConsumeUseEffect(TileMaterialDessert d) { dessert = d; }

            public override string GetEffectDisplay(Func<string, string> func)
                => func(dessert.GetConsumeEffectDisplay());

            public override Artifact GetEffectSource() => null;

            public override void Ingest(Player player) => dessert.ConsumeUse(player);

            public override string GetSoundEffectName()
            {
                return "Food";
            }
        }

        [Serializable]
        public class LastBiteEffect : Effect
        {
            private readonly TileMaterialDessert dessert;
            public LastBiteEffect(TileMaterialDessert d) { dessert = d; }

            public override string GetEffectDisplay(Func<string, string> func)
                => func("effect_tanghulu_used");

            public override Artifact GetEffectSource() => null;

            public override void Ingest(Player player)
            {
                dessert.ConsumeUse(player);
            }

            public override string GetSoundEffectName()
            {
                return "Food";
            }
        }
    }
}
