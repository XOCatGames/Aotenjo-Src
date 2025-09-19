using System.Collections.Generic;

namespace Aotenjo
{
    public class MagnifierArtifact : Artifact
    {
        public MagnifierArtifact() : base("magnifier", Rarity.RARE)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<OnIBookUpgradeYakuEvent>(PlayerOnUpgradeYakuFromIBookEvent);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<OnIBookUpgradeYakuEvent>(PlayerOnUpgradeYakuFromIBookEvent);
        }

        private void PlayerOnUpgradeYakuFromIBookEvent(OnIBookUpgradeYakuEvent evt)
        {
            IBook book = evt.book;
            if(book == null) return;
            List<Yaku> extraYaku = new List<Yaku>();
            for (int i = 0; i < 2; i++)
            {
                extraYaku.Add(LotteryPool<Yaku>.DrawFromCollection(book.GetYakuPool(evt.player), evt.player.GenerateRandomInt));
            }
            evt.drawYakusToUpgrade.AddRange(extraYaku);
        }
    }
}