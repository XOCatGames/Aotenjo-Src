namespace Aotenjo
{
    public class LongHandAchievement : Achievement
    {
        public LongHandAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.PostRoundStartEvent += Player_PostRoundStartEvent;
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
            player.PostRoundStartEvent -= Player_PostRoundStartEvent;
        }
    }
}