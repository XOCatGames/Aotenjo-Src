namespace Aotenjo
{
    public class OrizuruWizardAchievement : Achievement
    {
        public OrizuruWizardAchievement(string id) : base(id)
        {
        }


        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerEvents.PostObtainArtifactEvent>(player, PostObtainArtifact);
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerEvents.PostObtainArtifactEvent>(player, PostObtainArtifact);
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