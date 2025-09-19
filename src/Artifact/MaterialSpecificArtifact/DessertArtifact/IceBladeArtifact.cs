using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class IceBladeArtifact : Artifact
    {

        public bool winded = false;
        
        private const int CHANCE_TO_FROZEN = 10;
        public IceBladeArtifact() : base("ice_blade", Rarity.RARE)
        {
            SetHighlightRequirement((tile, player) => 
                tile.properties.material is TileMaterialDessert);
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(player, localizer), GetChanceMultiplier(player), CHANCE_TO_FROZEN);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.OnDessertTileConsumeAttemptEvent += OnDessertTileConsumeAttempt;
            player.PreAddScoringAnimationEffectEvent += OnPreAddScoringAnimationEffect;
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.OnDessertTileConsumeAttemptEvent -= OnDessertTileConsumeAttempt;
            player.PreAddScoringAnimationEffectEvent -= OnPreAddScoringAnimationEffect;
            EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        private void OnPreAddScoringAnimationEffect(Permutation perm, Player player, List<IAnimationEffect> arg3)
        {
            winded = false;
            if (perm == null) return;
            if (perm.GetYakus(player).Any(y => YakuTester.InfoMap[y].GetYakuCategories().Contains(0)))
            {
                winded = true;
            }
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            winded = false;
        }

        private void PostRoundEnd(PlayerEvent playerEvent)
        {
            winded = false;
        }

        

        private void OnDessertTileConsumeAttempt(PlayerEvent evt)
        {
            if (winded)
            {
                evt.canceled = true; // 阻止消耗
            }
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);

            // 冻结所有未打出的甜品牌
            var dessertTilesToFreeze = player.GetAccumulatedPermutation()?.ToTiles().Where(t => t.properties.material is TileMaterialDessert);

            if (dessertTilesToFreeze == null) return;
            effects.AddRange((dessertTilesToFreeze
                .Where(tile => player.GenerateRandomDeterminationResult(CHANCE_TO_FROZEN))
                .Select(tile => new FreezeEffect(this, tile).OnTile(tile))));
        }
    }
} 