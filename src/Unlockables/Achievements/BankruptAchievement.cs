namespace Aotenjo
{
    public class BankruptAchievement : Achievement
    {
        public BankruptAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<RunStatusEvent.End>(OnWonGame);
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<RunStatusEvent.End>(OnWonGame);
        }

        private void OnWonGame(RunStatusEvent.End eventData)
        {
            var player = eventData.player;
            var won = eventData.won;
            if (won && player.Level <= 16)
            {
                if (player.GetMoney() < 0)
                {
                    SetComplete();
                }
            }
        }
    }
}