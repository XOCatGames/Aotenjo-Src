using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class OpalDaggerArtifact : Artifact
    {
        private const int MUL = 2;
        private const int GIFT_COUNT = 5;
        public OpalDaggerArtifact() : base("opal_dagger", Rarity.RARE)
        {
            SetHighlightRequirement((tile, player) => tile.CompatWithMaterial(TileMaterial.Ore(), player));
        }
        
        public override void OnObtain(Player player)
        {
            base.OnObtain(player);
            foreach (var plainTiles in player.DrawPlainTilesFromPool(GIFT_COUNT))
            {
                plainTiles.SetMaterial(TileMaterial.Ore(), player);
            }
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MUL, GIFT_COUNT);
        }
        
        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerEvents.PostAddSingleTileAnimationEffectEvent>(player, OnPostAddSingleTileAnimationEffect);
        }

        private void OnPostAddSingleTileAnimationEffect(Permutation perm, Player player, List<OnTileAnimationEffect> effects, OnTileAnimationEffect effect, Tile tile)
        {
            if (effect.GetEffect() is not TileMaterialOre.TransformEffect) return;
            effects.Remove(effect);
            effects.Add(ScoreEffect.MulFan(MUL, this).OnTile(tile));
            effects.Add(effect);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerEvents.PostAddSingleTileAnimationEffectEvent>(player, OnPostAddSingleTileAnimationEffect);
        }
    }
}