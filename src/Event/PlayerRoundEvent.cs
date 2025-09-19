namespace Aotenjo
{
    public class PlayerRoundEvent : PlayerEvent
    {
        public PlayerRoundEvent(Player player) : base(player)
        {
        }
        
        public class Start : PlayerRoundEvent
        {
            public Start(Player player) : base(player)
            {
            }
            
            public class Pre : Start
            {
                public Pre(Player player) : base(player)
                {
                }
            }
            
            public class Post : Start
            {
                public Post(Player player) : base(player)
                {
                }
            }
        }
        
        public class End : PlayerRoundEvent
        {
            public End(Player player) : base(player)
            {
            }
            public class PrePre : End
            {
                public PrePre(Player player) : base(player)
                {
                }
            }
            public class Pre : End
            {
                public Pre(Player player) : base(player)
                {
                }
            }
            
            public class PostPre : End
            {
                public PostPre(Player player) : base(player)
                {
                }
            }
            
            public class Post : End
            {
                public Post(Player player) : base(player)
                {
                }
            }
        }
    }
}