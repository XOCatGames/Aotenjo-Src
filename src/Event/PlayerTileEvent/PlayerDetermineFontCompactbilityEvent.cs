namespace Aotenjo
{
    public class PlayerDetermineFontCompactbilityEvent : PlayerTileEvent
    {
        public TileFont font;
        public bool res;

        public PlayerDetermineFontCompactbilityEvent(Player player, Tile tile, TileFont font) : base(player, tile)
        {
            this.font = font;
            res = tile.properties.font == font || tile.properties.font.GetRegName().Equals(font.GetRegName());
        }
    }
}