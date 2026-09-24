namespace Aotenjo
{
    public class CrimsonThumbRingArtifact : Artifact
    {
        public CrimsonThumbRingArtifact() : base("jade_thumb_ring", Rarity.EPIC)
        {
            SetHighlightRequirement((t, p) => t.ContainsRed(p));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerEvents.DetermineForceDiscardTileEvent>(player, Player_DetermineDiscardTileEvent);
        }

        private void Player_DetermineDiscardTileEvent(PlayerDiscardTileEvent.DetermineForce obj)
        {
            if (obj.tile.ContainsRed(obj.player))
                obj.res = true;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerEvents.DetermineForceDiscardTileEvent>(player, Player_DetermineDiscardTileEvent);
        }
    }
}