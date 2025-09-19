using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class BlueCandleArtifact : Artifact
    {
        private const int CHANCE = 3; // 1/3 几率

        public BlueCandleArtifact() : base("blue_candle", Rarity.COMMON)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), CHANCE, GetChanceMultiplier(player));
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
            // 只对黄油牌响应
            if (dessert is TileMaterialButter)
            {
                var transformEffect = new BlueCandleEffect(this, tile).MaybeTriggerWithChance(CHANCE, "blue_candle");
                transformEffect.Ingest(player);
            }
        }

        private class BlueCandleEffect : TextEffect
        {
            private readonly Tile targetTile;

            public BlueCandleEffect(BlueCandleArtifact artifact, Tile tile) : base("effect_blue_candle", artifact, "Transform")
            {
                targetTile = tile;
            }

            public override void Ingest(Player player)
            {
                // 生成随机字牌（风牌：1-4z，三元牌：5-7z）
                List<Tile.Category> honorCategories = new List<Tile.Category> { Tile.Category.Feng, Tile.Category.Jian };
                Tile.Category selectedCategory = honorCategories[player.GenerateRandomInt(honorCategories.Count)];
                
                int order;
                if (selectedCategory == Tile.Category.Feng)
                {
                    order = player.GenerateRandomInt(4) + 1; // 1-4 for 风牌
                }
                else
                {
                    order = player.GenerateRandomInt(3) + 5; // 5-7 for 三元牌
                }
                
                targetTile.ModifyCarvedDesign(selectedCategory, order, player);
            }
        }
    }
}
