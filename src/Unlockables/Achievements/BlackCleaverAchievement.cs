namespace Aotenjo
{
    public class BlackCleaverAchievement : Achievement
    {
        public BlackCleaverAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerEvents.PostSettlePermutationEvent>(player, OnSettlePermutation);
        }

        private void OnSettlePermutation(PlayerPermutationEvent permutationEvent)
        {
            if (((FireAxeArtifact)Artifacts.FireAxe).Level >= 20)
            {
                SetComplete();
            }
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerEvents.PostSettlePermutationEvent>(player, OnSettlePermutation);
        }
    }
}