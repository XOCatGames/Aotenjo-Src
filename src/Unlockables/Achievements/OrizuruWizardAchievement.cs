namespace Aotenjo
{
    public class OrizuruWizardAchievement : Achievement
    {
        public OrizuruWizardAchievement(string id) : base(id)
        {
        }


        public override void SubscribeToPlayer(Player player)
        {
            player.PostObtainArtifactEvent += PostObtainArtifact;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PostObtainArtifactEvent -= PostObtainArtifact;
        }

        private void PostObtainArtifact(PlayerArtifactEvent evt)
        {
            if (RulerArtifact.GetOrizuruCount(evt.player) >= 5)
            {
                SetComplete();
            }
        }
    }
}