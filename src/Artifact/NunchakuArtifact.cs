namespace Aotenjo
{
    public class NunchakuArtifact : Artifact
    {
        public NunchakuArtifact() : base("chainsticks", Rarity.RARE)
        {
            SetHighlightRequirement((t, _) => t.IsNumbered() && t.GetOrder() == 5);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.DetermineYaojiuTileEvent += Player_DetermineYaojiuTileEvent;
        }

        private void Player_DetermineYaojiuTileEvent(PlayerTileEvent tileEvent)
        {
            Tile tile = tileEvent.tile;
            if (tile.GetOrder() == 5 && tile.IsNumbered()) tileEvent.canceled = false;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.DetermineYaojiuTileEvent -= Player_DetermineYaojiuTileEvent;
        }
    }
}