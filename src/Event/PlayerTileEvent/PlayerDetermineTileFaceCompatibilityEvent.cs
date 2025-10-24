namespace Aotenjo
{
    public class PlayerDetermineTileFaceCompatibilityEvent : PlayerTileEvent
    {
        public int catToTest;
        public int orderToTest;

        public bool res;

        public PlayerDetermineTileFaceCompatibilityEvent(Player player, Tile tile, int catToTest, int orderToTest) : base(player, tile)
        {
            bool catRes = catToTest == -1 || tile.CompatWithCategory((Tile.Category)catToTest);
            bool orderRes = orderToTest == -1 || tile.GetOrder() == orderToTest;

            res = catRes && orderRes;
        }
    }
}