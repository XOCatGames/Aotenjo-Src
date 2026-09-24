namespace Aotenjo
{
    public class FirstSparrowAchievement : Achievement
    {
        public FirstSparrowAchievement(string id) : base(id)
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
            if (won)
                SetComplete();
        }
    }
}