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
            player.PostAddTileEvent += OnPostAddTile;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostAddTileEvent -= OnPostAddTile;
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