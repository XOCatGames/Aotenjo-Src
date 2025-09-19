using System.Collections.Generic;

namespace Aotenjo
{
    public class OnIBookUpgradeYakuEvent : PlayerEvent
    {
        public readonly IBook book;
        public readonly List<Yaku> drawYakusToUpgrade;

        public OnIBookUpgradeYakuEvent(Player player, IBook book, List<Yaku> drawYakusToUpgrade) : base(player)
        {
            this.book = book;
            this.drawYakusToUpgrade = drawYakusToUpgrade;
        }
    }
}