namespace Aotenjo
{
    public class RunStatusEvent : PlayerEvent
    {
        public RunStatusEvent(Player player) : base(player)
        {
        }

        public class End : RunStatusEvent
        {
            public bool won;
            public PlayerStats stats;

            public End(Player player, bool won, PlayerStats stats) : base(player)
            {
                this.won = won;
                this.stats = stats;
            }

            public class Post : End
            {
                public Post(Player player, bool won, PlayerStats stats) : base(player, won, stats)
                {
                }
            }
        }
    }
}