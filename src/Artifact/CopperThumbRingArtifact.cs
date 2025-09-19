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
            player.DetermineForceDiscardTileEvent += HandlePlayerForceDiscardTile;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.DetermineForceDiscardTileEvent -= HandlePlayerForceDiscardTile;
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