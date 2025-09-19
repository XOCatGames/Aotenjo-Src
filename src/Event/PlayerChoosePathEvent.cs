namespace Aotenjo
{
    public class PlayerChoosePathEvent : PlayerEvent
    {
        public Direction direction;
        public Destination[] destinations;

        public PlayerChoosePathEvent(Player player, Direction direction, Destination[] destinations) : base(player)
        {
            this.direction = direction;
            this.destinations = destinations;
        }
    }
}