using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class AntiDragonBookArtifact : BambooArtifact, IBook
    {
        public AntiDragonBookArtifact() : base("antidragon_book", Rarity.EPIC)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            if (player is BambooDeckPlayer b)
            {
                b.RevealIndicatorEvent += OnRevealIndicator;
            }
        }

        private void OnRevealIndicator(PlayerEvent evt)
        {
            evt.canceled = true;
            this.UpgradeYaku(evt.player);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            if (player is BambooDeckPlayer b)
            {
                b.RevealIndicatorEvent -= OnRevealIndicator;
            }
        }

        public List<Yaku> GetYakuPool(Player player)
        {
            return player.GetSkillSet().GetUnlockedYakus().Where(y => y.rarity == Rarity.RARE).ToList();
        }

        public List<Yaku> DrawYakusToUpgrade(Player player)
        {
            LotteryPool<Yaku> yakuPool = new();
            yakuPool.AddRange(GetYakuPool(player), 5);
            return yakuPool.DrawRange(player.GenerateRandomInt, 3, false).ToList();
        }
    }
}