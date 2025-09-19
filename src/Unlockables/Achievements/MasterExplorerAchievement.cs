using System.Linq;

namespace Aotenjo
{
    public class MasterExplorerAchievement : Achievement
    {
        public MasterExplorerAchievement(string id) : base(id)
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
            if (MahjongDeck.ascensionDecks.All(d => stats.GetWonNumberByDeck(d.regName, 8) > 0))
                SetComplete();
        }
    }
}