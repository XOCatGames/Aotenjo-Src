using System.Linq;

namespace Aotenjo
{
    public class AmateurExplorerAchievement : Achievement
    {
        public AmateurExplorerAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.PostRunEndEvent += PostRunEnd;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PostRunEndEvent -= PostRunEnd;
        }

        private void PostRunEnd(Player player, bool won, PlayerStats stats)
        {
            if (MahjongDeck.decks.Any(d => stats.GetWonNumberByDeck(d.regName, 4) > 0))
                SetComplete();
        }
    }
}