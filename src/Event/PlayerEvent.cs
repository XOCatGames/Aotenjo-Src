namespace Aotenjo
{
    public class PlayerEvent
    {
        public Player player;
        public string message;
        public bool canceled = false;

        public PlayerEvent(Player player)
        {
            this.player = player;
            message = "NONE";
        }
    }
}