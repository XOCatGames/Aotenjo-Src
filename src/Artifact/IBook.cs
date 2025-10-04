using System.Collections.Generic;

namespace Aotenjo
{
    public interface IBook : IRegisterable
    {
        List<Yaku> GetYakuPool(Player player);
        List<Yaku> DrawYakusToUpgrade(Player player);
    }
    
    public static class IBookHelper
    {
        public static void UpgradeYaku(this IBook book, Player player)
        {
            var drawYakusToUpgrade = book.DrawYakusToUpgrade(player);
            
            EventBus.Publish(new OnIBookUpgradeYakuEvent(player, book, drawYakusToUpgrade));
            
            foreach (var yaku in drawYakusToUpgrade)
            {
                player.UpgradeYaku(yaku.GetYakuType(), 1);           
            }

            player.PostReadIBook(book, drawYakusToUpgrade);
        }
    }
}