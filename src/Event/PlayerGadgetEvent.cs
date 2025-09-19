namespace Aotenjo
{
    public class PlayerGadgetEvent : PlayerTileEvent
    {
        public Gadget gadget;

        public PlayerGadgetEvent(Player player, Gadget gadget, Tile tile = null) : base(player, tile)
        {
            this.gadget = gadget;
        }
    }

    public class PlayerSetTransformEvent : PlayerGadgetEvent
    {
        public TileTransform transform;

        public PlayerSetTransformEvent(Player player, Gadget gadget, TileTransform transform, Tile tile) : base(player,
            gadget, tile)
        {
            this.transform = transform;
        }
    }
}