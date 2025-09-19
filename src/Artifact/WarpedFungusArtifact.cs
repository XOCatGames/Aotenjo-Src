using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class WarpedFungusArtifact : LevelingArtifact, IMultiplierProvider
    {
        private const float MUL = 0.2f;

        public WarpedFungusArtifact() : base("warped_fungus", Rarity.EPIC, 0)
        {
            SetHighlightRequirement((t, p) => t.CompactWithMaterial(TileMaterial.HellWood(), p));
        }

        public override string GetDescription(Player p, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MUL, GetMul(p));
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToMulFanFormat(GetMul(player));
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.MulFan(GetMul(player), this));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        private void PostRoundEnd(PlayerEvent playerEvent)
        {
            Level = 0;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        public override void AddDiscardTileEffects(Player player, Tile tile,
            List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone)
        {
            base.AddDiscardTileEffects(player, tile, onDiscardTileEffects, withForce, isClone);
            if (tile.CompactWithMaterial(TileMaterial.HellWood(), player))
            {
                onDiscardTileEffects.Add(GetLevelUpEffect().OnTile(tile));
            }
        }

        public double GetMul(Player player)
        {
            return 1f + MUL * Level;
        }
    }
}