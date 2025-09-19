namespace Aotenjo
{
    public class BankruptAchievement : Achievement
    {
        public BankruptAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.OnEndRunEvent += OnWonGame;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.OnEndRunEvent -= OnWonGame;
        }

        private void OnWonGame(Player player, bool won)
        {
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