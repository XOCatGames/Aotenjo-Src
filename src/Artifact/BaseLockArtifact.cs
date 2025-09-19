using System;

namespace Aotenjo
{
    public abstract class BaseLockArtifact : LevelingArtifact
    {
        protected const double MUL_BASE = 2.0D;
        protected const double MUL_UPGRADED = 3.0D;
        protected const double MUL_PER_YAKU = 1.0D;
        
        protected const int YAKU_COUNT_TO_SILVER = 8;
        protected const int YAKU_COUNT_TO_GOLD = 12;

        protected BaseLockArtifact(string name, Rarity rarity) : base(name, rarity, 0)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.RetrieveYakuMultiplierEvent += PlayerOnRetrieveYakuMultiplierEvent;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.RetrieveYakuMultiplierEvent -= PlayerOnRetrieveYakuMultiplierEvent;
        }

        private void PlayerOnRetrieveYakuMultiplierEvent(PlayerYakuEvent.RetrieveMultiplier yakuEvent)
        {
            Yaku yaku = yakuEvent.yakuType.GetYakuDefinition();
            if (yaku.rarity >= Rarity.RARE) yakuEvent.canceled = true;
        }
    }
}