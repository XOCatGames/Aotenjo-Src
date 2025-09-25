namespace Aotenjo
{
    public class PlayerModifyCarvedDesignEvent : PlayerTileEvent
    {
        private readonly Tile.Category newCat;
        private readonly int newOrd;

        public PlayerModifyCarvedDesignEvent(Tile tile, Tile.Category newCat, int newOrd, Player player) : base(player, tile)
        {
            this.newCat = newCat;
            this.newOrd = newOrd;
        }

        public class Pre : PlayerModifyCarvedDesignEvent
        {
            public Pre(Tile tile, Tile.Category newCat, int newOrd, Player player) : base(tile, newCat, newOrd, player)
            {
            }
        }
        

        public class Post : PlayerModifyCarvedDesignEvent
        {
            public Post(Tile tile, Tile.Category newCat, int newOrd, Player player) : base(tile, newCat, newOrd, player)
            {
            }
        }
    }
}