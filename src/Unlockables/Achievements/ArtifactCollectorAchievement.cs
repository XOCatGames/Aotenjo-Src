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
            player.PostRunEndEvent += PostRunEnd;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PostRunEndEvent -= PostRunEnd;
        }

        private void PostRunEnd(Player player, bool won, PlayerStats stats)
        {
            if (Artifacts.ArtifactList.All(g => stats.ArtifactOwned(g)))
                SetComplete();
        }
    }
}