using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class TrashCanArtifact : LevelingArtifact
    {
        private const int BASE = 0;
        private const int LEVEL_INCREMENT = 1;

        public TrashCanArtifact() : base("trash_can", Rarity.RARE, 0)
        {
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player) => ToAddFuFormat(Level);

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), BASE + LEVEL_INCREMENT * Level, LEVEL_INCREMENT);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.AddFu(BASE + LEVEL_INCREMENT * Level, this));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostDiscardTileEvent += Trashed;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostDiscardTileEvent -= Trashed;
        }

        private void Trashed(PlayerDiscardTileEvent.Post eventData)
        {
            Level++;
        }
    }
}