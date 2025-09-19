namespace Aotenjo
{
    public class DuplicateTileEffect : ArtifactEffect
    {
        private Tile tile;
        private bool withProperties = true;

        public DuplicateTileEffect(Artifact artifact, Tile tile) : base("copy", artifact)
        {
            this.tile = tile;
        }

        public DuplicateTileEffect SetWithNoProperties()
        {
            withProperties = false;
            return this;
        }

        public override void Ingest(Player player)
        {
            Tile newTile = new Tile(tile);
            if (!withProperties)
            {
                newTile.properties = TileProperties.Plain();
            }

            player.AddNewTileToPool(newTile);
        }
    }
}