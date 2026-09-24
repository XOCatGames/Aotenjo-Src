using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class RosemaryArtifact : Artifact
    {
        private const int FU_BONUS = 30; // 额外符数

        public RosemaryArtifact() : base("rosemary", Rarity.EPIC)
        {
            SetHighlightRequirement((tile, player) => 
                tile.properties.material is TileMaterialDessert && 
                tile.ContainsGreen(player));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU_BONUS);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerEvents.OnDessertTileConsumeAttemptEvent>(player, OnDessertTileConsumeAttempt);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerEvents.OnDessertTileConsumeAttemptEvent>(player, OnDessertTileConsumeAttempt);
        }

        // 阻止含有绿色的甜品牌消耗
        private void OnDessertTileConsumeAttempt(PlayerEvent evt)
        {
            if (evt is PlayerConsumeDessertEvent dessertEvt)
            {
                // 如果牌含有绿色，阻止消耗
                if (dessertEvt.tile.ContainsGreen(dessertEvt.player))
                {
                    evt.canceled = true; // 阻止消耗
                }
            }
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            
            // 含有绿色的甜品牌在计分时额外+30符
            if (tile.properties.material is TileMaterialDessert && tile.ContainsGreen(player))
            {
                effects.Add(ScoreEffect.AddFu(FU_BONUS, this));
            }
        }
    }
} 