namespace Aotenjo
{
    public class MinimalistAchievement : Achievement
    {
        public MinimalistAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerRoundEvent.Start.Post>(OnRoundStart);
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerRoundEvent.Start.Post>(OnRoundStart);
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