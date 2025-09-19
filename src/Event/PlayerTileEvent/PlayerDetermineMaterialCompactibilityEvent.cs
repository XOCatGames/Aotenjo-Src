namespace Aotenjo
{
    public class PlayerDetermineMaterialCompactibilityEvent : PlayerTileEvent
    {
        public TileMaterial mat;
        public bool res;

        public PlayerDetermineMaterialCompactibilityEvent(Player player, Tile tile, TileMaterial mat) : base(player, tile)
        {
            this.mat = mat;
            res = tile.properties.material == mat || tile.properties.material.GetRegName().Equals(mat.GetRegName());
        }
    }
}