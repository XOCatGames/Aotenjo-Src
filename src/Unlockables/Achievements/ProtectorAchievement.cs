namespace Aotenjo
{
    public class ProtectorAchievement : Achievement
    {
        public ProtectorAchievement(string id) : base(id)
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
            if (!won || player.Level > 16) return;
            if (player.stats.GetCustomStats("tile_destoryed") == 0)
            {
                SetComplete();
            }
        }
    }
}