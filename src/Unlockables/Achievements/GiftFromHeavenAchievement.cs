namespace Aotenjo
{
    public class GiftFromHeavenAchievement : Achievement
    {
        public GiftFromHeavenAchievement(string id) : base(id)
        {
        }

        [SubscribeToEvent]
        private void OnWonGame(RunStatusEvent.End eventData)
        {
            bool won = eventData.won;
            Player player = eventData.player;
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