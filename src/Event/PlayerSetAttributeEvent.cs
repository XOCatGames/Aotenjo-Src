namespace Aotenjo
{
    public class PlayerSetAttributeEvent : PlayerTileEvent
    {
        public bool isCopy;
        public TileAttribute attributeToReceive;

        public PlayerSetAttributeEvent(Player player, Tile tile, TileAttribute attributeToChange, bool isCopy) : base(
            player, tile)
        {
            attributeToReceive = attributeToChange;
            this.isCopy = isCopy;
        }
    }

    public class PlayerSetPropertiesEvent : PlayerTileEvent
    {
        public bool isCopy;
        public TileProperties propertiesToChange;

        public PlayerSetPropertiesEvent(Player player, Tile tile, TileProperties propertiesToChange, bool isCopy) :
            base(player, tile)
        {
            this.propertiesToChange = propertiesToChange;
            this.isCopy = isCopy;
        }
    }
}