namespace Aotenjo
{
    public class BurntAmuletArtifact : Artifact
    {
        public BurntAmuletArtifact() : base("burnt_amulet", Rarity.COMMON)
        {
            SetHighlightRequirement((t, p) => t.ContainsRed(p));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerEvents.PostAddTileEvent>(player, OnPostAddTile);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerEvents.PostAddTileEvent>(player, OnPostAddTile);
        }

        private void OnPostAddTile(PlayerTileEvent tileEvent)
        {
            if (tileEvent.tile.ContainsRed(tileEvent.player))
            {
                tileEvent.tile.SetMaterial(TileMaterial.HellWood(), tileEvent.player);
            }
        }
    }
}