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
            player.PostDiscardTileEvent += PostDiscardTile;
            player.PostDrawTileEvent += PostDrawTile;
            player.PostRoundStartEvent += PostRoundStart;
            discarding = "";
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PostDiscardTileEvent -= PostDiscardTile;
            player.PostDrawTileEvent += PostDrawTile;
        }

        private void PostDiscardTile(PlayerDiscardTileEvent.Post discardTileEvent)
        {
            discarding = discardTileEvent.tile.ToString();
        }

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