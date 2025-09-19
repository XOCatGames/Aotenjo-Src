namespace Aotenjo
{
    public class PlayerTileEvent : PlayerEvent
    {
        public Tile tile;

        public PlayerTileEvent(Player player, Tile tile) : base(player)
        {
            this.tile = tile;
        }
        
        public class RetrieveBaseFu : PlayerTileEvent
        {
            public double baseFu;

            public RetrieveBaseFu(Player player, Tile tile, double baseFu) : base(player, tile)
            {
                this.baseFu = baseFu;
            }
        }
    }

    public class DeterminePlayerSelectingTileEvent : PlayerTileEvent
    {
        public bool res;

        public DeterminePlayerSelectingTileEvent(Player player, Tile tile, bool res) : base(player, tile)
        {
            this.res = res;
        }
    }

    public abstract class PlayerDrawTileEvent : PlayerTileEvent
    {
        public PlayerDrawTileEvent(Player player, Tile tile) : base(player, tile)
        {
        }

        public class Pre : PlayerDrawTileEvent
        {
            public Pre(Player player, Tile tile) : base(player, tile)
            {
            }
        }

        public class Post : PlayerDrawTileEvent
        {
            public Post(Player player, Tile tile) : base(player, tile)
            {
            }
        }
    }

    public abstract class PlayerDiscardTileEvent : PlayerTileEvent
    {
        public bool forced;

        public PlayerDiscardTileEvent(Player player, Tile tile) : base(player, tile)
        {
        }

        public class DetermineForce : PlayerDiscardTileEvent
        {
            public bool keepPos;
            public bool res;

            public DetermineForce(Player player, Tile tile, bool res, bool keepPos = false) : base(player, tile)
            {
                this.keepPos = keepPos;
                this.res = res;
            }
        }

        public class Determine : PlayerDiscardTileEvent
        {
            public bool consumeDiscardChances;
            public bool res;

            public Determine(Player player, Tile tile, bool res, bool forceDiscard,
                bool consumeDiscardChances = false) : base(player, tile)
            {
                this.consumeDiscardChances = consumeDiscardChances;
                this.res = res;
                forced = forceDiscard;
            }
        }

        public class Pre : PlayerDiscardTileEvent
        {
            public bool keepPos;

            public Pre(Player player, Tile tile, bool keepPos = false) : base(player, tile)
            {
                this.keepPos = keepPos;
            }
        }

        public class Post : PlayerDiscardTileEvent
        {
            public Post(Player player, Tile tile) : base(player, tile)
            {
            }
        }
    }

    public class PlayerKongTileEvent : PlayerTileEvent
    {
        public Permutation permutation;
        public Block block;

        public PlayerKongTileEvent(Player player, Tile tile, Permutation permutation, Block block) : base(player, tile)
        {
            this.permutation = permutation;
            this.block = block;
        }
    }
}