namespace Aotenjo
{
    public class ThisIsMahjong1Achievement : Achievement
    {
        private string discarding = "";

        public ThisIsMahjong1Achievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerEvents.PostDiscardTileEvent>(player, PostDiscardTile);
            EventBus.Subscribe<PlayerRoundEvent.Start.Post>(PostRoundStart);
            discarding = "";
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            base.UnsubscribeFromPlayer(player);
            EventBus.Unsubscribe<PlayerEvents.PostDiscardTileEvent>(player, PostDiscardTile);
            EventBus.Unsubscribe<PlayerRoundEvent.Start.Post>(PostRoundStart);
        }

        private void PostDiscardTile(PlayerDiscardTileEvent.Post discardTileEvent)
        {
            discarding = discardTileEvent.tile.ToString();
        }

        [SubscribeToEvent]
        private void PostDrawTile(PlayerDrawTileEvent.Post drawTileEvent)
        {
            if (drawTileEvent.tile.ToString() == discarding)
            {
                SetComplete();
            }

            discarding = "";
        }

        private void PostRoundStart(PlayerEvent playerEvent)
        {
            discarding = "";
        }
    }
}