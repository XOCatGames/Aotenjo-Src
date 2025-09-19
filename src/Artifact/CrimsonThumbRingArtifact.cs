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
            player.DetermineForceDiscardTileEvent += Player_DetermineDiscardTileEvent;
        }

        private void Player_DetermineDiscardTileEvent(PlayerDiscardTileEvent.DetermineForce obj)
        {
            if (obj.tile.ContainsRed(obj.player))
                obj.res = true;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.DetermineForceDiscardTileEvent -= Player_DetermineDiscardTileEvent;
        }
    }
}