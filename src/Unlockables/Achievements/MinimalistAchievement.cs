namespace Aotenjo
{
    public class MinimalistAchievement : Achievement
    {
        public MinimalistAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.PostRoundStartEvent += OnRoundStart;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PostRoundStartEvent -= OnRoundStart;
        }

        private void OnRoundStart(PlayerEvent playerEvent)
        {
            if (playerEvent.player.GetAllTiles().Count < 13)
            {
                SetComplete();
            }
        }
    }
}