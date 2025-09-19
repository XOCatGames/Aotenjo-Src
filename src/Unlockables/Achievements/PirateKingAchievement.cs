namespace Aotenjo
{
    public class PirateKingAchievement : Achievement
    {
        public PirateKingAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.PreRemoveArtifact += SellArtifact;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PreRemoveArtifact -= SellArtifact;
        }

        private void SellArtifact(PlayerArtifactEvent evt)
        {
            if (evt.artifact is PirateChestArtifact p && p.Level >= 100)
            {
                SetComplete();
            }
        }
    }
}