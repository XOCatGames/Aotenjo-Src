namespace Aotenjo
{
    public class GiftFromHeavenAchievement : Achievement
    {
        public GiftFromHeavenAchievement(string id) : base(id)
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
                if (player.stats.GetCustomStats("discard") == 0)
                {
                    SetComplete();
                }
            }
        }
    }
}