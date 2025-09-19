using System;

namespace Aotenjo
{
    public class IronHenArtifact : LevelingArtifact
    {
        public IronHenArtifact() : base("iron_hen", Rarity.RARE, 0)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), Level);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.SpendMoneyEvent += OnSpendMoney;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.SpendMoneyEvent -= OnSpendMoney;
        }

        private void OnSpendMoney(PlayerMoneyEvent evt)
        {
            if (evt.amount < 5 && evt.amount > 0)
            {
                evt.player.EarnMoney(1);
                MessageManager.Instance.OnArtifactEarnMoney(1, this);
                Level++;
            }
        }
    }
}