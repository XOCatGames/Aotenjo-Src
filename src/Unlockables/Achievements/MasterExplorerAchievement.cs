using System.Linq;

namespace Aotenjo
{
    public class MasterExplorerAchievement : Achievement
    {
        public MasterExplorerAchievement(string id) : base(id)
        {
        }

        [SubscribeToEvent]
        private void PostRunEnd(RunStatusEvent.End.Post eventData)
        {
            if (MahjongDeck.ascensionDecks.All(d => eventData.stats.GetWonNumberByDeck(d.regName, 8) > 0))
                SetComplete();
        }
    }
}