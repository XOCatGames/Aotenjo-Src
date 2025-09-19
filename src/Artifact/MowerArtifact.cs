using System;

namespace Aotenjo
{
    public class MowerArtifact : LevelingArtifact, IActivable, ICountable
    {
        private const int MAX_USAGE = 5;
        private const int EXTRA_CHANCE = 3;

        public MowerArtifact() : base("mower", Rarity.COMMON, MAX_USAGE)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), Level, EXTRA_CHANCE);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PreDiscardTileEvent += Backup;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PreDiscardTileEvent -= Backup;
        }

        private void Backup(PlayerDiscardTileEvent.Pre eventData)
        {
            Player player = eventData.player;

            if (Level > 0 && player.DiscardLeft < 1)
            {
                player.DiscardLeft += EXTRA_CHANCE;
                Level--;
                AudioSystem.PlayAddSwapChanceSound();
            }
        }

        public bool IsActivating()
        {
            return Level > 0;
        }

        public int GetMaxCounter() => MAX_USAGE;

        public int GetCurrentCounter() => MAX_USAGE - Level;
    }
}