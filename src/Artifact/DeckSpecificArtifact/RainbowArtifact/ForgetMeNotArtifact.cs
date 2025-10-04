using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class ForgetMeNotArtifact : LevelingArtifact, IMultiplierProvider
    {
        private const float MUL = 0.25f;

        public ForgetMeNotArtifact() : base("forget_me_not", Rarity.EPIC, 0)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MUL, 1 + MUL * Level);
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player) => ToMulFanFormat(GetMul(player));

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.MulFan(() => GetMul(player), this));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostDiscardTileEvent += OnPostDiscardTileEvent;
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        private void PostRoundEnd(PlayerEvent playerEvent)
        {
            Level = 0;
        }

        private void OnPostDiscardTileEvent(PlayerDiscardTileEvent.Post discardTileEvent)
        {
            if (discardTileEvent.tile is FlowerTile)
            {
                Level++;
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostDiscardTileEvent -= OnPostDiscardTileEvent;
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        public double GetMul(Player player) => 1 + MUL * Level;
    }
}