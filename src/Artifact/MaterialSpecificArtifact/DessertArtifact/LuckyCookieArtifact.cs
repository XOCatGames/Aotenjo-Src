using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class LuckyCookieArtifact : Artifact
    {
        public LuckyCookieArtifact() : base("lucky_cookie", Rarity.RARE)
        {
            SetHighlightRequirement((tile, player) => 
                tile.IsNumbered() && tile.properties.material is TileMaterialDessert);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.OnDessertTileConsumedEvent += OnDessertTileConsumed;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.OnDessertTileConsumedEvent -= OnDessertTileConsumed;
        }

        private void OnDessertTileConsumed(Player player, Tile tile, TileMaterialDessert dessert)
        {
            // 只对数牌响应
            if (!tile.IsNumbered()) return;

            ApplyLuckyCookieEffect(player, tile, dessert);

            // 如果牌山总数超过136枚，重复这个效果一次
            if (player.GetTilePool().Count > 136)
            {
                ApplyLuckyCookieEffect(player, tile, dessert);
            }
        }

        private void ApplyLuckyCookieEffect(Player player, Tile tile, TileMaterialDessert dessert)
        {
            int tileOrder = tile.GetOrder();
            
            // 在牌山中找相同数字的数牌
            var candidateTiles = player.GetTilePool()
                .Where(t => t.IsNumbered() && t.GetOrder() == tileOrder)
                .ToList();

            if (candidateTiles.Count > 0)
            {
                // 随机选择一张牌
                Tile targetTile = candidateTiles[player.GenerateRandomInt(candidateTiles.Count)];
                
                // 给目标牌添加新鲜的甜品材质（使用原来的甜品类型）
                TileMaterial freshDessert = dessert.CreateFreshCopy();
                if (freshDessert != null)
                {
                    targetTile.SetMaterial(freshDessert, player, true);
                }
            }
        }
    }
} 