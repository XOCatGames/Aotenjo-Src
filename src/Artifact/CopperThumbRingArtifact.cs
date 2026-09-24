namespace Aotenjo
{
    public class CopperThumbRingArtifact : Artifact
    {
        public CopperThumbRingArtifact() : base("copper_thumb_ring", Rarity.COMMON)
        {
            SetHighlightRequirement((t, _) => t.properties.material is TileMaterialWood);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerEvents.DetermineForceDiscardTileEvent>(player, HandlePlayerForceDiscardTile);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerEvents.DetermineForceDiscardTileEvent>(player, HandlePlayerForceDiscardTile);
        }

        private void HandlePlayerForceDiscardTile(PlayerDiscardTileEvent.DetermineForce evt)
        {
            if (!evt.res && evt.tile.properties.material is TileMaterialWood)
            {
                evt.player.SpendMoney(1);
                evt.res = true;
            }
        }
    }
}