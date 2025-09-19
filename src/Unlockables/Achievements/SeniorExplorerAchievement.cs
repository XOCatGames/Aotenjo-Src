using System.Linq;

namespace Aotenjo
{
    public class SeniorExplorerAchievement : Achievement
    {
        public SeniorExplorerAchievement(string id) : base(id)
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

        private void PostRunEnd(Player player, bool arg2, PlayerStats stats)
        {
            if (MahjongDeck.decks.Any(d => stats.GetWonNumberByDeck(d.regName, 8) > 0))
                SetComplete();
        }
    }
}