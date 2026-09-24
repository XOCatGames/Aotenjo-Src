using System.Linq;

namespace Aotenjo
{
    public class ArtifactCollectorAchievement : Achievement
    {
        public ArtifactCollectorAchievement(string id) : base(id)
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
            if (Artifacts.ArtifactList.All(eventData.stats.ArtifactOwned))
                SetComplete();
        }
    }
}