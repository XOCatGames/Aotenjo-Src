namespace Aotenjo
{
    public class SutraArtifact : Artifact
    {
        public SutraArtifact() : base("sutra", Rarity.COMMON)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerEvents.OnPreUpgradeYakuEvent>(player, OnPreUpgradeYaku);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerEvents.OnPreUpgradeYakuEvent>(player, OnPreUpgradeYaku);
        }

        private void OnPreUpgradeYaku(PlayerYakuEvent.Upgrade evt)
        {
            if (YakuTester.InfoMap[evt.yakuType].rarity == Rarity.COMMON)
            {
                evt.level++;
            }
        }
    }
}