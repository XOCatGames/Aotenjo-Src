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
            player.OnPreUpgradeYakuEvent += OnPreUpgradeYaku;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.OnPreUpgradeYakuEvent -= OnPreUpgradeYaku;
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