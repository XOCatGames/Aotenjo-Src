namespace Aotenjo
{
    public class PlayerDetermineFontCompatibilityEvent : PlayerTileEvent
    {
        public TileFont font;
        public bool res;

        public PlayerDetermineFontCompatibilityEvent(Player player, Tile tile, TileFont font) : base(player, tile)
        {
            this.font = font;
            res = tile.properties.font == font || tile.properties.font.GetRegName().Equals(font.GetRegName());
        }
    }
}