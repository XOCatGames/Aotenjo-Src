using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class FrogToyArtifact : LevelingArtifact, ICountable
    {
        private const int MAX = 3;

        public FrogToyArtifact() : base("frog_toy", Rarity.RARE, MAX)
        {
            SetHighlightRequirement((t, player) => t.IsYaoJiu(player));
        }


        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(localizer("artifact_frog_toy_description"), Level, MAX);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostDiscardTileEvent += PostDiscardTile;
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostDiscardTileEvent -= PostDiscardTile;
            EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        private void PostDiscardTile(PlayerDiscardTileEvent.Post eventData)
        {
            if (eventData.canceled || Level <= 0) return;
            Player player = eventData.player;
            Tile tile = eventData.tile;
            if (Level > 0 && tile.IsYaoJiu(player))
            {
                Level--;
                bool res = player.RemoveTileFromDiscarded(tile);
                if (!res) return;
                MessageManager.Instance.OnRemoveTileEvent(new List<Tile> { tile });
            }
        }

        public void PostRoundEnd(PlayerEvent playerEvent)
        {
            Level = MAX;
        }


        public int GetMaxCounter()
        {
            return initLevel;
        }

        public int GetCurrentCounter()
        {
            return initLevel - Level;
        }
    }
}