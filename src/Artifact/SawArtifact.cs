using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class SawArtifact : LevelingArtifact, ICountable
    {
        private const int MAX = 3;
        private const int DISCARD_RETURN = 2;

        public SawArtifact() : base("saw", Rarity.COMMON, MAX)
        {
            SetHighlightRequirement((t, _) => t.properties.mask.GetRegName() == TileMask.Fractured().GetRegName());
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), DISCARD_RETURN, Level, MAX);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostDiscardTileEvent += PostDiscardTile;
            EventBus.Subscribe<PlayerRoundEvent.Start.Post>(OnRoundStart);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostDiscardTileEvent -= PostDiscardTile;
            EventBus.Unsubscribe<PlayerRoundEvent.Start.Post>(OnRoundStart);
        }

        private void PostDiscardTile(PlayerDiscardTileEvent.Post eventData)
        {
            if (eventData.canceled || Level <= 0) return;
            Player player = eventData.player;
            Tile tile = eventData.tile;
            if (Level > 0 && tile.properties.mask.GetRegName() == TileMask.Fractured().GetRegName())
            {
                Level--;
                bool res = player.RemoveTileFromDiscarded(tile);
                player.DiscardLeft += DISCARD_RETURN;
                AudioSystem.PlayAddSwapChanceSound();
                if (!res) return;
                MessageManager.Instance.OnRemoveTileEvent(new List<Tile> { tile });
            }
        }

        public void OnRoundStart(PlayerEvent playerEvent)
        {
            Level = MAX;
        }

        public int GetMaxCounter() => initLevel;
        public int GetCurrentCounter() => initLevel - Level;
    }
}