namespace Aotenjo
{
    public class ARuleAchievement : Achievement
    {
        public ARuleAchievement(string id) : base(id)
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
            if (won && player.deck.regName == MahjongDeck.BambooDeck.regName &&
                player.stats.GetCustomStats("indicator_revealed") == 0 &&
                player.stats.GetFontPlayedCount(TileFont.RED) == 0)
            {
                SetComplete();
            }
        }
    }
}