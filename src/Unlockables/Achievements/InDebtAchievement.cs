namespace Aotenjo
{
    public class InDebtAchievement : Achievement
    {
        public InDebtAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerEvents.SpendMoneyEvent>(player, OnSpendMoney);
        }

        private void OnSpendMoney(PlayerMoneyEvent evt)
        {
            if (evt.player.GetMoney() - evt.amount <= -50)
            {
                SetComplete();
            }
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerEvents.SpendMoneyEvent>(player, OnSpendMoney);
        }
    }
}