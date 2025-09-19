namespace Aotenjo
{
    public class PlayerGetScarletCoreLevelEvent : PlayerEvent
    {
        public Tile.Category cat;
        public int level;

        public PlayerGetScarletCoreLevelEvent(Player player, Tile.Category cat, int level) : base(player)
        {
            this.cat = cat;
            this.level = level;
        }
    }
}