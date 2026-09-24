using System.Linq;

namespace Aotenjo
{
    public class SeniorExplorerAchievement : Achievement
    {
        public SeniorExplorerAchievement(string id) : base(id)
        {
        }

        
        [SubscribeToEvent]
        private void PostRunEnd(RunStatusEvent.End.Post eventData)
        {
            Player player = eventData.player;
            bool won = eventData.won;
            PlayerStats stats = eventData.stats;
            if (MahjongDeck.decks.Any(d => stats.GetWonNumberByDeck(d.regName, 8) > 0))
                SetComplete();
        }
    }
}