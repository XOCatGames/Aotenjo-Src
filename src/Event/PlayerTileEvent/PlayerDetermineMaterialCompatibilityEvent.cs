using System.Linq;

namespace Aotenjo
{
    public class PlayerDetermineMaterialCompatibilityEvent : PlayerTileEvent
    {
        public TileMaterial mat;
        public bool res;

        public PlayerDetermineMaterialCompatibilityEvent(Player player, Tile tile, TileMaterial mat) : base(player, tile)
        {
            this.mat = mat;
            res = tile.properties.material is TileMaterialMechPart installed && mat is TileMaterialMechPart requested
                ? requested.GetParts().All(part => installed.GetPartCount(part) > 0)
                : tile.properties.material == mat || tile.properties.material.GetRegName().Equals(mat.GetRegName());
        }
    }
}
