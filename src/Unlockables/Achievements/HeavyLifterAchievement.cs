namespace Aotenjo
{
    public class HeavyLifterAchievement : Achievement
    {
        public HeavyLifterAchievement(string id) : base(id)
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
            if (won && player.Level >= 16)
            {
                if (player.GetAllTiles().Count > 200)
                {
                    SetComplete();
                }
            }
        }
    }
}