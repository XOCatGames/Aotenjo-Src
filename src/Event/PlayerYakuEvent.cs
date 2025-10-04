namespace Aotenjo
{
    public class PlayerYakuEvent : PlayerEvent
    {
        public YakuType yakuType;

        public PlayerYakuEvent(Player player, YakuType yakuType) : base(player)
        {
            this.yakuType = yakuType;
        }

        public class Obtain : PlayerYakuEvent
        {
            public Obtain(Player player, YakuType yakuType) : base(player, yakuType)
            {
            }
        }

        public class Upgrade : PlayerYakuEvent
        {
            public int level;

            public Upgrade(Player player, YakuType yakuType, int level) : base(player, yakuType)
            {
                this.level = level;
            }
        }

        public class Delete : PlayerYakuEvent
        {
            public int level;

            public Delete(Player player, YakuType yakuType, int level) : base(player, yakuType)
            {
                this.level = level;
            }
        }

        public class Reroll : PlayerYakuEvent
        {
            public YakuType target;
            public YakuPack pack;

            public Reroll(Player player, YakuPack pack, YakuType yakuType, YakuType target) : base(player, yakuType)
            {
                this.target = target;
                this.pack = pack;
            }
        }
        
        public class RetrieveMultiplier : PlayerYakuEvent
        {
            public double multiplier;

            public RetrieveMultiplier(Player player, YakuType yakuType, double multiplier) : base(player, yakuType)
            {
                this.multiplier = multiplier;
            }
        }

        public class ReadBookResult : PlayerYakuEvent
        {
            public Yaku[] results;
            public IBook book;

            public ReadBookResult(Player player, Yaku[] results, IBook book) : base(player, FixedYakuType.Base)
            {
                this.results = results;
                this.book = book;
            }
        }
    }
}