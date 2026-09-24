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
            EventBus.Subscribe<RunStatusEvent.End.Post>(PostRunEnd);
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            
            EventBus.Unsubscribe<RunStatusEvent.End.Post>(PostRunEnd);
        }

        private void PostRunEnd(RunStatusEvent.End.Post eventData)
        {
            if (MahjongDeck.decks.Any(d => eventData.stats.GetWonNumberByDeck(d.regName, 4) > 0))
                SetComplete();
        }
    }
}