namespace Aotenjo
{
    public class ARuleAchievement : Achievement
    {
        public ARuleAchievement(string id) : base(id)
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
            var playerStats = eventData.player.stats;
            if (eventData.won && eventData.player.deck.regName == MahjongDeck.BambooDeck.regName &&
                playerStats.GetCustomStats("indicator_revealed") == 0 &&
                playerStats.GetFontPlayedCount(TileFont.RED) == 0)
            {
                SetComplete();
            }
        }
    }
}