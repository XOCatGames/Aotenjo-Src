namespace Aotenjo
{
    public class LongHandAchievement : Achievement
    {
        public LongHandAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerRoundEvent.Start.Post>(Player_PostRoundStartEvent);
        }

        private void Player_PostRoundStartEvent(PlayerEvent playerEvent)
        {
            if (playerEvent.player.GetHandDeckCopy().Count >= 18)
            {
                SetComplete();
            }
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerRoundEvent.Start.Post>(Player_PostRoundStartEvent);
        }
    }
}