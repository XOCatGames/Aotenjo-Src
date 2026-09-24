using System.Linq;

namespace Aotenjo
{
    public class JadeMasterAchievement : Achievement
    {
        public JadeMasterAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerEvents.PostSettlePermutationEvent>(player, OnSettlePermutation);
        }

        private void OnSettlePermutation(PlayerPermutationEvent permutationEvent)
        {
            if (permutationEvent.permutation.ToTiles()
                .Any(t => t.properties.material is TileMaterialJade jade && jade.level >= 5))
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