using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CakeKnifeArtifact : Artifact
    {
        public CakeKnifeArtifact() : base("cake_knife", Rarity.COMMON)
        {
            SetHighlightRequirement((tile, player) => tile.properties.material is TileMaterialDessert);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.OnDessertTileConsumeAttemptEvent += OnDessertTileConsumeAttempt;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.OnDessertTileConsumeAttemptEvent -= OnDessertTileConsumeAttempt;
        }

        // 拦截甜品消耗，直接让其完全消耗并碎裂
        private void OnDessertTileConsumeAttempt(PlayerEvent evt)
        {
            if (evt is PlayerConsumeDessertEvent dessertEvt)
            {
                // 如果事件已经被其他神器取消，则不执行蛋糕刀的效果
                if (evt.canceled)
                {
                    return;
                }
                
                // 使用甜品的完全消耗方法
                dessertEvt.dessert.ConsumeCompletely(dessertEvt.player);
                
                // 然后设置为碎裂状态
                dessertEvt.tile.SetMask(TileMask.Fractured(), dessertEvt.player);
                
                evt.canceled = true; // 阻止正常消耗
            }
        }
    }
} 