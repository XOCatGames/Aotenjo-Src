using System.Linq;
using Aotenjo;

namespace Aotenjo
{
    public class AltarArtifact : BambooArtifact
    {
        public AltarArtifact() : base("altar", Rarity.RARE)
        {
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

        // 检查指示区内是否已存在相同类型的甜品牌体
        private bool IsDesertMaterialInIndicators(Player player, TileMaterialDessert dessert)
        {
            if (!(player is BambooDeckPlayer bambooPlayer)) return false;

            // 获取指示区中的所有牌
            var indicators = bambooPlayer.GetIndicators();
            
            // 检查是否有相同类型的甜品材质
            return indicators.Any(indicator => 
                indicator.tile.properties.material is TileMaterialDessert indicatorDessert &&
                indicatorDessert.GetRegName() == dessert.GetRegName());
        }

        // 阻止指示区内已存在的甜品牌体失去新鲜度
        private void OnDessertTileConsumeAttempt(PlayerEvent evt)
        {
            if (evt is PlayerConsumeDessertEvent dessertEvt)
            {
                // 检查指示区内是否已存在相同类型的甜品牌体
                if (IsDesertMaterialInIndicators(dessertEvt.player, dessertEvt.dessert))
                {
                    evt.canceled = true; // 阻止消耗
                }
            }
        }
    }
}