using System.Linq;

namespace Aotenjo
{
    public class SparrowSaverAchievement : Achievement
    {
        public SparrowSaverAchievement(string id) : base(id)
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
            if (MahjongDeck.decks.Where(d => stats.GetWonNumberByDeck(d.regName) > 0).Count() >= 4)
                SetComplete();
        }
    }
}