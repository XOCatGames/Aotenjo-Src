namespace Aotenjo
{
    public class PlayerDetermineTileFaceCompactbilityEvent : PlayerTileEvent
    {
        public int catToTest;
        public int orderToTest;

        public bool res;

        public PlayerDetermineTileFaceCompactbilityEvent(Player player, Tile tile, int catToTest, int orderToTest) : base(player, tile)
        {
            bool catRes = catToTest == -1 || tile.CompactWithCategory((Tile.Category)catToTest);
            bool orderRes = orderToTest == -1 || tile.GetOrder() == orderToTest;

            res = catRes && orderRes;
        }
    }
}