namespace Aotenjo
{
    public class PlayerConsumeDessertEvent : PlayerEvent
    {
        public Tile tile;
        public TileMaterialDessert dessert;

        public PlayerConsumeDessertEvent(Player player, Tile tile, TileMaterialDessert dessert) : base(player)
        {
            this.tile = tile;
            this.dessert = dessert;
        }
    }
}