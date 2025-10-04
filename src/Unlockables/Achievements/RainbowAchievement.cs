namespace Aotenjo
{
    public class RainbowAchievement : Achievement
    {
        public RainbowAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerRoundEvent.End.PostPre>(OnRoundEnd);
            
        }

        private void OnRoundEnd(PlayerEvent playerEvent)
        {
            if (playerEvent.player.GetAotenjoBonusMoney() >= 12)
            {
                SetComplete();
            }
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerRoundEvent.End.PostPre>(OnRoundEnd);
        }
    }
}